using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{

	private float deltaTime;

	// Use this for initialization
	void Start()
	{
		this.deltaTime = 0.0f;
	}

	// Update is called once per frame
	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(10, 10, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		/*
		float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
		*/
		GUI.Label(rect, "FPS = " + ((int)(1.0f / Time.smoothDeltaTime)).ToString(), style);
	}
}