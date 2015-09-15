using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DeadEnemies : MonoBehaviour {
	
	public List<Vector3> deadEnemies;
	
	void Awake()
	{
		deadEnemies = new List<Vector3>();
	}
	
	public void Add (Vector3 v) {
		deadEnemies.Add(v);
	}
}

public partial class Static {
    public static DeadEnemies DeadEnemies {
        get { return FindObjectOfType(typeof(DeadEnemies)) as DeadEnemies; }
    }
}
