using UnityEngine;
using System.Collections;

public class MenuInGame : MonoBehaviour {

	public Texture2D background;
	public Texture2D title;
	public Texture2D exampleButton;
	
	public GUIStyle inGameMenuGUIStyle;
	public GUIStyle resumeGameButtonStyle;
	public GUIStyle saveGameButtonStyle;
	public GUIStyle tutorialButtonStyle;
	public GUIStyle exitMainMenuButtonStyle;
	public GUIStyle exitWindowsButtonStyle;
	
	public static bool showMenuInGame = false;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(showMenuInGame == true)
				showMenuInGame = false;
			else
				showMenuInGame = true;
		}
	}
	
	void OnGUI () {
		if(!showMenuInGame)
			return;
		
		GUI.Box(new Rect((Screen.width-background.width)/2, (Screen.height-background.height)/2, background.width, background.height), background, inGameMenuGUIStyle);
		
		GUI.BeginGroup(new Rect((Screen.width-background.width)/2, (Screen.height-background.height-10)/2, background.width, background.height));
		
		GUI.Box(new Rect((background.width-title.width)/2, 10, title.width, title.height), title, inGameMenuGUIStyle);
		if(GUI.Button(new Rect((background.width-exampleButton.width)/2, title.height+10, exampleButton.width, exampleButton.height), "",resumeGameButtonStyle))
			showMenuInGame = false;
		if(GUI.Button(new Rect((background.width-exampleButton.width)/2, exampleButton.height+5+title.height+10, exampleButton.width, exampleButton.height), "",saveGameButtonStyle))
			print ("save game!");
		if(GUI.Button(new Rect((background.width-exampleButton.width)/2, (exampleButton.height+5)*2+title.height+10, exampleButton.width, exampleButton.height), "",exitMainMenuButtonStyle))
		{
			showMenuInGame = false;
			Application.LoadLevel("StartMenu");
		}
		if(GUI.Button(new Rect((background.width-exampleButton.width)/2, (exampleButton.height+5)*3+title.height+10, exampleButton.width, exampleButton.height), "",exitWindowsButtonStyle))
			Application.Quit();
		
		GUI.EndGroup();
	}
}
