using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public float movementSpeed;

	public GameObject targetGO;
	private Transform target;

	// Use this for initialization
	void Start ()
	{
		target = targetGO.transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update movement by keyobard input

		Vector3 curPos = target.position;

		if (Input.GetKey(KeyCode.W))
		{
			curPos += Vector3.up * Time.deltaTime * movementSpeed;
		}

		if (Input.GetKey(KeyCode.S))
		{
			curPos += Vector3.down * Time.deltaTime * movementSpeed;
		}

		if (Input.GetKey(KeyCode.A))
		{
			curPos += Vector3.left * Time.deltaTime * movementSpeed;
		}

		if (Input.GetKey(KeyCode.D))
		{
			curPos += Vector3.right * Time.deltaTime * movementSpeed;
		}

		int x = (int) curPos.x;
		int y = (int) curPos.y;

		target.position = new Vector3(x, y, curPos.z);
	}
}
