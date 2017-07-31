using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour {
	public Text onlineLabel;
	public Text offlineLabel;
	public Image layout;

	public Color onlineColor;
	public Color offlineColor;

	public Button pcButton;
	public Button peButton;
	public Button msButton;

	private List<GameObject> pcUpgradeBars;
	private List<GameObject> peUpgradeBars;
	private List<GameObject> msUpgradeBars;

	private float pcUpgradeBarY = 55;
	private float peUpgradeBarY = -9;
	private float msUpgradeBarY = -77;

	private float upgradeBarX = -101;
	private float upgradeBarXOffset = 30.5f;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void init()
	{
		//SoundManager.getInstance().addSource("pcUpgrade", pcButton.GetComponent<AudioSource>());
		//SoundManager.getInstance().addSource("peUpgrade", peButton.GetComponent<AudioSource>());
		//SoundManager.getInstance().addSource("msUpgrade", msButton.GetComponent<AudioSource>());

		pcUpgradeBars = new List<GameObject>();
		peUpgradeBars = new List<GameObject>();
		msUpgradeBars = new List<GameObject>();

		for (int i = 0; i < 10; i++)
		{
			float offsetX = upgradeBarXOffset * ((float)i);

			GameObject pcUpgradeBar = Instantiate(Resources.Load("prefabs/UpgradeBar")) as GameObject;
			pcUpgradeBar.transform.SetParent(this.gameObject.transform);
			pcUpgradeBars.Add(pcUpgradeBar);
			pcUpgradeBar.transform.localPosition = new Vector3(upgradeBarX + offsetX, pcUpgradeBarY, 0);
			pcUpgradeBar.SetActive(false);

			GameObject peUpgradeBar = Instantiate(Resources.Load("prefabs/UpgradeBar")) as GameObject;
			peUpgradeBar.transform.SetParent(this.gameObject.transform);
			peUpgradeBars.Add(peUpgradeBar);
			peUpgradeBar.transform.localPosition = new Vector3(upgradeBarX + offsetX, peUpgradeBarY, 0);
			peUpgradeBar.SetActive(false);

			GameObject msUpgradeBar = Instantiate(Resources.Load("prefabs/UpgradeBar")) as GameObject;
			msUpgradeBar.transform.SetParent(this.gameObject.transform);
			msUpgradeBars.Add(msUpgradeBar);
			msUpgradeBar.transform.localPosition = new Vector3(upgradeBarX + offsetX, msUpgradeBarY, 0);
			msUpgradeBar.SetActive(false);
		}
	}

	public void refresh(int pcLevel, int peLevel, int msLevel, bool online)
	{
		if(online)
		{
			layout.color = onlineColor;
			onlineLabel.enabled = true;
			offlineLabel.enabled = false;

			if(pcLevel == 10)
			{
				pcButton.interactable = false;
			}
			else
			{
				pcButton.interactable = true;
			}

			if (peLevel == 10)
			{
				peButton.interactable = false;
			}
			else
			{
				peButton.interactable = true;
			}

			if (msLevel == 10)
			{
				msButton.interactable = false;
			}
			else
			{
				msButton.interactable = true;
			}

			toggleBarColor(true);
		}
		else
		{
			layout.color = offlineColor;
			onlineLabel.enabled = false;
			offlineLabel.enabled = true;

			pcButton.interactable = false;
			peButton.interactable = false;
			msButton.interactable = false;

			toggleBarColor(false);
		}

		this.gameObject.SetActive(true);
	}

	public void close()
	{
		this.gameObject.SetActive(false);
	}

	public bool isOpened()
	{
		return gameObject.activeSelf;
	}

	public void updatePowerCoreUpgradeLevel(int level)
	{
		pcUpgradeBars[level - 1].SetActive(true);
	}

	public void updatePowerEfficiencyUpgradeLevel(int level)
	{
		peUpgradeBars[level - 1].SetActive(true);
	}

	public void updateMovementSpeedUpgradeLevel(int level)
	{
		msUpgradeBars[level - 1].SetActive(true);
	}

	public void disableButtons()
	{
		pcButton.interactable = false;
		peButton.interactable = false;
		msButton.interactable = false;
	}

	public void switchToOffline()
	{
		layout.color = offlineColor;
		onlineLabel.enabled = false;
		offlineLabel.enabled = true;

		pcButton.interactable = false;
		peButton.interactable = false;
		msButton.interactable = false;
	}

	public void toggleBarColor(bool online)
	{
		for (int i = 0; i < 10; i++)
		{
			pcUpgradeBars[i].GetComponent<Image>().color = online ? onlineColor : offlineColor;
			peUpgradeBars[i].GetComponent<Image>().color = online ? onlineColor : offlineColor;
			msUpgradeBars[i].GetComponent<Image>().color = online ? onlineColor : offlineColor;
		}
	}
}
