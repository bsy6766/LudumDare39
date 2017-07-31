using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumeStatus
{
	public float duration;
	public float consumeRateMod;
	public bool rageMode;
	public bool broken;
}

public class PowerGenerator : MonoBehaviour {
	public int id;

	private float powerStored;
	private float maxPower;

	private float genRate;
	private float consumeRate;

	private bool coreInstalled;

	public Image powerBar;

	private Bounds interactionBB;

	public GameObject glass;
	public GameObject barBG;
	public Canvas barCanvas;

	public Color green;
	public Color yellow;
	public Color red;
	public Color purple;

	public Color brokenColor;
	public Color rageColor;

	private float elapsedTime = 0;
	private float expParticleSpawnCooldown;
	private int expParticleSpawnChance = 12;
	private int megaExpParticleSpawnChance = 2;

	private List<GameObject> expBalls;

	public Player player;

	private bool paused;
	
	private List<ConsumeStatus> consumeStatusList;
	private float consumeStatusElapsedTime;
	private int curCSIndex;

	public RepairBar repairBar;

	// Use this for initialization
	void Start () {
		coreInstalled = false;
		
		genRate = 7.0f;
		consumeRate = 1.0f;

		maxPower = 100;
		powerStored = Random.Range(70, 80) ;

		updatePowerBar();

		Bounds bb = GetComponent<BoxCollider2D>().bounds;
		float pad = 30.0f;
		Vector3 min = bb.min;
		Vector3 max = bb.max;

		min.x -= pad;
		min.y -= pad;

		max.x += pad;
		max.y += pad;

		interactionBB = bb;

		interactionBB.SetMinMax(min, max);

		//interactionBB = new Bounds(bb.center, bb.size + new Vector3(pad, pad, 0));

		expBalls = new List<GameObject>();

		expParticleSpawnCooldown = Random.Range(2.0f, 4.0f);

		paused = true;

		curCSIndex = 0;
		generateConsumeStatus();
		clearEffect();
	}
	
	private void generateConsumeStatus()
	{
		consumeStatusElapsedTime = 0;
		consumeStatusList = new List<ConsumeStatus>();
		List<int> randomIndex = new List<int>();

		// Generate random consume status/event
		for (int i = 0; i < 30; i++)
		{
			ConsumeStatus cs = new ConsumeStatus();
			cs.broken = false;
			cs.rageMode = false;
			cs.duration = Random.Range(5, 7);
			cs.consumeRateMod = Random.Range(-0.15f, 0.15f);

			consumeStatusList.Add(cs);

			if(i > 0)
			{
				randomIndex.Add(i);
			}
		}

		int count = randomIndex.Count;
		int last = count - 1;
		for(int i = 0; i<last; ++i)
		{
			int r = Random.Range(i, count);
			int temp = randomIndex[i];
			randomIndex[i] = randomIndex[r];
			randomIndex[r] = temp;
		}

		// add 2~4 broken event
		int numBrokenEvent = Random.Range(2, 4);
		for(int i = 0; i < numBrokenEvent; i++)
		{
			consumeStatusList[randomIndex[i]].broken = true;
		}

		// add 1~3 rage mode event
		int numRangeEvent = Random.Range(1, 3);
		for(int i = numBrokenEvent; i < numBrokenEvent + numRangeEvent; i++)
		{
			consumeStatusList[randomIndex[i]].rageMode = true;
			consumeStatusList[randomIndex[i]].duration -= 3.0f;
		}

		if(id == 3)
		{
			/*
			Debug.Log("Printing consume status list for gen#4");
			for(int i = 0; i < 30; i++)
			{
				Debug.Log("#" + i.ToString() + ", d = " + consumeStatusList[i].duration.ToString() + ", m = " + consumeStatusList[i].consumeRateMod.ToString() + ", b = " + consumeStatusList[i].broken.ToString() + ", r = " + consumeStatusList[i].rageMode.ToString());
			}
			*/
			//consumeStatusList[0].broken = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if(paused)
		{
			return;
		}

		bool broken = consumeStatusList[curCSIndex].broken;
		bool raged = consumeStatusList[curCSIndex].rageMode;

		if (coreInstalled)
		{
			// power core installed
			if (broken || raged)
			{
				// if it's broken, consume power instead of generating
				consumePower(broken, raged);
			}
			else
			{
				// generate power only if core is installed, not broken and not raged
				generatePower();
			}
		}
		else
		{
			// consume power if powercore is not installed
			consumePower(broken, raged);
		}

		if (isWorking())
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > expParticleSpawnCooldown)
			{
				elapsedTime -= expParticleSpawnCooldown;
				// spawn. 
				int rand = Random.Range(0, 100);

				int bonusChance = coreInstalled ? 3 : 0;

				if (rand < bonusChance + megaExpParticleSpawnChance)
				{
					//spawn mega exp ball
					spawnExpBall(true);
				}
				else if (rand < bonusChance + expParticleSpawnChance)
				{
					// spawn regular exp ball
					spawnExpBall(false);
				}
			}
		}

		for (int i = expBalls.Count - 1; i > -1; i--)
		{
			if (expBalls[i].GetComponent<ExpBall>().needToDestory())
			{
				Destroy(expBalls[i]);
				expBalls.RemoveAt(i);
			}
		}
	}

	private void updatePowerBar()
	{
		powerBar.fillAmount = powerStored / maxPower;
	}

	public bool hasCore()
	{
		return coreInstalled;
	}

	public void insertCore()
	{
		coreInstalled = true;
		//Debug.Log("Core instered");

		// inserting core to raged machine gets instantly resolved
		if(consumeStatusList[curCSIndex].rageMode)
		{
			updateConsumeSTatus(true);
			generatingEffect();
			//Debug.Log("PG raged. Resolved and make green");
		}
		else 
		{
			if(consumeStatusList[curCSIndex].broken)
			{
				// if core is inserted in broken machine, do nothing
				//Debug.Log("PG broken. do nothing");
				return;
			}
			else
			{
				// not broken, not raged
				generatingEffect();
				//Debug.Log("PG not raged, not borken. Set green");
			}
		}
	}

	public void removeCore()
	{
		coreInstalled = false;
		if(consumeStatusList[curCSIndex].broken)
		{
			// if power gen is broken, do nothing and keep purple
			brokenEffect();
			return;
		}
		else
		{
			clearEffect();
		}
	}

	public bool isInteracting(Vector3 pos)
	{
		return interactionBB.Contains(pos);
	}

	public void updateSpriteOrder()
	{
		int order = LD39.Utility.Math.yPosToSortingOrder(transform.parent.transform.position.y); ;
		GetComponent<SpriteRenderer>().sortingOrder = order;

		glass.GetComponent<SpriteRenderer>().sortingOrder = order + 2;
		barBG.GetComponent<SpriteRenderer>().sortingOrder = order - 2;

		barCanvas.sortingOrder = order - 1;
	}

	public int getCoreSpriteOrder()
	{
		return GetComponent<SpriteRenderer>().sortingOrder + 1;
	}

	public bool isWorking()
	{
		return coreInstalled || (powerStored > 0);
	}

	public void spawnExpBall(bool mega)
	{
		if(expBalls.Count < LD39.Constants.maxExpBallPerPowerGen)
		{
			GameObject expBall = Instantiate(Resources.Load("prefabs/expParticle")) as GameObject;
			expBall.GetComponent<ExpBall>().init(mega ? LD39.Constants.megaExpBallExpAmount : LD39.Constants.expBallExpAmount, mega, transform.parent.transform.position);

			expBalls.Add(expBall);

			expParticleSpawnCooldown = Random.Range(2.0f, 4.0f);
		}
	}

	public void collectExpBalls(Vector3 pos)
	{
		if(expBalls.Count > 0)
		{
			foreach(GameObject expBall in expBalls)
			{
				if(Mathf.Abs(Vector3.Distance(expBall.transform.position, pos)) < 40.0f)
				{
					ExpBall ball = expBall.GetComponent<ExpBall>();
					ball.collect();
					SoundManager.getInstance().play("collect");
				}
			}
		}
	}

	public Vector3 getPowerCorePos()
	{
		return transform.position;
	}

	public void pause()
	{
		paused = true;
	}

	public void resume()
	{
		paused = false;
	}

	private void generatePower()
	{
		float powerCoreBoost = ((float)player.getPowerCoreLevel()) * 0.05f;
		powerStored += Time.deltaTime * genRate + powerCoreBoost;

		if (powerStored > maxPower)
		{
			powerStored = maxPower;
		}

		updatePowerBar();
	}

	private void consumePower(bool broken, bool raged)
	{
		if (powerStored > 0)
		{
			float powerEfficiencyLevel = (float)player.getPowerEfficiencyLevel() * 0.05f;

			float statusMod = 1.0f;

			if (raged) statusMod = 15.0f;
			else if (broken) statusMod = 3.0f;

			powerStored -= Time.deltaTime * (consumeRate - powerEfficiencyLevel + consumeStatusList[curCSIndex].consumeRateMod) * statusMod;
			updatePowerBar();

			updateConsumeSTatus(false);
		}

		// Check if power is all consumed. 
		if (powerStored <= 0)
		{
			powerStored = 0;
			if (raged)
			{
				// End rage mode if there is no power
				updateConsumeSTatus(true);
			}
			else if (broken)
			{
				// pause consume status if it's broken
				return;
			}
		}
	}

	private void updateConsumeSTatus(bool forceUpdate)
	{
		if(forceUpdate)
		{
			consumeStatusElapsedTime = 0;
			curCSIndex++;
			if (curCSIndex >= 30)
			{
				curCSIndex = 0;
				generateConsumeStatus();
			}
		}
		else
		{
			if(!consumeStatusList[curCSIndex].broken)
			{
				// if machine is broken, need to fix. stop update
				consumeStatusElapsedTime += Time.deltaTime;
			}

			if (consumeStatusElapsedTime >= consumeStatusList[curCSIndex].duration)
			{
				consumeStatusElapsedTime -= consumeStatusList[curCSIndex].duration;
				curCSIndex++;
				if (curCSIndex >= 30)
				{
					curCSIndex = 0;
					generateConsumeStatus();
				}

				if (consumeStatusList[curCSIndex].broken)
				{
					// borken effect
					brokenEffect();
					//Debug.Log("Generator #" + id.ToString() + " is broken");
				}
				else if (consumeStatusList[curCSIndex].rageMode)
				{
					// rage effect
					rageEffect();
					//Debug.Log("Generator #" + id.ToString() + " is raged");
				}
				else
				{
					if (hasCore())
					{
						generatingEffect();
						//Debug.Log("Generator #" + id.ToString() + " is generating");
					}
					else
					{
						clearEffect();
						//Debug.Log("Generator #" + id.ToString() + " is consuming");
					}
				}
			}
		}
	}

	private void brokenEffect()
	{
		powerBar.color = purple;
		GetComponent<SpriteRenderer>().color = brokenColor;
	}

	private void clearEffect()
	{
		powerBar.color = yellow;
		GetComponent < SpriteRenderer>().color = Color.white;
	}

	private void generatingEffect()
	{
		powerBar.color = green;
		GetComponent<SpriteRenderer>().color = Color.white;
	}

	private void rageEffect()
	{
		powerBar.color = red;
		GetComponent<SpriteRenderer>().color = rageColor;
	}

	public bool isBroken()
	{
		return consumeStatusList[curCSIndex].broken;
	}

	public Vector3 getRepairLabelPos()
	{
		return transform.position + new Vector3(0, 40.0f, 0);
	}

	// returns true if all repaired
	public bool repair()
	{
		repairBar.attempRepair();

		repairBar.show();

		bool repaired = repairBar.allFixed();

		if(repaired)
		{
			if (hasCore())
				generatingEffect();
			else
				clearEffect();
			
			updateConsumeSTatus(true);
			Debug.Log("Gen #" + id.ToString() + " is repaired.");

			repairBar.hideAndReset();
		}

		return repaired;
	}
}
