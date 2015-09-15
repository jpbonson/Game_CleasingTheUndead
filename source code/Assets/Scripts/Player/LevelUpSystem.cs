using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LevelUpSystem : MonoBehaviour {
	
	public int experience = 0;
	public int level = 1;
	public int skillPoints = 1;
	
	public List<ActiveSpell> actives;
	public List<PassiveSpell> passives;
	
	public int maxSkilllvl = 3;
	
	public PackedSprite skillSprite;
	public PackedSprite skillSprite2;
	public PackedSprite playerEffectSprite;
	
	public bool intervention = false;
	
	public List<AudioClip> soundActives;
	public AudioClip soundLvlUp;
	
	void Awake () {
		instantiateSkills();
		
		if(Static.SceneLoader == null)
			return;
		
		if(Static.SceneLoader.experience != -1)
			experience = Static.SceneLoader.experience;
		if(Static.SceneLoader.level != -1)
			level = Static.SceneLoader.level;
		if(Static.SceneLoader.skillPoints != -1)
			skillPoints = Static.SceneLoader.skillPoints;
		
		for(int i = 1; i < level; i++)
		{
			upLevel();
		}
		
		if(Static.SceneLoader.skillLvlPassive0 != -1)
		{
			for(int i = 0; i < Static.SceneLoader.skillLvlPassive0; i++)
			{
				skillPoints++;
				up_skill(passives[0]);
			}
		}
		
		if(Static.SceneLoader.skillLvlPassive1 != -1)
		{
			for(int i = 0; i < Static.SceneLoader.skillLvlPassive1; i++)
			{
				skillPoints++;
				up_skill(passives[1]);
			}
		}
		
		if(Static.SceneLoader.skillLvlActive0 != -1)
		{
			for(int i = 0; i < Static.SceneLoader.skillLvlActive0; i++)
			{
				skillPoints++;
				up_skill(actives[0]);
			}
		}
		
		if(Static.SceneLoader.skillLvlActive1 != -1)
		{
			for(int i = 0; i < Static.SceneLoader.skillLvlActive1; i++)
			{
				skillPoints++;
				up_skill(actives[1]);
			}
		}
		
		if(Static.SceneLoader.skillLvlActive2 != -1)
		{
			for(int i = 0; i < Static.SceneLoader.skillLvlActive2; i++)
			{
				skillPoints++;
				up_skill(actives[2]);
			}
		}
		
		if(Static.SceneLoader.skillLvlActive3 != -1)
		{
			for(int i = 0; i < Static.SceneLoader.skillLvlActive3; i++)
			{
				skillPoints++;
				up_skill(actives[3]);
			}
		}
	}

	void Update()
	{
		if(intervention && Time.time - actives[3].lastCastTime > actives[3].skillValue)
		{
			skillSprite2.StopAnim();
			intervention = false;
		}
		
		if(Input.GetKeyDown(KeyCode.P))
		{
			if(level == 12)
				return;
			level+=11;
			skillPoints+=11;
			for(int i = 0; i < 11; i++)
			{
				GetComponent<PlayerAttributes>().maxHP+=50+(int)(50*(passives[1].valueIncreasePerLvl*passives[1].level)/100);
				GetComponent<PlayerAttributes>().atkDamage+=5;
				GetComponent<PlayerAttributes>().maxMana+=10;	
			}
		}
		if(Input.GetKeyDown(KeyCode.Alpha0))
		{
			transform.position = new Vector3(25.70168f,38.12088f,0);	
			Camera.main.transform.position = new Vector3(0, 38, -10);
		}
		if(Input.GetKeyDown(KeyCode.H))
		{
			GetComponent<PlayerAttributes>().curHP = GetComponent<PlayerAttributes>().maxHP;
		}
	}
	
	public void instantiateSkills()
	{
		PassiveSpell passive1 = new PassiveSpell("Increase Attack", 
			"Increases the attack damage by ", ".", 20);
		PassiveSpell passive2 = new PassiveSpell("Increase Max HP",
			"Increases the maximum HP by ", "%.",10);
		passives.Add(passive1);
		passives.Add(passive2);
		
		ActiveSpell active1 = new ActiveSpell("Heal", 
			"Heals the paladin in ", " health points.", 
			5, 0, 100, 0, 20, 0);
		ActiveSpell active2 = new ActiveSpell("Vengeance", 
			"Releases an wave which deals ", " damage to the enemies it passes through.\n" +
			"[Can't be used while jumping]", 
			5, 0, 150, -1, 30, 0);
		ActiveSpell active3 = new ActiveSpell("Judgement", 
			"Creates a explosion of energy around the paladin, dealing ", " damage.\n" +
			"[Can't be used while jumping]", 
			25, 300, 100, -5, 60, 0);
		ActiveSpell active4 = new ActiveSpell("Intervention", 
			"The paladin is immune to all damage during ", " seconds.", 
			30, 2, 1, 0, 100, 0);
		actives.Add(active1);
		actives.Add(active2);
		actives.Add(active3);
		actives.Add(active4);
			
	}
	
	public void resetSkills()
	{
		passives.Clear();
		actives.Clear();
		instantiateSkills();
	}
	
	public void up_skill(Spell skill)
	{
		if(skill.Equals(actives[0])) // active skill 1
		{
			if(actives[0].level < maxSkilllvl  && skillPoints > 0)
			{
				actives[0].level++;
				skillPoints--;
				actives[0].skillValue += actives[0].valueIncreasePerLvl;
			}
		}
		
		if(skill.Equals(actives[1])) // active skill 2
		{
			if(actives[1].level < maxSkilllvl  && skillPoints > 0)
			{
				actives[1].level++;
				skillPoints--;
				actives[1].skillValue += actives[1].valueIncreasePerLvl;
				actives[1].cooldown += actives[1].cooldownChangePerLvl;
			}
		}
		
		if(skill.Equals(actives[2])) // active skill 3
		{
			if(actives[2].level < maxSkilllvl  && skillPoints > 0)
			{
				actives[2].level++;
				skillPoints--;
				actives[2].skillValue += actives[2].valueIncreasePerLvl;
				actives[2].cooldown += actives[2].cooldownChangePerLvl;
			}
		}
		
		if(skill.Equals(actives[3])) // active skill 3
		{
			if(actives[3].level < maxSkilllvl  && skillPoints > 0)
			{
				actives[3].level++;
				skillPoints--;
				actives[3].skillValue += actives[3].valueIncreasePerLvl;
			}
		}
		
		if(skill.Equals(passives[0])) // passive skill 1
		{
			if(passives[0].level < maxSkilllvl && skillPoints > 0)
			{
				passives[0].level++;
				skillPoints--;
				GetComponent<PlayerAttributes>().atkDamage+=passives[0].valueIncreasePerLvl;
			}
		}
		
		if(skill.Equals(passives[1])) // passive skill 2
		{
			if(passives[1].level < maxSkilllvl && skillPoints > 0)
			{
				passives[1].level++;
				skillPoints--;
				GetComponent<PlayerAttributes>().maxHP+=
					((int)GetComponent<PlayerAttributes>().maxHP*passives[1].valueIncreasePerLvl)/100;
				GetComponent<PlayerAttributes>().curHP+=
					((int)GetComponent<PlayerAttributes>().maxHP*passives[1].valueIncreasePerLvl)/100;
			}
		}
	}
	
	public void activate_skill(Spell skill)
	{
		if(skill.Equals(actives[0]))
		{
			GetComponent<PlayerAttributes>().curHP += actives[0].skillValue+1;
			if(GetComponent<PlayerAttributes>().curHP >= GetComponent<PlayerAttributes>().maxHP)
				GetComponent<PlayerAttributes>().curHP = GetComponent<PlayerAttributes>().maxHP;
			actives[0].lastCastTime = Time.time;
			GetComponent<PlayerAttributes>().curMana -= actives[0].manaCost;
			skillSprite.DoAnim("Heal");
			GetComponent<Controls>().doSkill1();
			audio.PlayOneShot(soundActives[0]);
			return;
		}
		if(skill.Equals(actives[1]))
		{
			actives[1].lastCastTime = Time.time;
			GetComponent<PlayerAttributes>().curMana -= actives[1].manaCost;
			GetComponent<Controls>().doSkill2();
			audio.PlayOneShot(soundActives[1]);
			return;
		}
		if(skill.Equals(actives[2]))
		{
			actives[2].lastCastTime = Time.time;
			GetComponent<PlayerAttributes>().curMana -= actives[2].manaCost;
			GetComponent<Controls>().doSkill3();
			audio.PlayOneShot(soundActives[2]);
			return;
		}
		if(skill.Equals(actives[3]))
		{
			GetComponent<Controls>().doSkill4();
			skillSprite2.DoAnim("Intervention");
			intervention = true;
			GetComponent<PlayerAttributes>().curMana -= actives[3].manaCost;
			actives[3].lastCastTime = Time.time;
			audio.PlayOneShot(soundActives[3]);
			return;
		}
	}
	
	public int getExpToNextLvl()
	{
		// Y = 155.4*1.287^X
		switch(level)
		{
		   case 1:
				return 200;
		   case 2:
				return 260;
		   case 3:
				return 330;
		   case 4:
				return 425; // 1215
		   case 5:
				return 550;
		   case 6:
				return 705; // 2470
			case 7:
				return 910;
			case 8:
				return 1170;
			case 9:
				return 1505; // 6055
			case 10:
				return 1880; // 7935
			case 11:
				return 2340; // 10275
		}
		/*
		1: lvl 6 (1765) + 235 xp
		2000 xp
		
		2: lvl 10 (6055) + 320 xp
		4375 xp
		
		3: lvl 12 (10275)
		3900 xp 
		 */
		return 100;
	}
	
	public void increase_experience(int exp)
	{
		if(level == 12)
			return;
			
		experience += exp;
		
		if(experience >= getExpToNextLvl())
		{
			experience -= getExpToNextLvl();
			level++;
			skillPoints++;
			upLevel();
			playerEffectSprite.DoAnim("LevelUp");
			audio.PlayOneShot(soundLvlUp);
		}
		else
		{
			Vector3 v = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x,transform.position.y+1,transform.position.z));
			GameObject dam = Instantiate(GetComponent<Controls>().floatingDamage,new Vector3(v.x,v.y,-1),transform.rotation) as GameObject;
			dam.guiText.material.color = Color.magenta;
			dam.GetComponent<FloatingDamage>().duration = 0.4f;
			dam.guiText.fontSize = 20;
			dam.guiText.fontStyle = FontStyle.Normal;
			dam.guiText.text = exp.ToString()+" EXP";	
		}
	}
	
	private void upLevel()
	{
		GetComponent<PlayerAttributes>().maxHP+=50+(int)(50*(passives[1].valueIncreasePerLvl*passives[1].level)/100);
		GetComponent<PlayerAttributes>().atkDamage+=5;
		GetComponent<PlayerAttributes>().maxMana+=10;
	}
	
	[System.Serializable]
	public abstract class Spell
	{
		public string name;
		public string description1;
		public string description2;
		public int level = 0;
		
		public abstract string getDescription();
	}
	
	[System.Serializable]
	public class ActiveSpell: Spell
	{
		public float cooldown;
		public float cooldownChangePerLvl;
		private string cooldownDescription;
		public float lastCastTime = -1000;
		public int skillValue;
		public int valueIncreasePerLvl;
		private string valueDescription;
		public float manaCost;
		public float manaCostChangePerLvl;
		private string manaDescription;
		
		public ActiveSpell(string name, string description1, string description2, 
							float cooldown, int startValue,
							int valueIncreasePerLvl, float cooldownChangePerLvl,
							float manaCost, float manaCostChangePerLvl)
		{
			this.name = name;
			this.description1 = description1;
			this.description2 = description2;
			this.cooldown = cooldown;
			this.cooldownChangePerLvl = cooldownChangePerLvl;
			cooldownDescription = (cooldown+cooldownChangePerLvl)+"/"+
					(cooldown+2*cooldownChangePerLvl)+"/"+(cooldown+3*cooldownChangePerLvl);
			this.skillValue = startValue;
			this.valueIncreasePerLvl = valueIncreasePerLvl;
			valueDescription = (skillValue+valueIncreasePerLvl)+"/"+
					(skillValue+2*valueIncreasePerLvl)+"/"+(skillValue+3*valueIncreasePerLvl);
			this.manaCost = manaCost;
			this.manaCostChangePerLvl = manaCostChangePerLvl;
			manaDescription = (manaCost+manaCostChangePerLvl)+"/"+
					(manaCost+2*manaCostChangePerLvl)+"/"+(manaCost+3*manaCostChangePerLvl);
		}

		public bool onCoolDown()
		{
			return Time.time - lastCastTime < cooldown;
		}
		
		public bool isAvailable()
		{
			if(level > 0 && !onCoolDown())
				return true;
			else
				return false;
		}
		
		public string getInfo()
		{
			return "Cooldown: "+cooldownDescription+"\n"+
				   "Mana Cost: "+manaDescription;
		}
		
		public override string getDescription()
		{
			return description1+valueDescription+description2;
		}
	}
	
	[System.Serializable]
	public class PassiveSpell: Spell
	{
		public int valueIncreasePerLvl;
		
		public PassiveSpell(string name, string description1, string description2, int valueIncreasePerLvl)
		{
			this.name = name;
			this.description1 = description1;
			this.description2 = description2;
			this.valueIncreasePerLvl = valueIncreasePerLvl;
		}
		
		public override string getDescription()
		{
			return description1+valueIncreasePerLvl+"/"+valueIncreasePerLvl*2+"/"+valueIncreasePerLvl*3+description2;;
		}
	}
}
