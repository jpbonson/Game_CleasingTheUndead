using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BossControl : MonoBehaviour {
	
	public float maxHP = 3500;
	public float curHP;
	public static float icicleDamage = 200;
	public static float blameDamage = 500;
	public GameObject floatingDamage;
	
	public AudioClip sound_damaged;
	public AudioClip sound_teleport;
	public AudioClip sound_drain;
	public AudioClip sound_summon;
	public AudioClip sound_curse;
	public AudioClip sound_dying;
	
	public GameObject victory;
	public GameObject summonSmoke;
	
	public PackedSprite sprites;
	public PackedSprite skillTeleport;
	private PackedSprite skillCurse;
	private PackedSprite skillLifeDrain;
	private PackedSprite skillLifeDrainPlayer;
	private PackedSprite moreSkillsSprite;
	protected string lastAnimation = "Standing";
	private Direction currentDirection = Direction.Left;
	private enum Direction{Right, Left};
	private int hitCooldown = 0;
	private bool isDying = false;
	private GameObject player;
	
	private GameObject teleportPoint_central;
	private GameObject teleportPoint_right1;
	private GameObject teleportPoint_left1;
	private GameObject teleportPoint_right2;
	private GameObject teleportPoint_left2;
	private GameObject teleportPoint_left_center;
	private GameObject teleportPoint_right_center;
	
	private List<GameObject> teleportPoints;
	private int teleportCooldown = 200;
	private int curTeleportCooldown;
	private bool wasCastingTeleport = false;
	
	public GameObject mobMelee;
	public GameObject mobRanged;
	public GameObject mobMage;
	private List<GameObject> instancedMob;
	private int summonCooldown = 2000;
	private int curSummonCooldown;
	private bool wasCastingSummon = false;
	private enum SummonTurn{Melee, Ranged, Mage};
	private SummonTurn currentSummonTurn = SummonTurn.Melee;
	public GameObject skillSummonUndead;
	
	private int curseCooldown = 1200;
	private int curCurseCooldown;
	private bool wasCastingCurse = false;
	public GameObject bat;
	
	private int icicleCooldown = 500;
	private int curIcicleCooldown;
	private bool wasCastingIcicle = false;
	public GameObject skillIcicle_left;
	public GameObject skillIcicle_right;
	public GameObject skillIcicle_area;
	
	private int blameCooldown = 500;
	private int curBlameCooldown;
	private bool wasCastingBlame = false;
	public GameObject skillBlame;
	
	private int lifeDrainCooldown = 500;
	private int curLifeDrainCooldown;
	private bool wasCastingLifeDrain = false;
	
	private bool wasCasting = false;
	private bool gotHit = false;
	private bool isCentralized = false;
	private bool isNearPlayer = false;
	
	private int globalCooldown = 10;
	private int curGlobalCooldown = 0;
	private float lastCastTime = -1;
	
	private bool control = false;
	private bool magicInterrupted = false;
	
	private System.Random random = new System.Random();
	
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		
		ignoreCollisionWithEnemies();
		curHP = maxHP;
		
		teleportPoints = GameObject.FindGameObjectsWithTag("TeleportPoint").ToList();
		
		instancedMob = new List<GameObject>();
		
		for(int i = 0; i < teleportPoints.Count; i++)
		{
			if(teleportPoints[i].name == "Teleport Point - Center")
				teleportPoint_central = teleportPoints[i];
			if(teleportPoints[i].name == "Teleport Point - Right 1")
				teleportPoint_right1 = teleportPoints[i];
			if(teleportPoints[i].name == "Teleport Point - Left 1")
				teleportPoint_left1 = teleportPoints[i];
			if(teleportPoints[i].name == "Teleport Point - Right 2")
				teleportPoint_right2 = teleportPoints[i];
			if(teleportPoints[i].name == "Teleport Point - Left 2")
				teleportPoint_left2 = teleportPoints[i];
			if(teleportPoints[i].name == "Teleport Point - Floor Right Center")
				teleportPoint_right_center = teleportPoints[i];
			if(teleportPoints[i].name == "Teleport Point - Floor Left Center")
				teleportPoint_left_center = teleportPoints[i];
		}
		
		List<PackedSprite> spritesSet = GetComponentsInChildren<PackedSprite>().ToList();
		for(int i = 0; i < spritesSet.Count; i++)
		{
			if(spritesSet[i].name == "Skill Curse")
				skillCurse = spritesSet[i];
			if(spritesSet[i].name == "Skill LifeDrain")
				skillLifeDrain = spritesSet[i];
			if(spritesSet[i].name == "More Skill Cast 1")
				moreSkillsSprite = spritesSet[i];
		}
		
		spritesSet = player.GetComponentsInChildren<PackedSprite>().ToList();
		for(int i = 0; i < spritesSet.Count; i++)
		{
			if(spritesSet[i].name == "Skill LifeDrain")
				skillLifeDrainPlayer = spritesSet[i];
		}
		
		curTeleportCooldown = teleportCooldown/2;
		curSummonCooldown = summonCooldown/2;
		curCurseCooldown = curseCooldown/2;
		curIcicleCooldown = icicleCooldown/2;
		curBlameCooldown = blameCooldown/2;
		curLifeDrainCooldown = lifeDrainCooldown/2;
		
		Instantiate(summonSmoke, transform.position, transform.rotation);
	}
	
	void Update () {
		decreaseCooldowns();

		if(lastCastTime != -1)
		{
			doAnimToDirection(lastAnimation);

			if(wasCasting)
			{
				if(wasCastingSummon)
				{
					if(isCentralized)
					{
						skillSummonInvoked();
					}
					else
					{
						if(positionizeSkillInvoked(teleportPoint_central))
							skillSummonInvoked();
					}
					return;
				}
				if(wasCastingIcicle)
				{
					skillIcicleInvoked();
					return;
				}
				if(wasCastingBlame)
				{
					if(isCentralized)
					{
						skillBlameInvoked();
					}
					else
					{
						if(positionizeSkillInvoked(teleportPoint_central))
							skillBlameInvoked();
					}
					return;
				}
				if(wasCastingCurse)
				{
					if(isNearPlayer)
						skillCurseInvoked();
					else
					{
						List<GameObject> unsafePoints = unsafeTeleportPoints();
						int pos = random.Next(0, unsafePoints.Count);
						if(positionizeSkillInvoked(unsafePoints[pos]))
							skillCurseInvoked();
					}
				}
				if(wasCastingLifeDrain)
				{
					if(isNearPlayer)
						skillLifeDrainInvoked();
					else
					{
						List<GameObject> unsafePoints = unsafeTeleportPoints();
						int pos = random.Next(0, unsafePoints.Count);
						if(positionizeSkillInvoked(unsafePoints[pos]))
							skillLifeDrainInvoked();
					}
				}
				if(wasCastingTeleport)
				{
					skillTeleportInvoked(null);
					return;
				}
			}
			if(Time.time - lastCastTime > sprites.GetAnim("Hitted (L)").GetDuration() && gotHit)
			{
				gotHit = false;
				lastCastTime = -1;
			}
			if(Time.time - lastCastTime > sprites.GetAnim("Dead (L)").GetDuration() && isDying)
			{
				sprites.StopAnim();
				sprites.Hide(true);
			}
			if(Time.time - lastCastTime > sound_dying.length && isDying)
			{
				Instantiate(victory);
				for(int i = 0; i < instancedMob.Count; i++)
					Destroy(instancedMob[i]);
				instancedMob.Clear();
				Destroy(gameObject);	
			}
			magicInterrupted = false;
			return;
		}
		
		if(curHP == 0)
		{
			doAnimToDirection("Dead");
			audio.PlayOneShot(sound_dying);
			Physics.IgnoreCollision(player.GetComponent<CharacterController>().collider, GetComponent<CharacterController>().collider, true);
			lastCastTime = Time.time;
			isDying = true;
			return;
		}
		
		if(curGlobalCooldown > 0)
		{
			doAnimToDirection("Standing");
			return;
		}
		
		if(curLifeDrainCooldown == 0 && curHP < maxHP*25/100)
		{
			wasCasting = true;
			wasCastingLifeDrain = true;
			lastCastTime = Time.time;
			if(nearPlayer(gameObject))
				isNearPlayer = true;
			else
				isNearPlayer = false;
		}
		else if(curSummonCooldown == 0)
		{
			wasCasting = true;
			wasCastingSummon = true;
			lastCastTime = Time.time;
			if(transform.position == teleportPoint_central.transform.position)
				isCentralized = true;
			else
				isCentralized = false;
		}
		else if(curCurseCooldown == 0)
		{
			wasCasting = true;
			wasCastingCurse = true;
			lastCastTime = Time.time;
			if(nearPlayer(gameObject))
				isNearPlayer = true;
			else
				isNearPlayer = false;
		}
		else if(curIcicleCooldown == 0)
		{
			wasCasting = true;
			wasCastingIcicle = true;
			lastCastTime = Time.time;
		}
		else if(curBlameCooldown == 0)
		{
			wasCasting = true;
			wasCastingBlame = true;
			lastCastTime = Time.time;
			if(transform.position == teleportPoint_central.transform.position)
				isCentralized = true;
			else
				isCentralized = false;
		}
		else if(curTeleportCooldown == 0 && nearPlayer(gameObject)) // there is no cooldown to attack, try to run
		{
			doAnimToDirection("Casting 1");
			wasCasting = true;
			wasCastingTeleport = true;
			lastCastTime = Time.time;
			if(currentDirection == Direction.Left)
				skillTeleport.DoAnim("Departure (L)");
			else
				skillTeleport.DoAnim("Departure (R)");
			audio.PlayOneShot(sound_teleport);
		}
		else
			doAnimToDirection("Standing");
	}
	
	private int controlLifeDrain = 0;
	private void skillLifeDrainInvoked()
	{
		float preCooldown;
		if(isNearPlayer)
			preCooldown = 0;
		else
			preCooldown = skillTeleport.GetAnim("Departure (L)").GetDuration() + 
				skillTeleport.GetAnim("Arrival (L)").GetDuration();
		
		float conjurationTime1 = sprites.GetAnim("Casting 1 (L)").GetDuration();
		float conjurationTime2 = 1;
		
		if(Time.time - lastCastTime < preCooldown + conjurationTime1)
		{
			doAnimToDirection("Casting 1");
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2*1 && !magicInterrupted)
		{
			doAnimToDirection("Casting 2");
			skillLifeDrainPlayer.GetComponent<LifeDrainPlayer>().activate();
			skillLifeDrain.DoAnim("Effect");
			float stolenHP;
			if(controlLifeDrain == 0)
			{
				audio.PlayOneShot(sound_drain);
				float stealDamage = 10;
				stolenHP = player.GetComponent<PlayerAttributes>().maxHP*stealDamage/100;
				player.GetComponent<Controls>().gotDamaged(stolenHP, Controls.Enemy.BossLifeDrain);
				curHP += stolenHP*1.5f;
				if(curHP > maxHP)
					curHP = maxHP;
				controlLifeDrain = 1;
				moreSkillsSprite.DoAnim("LifeDrain");
			}
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2*2 && !magicInterrupted)
		{
			doAnimToDirection("Casting 2");
			skillLifeDrain.DoAnim("Effect");
			float stolenHP;
			if(controlLifeDrain == 1)
			{
				float stealDamage = 10;
				stolenHP = player.GetComponent<PlayerAttributes>().maxHP*stealDamage/100;
				player.GetComponent<Controls>().gotDamaged(stolenHP, Controls.Enemy.BossLifeDrain);
				curHP += stolenHP*1.5f;
				if(curHP > maxHP)
					curHP = maxHP;
				controlLifeDrain = 2;
				moreSkillsSprite.DoAnim("LifeDrain");
			}
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2*3 && !magicInterrupted)
		{
			doAnimToDirection("Casting 2");
			skillLifeDrain.DoAnim("Effect");
			float stolenHP;
			if(controlLifeDrain == 2)
			{
				float stealDamage = 10;
				stolenHP = player.GetComponent<PlayerAttributes>().maxHP*stealDamage/100;
				player.GetComponent<Controls>().gotDamaged(stolenHP, Controls.Enemy.BossLifeDrain);
				curHP += stolenHP*1.5f;
				if(curHP > maxHP)
					curHP = maxHP;
				controlLifeDrain = 3;
				moreSkillsSprite.DoAnim("LifeDrain");
			}
		}
		else
		{
			skillLifeDrain.StopAnim();
			lastCastTime = -1;
			wasCasting = false;
			wasCastingLifeDrain = false;
			skillLifeDrainPlayer.GetComponent<LifeDrainPlayer>().desactivate();
			controlLifeDrain = 0;
			doAnimToDirection("Standing");
			curLifeDrainCooldown = lifeDrainCooldown;
			curGlobalCooldown = globalCooldown;
		}
	}
	
	private int controlBlame = 0;
	private bool controlBlame2 = false;
	private void skillBlameInvoked()
	{
		float preCooldown;
		if(isCentralized)
			preCooldown = 0;
		else
			preCooldown = skillTeleport.GetAnim("Departure (L)").GetDuration() + 
				skillTeleport.GetAnim("Arrival (L)").GetDuration();
		
		float conjurationTime1 = sprites.GetAnim("Casting 1 (L)").GetDuration();
		float conjurationTime2 = 0.2f;
		
		if(Time.time - lastCastTime < preCooldown + conjurationTime1)
		{
			doAnimToDirection("Casting 1");
			if(controlBlame2 == false)
			{
				if(currentDirection == Direction.Right)
					moreSkillsSprite.DoAnim("Blame (R)");
				else
					moreSkillsSprite.DoAnim("Blame (L)");
				controlBlame2 = true;
			}
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2*1)
		{
			doAnimToDirection("Casting 2");
			Vector3 position;
			if(controlBlame == 0)
			{
				position = new Vector3(transform.position.x+0.8f, transform.position.y+2, -2);
				Instantiate(skillBlame, position, transform.rotation);
				position = new Vector3(transform.position.x-0.8f, transform.position.y+2, -2);
				Instantiate(skillBlame, position, transform.rotation);
				controlBlame = 1;
			}
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2*2)
		{
			doAnimToDirection("Casting 2");
			Vector3 position;
			if(controlBlame == 1)
			{
				position = new Vector3(transform.position.x+3, transform.position.y+2, -2);
				Instantiate(skillBlame, position, transform.rotation);
				position = new Vector3(transform.position.x-3, transform.position.y+2, -2);
				Instantiate(skillBlame, position, transform.rotation);
				controlBlame = 2;
			}
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2*3)
		{
			doAnimToDirection("Casting 2");
			Vector3 position;
			if(controlBlame == 2)
			{
				position = new Vector3(transform.position.x+5, transform.position.y+2, -2);
				Instantiate(skillBlame, position, transform.rotation);
				position = new Vector3(transform.position.x-5, transform.position.y+2, -2);
				Instantiate(skillBlame, position, transform.rotation);
				controlBlame = 3;
			}
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2*4)
		{
			doAnimToDirection("Casting 2");
			Vector3 position;
			if(controlBlame == 3)
			{
				position = new Vector3(transform.position.x+7, transform.position.y+2, -2);
				Instantiate(skillBlame, position, transform.rotation);
				position = new Vector3(transform.position.x-6.8f, transform.position.y+2, -2);
				Instantiate(skillBlame, position, transform.rotation);
				controlBlame = 4;
			}
		}		
		else
		{
			lastCastTime = -1;
			wasCasting = false;
			wasCastingBlame = false;
			controlBlame = 0;
			controlBlame2 = false;
			curBlameCooldown = blameCooldown;
			doAnimToDirection("Standing");
			curGlobalCooldown = globalCooldown;
		}
	}
	
	private bool controlIcicle = false;
	private bool controlIcicle2 = false;
	private void skillIcicleInvoked()
	{
		float preCooldown = 0;
		float conjurationTime1 = sprites.GetAnim("Casting 1 (L)").GetDuration();
		float conjurationTime2 = 0.6f;

		if(Time.time - lastCastTime < preCooldown + conjurationTime1)
		{
			doAnimToDirection("Casting 1");
			if(controlIcicle2 == false)
			{
				moreSkillsSprite.DoAnim("Icicle");
				controlIcicle2 = true;
			}
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2)
		{
			doAnimToDirection("Casting 2");
			if(!controlIcicle)
			{
				for(int i = 0; i < 9; i++)
				{
					int pos = random.Next(0, (int)(skillIcicle_area.renderer.bounds.size.x*10000));
					
					int direction = random.Next(0, 2);
					GameObject skillIcicle;
					if(direction == 0)
						skillIcicle = skillIcicle_left;
					else
						skillIcicle = skillIcicle_right;
					
					var position = new Vector3(skillIcicle_area.transform.position.x-8+pos/10000,
						skillIcicle_area.transform.position.y,
						-2);
					Instantiate(skillIcicle, position, transform.rotation);
				}
				controlIcicle = true;	
			}
		}		
		else if(Time.time - lastCastTime > preCooldown + conjurationTime1 + conjurationTime2)
		{
			lastCastTime = -1;
			wasCasting = false;
			wasCastingIcicle = false;
			controlIcicle = false;
			controlIcicle2 = false;
			curIcicleCooldown = icicleCooldown;
			doAnimToDirection("Standing");
			curGlobalCooldown = globalCooldown;
		}
	}
	
	private bool controlCurse = false;
	private void skillCurseInvoked()
	{
		float preCooldown;
		if(isNearPlayer)
			preCooldown = 0;
		else
			preCooldown = skillTeleport.GetAnim("Departure (L)").GetDuration() + 
				skillTeleport.GetAnim("Arrival (L)").GetDuration();
		
		if(Time.time - lastCastTime < sprites.GetAnim("Casting 1 (L)").GetDuration() + preCooldown)
		{
			doAnimToDirection("Casting 1");
			skillCurse.DoAnim("Effect");
			if(!controlCurse)
			{
				audio.PlayOneShot(sound_curse);
				controlCurse = true;
			}
		}
		else
		{
			player.GetComponent<PlayerAttributes>().curse(4);
			Instantiate(bat);
			lastCastTime = -1;
			wasCasting = false;
			wasCastingCurse = false;
			controlCurse = false;
			doAnimToDirection("Standing");
			curCurseCooldown = curseCooldown;
			curGlobalCooldown = globalCooldown;
		}
	}
	
	private bool controlSummon2 = false;
	private void skillSummonInvoked()
	{
		float preCooldown;
		if(isCentralized)
			preCooldown = 0;
		else
			preCooldown = skillTeleport.GetAnim("Departure (L)").GetDuration() + 
				skillTeleport.GetAnim("Arrival (L)").GetDuration();
		
		float conjurationTime1 = sprites.GetAnim("Casting 1 (L)").GetDuration();
		float conjurationTime2 = 2;
		
		if(Time.time - lastCastTime < preCooldown + conjurationTime1)
		{
			doAnimToDirection("Casting 1");
			if(controlSummon2 == false)
			{
				GetComponentInChildren<SkillSummonUndead>().activate();
				audio.PlayOneShot(sound_summon);
				controlSummon2 = true;
			}
		}
		else if(Time.time - lastCastTime < preCooldown + conjurationTime1 + conjurationTime2)
		{
			doAnimToDirection("Casting 2");
		}		
		else if(Time.time - lastCastTime > preCooldown + conjurationTime1 + conjurationTime2)
		{
			for(int i = 0; i < instancedMob.Count; i++)
				Destroy(instancedMob[i]);
			instancedMob.Clear();
			
			GameObject enemy1 = null;
			GameObject enemy2 = null;
			if(currentSummonTurn == SummonTurn.Melee)
			{
				var position = new Vector3(teleportPoint_right_center.transform.position.x,
					teleportPoint_right_center.transform.position.y+0.5f,
					teleportPoint_right_center.transform.position.z);
				Instantiate(summonSmoke, teleportPoint_right_center.transform.position, transform.rotation);
				enemy1 = Instantiate(mobMelee, position, transform.rotation) as GameObject;
				position = new Vector3(teleportPoint_left_center.transform.position.x,
					teleportPoint_right_center.transform.position.y+0.5f,
					teleportPoint_right_center.transform.position.z);
				Instantiate(summonSmoke, teleportPoint_left_center.transform.position, transform.rotation);
				enemy2 = Instantiate(mobMelee, position, transform.rotation) as GameObject;
				currentSummonTurn = SummonTurn.Ranged;
			}
			else if(currentSummonTurn == SummonTurn.Ranged)
			{
				Instantiate(summonSmoke, teleportPoint_right2.transform.position, transform.rotation);
				enemy1 = Instantiate(mobRanged, teleportPoint_right2.transform.position, transform.rotation) as GameObject;
				Instantiate(summonSmoke, teleportPoint_left2.transform.position, transform.rotation);
				enemy2 = Instantiate(mobRanged, teleportPoint_left2.transform.position, transform.rotation) as GameObject;
				currentSummonTurn = SummonTurn.Mage;
			}
			else if(currentSummonTurn == SummonTurn.Mage)
			{
				Instantiate(summonSmoke, teleportPoint_right1.transform.position, transform.rotation);
				enemy1 = Instantiate(mobMage, teleportPoint_right1.transform.position, transform.rotation) as GameObject;
				Instantiate(summonSmoke, teleportPoint_left1.transform.position, transform.rotation);
				enemy2 = Instantiate(mobMage, teleportPoint_left1.transform.position, transform.rotation) as GameObject;
				currentSummonTurn = SummonTurn.Melee;
			}
			instancedMob.Add(enemy1);
			instancedMob.Add(enemy2);
			lastCastTime = -1;
			wasCasting = false;
			wasCastingSummon = false;
			controlSummon2 = false;
			curSummonCooldown = summonCooldown;
			GetComponentInChildren<SkillSummonUndead>().desactivate();
			doAnimToDirection("Standing");
			curGlobalCooldown = globalCooldown;
		}
	}
	
	private bool controlPositionizeTeleport = false;
	private bool positionizeSkillInvoked(GameObject teleportPoint)
	{
		if(Time.time - lastCastTime < 
				skillTeleport.GetAnim("Departure (L)").GetDuration() + 
				skillTeleport.GetAnim("Arrival (L)").GetDuration() + sprites.GetAnim("Casting 1 (L)").GetDuration())
		{
			if(Time.time - lastCastTime < skillTeleport.GetAnim("Departure (L)").GetDuration()/2)
			{
				doAnimToDirection("Casting 1");
				if(currentDirection == Direction.Left)
					skillTeleport.DoAnim("Departure (L)");
				else
					skillTeleport.DoAnim("Departure (R)");
				if(controlPositionizeTeleport == false)
				{
					audio.PlayOneShot(sound_teleport);
					controlPositionizeTeleport = true;
				}
				return false;
			}
			if(Time.time - lastCastTime < 
				skillTeleport.GetAnim("Departure (L)").GetDuration() + 
				skillTeleport.GetAnim("Arrival (L)").GetDuration())
			{
				skillTeleportInvoked(teleportPoint);
				return false;
			}
			if(Time.time - lastCastTime > 
				skillTeleport.GetAnim("Departure (L)").GetDuration() + 
				skillTeleport.GetAnim("Arrival (L)").GetDuration())
			{
				control = false;
				controlPositionizeTeleport = false;
				return true;
			}
			return false;
		}
		else
		{
			return true;
		}
	}
	
	private void skillTeleportInvoked(GameObject teleportPoint)
	{
		if(Time.time - lastCastTime > 
			skillTeleport.GetAnim("Departure (L)").GetDuration() + 
			skillTeleport.GetAnim("Arrival (L)").GetDuration())
		{
			if(teleportPoint == null)
			{
				lastCastTime = -1;
				wasCasting = false;
				curTeleportCooldown = teleportCooldown;
				wasCastingTeleport = false;
				curGlobalCooldown = globalCooldown;
			}
			control = false;
			return;
		}
		if(Time.time - lastCastTime > skillTeleport.GetAnim("Departure (L)").GetDuration() +
			skillTeleport.GetAnim("Arrival (L)").GetDuration()/1.5)
		{
			doAnimToDirection("Casting 2");
			return;
		}
		if(Time.time - lastCastTime > skillTeleport.GetAnim("Departure (L)").GetDuration() && !control)
		{
			if(teleportPoint != null)
				transform.position = teleportPoint.transform.position;
			else
			{
				List<GameObject> safePoints = safeTeleportPoints();
				int pos = random.Next(0, safePoints.Count);
				transform.position = safePoints[pos].transform.position;
			}
			if(currentDirection == Direction.Left)
				skillTeleport.DoAnim("Arrival (L)");
			else
				skillTeleport.DoAnim("Arrival (R)");
			control = true;
			return;
		}
		if(Time.time - lastCastTime > skillTeleport.GetAnim("Departure (L)").GetDuration()/2)
		{
			doAnimToDirection("Nothing");
			return;
		}
	}
	
	List<GameObject> safeTeleportPoints()
	{
		List<GameObject> safePoints = new List<GameObject>();
		GameObject point;
		for(int i = 0; i < teleportPoints.Count; i++)
		{
			point = teleportPoints[i];
			if(!nearPlayer(point))
				safePoints.Add(point);
		}
		return safePoints;
	}
	
	List<GameObject> unsafeTeleportPoints()
	{
		List<GameObject> unsafePoints = new List<GameObject>();
		GameObject point;
		for(int i = 0; i < teleportPoints.Count; i++)
		{
			point = teleportPoints[i];
			if(nearPlayer(point))
				unsafePoints.Add(point);
		}
		return unsafePoints;
	}
	
	bool nearPlayer(GameObject teleportPoint)
	{
		Vector3 enemyPosition = teleportPoint.transform.position;
		Vector3 playerPosition = player.transform.position;
		Vector3 vecResult = enemyPosition - playerPosition;
		float mag = vecResult.magnitude;
		float nearDistance = 4;

		if(mag < nearDistance)
			return true;
		else
			return false;
	}
	
	void decreaseCooldowns()
	{
		if(hitCooldown > 0)
			hitCooldown--;
		if(curTeleportCooldown > 0)
			curTeleportCooldown--;
		if(curSummonCooldown > 0)
			curSummonCooldown--;
		if(curCurseCooldown > 0)
			curCurseCooldown--;
		if(curIcicleCooldown > 0)
			curIcicleCooldown--;
		if(curBlameCooldown > 0)
			curBlameCooldown--;
		if(curGlobalCooldown > 0)
			curGlobalCooldown--;
		if(curLifeDrainCooldown > 0)
			curLifeDrainCooldown--;
	}
	
	void doAnimToDirection(string s)
	{
		if(s == "Nothing")
		{
			sprites.DoAnim(s);
			lastAnimation = s;
			return;
		}
		
		if(currentDirection == Direction.Right)
			sprites.DoAnim(s+" "+"(R)");
		else
			sprites.DoAnim(s+" "+"(L)");
		lastAnimation = s;
	}
	
	private void ignoreCollisionWithEnemies()
	{
		Collider enemyCollider = GetComponent<CharacterController>().collider;
		
		List<Collider> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList()
			.ConvertAll<Collider>(
				(x) => x.GetComponent<Collider>()
				).ToList();
		
		foreach(var enemy in enemies)
		{
			if(enemy != null && enemyCollider != enemy)
				Physics.IgnoreCollision(enemyCollider, enemy, true);
		}
	}
	
	private void receiveKnockback()
	{
		float knockback_value = 0.2f;
		if(currentDirection == Direction.Right)
			GetComponent<CharacterController>().Move(new Vector3(-knockback_value,0,0));
		else
			GetComponent<CharacterController>().Move(new Vector3(knockback_value,0,0));
	}
	
	private bool playerIsvertical()
	{
		Vector3 enemyPosition = transform.position;
		Vector3 playerPosition = player.transform.position;
		Vector3 vecResult = enemyPosition - playerPosition;
		
		if(vecResult.x > -0.1 && vecResult.x < 0.1)
			return true;
		return false;
	}
	
	private void facePlayer()
	{
		Vector3 enemyPosition = transform.position;
		Vector3 playerPosition = player.transform.position;
		Vector3 vecResult = enemyPosition - playerPosition;
		
		if(vecResult.x > 0 && currentDirection == Direction.Right)
			currentDirection = Direction.Left;
		if(vecResult.x < 0 && currentDirection == Direction.Left)
			currentDirection = Direction.Right;
	}
	
	private void gotDamaged(float damage, int cooldown)
	{
		if(lastAnimation == "Nothing")
			return;

		if(hitCooldown > 0)
			return;
		hitCooldown = cooldown;
		if(curHP == 0)
			return;
		curHP -= damage;
		showDamageTaken(damage);
		//audio.PlayOneShot(sound_damaged);
		if(curHP < 0)
		{
			curHP = 0;
		}
		if(!wasCasting)
		{
			lastCastTime = Time.time;
			gotHit = true;
			doAnimToDirection("Hitted");
		}
		else
			magicInterrupted = true;
		receiveKnockback();
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Paladin_AttackCollision") {
			gotDamaged(player.GetComponent<PlayerAttributes>().atkDamage, 30);
		}
		if(other.gameObject.tag == "Paladin_SkillVengeance") {
			gotDamaged(player.GetComponent<LevelUpSystem>().actives[1].skillValue, 30);
		}
		if(other.gameObject.tag == "Paladin_SkillJudgement") {
			gotDamaged(player.GetComponent<LevelUpSystem>().actives[2].skillValue, 30);
		}
	}
	
	void showDamageTaken(float damage)
	{
		Vector3 v = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x,transform.position.y+1,transform.position.z));
		GameObject dam = Instantiate(floatingDamage,new Vector3(v.x,v.y,-1),transform.rotation) as GameObject;
		dam.guiText.material.color = Color.white;
		dam.GetComponent<FloatingDamage>().duration = 0.4f;
		dam.guiText.text = damage.ToString();
	}
}
