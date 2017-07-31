using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	public GameObject playerGO;
	public GameObject powerCoreGO;

	public GameObject quitArea;

	public GameObject consoleGO;
	public GameObject consoleUI;
	private Bounds consoleBB;

	public GameObject coreHolder;
	private Bounds coreHolderBound;

	private BoxCollider2D playerBB;
	//private SpriteRenderer powerCoreSR;

	private Player player;
	private PowerCore powerCore;

	private bool gameRunning;
	private bool fadeOut;
	private bool showingHowToPlay;
	
	public PowerGenerator[] powerGenerators;

	public GameObject floor;
	private Bounds floorBB;

	public GameObject propGO;

	private List<BoxCollider2D> staticBBs;

	public Tooltip tooltip;

	private float expGenerated;

	private float curAlertLevel;
	private float maxAlertLevel;
	public GameObject alertEffect;
	public Image alertBar;
	public Image gameOverEffect;

	public GameObject menuPanel;

	public Image howToPlay;

	// Use this for initialization
	void Start ()
	{
		showingHowToPlay = true;
		gameRunning = false;
		fadeOut = false;

		playerBB = playerGO.GetComponent<BoxCollider2D>();
		//powerCoreSR = powerCoreGO.GetComponent<SpriteRenderer>();

		player = playerGO.GetComponent<Player>();
		powerCore = powerCoreGO.GetComponent<PowerCore>();

		floorBB = floor.GetComponent<BoxCollider2D>().bounds;

		staticBBs = new List<BoxCollider2D>();

		for (int i = 0; i < LD39.Constants.TOTAL_POWER_GENERATORS; i++)
		{
			staticBBs.Add(powerGenerators[i].GetComponent<BoxCollider2D>());
		}

		foreach (Transform child in propGO.transform)
		{
			staticBBs.Add(child.GetComponent<BoxCollider2D>());
		}
		staticBBs.Add(consoleGO.GetComponent<BoxCollider2D>());
		staticBBs.Add(coreHolder.GetComponent<BoxCollider2D>());

		//Debug.Log("Total " + staticBBs.Count.ToString() + " (4 pg + props) static BB found");

		updatePowerGenSpriteOrder();

		expGenerated = 0;

		ExpBall.setPlayer(playerGO);

		coreHolderBound = coreHolder.GetComponent<BoxCollider2D>().bounds;

		float pad = 30.0f;
		Vector3 min = coreHolderBound.min;
		Vector3 max = coreHolderBound.max;

		min.x -= pad;
		min.y -= pad;

		max.x += pad;
		max.y += pad;

		coreHolderBound.SetMinMax(min, max);

		consoleBB = consoleGO.GetComponent<BoxCollider2D>().bounds;

		Vector3 consoleMin = consoleBB.min;
		//consoleMin.x -= pad;
		consoleMin.y -= pad;
		consoleBB.SetMinMax(consoleMin, consoleBB.max);

		consoleUI.GetComponent<Console>().init();

		curAlertLevel = 0;
		maxAlertLevel = 100;
		alertBar.fillAmount = 0;

		gameOverEffect.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!showingHowToPlay)
		{
			if (fadeOut)
			{
				Color fadeColor = Color.Lerp(gameOverEffect.GetComponent<Image>().color, Color.black, 0.1f);
				gameOverEffect.GetComponent<Image>().color = fadeColor;

				if (fadeColor == Color.black)
				{
					UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
				}
			}

			if (!gameRunning)
			{
				Color c = gameOverEffect.GetComponent<Image>().color;
				c.a += Time.deltaTime * 0.5f;

				if (c.a >= 1.0f)
				{
					c.a = 1.0f;
					fadeOut = true;
				}

				gameOverEffect.GetComponent<Image>().color = c;

				return;
			}
		}
		/*
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			player.giveExp(100);
		}

		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			OnPowerCoreUpgradeButtonClick();
		}
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			OnPowerEfficiencyUpgradeButtonClick();
		}
		if (Input.GetKeyDown(KeyCode.Keypad3))
		{
			OnMovementSpeedUpgradeButtonClick();
		}
		*/
		// Check if player pressed escape. if so, ignore all update
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(showingHowToPlay)
			{
				startGame();
			}
			else
			{
				if (openingConsole())
				{
					// close console
					closeConsole();
				}
				else if (menuPanel.activeSelf)
				{
					closeMenu();
					return;
				}
				else
				{
					// bring menu
					openMenu();
					return;
				}
			}
		}

		if(showingHowToPlay)
		{
			return;
		}

		// Block any input while console is opened except ESC
		if (openingConsole() || menuPanel.activeSelf)
		{
			return;
		}

		if(quitArea.GetComponent<BoxCollider2D>().bounds.Intersects(playerBB.bounds))
		{
			if(!tooltip.getQuitLabelVisibility())
			{
				tooltip.toggleQuitLabel(true);
			}
		}
		else
		{
			if (tooltip.getQuitLabelVisibility())
			{
				tooltip.toggleQuitLabel(false);
			}
		}


		// update player movement
		bool moved = updatePlayerMovement();

		player.playAnimation(moved);

		// Check if player is out of boundary. If so, move back in to boundary
		updateFloorBoundary();

		// update player collision with static objects
		updateStaticCollisionWithPlayer();

		// apply final next position
		player.applyNextPosition();
		player.updateSpriteOrder();

		if(player.isGrabbingPowerCore())
		{
			powerCore.updateSpriteOrder(player.getBodySpriteOrder());
		}

		// collect exp
		if(player.isExpFull() == false)
		{
			collectExpBalls();
		}

		// check power gen
		int numWorking = countWorkingPowerGen();

		if(numWorking > 0)
		{
			// give player exp point

			float multiplier = (float)numWorking;
			multiplier *= LD39.Constants.powerGenExpWorth;

			expGenerated += LD39.Constants.expTick * multiplier * Time.deltaTime;

			if(expGenerated > 1.0f)
			{
				int amount = (int)expGenerated;

				expGenerated = expGenerated - Mathf.Floor(expGenerated);

				player.giveExp(amount);
			}
		}

		if (numWorking <= LD39.Constants.TOTAL_POWER_GENERATORS)
		{
			// something is out of power. increase alert level
			curAlertLevel += Time.deltaTime * LD39.Constants.alertTick * ((float)LD39.Constants.TOTAL_POWER_GENERATORS - numWorking);

			if (curAlertLevel >= maxAlertLevel)
			{
				gameOver();
			}

			alertBar.fillAmount = curAlertLevel / maxAlertLevel;
		}

		bool nearConsole = false;
		if (playerBB.bounds.Intersects(consoleBB))
		{
			nearConsole = true;
			if(!tooltip.getConsoleLabelVisibility())
			{
				tooltip.toggleConsoleLabel(true);
			}
		}
		else
		{
			if(tooltip.getConsoleLabelVisibility())
			{
				tooltip.toggleConsoleLabel(false);
			}
		}

		int powerGenIndex = powerGenCollisionCheck();
		if(powerGenIndex >= 0)
		{
			// First check if it's broken.
			if(powerGenerators[powerGenIndex].isBroken())
			{
				// it's broken. show repair tooltip
				if(!tooltip.getRepairLabelVisibility())
				{
					// not showing. show
					tooltip.toggleRepairLabel(true);
				}
				Vector3 powerCorePos = powerGenerators[powerGenIndex].getRepairLabelPos();
				tooltip.updateRepairLabelPos(powerCorePos);
			}

			// player interacting near power gen
			if(player.isGrabbingPowerCore())
			{
				// grabbing power core
				if (!tooltip.getInsertCoreLabelVisibility())
				{
					tooltip.toggleInserCoreLabel(true);
				}
				Vector3 powerCorePos = powerCore.getLabelUIPos();
				tooltip.updateInsertCoreLabelPos(powerCorePos);
			}
			else
			{
				// not grabbing power core
				if(powerGenerators[powerGenIndex].hasCore())
				{
					// power gen has core
					if (!tooltip.getTakeOutCoreLabelVisibility())
					{
						tooltip.toggleTakeOutCoreLabel(true);
					}
					Vector3 powerCorePos = powerGenerators[powerGenIndex].getPowerCorePos();
					tooltip.updateTakeOutCoreLabelPos(powerCorePos);
				}
				else
				{
					if (tooltip.getTakeOutCoreLabelVisibility())
					{
						tooltip.toggleTakeOutCoreLabel(false);
					}
				}
			}
		}
		else
		{
			if (tooltip.getRepairLabelVisibility())
			{
				// not showing. show
				tooltip.toggleRepairLabel(false);
			}

			if (player.isGrabbingPowerCore())
			{
				if (tooltip.getInsertCoreLabelVisibility())
				{
					tooltip.toggleInserCoreLabel(false);
				}
			}
			else
			{
				if (tooltip.getTakeOutCoreLabelVisibility())
				{
					tooltip.toggleTakeOutCoreLabel(false);
				}
			}
		}

		//updateCursorTooltip();

		if (Input.GetKeyDown(KeyCode.Space))
		{
			// Check if using console
			if(nearConsole)
			{
				// use console
				openConsole();
			}
			else
			{
				if (player.isGrabbingPowerCore())
				{
					// If player is holding power core, check if player wants to drop it or want to place on power generator
					if (powerGenIndex >= 0)
					{
						powerGenerators[powerGenIndex].insertCore();

						tooltip.toggleInserCoreLabel(false);

						player.releasePowerCore();
						powerCore.removeFromParent();
						powerCore.setParent(powerGenerators[powerGenIndex].transform);
						powerCore.updateSpriteOrder(powerGenerators[powerGenIndex].getCoreSpriteOrder());
						powerCore.startGeneratePower();
					}
					else
					{
						// didn't press space near power generator. drop power core
						player.releasePowerCore();
						powerCore.removeFromParent();
					}
				}
				else
				{
					// If player is not holding power core, check if player wants to pick up power core or interact with others
					if (powerGenIndex >= 0)
					{
						if (powerGenerators[powerGenIndex].GetComponent<PowerGenerator>().hasCore())
						{
							powerGenerators[powerGenIndex].GetComponent<PowerGenerator>().removeCore();
							tooltip.toggleTakeOutCoreLabel(false);
							player.grabPowerCore();
							powerCore.setParent(playerGO.transform);
							powerCore.setPosition(player.getCorePos());
							powerCore.updateSpriteOrder(player.getBodySpriteOrder());
							powerCore.stopGeneratingPower();
							return;
						}
					}

					// Not interactig with power generator
					// Check if it tries to grab core for the first time
					if (powerCore.isIdle())
					{
						if (coreHolderBound.Intersects(playerBB.bounds))
						{
							// grab power core for the first time
							powerCore.setIdle(false);
							powerCore.setPosition(Vector3.zero);
							player.grabPowerCore();
							powerCore.setParent(playerGO.transform);
							powerCore.setPosition(player.getCorePos());
							powerCore.updateSpriteOrder(player.getBodySpriteOrder());
						}
					}
					else
					{
						// power core is not idle

						if (playerBB.bounds.Intersects(powerCoreGO.GetComponent<BoxCollider2D>().bounds))
						{
							// Grab power core
							player.grabPowerCore();
							powerCore.setParent(playerGO.transform);
							powerCore.setPosition(player.getCorePos());
							powerCore.updateSpriteOrder(player.getBodySpriteOrder());
						}
					}
				}
			}
		}
		else if(Input.GetKeyDown(KeyCode.E))
		{
			if(powerGenIndex >= 0)
			{
				if (powerGenerators[powerGenIndex].isBroken())
				{
					// fix
					bool repaired = powerGenerators[powerGenIndex].repair();
					if(repaired)
					{
						tooltip.toggleRepairLabel(false);
					}
				}
			}
		}

		updateAlertEffectColor();
	}

	private bool updatePlayerMovement()
	{
		bool moved = false;

		if (Input.GetKey(KeyCode.W))
		{
			player.moveByKeyInput(KeyCode.W);
			moved = true;
		}

		if (Input.GetKey(KeyCode.S))
		{
			player.moveByKeyInput(KeyCode.S);
			moved = true;
		}

		if (Input.GetKey(KeyCode.A))
		{
			player.moveByKeyInput(KeyCode.A);
			moved = true;
		}

		if (Input.GetKey(KeyCode.D))
		{
			player.moveByKeyInput(KeyCode.D);
			moved = true;
		}

		return moved;
	}

	// Limit player's movement in to floor only
	private void updateFloorBoundary()
	{
		Vector3 nextPos = player.getNextPosition();

		if(!floorBB.Contains(nextPos))
		{
			float px = nextPos.x;
			float py = nextPos.y;

			if(px < floorBB.min.x)
			{
				px = floorBB.min.x;
				player.modifyNextPosX(px);
			}
			else if(px > floorBB.max.x)
			{
				px = floorBB.max.x;
				player.modifyNextPosX(px);
			}

			if (py < floorBB.min.y)
			{
				py = floorBB.min.y;
				player.modifyNextPosY(py);
			}
			else if (py > floorBB.max.y)
			{
				py = floorBB.max.y;
				player.modifyNextPosY(py);
			}
		}
	}

	// Check collision between player and power generations
	private void updateStaticCollisionWithPlayer()
	{
		// Trick here, iterate static objects twice in terms of x axis and y axis.
		// By doing this, we can avoid corner case and wrong collision resolution.
		Vector3 resolvingPos = playerGO.transform.position;
		Vector3 nextPos = player.getNextPosition();
		Vector3 movedDist = player.getMovedDistance();

		bool resolved = false;

		for(int i = 0; i< 2; i++)
		{
			Bounds nextPlayerBB = playerBB.bounds;

			if (i == 0)
			{
				nextPlayerBB.center = new Vector3(nextPlayerBB.center.x + movedDist.x, nextPlayerBB.center.y, 0);
				resolvingPos.x = nextPos.x;
			}
			else
			{
				nextPlayerBB.center = new Vector3(nextPlayerBB.center.x, nextPlayerBB.center.y + movedDist.y, 0);
				resolvingPos.y = nextPos.y;
			}

			foreach (BoxCollider2D collider in staticBBs)
			{
				// get new bound
				//nextPlayerBB.center = resolvingPos;

				if (nextPlayerBB.Intersects(collider.bounds))
				{
					Bounds intersectingArea = LD39.Utility.Math.getIntersectingBounds(nextPlayerBB, collider.bounds);

					//Debug.Log("PlayerBB = " + playerBB.bounds.ToString());
					//Debug.Log("StaticBB = " + collider.bounds.ToString());
					//Debug.Log("Intersecting  = " + intersectingArea.ToString());

					if (i == 0)
					{
						// X axis. Resolve collision only with x axis
						// Check if player tried to move left or right.
						float movedX = player.getMovedDistance().x;

						if(movedX > 0.0f)
						{
							resolvingPos.x -= intersectingArea.size.x + 0.05f;
							resolved = true;
							//player.modifyNextPosX(resolvingPos.x);
						}
						else if(movedX <  0.0f)
						{
							resolvingPos.x += intersectingArea.size.x + 0.05f;
							resolved = true;
							//player.modifyNextPosX(resolvingPos.x);
						}
					}
					else
					{
						// Y axis
						// Check if player tried to move top or down
						float movedY = player.getMovedDistance().y;

						if(movedY > 0.0f)
						{
							resolvingPos.y -= intersectingArea.size.y + 0.05f;
							resolved = true;
							//player.modifyNextPosY(resolvingPos.y);
						}
						else if(movedY < 0.0f)
						{
							resolvingPos.y += intersectingArea.size.y + 0.05f;
							resolved = true;
							//player.modifyNextPosY(resolvingPos.y);
						}
					}
				}
			}
		}

		if(resolved)
		{
			player.setNextPos(resolvingPos);
		}
	}

	private int powerGenCollisionCheck()
	{
		for(int i = 0; i < LD39.Constants.TOTAL_POWER_GENERATORS; i++)
		{
			if(powerGenerators[i].isInteracting(player.transform.position))
			{
				//Debug.Log("Interacting with PG#" + i.ToString());
				return i;
			}
		}

		return -1;
	}

	private void updatePowerGenSpriteOrder()
	{
		for (int i = 0; i < LD39.Constants.TOTAL_POWER_GENERATORS; i++)
		{
			powerGenerators[i].updateSpriteOrder();
		}
	}

	private int countWorkingPowerGen()
	{
		int c = 0;

		for (int i = 0; i < LD39.Constants.TOTAL_POWER_GENERATORS; i++)
		{
			if (powerGenerators[i].isWorking())
			{
				c++;
			}
		}

		return c;
	}

	private void collectExpBalls()
	{
		for (int i = 0; i < LD39.Constants.TOTAL_POWER_GENERATORS; i++)
		{
			powerGenerators[i].collectExpBalls(playerGO.transform.position);
		}
	}

	/*
	private void updateCursorTooltip()
	{
		CursorManager cm = CursorManager.getInstance();

		if(console.GetComponent<SpriteRenderer>().bounds.Contains(CursorManager.getInstance().getMouseScreenPos()))
		{
			cm.setToolTip(CursorManager.TOOLTIP.CONSOLE);
		}
	}
	*/

	public void openConsole()
	{
		int pcLevel = player.getPowerCoreLevel();
		int peLevel = player.getPowerEfficiencyLevel();
		int msLevel = player.getMovementSpeedLevel();

		bool online = player.isExpFull() ? true : false;

		consoleUI.GetComponent<Console>().refresh(pcLevel, peLevel, msLevel, online);
	}

	public bool openingConsole()
	{
		return consoleUI.GetComponent<Console>().isOpened();
	}

	public void closeConsole()
	{
		consoleUI.GetComponent<Console>().close();
	}

	public void OnPowerCoreUpgradeButtonClick()
	{
		player.levelUpPowerCore();
		consoleUI.GetComponent<Console>().updatePowerCoreUpgradeLevel(player.getPowerCoreLevel());
		player.useExp();
		consoleUI.GetComponent<Console>().disableButtons();
		consoleUI.GetComponent<Console>().switchToOffline();
		consoleUI.GetComponent<Console>().toggleBarColor(false);
		SoundManager.getInstance().play("upgrade");
	}

	public void OnPowerEfficiencyUpgradeButtonClick()
	{
		player.levelUpPowerEfficiency();
		consoleUI.GetComponent<Console>().updatePowerEfficiencyUpgradeLevel(player.getPowerEfficiencyLevel());
		player.useExp();
		consoleUI.GetComponent<Console>().disableButtons();
		consoleUI.GetComponent<Console>().switchToOffline();
		consoleUI.GetComponent<Console>().toggleBarColor(false);
		SoundManager.getInstance().play("upgrade");
	}

	public void OnMovementSpeedUpgradeButtonClick()
	{
		player.levelUpMovementSpeed();
		consoleUI.GetComponent<Console>().updateMovementSpeedUpgradeLevel(player.getMovementSpeedLevel());
		player.useExp();
		consoleUI.GetComponent<Console>().disableButtons();
		consoleUI.GetComponent<Console>().switchToOffline();
		consoleUI.GetComponent<Console>().toggleBarColor(false);
		SoundManager.getInstance().play("upgrade");
	}

	public void gameOver()
	{
		gameRunning = false;
		consoleUI.GetComponent<Console>().close();
	}

	public void startGame()
	{
		foreach(PowerGenerator powerGen in powerGenerators)
		{
			powerGen.resume();
		}
		howToPlay.gameObject.SetActive(false);
		SoundManager.getInstance().play("general");
		gameRunning = true;
		showingHowToPlay = false;
	}

	public void updateAlertEffectColor()
	{
		if(curAlertLevel < 10)
		{
			return;
		}

		Color curColor = alertEffect.GetComponent<SpriteRenderer>().color;

		float newAlpha = (LD39.Constants.maxAlertAlhpa) / maxAlertLevel * curAlertLevel / 256.0f;

		float variation = ((float)Mathf.Sin(Time.time * 5.0f) * 0.05f);

		curColor.a = newAlpha + variation;

		alertEffect.GetComponent<SpriteRenderer>().color = curColor;
	}

	public void openMenu()
	{
		menuPanel.SetActive(true);

		foreach(PowerGenerator pg in powerGenerators)
		{
			pg.pause();
		}
	}

	public void closeMenu()
	{
		menuPanel.SetActive(false);

		foreach (PowerGenerator pg in powerGenerators)
		{
			pg.resume();
		}
	}

	public void OnResumeClick()
	{
		menuPanel.SetActive(false);
	}

	public void OnExitClick()
	{
		Application.Quit();
	}

	public void OnBackToMenuClick()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
	}
}
