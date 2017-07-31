using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBall : MonoBehaviour {
	private int expAmount;
	private Vector3 scaleTarget;

	private static GameObject player = null;

	public enum STATE
	{
		READY,
		SPAWNING,
		SPAWNED,
		COLLECTING,
		DESTROY
	}

	STATE curState;

	private void Awake()
	{
		curState = STATE.READY;
		transform.localScale = new Vector3(0, 0, 1);
		scaleTarget = new Vector3(0, 0, 1);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(curState == STATE.SPAWNING)
		{
			Vector3 newScale = Vector3.Lerp(transform.localScale, scaleTarget, 0.2f);
			transform.localScale = newScale;

			if(Mathf.Abs(scaleTarget.x - transform.localScale.x) < 0.05f)
			{
				transform.localScale = scaleTarget;
				curState = STATE.SPAWNED;
			}
		}
		else if(curState == STATE.SPAWNED)
		{
			transform.position = new Vector3(transform.position.x,
										 transform.position.y + ((float)Mathf.Sin(Time.time * 5.0f)) * 0.25f,
										 0);
		}
		else if(curState == STATE.COLLECTING)
		{
			Vector3 newPos = Vector3.Lerp(transform.position, player.transform.position + new Vector3(0, 30.0f, 0), 0.2f);
			Vector3 newScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);

			transform.position = newPos;
			transform.localScale = newScale;

			float diff = Mathf.Abs(newScale.x);

			if (diff < 0.01f)
			{
				curState = STATE.DESTROY;
				player.GetComponent<Player>().giveExp(expAmount);
			}
		}
		else if(curState == STATE.DESTROY)
		{
		}
	}

	public void init(int exp, bool mega, Vector3 origin)
	{
		expAmount = exp;
		scaleTarget = mega ? new Vector3(1, 1, 1) : new Vector3(0.6f, 0.6f, 1);
		curState = STATE.SPAWNING;

		Vector3 pos = new Vector3(origin.x + 50.0f, origin.y, 0);
		transform.position = pos;

		transform.RotateAround(origin, Vector3.forward, Random.Range(190, 350));

		GetComponent<SpriteRenderer>().sortingOrder = LD39.Utility.Math.yPosToSortingOrder(transform.position.y);
	}

	public static void setPlayer(GameObject playerObj)
	{
		player = playerObj;
	}

	public void collect()
	{
		curState = STATE.COLLECTING;
	}

	public bool needToDestory()
	{
		return curState == STATE.DESTROY;
	}
}
