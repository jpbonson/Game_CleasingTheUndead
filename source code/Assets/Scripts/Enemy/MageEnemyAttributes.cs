using UnityEngine;
using System.Collections;

public class MageEnemyAttributes : MonoBehaviour {

	public float maxHP;
	public float movementSpd;
	public int atkDamage;
	public int atkCooldown;
	public int hurricaneCooldown;
	public int lightningDamage;
	public int lightningCooldown;
	public int sightDistance;
	public int attackRange;
	public int experience;
	
	public void Awake()
	{
		if(Application.loadedLevelName == "Stage2")
		{
			maxHP = 330;
			movementSpd = 3;
			atkDamage = 150;
			atkCooldown = 150;
			hurricaneCooldown = 250;
			lightningDamage = 200;
			lightningCooldown = 600;
			sightDistance = 8;
			attackRange = 6;
			experience = 125;
		}
		if(Application.loadedLevelName == "Stage3")
		{
			maxHP = 410;
			movementSpd = 3;
			atkDamage = 160;
			atkCooldown = 130;
			hurricaneCooldown = 150;
			lightningDamage = 250;
			lightningCooldown = 500;
			sightDistance = 8;
			attackRange = 6;
			experience = 150;
		}
	}
	
	public void Load()
	{
		DontDestroyOnLoad(gameObject);
	}
	
}

public partial class Static {
    public static MageEnemyAttributes MageEnemyAttributes {
        get { return FindObjectOfType(typeof(MageEnemyAttributes)) as MageEnemyAttributes; }
    }
}