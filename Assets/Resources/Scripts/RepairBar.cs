using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairBar : MonoBehaviour {
	public Canvas canvas;
	public Image bar;
	private float curRepairedAmount;
	private float maxRepairAmount;

	// Use this for initialization
	void Awake () {
		curRepairedAmount = 0;
		maxRepairAmount = 100;

		bar.fillAmount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void attempRepair()
	{
		curRepairedAmount += Random.Range(30.0f, 50.0f);
		if(curRepairedAmount > maxRepairAmount)
		{
			curRepairedAmount = maxRepairAmount;
		}

		Debug.Log("curRepair = " + curRepairedAmount.ToString());
		bar.fillAmount = curRepairedAmount / maxRepairAmount;
	}

	public bool allFixed()
	{
		return curRepairedAmount == maxRepairAmount;
	}

	public void show()
	{
		canvas.gameObject.SetActive(true);
		gameObject.SetActive(true);
	}

	public void hideAndReset()
	{
		canvas.gameObject.SetActive(false);
		gameObject.SetActive(false);
		curRepairedAmount = 0;
		bar.fillAmount = 0;
	}
}
