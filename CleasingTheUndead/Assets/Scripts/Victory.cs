using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {

	public Texture2D background;
	public GUIStyle inGameMenuGUIStyle;
	
	private float fade = 0;
	private float lastCastTime = -1;
	private bool check = false;
	
	public AudioClip victoryMusic;
	
	void Start()
	{
		var music = GameObject.FindGameObjectWithTag("BackgroundMusic");
		music.GetComponent<AudioSource>().clip = victoryMusic;
		music.GetComponent<AudioSource>().volume = 1;
		music.GetComponent<AudioSource>().Play();	
	}
	
	void Update () {
		if(fade < 1)
		{
			fade += Time.deltaTime*0.5f;
			if(fade >= 1)
			{
				fade = 1;
				if(lastCastTime == -1)
				{
					lastCastTime = Time.time;
				}
			}
		}

		if(fade == 1 && Time.time - lastCastTime > 1 && check == false)
		{
			Credits.showCredits = true;
			check = true;
		}
	}
	
	void OnGUI () {
		if (fade > 0)
	    {
	        GUI.color = Color.white;
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, fade);
			GUI.Box(new Rect((Screen.width-background.width)/2, (Screen.height-background.height)/4, background.width, background.height), background, inGameMenuGUIStyle);
	        GUI.color = Color.white;
	    }
	}
}
