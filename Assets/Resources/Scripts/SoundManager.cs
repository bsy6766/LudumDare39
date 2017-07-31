using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance = null;

	public AudioSource BGMSource;
	public AudioSource SFXSource;

	public AudioClip upgradeButtonSFX;
	public AudioClip generalButtonSFX;
	public AudioClip collectSFX;

	private Dictionary<string, AudioClip> clips;

	public static SoundManager getInstance()
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

		clips = new Dictionary<string, AudioClip>();

		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		BGMSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void clearSources()
	{
		clips = new Dictionary<string, AudioClip>();

		clips.Add("upgrade", upgradeButtonSFX);
		clips.Add("general", generalButtonSFX);
		clips.Add("collect", collectSFX);
	}

	public void muteBGM()
	{
		BGMSource.mute = true;
	}

	public void unmuteBGM()
	{
		BGMSource.mute = false;
	}

	public void stopBGM()
	{
		BGMSource.Stop();
	}

	public void playBGM()
	{
		BGMSource.Play();
	}

	public void play(string key)
	{
		SFXSource.clip = clips[key];
		SFXSource.Play();
	}
}
