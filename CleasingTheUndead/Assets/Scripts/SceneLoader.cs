using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour {
	
	public string nextScene;
	
	// Any Data you want to access from last scene is fill in.
	public Vector3 position = Vector3.zero;
	public int experience = -1;
	public int level = -1;
	public int skillPoints = -1;
	public int skillLvlPassive0 = -1;
	public int skillLvlPassive1 = -1;
	public int skillLvlActive0 = -1;
	public int skillLvlActive1 = -1;
	public int skillLvlActive2 = -1;
	public int skillLvlActive3 = -1;
	public List<Vector3> deadEnemies;
	
	void Awake()
	{
		deadEnemies = new List<Vector3>();
	}
	
	public void Load() {
		DontDestroyOnLoad(gameObject);
		Application.LoadLevel(nextScene);
	}
}

public partial class Static {
    public static SceneLoader SceneLoader {
        get { return FindObjectOfType(typeof(SceneLoader)) as SceneLoader; }
    }
}