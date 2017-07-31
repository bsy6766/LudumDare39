using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	private bool grabbingPowerCore;
	public float movementSpeed;
	public float movementSpeedBoost;

	private int exp;
	private int maxExp;
	public ExpBar expBar;

	public Anima2D.SpriteMeshInstance body;
	public Anima2D.SpriteMeshInstance head;
	public Anima2D.SpriteMeshInstance leg;

	private enum ANIM_FLIP
	{
		LEFT,
		RIGHT
	}

	ANIM_FLIP animFlip = ANIM_FLIP.LEFT;

	private Animator animator;

	private Vector3 nextPosition;

	public Anima2D.Bone2D armBone;

	private int powerCoreLevel;
	private int powerEfficiencyLevel;
	private int movementSpeedLevel;

	private void Awake()
	{
		Screen.SetResolution(960, 540, false);
	}

	// Use this for initialization
	void Start ()
	{
		grabbingPowerCore = false;
		nextPosition = transform.position;
		exp = 0;
		maxExp = 100;

		animator = GetComponent<Animator>();

		powerCoreLevel = 0;
		powerEfficiencyLevel = 0;
		movementSpeedLevel = 0;

		movementSpeedBoost = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void grabPowerCore()
	{
		grabbingPowerCore = true;
		movementSpeed *= 0.75f;
	}

	public void releasePowerCore()
	{
		grabbingPowerCore = false;
		movementSpeed = 200.0f;
	}

	public bool isGrabbingPowerCore()
	{
		return grabbingPowerCore;
	}

	public void moveByKeyInput(KeyCode key)
	{
		// Update movement by keyobard input
		float speed = Time.deltaTime * (movementSpeed + movementSpeedBoost);

		switch (key)
		{
			case KeyCode.W:
				nextPosition += Vector3.up * speed;
				break;
			case KeyCode.S:
				nextPosition += Vector3.down * speed;
				break;
			case KeyCode.A:
				nextPosition += Vector3.left * speed;
				if (animFlip == ANIM_FLIP.RIGHT)
				{
					animFlip = ANIM_FLIP.LEFT;
					transform.localScale = new Vector3(1, 1, 1);
				}
				break;
			case KeyCode.D:
				nextPosition += Vector3.right * speed;
				if (animFlip == ANIM_FLIP.LEFT)
				{
					animFlip = ANIM_FLIP.RIGHT;
					transform.localScale = new Vector3(-1, 1, 1);
				}
				break;
			default:
				break;
		}
	}

	public Vector3 getNextPosition()
	{
		return nextPosition;
	}

	public Vector3 getMovedDistance()
	{
		return nextPosition - transform.position;
	}

	public void modifyNextPosX(float x)
	{
		nextPosition.x = x;
	}

	public void modifyNextPosY(float y)
	{
		nextPosition.y = y;
	}

	public void applyNextPosition()
	{
		transform.position = nextPosition;
	}

	public void setNextPos(Vector3 nextPos)
	{
		nextPosition = nextPos;
	}

	public void updateSpriteOrder()
	{
		//GetComponent<SpriteRenderer>().sortingOrder = LD39.Utility.Math.yPosToSortingOrder(transform.position.y);
		body.sortingOrder = LD39.Utility.Math.yPosToSortingOrder(transform.position.y);
		head.sortingOrder = LD39.Utility.Math.yPosToSortingOrder(transform.position.y) + 1;
		leg.sortingOrder = LD39.Utility.Math.yPosToSortingOrder(transform.position.y) - 1;
	}

	public int getBodySpriteOrder()
	{
		return body.sortingOrder;
	}

	public Transform getArmBoneTransform()
	{
		return armBone.transform;
	}

	public Vector3 getCorePos()
	{
		return new Vector3(-20.0f, 7.0f, 0);
	}

	public void giveExp(int newExp)
	{
		if(exp < maxExp)
		{
			exp += newExp;
			if(exp > maxExp)
			{
				exp = maxExp;
			}

			expBar.addExp(newExp);
		}
	}

	public bool isExpFull()
	{
		return exp == maxExp;
	}

	public void playAnimation(bool moved)
	{
		if(moved)
		{
			// play walk anim only if not playing yet
			if(animator.GetBool("walk") == false)
			{
				animator.SetBool("idle", false);
				animator.SetBool("walk", true);
			}
		}
		else
		{
			if(animator.GetBool("idle") == false)
			{
				animator.SetBool("idle", true);
				animator.SetBool("walk", false);
			}
		}
	}

	public void levelUpPowerCore()
	{
		if(powerCoreLevel < 10)
		{
			powerCoreLevel++;
		}
	}

	public void levelUpPowerEfficiency()
	{
		if(powerEfficiencyLevel <10)
		{
			powerEfficiencyLevel++;
		}
	}

	public void levelUpMovementSpeed()
	{
		if(movementSpeedLevel < 10)
		{
			movementSpeedLevel++;
			movementSpeedBoost += 20.0f;
		}
	}

	public int getPowerCoreLevel()
	{
		return powerCoreLevel;
	}

	public int getPowerEfficiencyLevel()
	{
		return powerEfficiencyLevel;
	}

	public int getMovementSpeedLevel()
	{
		return movementSpeedLevel;
	}

	public void useExp()
	{
		exp = 0;
		expBar.resetExp();
	}
}
