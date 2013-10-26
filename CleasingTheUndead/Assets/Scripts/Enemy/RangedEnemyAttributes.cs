using UnityEngine;
using System.Collections;

public class RangedEnemyAttributes : MonoBehaviour {

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
			maxHP = 180;
			movementSpd = 4;
			atkDamage = 150;
			atkCooldown = 200;
			sightDistance = 8;
			attackRange = 5;
			experience = 100;
		}
		if(Application.loadedLevelName == "Stage2")
		{
			maxHP = 270;
			movementSpd = 4;
			atkDamage = 200;
			atkCooldown = 175;
			sightDistance = 9;
			attackRange = 7;
			experience = 125;
		}
		if(Application.loadedLevelName == "Stage3")
		{
			maxHP = 360;
			movementSpd = 4;
			atkDamage = 250;
			atkCooldown = 150;
			sightDistance = 9;
			attackRange = 7;
			experience = 150;
		}
	}
	
	public void Load()
	{
		DontDestroyOnLoad(gameObject);
	}
	
}

public partial class Static {
    public static RangedEnemyAttributes RangedEnemyAttributes {
        get { return FindObjectOfType(typeof(RangedEnemyAttributes)) as RangedEnemyAttributes; }
    }
}
