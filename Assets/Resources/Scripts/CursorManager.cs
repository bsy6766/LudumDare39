using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
	public Texture2D defaultCursorTex;

	public static CursorManager instance = null;
	
	public enum TOOLTIP
	{
		CONSOLE,
		POWERCORE,
		POWERGENERATOR
	}

	public static CursorManager getInstance()
	{
		return instance;
	}

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(this.gameObject);
		}

		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start()
	{
		Cursor.SetCursor(defaultCursorTex, Vector2.zero, CursorMode.Auto);
	}

	public Vector3 getMouseScreenPos()
	{
		return Input.mousePosition - new Vector3(540, 270, 0);
	}

	/*
	public void setToolTip(TOOLTIP toolTip)
	{
		if(toolTip == TOOLTIP.CONSOLE)
		{
			tooltipLabel.text = "CONSOLE";
		}

		//updateLabelPos();
	}

	private void updateLabelPos()
	{
		tooltipLabel.transform.position = Input.mousePosition + new Vector3(100.0f, -30.0f, 0);
	}
	*/
}