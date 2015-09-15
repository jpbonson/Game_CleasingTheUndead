using UnityEngine;
using System.Collections;

public class StageGUI : MonoBehaviour {

	public Texture2D background;
	public GUIStyle inGameMenuGUIStyle;
	
	private float fade = 1;
	
	void Update () {
		fade -= Time.deltaTime*0.5f;
		if(fade <= 0)
		{
			Destroy (gameObject);
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
