using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

	public GameObject consoleLabel;
	public GameObject takeOutCoreLabel;
	public GameObject insertCoreLabel;
	public GameObject quitLabel;
	public GameObject repairLabel;
	
	public void toggleConsoleLabel(bool mode)
	{
		consoleLabel.SetActive(mode);
	}

	public bool getConsoleLabelVisibility()
	{
		return consoleLabel.activeSelf;
	}

	public void toggleInserCoreLabel(bool mode)
	{
		insertCoreLabel.SetActive(mode);
	}

	public void updateInsertCoreLabelPos(Vector3 pos)
	{
		insertCoreLabel.transform.localPosition = pos;
	}

	public void updateTakeOutCoreLabelPos(Vector3 pos)
	{
		takeOutCoreLabel.transform.localPosition = pos;
	}

	public bool getInsertCoreLabelVisibility()
	{
		return insertCoreLabel.activeSelf;
	}

	public void toggleTakeOutCoreLabel(bool mode)
	{
		takeOutCoreLabel.SetActive(mode);
	}

	public bool getTakeOutCoreLabelVisibility()
	{
		return takeOutCoreLabel.activeSelf;
	}

	public bool getQuitLabelVisibility()
	{
		return quitLabel.activeSelf;
	}

	public void toggleQuitLabel(bool mode)
	{
		quitLabel.SetActive(mode);
	}

	public bool getRepairLabelVisibility()
	{
		return repairLabel.activeSelf;
	}

	public void toggleRepairLabel(bool mode)
	{
		repairLabel.SetActive(mode);
	}

	public void updateRepairLabelPos(Vector3 pos)
	{
		repairLabel.transform.localPosition = pos;
	}
}
