using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {
	
	public Texture2D background;
	public GUIStyle creditsGUIStyle;
	public GUIStyle creditsTextStyle;
	public GUIStyle exitGUIStyle;
	public Texture2D exitButton;
	public static bool showCredits = false;
	
	void OnGUI () {
		if(!showCredits)
			return;
		GUI.Box(new Rect((Screen.width-background.width)/2, (Screen.height-background.height)/2, background.width, background.height), background, creditsGUIStyle);
		
		GUI.BeginGroup(new Rect((Screen.width-background.width)/2, (Screen.height-background.height-10)/2, background.width, background.height));
		
		creditsTextStyle.fontStyle = FontStyle.Bold;
		creditsTextStyle.fontSize = 16;
		GUI.Label(new Rect(0,0,background.width,background.height),
				"Game Developer & Secondary Game Designer: ", creditsTextStyle);
		creditsTextStyle.fontStyle = FontStyle.Normal;
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\nJ\u00E9ssica Pauli de Castro Bonson", creditsTextStyle);
		creditsTextStyle.fontStyle = FontStyle.Bold;
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\nPrimary Game Designer: ", creditsTextStyle);
		creditsTextStyle.fontStyle = FontStyle.Normal;
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\nPedro Eln\u00EDzio T\u00E1vora Pinho", creditsTextStyle);
		
		creditsTextStyle.fontStyle = FontStyle.Normal;
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\nGame developed for the class CSCI4168 Game Design & Development " +
				"in the Computer Science undergraduate program at Dalhousie University.", creditsTextStyle);
		
		creditsTextStyle.fontStyle = FontStyle.Bold;
		creditsTextStyle.fontSize = 14;
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\n\n\n\n\nGame References: ", creditsTextStyle);
		creditsTextStyle.fontStyle = FontStyle.Normal;
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\n\n\n\n\n\n- Ragnarok Online (sprites, sounds, musics, start menu image and some skills)", creditsTextStyle);
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\n\n\n\n\n\n\n- DeviantArt @admin2gd1 (background image)", creditsTextStyle);
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\n\n\n\n\n\n\n\n- Pixelation @Gromit (textures for final arena)", creditsTextStyle);
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\n\n\n\n\n\n\n\n\n- King of Fighters 97 & 2000 (boss skills \"Blame\" & \"Icicle\")", creditsTextStyle);
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n- Megaman X4 (mage skill \"Thunderball\")", creditsTextStyle);
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n- My Pet Protector 3 (skill icons)", creditsTextStyle);
		GUI.Label(new Rect(0,0,background.width,background.height),
				"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n- Mirrors Edge (victory music)", creditsTextStyle);
		
		if(GUI.Button(new Rect(background.width-exitButton.width, 5, exitButton.width, exitButton.height), "",exitGUIStyle))
			showCredits = false;
		
		GUI.EndGroup();
	}
}
