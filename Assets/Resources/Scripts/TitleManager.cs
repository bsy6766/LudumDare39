using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

		SoundManager.getInstance().clearSources();
		SoundManager.getInstance().stopBGM();
		SoundManager.getInstance().playBGM();
	}

	// Update is called once per frame
	void Update () {
		if(Input.anyKeyDown)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
			SoundManager.getInstance().play("general");
		}	
	}
}
