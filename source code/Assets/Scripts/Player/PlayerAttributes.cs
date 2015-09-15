using UnityEngine;
using System.Collections;

public class PlayerAttributes : MonoBehaviour {
	
	public float maxHP = 1000;
	public float curHP;
	public float hpRegen = 1;
	public float maxMana = 100;
	public float curMana;
	public float manaRegen = 2;
	public float atkDamage = 100;
	public float movementSpd = 3;
	public bool endGame = false;
	public bool isCursed = false;
	private float curseDuration = 0;
	private float curseStart = 0;
	
	void Start()
	{
		curHP = maxHP;
		curMana = maxMana;
	}
	
	// Update is called once per frame
	void Update () {
		if(isCursed && Time.time - curseStart > curseDuration || GetComponent<LevelUpSystem>().intervention)
		{
			isCursed = false;
			movementSpd = 3;
			GetComponent<Controls>().playerSprite.SetColor(Color.white);
		}
		
		if(!endGame && curHP > 0)
		{
			if(!isCursed || GetComponent<LevelUpSystem>().intervention)
			{
				curHP = Mathf.Clamp(curHP+maxHP*hpRegen/100*Time.deltaTime, 0, maxHP+1);
				curMana = Mathf.Clamp(curMana+maxMana*manaRegen/100*Time.deltaTime, 0, maxMana+1);
			}
		}
		else
		{
			curHP = 0;
		}
	}
	
	public void curse(float duration)
	{
		curseDuration = duration;
		isCursed = true;
		curseStart = Time.time;
		movementSpd = 1;
		GetComponent<Controls>().playerSprite.SetColor(Color.red);
	}
}
