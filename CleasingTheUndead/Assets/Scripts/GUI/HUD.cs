using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour {
	
	public GameObject player;
	private LevelUpSystem lvlSystem;
	
	public Texture2D background;
	public GUIStyle backgroundGUIStyle;
	
	public AudioClip outOfMana;
	
	private float health;
	private float displayedHealth;
	private float maxHealth;
	private float mana;
	private float displayedMana;
	private float maxMana;
	private float displayedExp;
	
	public Texture2D icon_skill_lvl;
	public List<Texture2D> iconPassiveSkills;
	public List<Texture2D> iconActiveSkills;
	
	public GUIStyle spellButtonStyle;
	public GUIStyle healthBarStyle;
	public GUIStyle manaBarStyle;
	public GUIStyle expBarStyle;
	public GUIStyle hpBarTextStyle;
	public GUIStyle manaBarTextStyle;
	public GUIStyle coolDownTextStyle;
	public GUIStyle skilllvlTextStyle;
	public GUIStyle lvlTextStyle;
	public GUIStyle lvlupskillStyle;
	public GUIStyle skillTitleTextStyle;
	public GUIStyle skillDescriptionTextStyle;
	public GUIStyle expTextStyle;
	
	public GUIStyle menuButtonGUIStyle;
	public GUIStyle tutorialButtonStyle;
	
	public Texture2D icon_lvlupskill;
	
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		lvlSystem = player.GetComponent<LevelUpSystem>();
	}
	
	void Update(){
		health = player.GetComponent<PlayerAttributes>().curHP;
		maxHealth = player.GetComponent<PlayerAttributes>().maxHP;
		mana = player.GetComponent<PlayerAttributes>().curMana;
		maxMana = player.GetComponent<PlayerAttributes>().maxMana;
	}
	
	void OnGUI()
	{	
		var toolbarRect = new Rect(0, 0, background.width,background.height);
		GUI.Box(toolbarRect,background,	backgroundGUIStyle);
		
		if(GUI.Button(new Rect(6, background.height-1, 57, 25), "",menuButtonGUIStyle))
		{
			if(MenuInGame.showMenuInGame)
				MenuInGame.showMenuInGame = false;
			else
				MenuInGame.showMenuInGame = true;
		}
		if(GUI.Button(new Rect(6+57+3, background.height-1, 25, 25), "",tutorialButtonStyle))
		{
			if(Tutorial.showTutorial)
				Tutorial.showTutorial = false;
			else
				Tutorial.showTutorial = true;
		}
		
		displayedHealth = Mathf.Lerp(displayedHealth, health, Time.deltaTime * 5);
		displayedMana = Mathf.Lerp(displayedMana, mana, Time.deltaTime * 5);
		displayedExp = Mathf.Lerp(displayedExp, lvlSystem.experience, Time.deltaTime * 5);

		PassiveSpellButton(new Rect(339, 47+6, iconPassiveSkills[0].width-3, iconPassiveSkills[0].height-3),
							lvlSystem.passives[0], iconPassiveSkills[0], spellButtonStyle);
		PassiveSpellButton(new Rect(339, 80+6, iconPassiveSkills[1].width-3, iconPassiveSkills[1].height-3),
							lvlSystem.passives[1], iconPassiveSkills[1], spellButtonStyle);
		
		SpellButton(new Rect(156,59+6,iconActiveSkills[0].width-8,iconActiveSkills[0].height-8),
					lvlSystem.actives[0],
					iconActiveSkills[0],
					KeyCode.Alpha1, spellButtonStyle);
		
		SpellButton(new Rect(201,59+6,iconActiveSkills[1].width-8,iconActiveSkills[1].height-8),
					lvlSystem.actives[1],
					iconActiveSkills[1],
					KeyCode.Alpha2, spellButtonStyle);
		
		SpellButton(new Rect(247,59+6,iconActiveSkills[2].width-8,iconActiveSkills[2].height-8),
					lvlSystem.actives[2],
					iconActiveSkills[2],
					KeyCode.Alpha3, spellButtonStyle);
		
		SpellButton(new Rect(293,59+6,iconActiveSkills[3].width-8,iconActiveSkills[3].height-8),
					lvlSystem.actives[3],
					iconActiveSkills[3],
					KeyCode.Alpha4, spellButtonStyle);

		
		DrawHorizontalBar(new Rect(132,6,247,19), displayedHealth, maxHealth, healthBarStyle, hpBarTextStyle);
		
		DrawHorizontalBar(new Rect(132,26,247,10), displayedMana, maxMana, manaBarStyle, manaBarTextStyle);
		
		DrawExpBar(new Rect(130,30+10,12,95-10), displayedExp, lvlSystem.getExpToNextLvl(), expBarStyle);

		
		GUI.Label(new Rect(89,60,30,95),
				string.Format("{0}", lvlSystem.level), lvlTextStyle);
	
		
		if(lvlSystem.passives[0].level < lvlSystem.maxSkilllvl)
			createLvlUpSkillButton(lvlSystem.passives[0],
								new Rect(336+32, 47+4+6, icon_lvlupskill.width, icon_lvlupskill.height));
		if(lvlSystem.passives[1].level < lvlSystem.maxSkilllvl)
			createLvlUpSkillButton(lvlSystem.passives[1],
								new Rect(336+32, 80+4+6, icon_lvlupskill.width, icon_lvlupskill.height));
		
		if(lvlSystem.actives[0].level < lvlSystem.maxSkilllvl)
			createLvlUpSkillButton(lvlSystem.actives[0],
								new Rect(156+8, 60+37+6, icon_lvlupskill.width, icon_lvlupskill.height));
		if(lvlSystem.actives[1].level < lvlSystem.maxSkilllvl)
			createLvlUpSkillButton(lvlSystem.actives[1],
								new Rect(201+8, 60+37+6, icon_lvlupskill.width, icon_lvlupskill.height));
		if(lvlSystem.actives[2].level < lvlSystem.maxSkilllvl)
			createLvlUpSkillButton(lvlSystem.actives[2],
								new Rect(247+8, 60+37+6, icon_lvlupskill.width, icon_lvlupskill.height));
		if(lvlSystem.actives[3].level < lvlSystem.maxSkilllvl)
			createLvlUpSkillButton(lvlSystem.actives[3],
								new Rect(293+8, 60+37+6, icon_lvlupskill.width, icon_lvlupskill.height));
	}
	
	private float glow = 0;
	void createLvlUpSkillButton(LevelUpSystem.Spell skill, Rect r)
	{
		glow += Time.deltaTime*0.3f;
		GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b,
			Mathf.Sin(glow)+1.3f);
		if(lvlSystem.skillPoints > 0)
		{
			if(GUI.Button(r, "",lvlupskillStyle))
				lvlSystem.up_skill(skill);
		}
		else
		{
			glow = 0;	
		}
		GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);
	}
	
	void PassiveSpellButton(Rect r, LevelUpSystem.PassiveSpell passive, Texture2D icon, GUIStyle style)
	{
		createToolTip(r, passive);
		GUI.Button(r,icon, style);
		addSkillLevelIcon(r, passive, icon);
	}
	
	void createToolTip(Rect r, LevelUpSystem.PassiveSpell skill)
	{
		if(r.Contains (Event.current.mousePosition))
		{
			GUI.color = Color.yellow;
			GUI.Box (new Rect(156, 120,200,100), "");
			GUI.Box (new Rect(156, 120,200,100), skill.name+" (lvl "+skill.level+")", skillTitleTextStyle);
			GUI.color = Color.white;
			if(skill == lvlSystem.passives[0])
			{
				GUI.Box (new Rect(156, 120,200,100), "\n\nAttack Damage: "+
					player.GetComponent<PlayerAttributes>().atkDamage, skillDescriptionTextStyle);
			}
			else
			{
				GUI.Box (new Rect(156, 120,200,100), "\n\nMax. HP: "+maxHealth, skillDescriptionTextStyle);
			}
			GUI.Box (new Rect(156, 120,200,100), "\n\n\n\n"+skill.getDescription(), skillDescriptionTextStyle);
			
		}
	}
	
	void createToolTip(Rect r, LevelUpSystem.ActiveSpell skill)
	{
		if(r.Contains (Event.current.mousePosition))
		{
			GUI.color = Color.yellow;
			GUI.Box (new Rect(156, 120,200,130), "");
			GUI.Box (new Rect(156, 120,200,130), skill.name+" (lvl "+skill.level+")", skillTitleTextStyle);
			GUI.color = Color.white;
			GUI.Box (new Rect(156, 120,200,130), "\n\n"+skill.getInfo(), skillDescriptionTextStyle);
			GUI.Box (new Rect(156, 120,200,130), "\n\n\n\n\n"+skill.getDescription(), skillDescriptionTextStyle);
			
		}
	}
	
	void SpellButton(Rect r, LevelUpSystem.ActiveSpell active, Texture2D icon, KeyCode key, GUIStyle style){
		createToolTip(r, active);
		if(!active.isAvailable())
			GUI.color = new Color(0.4f,0.4f,0.4f,1);
		if((Event.current.type == EventType.KeyDown && Event.current.keyCode == key ||
			GUI.Button(r,icon, style)) && active.isAvailable() && player.GetComponent<Controls>().cantMove == 0)
		{
			if(active.manaCost <= mana)
			{
				if(active.Equals(lvlSystem.actives[0]))
				{
					if(health < maxHealth)
						lvlSystem.activate_skill(active);
				}
				else if(active.Equals(lvlSystem.actives[1]) || active.Equals(lvlSystem.actives[2]))
				{
					if(player.GetComponent<Controls>().grounded)
						lvlSystem.activate_skill(active);
				}
				else
					lvlSystem.activate_skill(active);
			}
			else
			{
				audio.PlayOneShot(outOfMana);
			}
		}
		if(active.onCoolDown() && active.level > 0){
			GUI.Label(r,string.Format("{0:F1}",active.cooldown -(Time.time-active.lastCastTime)), coolDownTextStyle);
		}
		addSkillLevelIcon(r, active, icon);
		GUI.color = Color.white;
	}
	
	void addSkillLevelIcon(Rect r, LevelUpSystem.PassiveSpell spell, Texture2D icon)
	{
		GUI.Button(new Rect(r.x+icon.width-3-icon_skill_lvl.width+5,
							r.y+icon.height-3-icon_skill_lvl.height+4,
							icon_skill_lvl.width, icon_skill_lvl.height),icon_skill_lvl, spellButtonStyle);
		GUI.Label(new Rect(r.x+7, r.y+7, r.width, r.height),
				string.Format("{0}", spell.level), skilllvlTextStyle);
	}
	
	void addSkillLevelIcon(Rect r, LevelUpSystem.ActiveSpell spell, Texture2D icon)
	{
		GUI.Button(new Rect(r.x+icon.width-3-icon_skill_lvl.width-2,
							r.y+icon.height-3-icon_skill_lvl.height-1,
							icon_skill_lvl.width, icon_skill_lvl.height),icon_skill_lvl, spellButtonStyle);
		GUI.Label(new Rect(r.x+10, r.y+11, r.width, r.height),
				string.Format("{0}", spell.level), skilllvlTextStyle);
	}
	
	void DrawHorizontalBar(Rect r, float health, float totalHealth, GUIStyle style, GUIStyle textStyle)
	{
		Rect actualHealth = new Rect(r.x,r.y, r.width * (health/totalHealth), r.height);
		if(health > totalHealth-1)
			health = totalHealth;
		GUI.Label(actualHealth,"", style);
		GUI.Label(r, string.Format("{0}/{1}", (int)health, (int)totalHealth), textStyle);
	}
	
	void DrawExpBar(Rect r, float exp, float totalExp, GUIStyle style)
	{
		Rect actualExp = new Rect(r.x,r.y+r.height, r.width, -(r.height * (exp/totalExp)));
		GUI.Label(actualExp,"", style);
		if(r.Contains (Event.current.mousePosition))
		{
			GUI.color = Color.white;
			GUI.Box (new Rect(r.x - r.width/2,r.y+r.height+5,200,100),  lvlSystem.experience+" / "+lvlSystem.getExpToNextLvl()+" EXP", expTextStyle);
			
		}
	}
}
