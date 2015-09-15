using UnityEngine;
using System.Collections;

public class MeleeEnemyAttributes : MonoBehaviour {
	
	public float maxHP;
	public float movementSpd;
	public int atkDamage;
	public int atkCooldown;
	public int sightDistance;
	public int attackRange;
	public int experience;
	
	public void Awake()
	{
		if(Application.loadedLevelName == "Stage1")
		{
			maxHP = 400;
			movementSpd = 4;
			atkDamage = 120;
			atkCooldown = 100;
			sightDistance = 6;
			attackRange = 1;
			experience = 100;
		}
		if(Application.loadedLevelName == "Stage2")
		{
			maxHP = 500;
			movementSpd = 5;
			atkDamage = 150;
			atkCooldown = 90;
			sightDistance = 6;
			attackRange = 1;
			experience = 125;
		}
		if(Application.loadedLevelName == "Stage3")
		{
			maxHP = 600;
			movementSpd = 5;
			atkDamage = 180;
			atkCooldown = 80;
			sightDistance = 6;
			attackRange = 1;
			experience = 150;
		}
	}
	
	public void Load()
	{
		DontDestroyOnLoad(gameObject);
	}
	
}

public partial class Static {
    public static MeleeEnemyAttributes MeleeEnemyAttributes {
        get { return FindObjectOfType(typeof(MeleeEnemyAttributes)) as MeleeEnemyAttributes; }
    }
}
