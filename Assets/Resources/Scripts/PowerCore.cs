using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCore : MonoBehaviour
{
	private bool generatingPower;
	private bool idle;

	private float originX;
	private float originY;
	private float idleOriginY;

	public float floatingSpeed;
	public float floatingPower;

	// Use this for initialization
	void Start ()
	{
		generatingPower = false;
		originX = 4.3f;
		originY = -20.0f;
		idleOriginY = -25.0f;
		idle = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(generatingPower)
		{
			transform.localPosition = new Vector3(originX,
										 originY + ((float)Mathf.Sin(Time.time * floatingSpeed) * floatingPower),
										 0);
		}
		else if(idle)
		{
			transform.localPosition = new Vector3(transform.localPosition.x,
										 idleOriginY + ((float)Mathf.Sin(Time.time * floatingSpeed) * floatingPower),
										 0);
		}
	}

	public void setParent(Transform parent)
	{
		transform.SetParent(parent);
		//transform.localPosition = Vector3.zero;
	}

	public void removeFromParent()
	{
		transform.SetParent(null);
		//transform.position = Vector3.zero;
		//transform.localPosition = Vector3.zero;
	}

	public void startGeneratePower()
	{
		generatingPower = true;
		transform.localPosition = new Vector3(0, originY, 0);
	}

	public void stopGeneratingPower()
	{
		generatingPower = false;
	}

	public void updateSpriteOrder(int order)
	{
		GetComponent<SpriteRenderer>().sortingOrder = order;
	}

	public void setPosition(Vector3 pos)
	{
		transform.localPosition = pos;
	}

	public bool isIdle()
	{
		return idle;
	}

	public void setIdle(bool mode)
	{
		idle = mode;
	}

	public Vector3 getLabelUIPos()
	{
		return transform.position + new Vector3(-20, 20, 0);
	}
}
