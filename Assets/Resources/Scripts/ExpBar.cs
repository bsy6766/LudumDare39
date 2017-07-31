using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour {
	private float curExp;
	private float targetExp;
	private float maxExp = 100.0f;

	public Image bar;
	public Text label;

	public Color defaultColor;
	public Color pulseColor;

	// Use this for initialization
	void Start ()
	{
		curExp = 0;
		label.text = "0 / 100";
		resetExp();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(curExp < targetExp || curExp > targetExp)
		{
			//Debug.Log("Target exp = " + targetExp.ToString());
			//Debug.Log("cur exp = " + curExp.ToString());
			curExp = Mathf.Lerp(curExp, targetExp, 0.3f);
			//Debug.Log("cur exp updated = " + curExp.ToString());
			if(Mathf.Abs(curExp - targetExp) < 0.05f)
			{
				curExp = targetExp;
				//Debug.Log("bar update finished");
			}

			float percentage = curExp / maxExp;
			//Debug.Log("percentage = " + percentage.ToString());
			bar.fillAmount = percentage;

			percentage *= 100.0f;

			int amount = (int)percentage;
			//Debug.Log("Amount = " + amount.ToString());
			label.text = amount.ToString() + " / 100";
		}

		if(curExp >= maxExp)
		{
			float ratio = Mathf.Abs(Mathf.Sin(Time.time * 5.0f));
			
			Color mixed = Color.Lerp(defaultColor, pulseColor, ratio);
			bar.GetComponent<Image>().color = mixed;
		}
	}

	public void addExp(int amount)
	{
		if(targetExp < maxExp)
		{
			targetExp += (float)amount;
			if (targetExp > maxExp)
			{
				targetExp = maxExp;
			}
		}
	}

	public void resetExp()
	{
		bar.fillAmount = curExp / maxExp;
		targetExp = 0;
		bar.color = defaultColor;
	}
}
