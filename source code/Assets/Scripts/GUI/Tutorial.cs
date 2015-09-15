using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	public Texture2D background;
	public GUIStyle tutorialGUIStyle;
	
	public static bool showTutorial = false;
	
	void OnGUI () {
		if(!showTutorial)
			return;
		
		GUI.Box(new Rect((Screen.width-background.width)/2, (Screen.height-background.height)/2, background.width, background.height), background, tutorialGUIStyle);
	}
}
