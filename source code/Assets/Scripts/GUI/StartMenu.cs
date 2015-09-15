using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {
	
	public Texture2D background;
	public Texture2D paladinWithoutColors;
	public Texture2D title;
	public GUIStyle startMenuGUIStyle;
	
	public Texture2D exampleButton;
	public GUIStyle newGameButtonStyle;
	public GUIStyle loadGameButtonStyle;
	public GUIStyle creditsButtonStyle;
	public GUIStyle exitButtonStyle;
	
	public GameObject credits;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.N))
		{
			var sl = new GameObject().AddComponent<SceneLoader>();
			sl.nextScene = "Stage2";
			sl.Load();
		}
		
		if(Input.GetKeyDown(KeyCode.M))
		{
			var sl = new GameObject().AddComponent<SceneLoader>();
			sl.nextScene = "Stage3";
			sl.Load();
		}
	}
	
	// Update is called once per frame
	void OnGUI () {
		GUI.Box(new Rect(0, 0, Screen.width, Screen.height), background, startMenuGUIStyle);
		GUI.Box(new Rect(0, 0, paladinWithoutColors.width, paladinWithoutColors.height), paladinWithoutColors, startMenuGUIStyle);
		GUI.Box(new Rect(350, -60, title.width, title.height), title, startMenuGUIStyle);
		
		GUI.BeginGroup(new Rect(650,260,511,600));
		if(GUI.Button(new Rect(0, 0, exampleButton.width, exampleButton.height), "",newGameButtonStyle))
		{
			var sl = new GameObject().AddComponent<SceneLoader>();
			sl.nextScene = "Stage1";
			sl.Load();
		}
		if(GUI.Button(new Rect(0, exampleButton.height+2, exampleButton.width, exampleButton.height), "",loadGameButtonStyle))
			print ("load game!");
		if(GUI.Button(new Rect(0, (exampleButton.height+2)*2, exampleButton.width, exampleButton.height), "",creditsButtonStyle))
		{
			Credits.showCredits = true;
		}
		if(GUI.Button(new Rect(0, (exampleButton.height+2)*3, exampleButton.width, exampleButton.height), "",exitButtonStyle))
			Application.Quit();
		GUI.EndGroup();
	}
}
