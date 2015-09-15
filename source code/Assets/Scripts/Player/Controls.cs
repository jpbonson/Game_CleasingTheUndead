using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Controls : MonoBehaviour {  
      
    public PackedSprite playerSprite;
	public PackedSprite skillSprite;
	public PackedSprite playerHitEffectSprite;
	public GameObject AttackCollision_Left;
	public GameObject AttackCollision_Right;
	public GameObject AttackCollision_Down;
	public GameObject stage1GUI;
	public GameObject stage2GUI;
	public GameObject stage3GUI;
	public GameObject floatingDamage;
	
	public Vector3 currentVelocity;
	public float gravity = -20;
	public float jumpingVelocity = 6;
	public float holdJumpModifier = 0.5f;
	public bool grounded;
	private List<Collider> platforms;
	private Collider playerCollider;
	
	public Direction lastMovement = Direction.Right;
	public enum Direction{Right, Left};
	public enum Enemy{Melee, Ranged, Mage, AcidWater, BossIcicle, BossBlame, BossLifeDrain};
	private bool wasAttacking = false;
	private bool wasCastingSkill1 = false;
	private bool wasCastingSkill2 = false;
	private int contDownKey = 0;
	private int atkAnimation_cooldown = 54;
	private int ajumpAnimation_cooldown = 30;
	
	public int cantMove = 0;
	public string lastAnimation = "Walking";
	private bool dying = false;
	private bool isDefending = false;
	private bool gotDamage = false;
	private bool isDead = false;
	protected int hitCooldown = 0;
	
	public Transform vengeanceWave_r;
	public Transform vengeanceWave_l;
	public Transform judgement_r;
	public Transform judgement_l;
	
	//public AudioClip sound_walking;
	public AudioClip sound_hitted_by_melee;
	public AudioClip sound_hitted_by_ranged;
	public AudioClip sound_hitted_by_mage;
	public AudioClip sound_defending_melee;
	public AudioClip sound_defending_ranged;
	public AudioClip sound_attacking;
	public AudioClip sound_attacking_jumping;
	public AudioClip sound_acid_water;
	
	private int waitDead = 20;
	private bool checkpoint = false;
	
	private bool isFloor = false;
	
	void Awake()
	{
		//if(Static.SceneLoader == null)
		//	return;
		// Debug code:
		if(Static.SceneLoader == null)
		{
			var sl = new GameObject().AddComponent<SceneLoader>();
			sl.nextScene = Application.loadedLevelName;	
			return;
		}
		
		if(Static.SceneLoader.position != Vector3.zero)
		{
			transform.position = Static.SceneLoader.position;
			Camera.main.transform.position = 
				new Vector3(0, Static.SceneLoader.position.y, -10);
		}
	}
	
	void Start()
	{
		playerCollider = GetComponent<CharacterController>().collider;
		
		platforms = GameObject.FindGameObjectsWithTag("TransposablePlatform").ToList()
			.ConvertAll<Collider>(
				(x) => x.GetComponent<Collider>()
				).ToList();
		
		ignoreCollisionWithBarriers();
		
		ignoreCollisionWithEnemies();
		
		if(Application.loadedLevelName == "Stage1" && checkpoint == false)
		{
			Instantiate(stage1GUI);
		}
		if(Application.loadedLevelName == "Stage2" && checkpoint == false && transform.position.y == 0)
		{
			Instantiate(stage2GUI);
		}
		if(Application.loadedLevelName == "Stage3" && checkpoint == false && transform.position.y == 0)
		{
			Instantiate(stage3GUI);
		}
		
		if(Application.loadedLevelName == "Stage3" && checkpoint == true)
		{
			Camera.main.GetComponent<FollowCam>().startOfLevel = Camera.main.GetComponent<FollowCam>().endOfLevel;
			var door = GameObject.FindGameObjectWithTag("Door");
			door.GetComponent<BoxCollider>().enabled = true;
		}
		
		
		
		//audio.clip = sound_walking;
		//audio.Play();
		//audio.loop = true;
	}
	
	void Update () {
		//if(lastAnimation != "Walking")
		//	audio.loop = false;
		//else
		//	audio.loop = true;
		
		if(GetComponent<PlayerAttributes>().curHP == 0)
			isDead = true;
		
		if(GetComponent<PlayerAttributes>().endGame)
		{
			doAnimToDirection("Dead");
			ignoreEnemyCollisions();
			waitDead--;
			if(waitDead == 0)
			{
				waitDead = 20;
				Static.SceneLoader.Load();
			}
			return;
		}
		
		if(isDead && !dying)
		{
			doAnimToDirection("Dying");
			cantMove = 10;
			dying = true;
			return;
		}
		
		if(hitCooldown > 0)
			hitCooldown--;
		
		if(cantMove > 0)
		{
			doAnimToDirection(lastAnimation);
			cantMove--;
			if(cantMove > 0 && isDefending && (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.K)) && grounded)
				cantMove = 10;
			if(cantMove == 0 && lastAnimation == "Dying")
				GetComponent<PlayerAttributes>().endGame = true;
			if(wasAttacking && !grounded)
			{
				doMovementWithoutAnimation();
				doAttack(true);
			}
			if(wasCastingSkill1 && !grounded)
			{
				doMovementWithoutAnimation();
			}
			if(cantMove == atkAnimation_cooldown/2 && wasCastingSkill2  && grounded)
			{
				if(lastMovement == Direction.Right)
					Instantiate(vengeanceWave_r,
							GetComponent<CharacterController>().transform.position + new Vector3(0,0.1f,0),
							GetComponent<CharacterController>().transform.rotation);
				else
					Instantiate(vengeanceWave_l,
							GetComponent<CharacterController>().transform.position + new Vector3(0,0.1f,0),
							GetComponent<CharacterController>().transform.rotation);
				
				wasCastingSkill2 = false;
			}
			if(cantMove == atkAnimation_cooldown/2 && wasAttacking  && grounded)
			{
				doAttack(true);
				audio.PlayOneShot(sound_attacking);
			}
			if(cantMove == 0 && wasAttacking)
			{
				doAttack(false);
				wasAttacking = false;
			}
			if(cantMove == 0 && wasCastingSkill1)
			{
				wasCastingSkill1 = false;
			}
			return;
		}
		
		if(currentVelocity.x == GetComponent<PlayerAttributes>().movementSpd)
		{
			lastMovement = Direction.Right;
		}
		else if(currentVelocity.x == -GetComponent<PlayerAttributes>().movementSpd)
		{
			lastMovement = Direction.Left;
		}
		
		chooseAction();
	}
	
	void chooseAction() // dying > dead > attack > defend > hitted > cast > walking/jumping
	{
		if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.J))
		{
			wasAttacking = true;
			if(!grounded)
			{
				doMovementWithoutAnimation();
				cantMove = ajumpAnimation_cooldown;
				doAnimToDirection("AJump");
				audio.PlayOneShot(sound_attacking_jumping);
			}
			else
			{
				cantMove = atkAnimation_cooldown;
				doAnimToDirection("Attacking");
			}
			return;
		}
		
		isDefending = false;
		if((Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.K))&& grounded)
		{
			doAnimToDirection("Defending");
			isDefending = true;
			cantMove = 10;
			return;
		}
		
		if(gotDamage)
		{
			gotDamage = false;
			doAnimToDirection("Hitted");
			cantMove = 8;
			return;
		}
		
		doMovement();
		
		return;
	}
	
	public void doSkill1()
	{
		wasCastingSkill1 = true;
		doAnimToDirection("Cast");
		cantMove = 10;
		isDefending = false;
	}
	
	public void doSkill2()
	{
		if(grounded)
		{
			wasCastingSkill2 = true;
			cantMove = atkAnimation_cooldown;
			doAnimToDirection("Attacking");
			isDefending = false;
			if(lastMovement == Direction.Right)
				skillSprite.DoAnim("Vengeance Charge (R)");
			else
				skillSprite.DoAnim("Vengeance Charge (L)");
		}
	}
	
	public void doSkill3()
	{
		if(lastMovement == Direction.Right)
			Instantiate(judgement_r,
					GetComponent<CharacterController>().transform.position + new Vector3(0.3f,-0.1f,0),
					GetComponent<CharacterController>().transform.rotation);
		else
			Instantiate(judgement_l,
					GetComponent<CharacterController>().transform.position + new Vector3(-0.1f,-0.1f,0),
					GetComponent<CharacterController>().transform.rotation);
		doAnimToDirection("Cast");
		cantMove = 50;
		isDefending = false;
	}
	
	public void doSkill4()
	{
		wasCastingSkill1 = true;
		doAnimToDirection("Cast");
		cantMove = 30;
		isDefending = false;
	}
	
	void doMovementWithoutAnimation()
	{
		// calculate currentVelocity.y
		float currentGravModifier = 1;
		
		var flags = GetComponent<CharacterController>().Move(currentVelocity * Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
		if(grounded)
		{
			currentVelocity.y = 0;
			if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
			{
				currentVelocity.y += jumpingVelocity;
				disablePlatformCollisions();
				grounded = false;
			}
			else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
			{
				if(isFloor == false)
				{
					disablePlatformCollisions();
					grounded = false;
					contDownKey = 20;
				}
			}
		}
		else
		{
			if(currentVelocity.y > 0) // jumping
			{
				currentGravModifier = holdJumpModifier;
			}
			else // falling
			{
				if(contDownKey == 0)
					enablePlatformCollisions();
				else
					contDownKey--;
			}
		}
		
		currentVelocity.y += gravity * currentGravModifier * Time.deltaTime;
		
		// calculate currentVelocity.x
		if(Input.GetAxis("Horizontal") > 0)
			currentVelocity.x = GetComponent<PlayerAttributes>().movementSpd;
		else if (Input.GetAxis("Horizontal") < 0)
			currentVelocity.x = -GetComponent<PlayerAttributes>().movementSpd;
		else
			currentVelocity.x = 0;
		
		if(contDownKey != 0)
			currentVelocity.x = 0;
		
		// applies new currentVelocity
		GetComponent<CharacterController>().Move(currentVelocity * Time.deltaTime +Vector3.forward * -transform.position.z);
	}
	
	void doMovement()
	{
		// calculate currentVelocity.y
		float currentGravModifier = 1;
		
		var flags = GetComponent<CharacterController>().Move(currentVelocity * Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;

		if(grounded)
		{
			currentVelocity.y = 0;
			if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
			{
				currentVelocity.y += jumpingVelocity;
				disablePlatformCollisions();
				grounded = false;
				doAnimToDirection("Jumping");
			}
			else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
			{
				if(isFloor == false)
				{
					disablePlatformCollisions();
					grounded = false;
					contDownKey = 20;
				}
			}
			else if(currentVelocity.x != 0)
			{
				doAnimToDirection("Walking");
			}
			else
			{
				doAnimToDirection("Standing");
			}
		}
		else
		{
			if(currentVelocity.y > 0) // jumping
			{
				currentGravModifier = holdJumpModifier;
				doAnimToDirection("Jumping");
			}
			else // falling
			{
				if(contDownKey == 0)
					enablePlatformCollisions();
				else
					contDownKey--;
				doAnimToDirection("Falling");
			}
		}
		
		currentVelocity.y += gravity * currentGravModifier * Time.deltaTime;
		
		// calculate currentVelocity.x
		if(Input.GetAxis("Horizontal") > 0)
			currentVelocity.x = GetComponent<PlayerAttributes>().movementSpd;
		else if (Input.GetAxis("Horizontal") < 0)
			currentVelocity.x = -GetComponent<PlayerAttributes>().movementSpd;
		else
			currentVelocity.x = 0;
		
		if(contDownKey != 0)
			currentVelocity.x = 0;
		
		// applies new currentVelocity
		GetComponent<CharacterController>().Move(currentVelocity * Time.deltaTime +Vector3.forward * -transform.position.z);
	}
	
	void doAttack(bool doAttack)
	{
		if(doAttack)
		{
			if(!grounded)
			{
				AttackCollision_Down.GetComponent<BoxCollider>().enabled = true;
			}
			else if(lastMovement == Direction.Right)
			{
				AttackCollision_Right.GetComponent<BoxCollider>().enabled = true;
			}
			else
			{
				AttackCollision_Left.GetComponent<BoxCollider>().enabled = true;
				
			}
		}
		else
		{
			AttackCollision_Down.GetComponent<BoxCollider>().enabled = false;
			AttackCollision_Right.GetComponent<BoxCollider>().enabled = false;
			AttackCollision_Left.GetComponent<BoxCollider>().enabled = false;
		}
	}
	
	void enablePlatformCollisions()
	{
		foreach(var platform in platforms)
		{
			if(platform != null)
				Physics.IgnoreCollision(playerCollider, platform, false);
		}	
	}
	
	void disablePlatformCollisions()
	{
		foreach(var platform in platforms)
		{
			if(platform != null)
				Physics.IgnoreCollision(playerCollider, platform, true);
		}
	}
	
	void doAnimToDirection(string s)
	{
		if(lastMovement == Direction.Right)
			playerSprite.DoAnim(s+" "+"(R)");
		else
			playerSprite.DoAnim(s+" "+"(L)");
		lastAnimation = s;
	}
	
	public void gotDamaged(float damage, Enemy enemy)
	{
		if(GetComponent<LevelUpSystem>().intervention)
			return;
		if(hitCooldown > 0)
			return;
		hitCooldown = 30;
		if(enemy == Enemy.Melee)
		{
			if(isDefending)
			{
				damage = damage/2;
				audio.PlayOneShot(sound_defending_melee);
				if(lastMovement == Direction.Right)
				{
					playerHitEffectSprite.PlayAnim("Melee Def (R)");
				}
				else
				{
					playerHitEffectSprite.PlayAnim("Melee Def (L)");
				}
				showDamageDefended(damage);
			}
			else
			{
				gotDamage = true;
				receiveKnockback();
				audio.PlayOneShot(sound_hitted_by_melee);
				if(lastMovement == Direction.Right)
				{
					playerHitEffectSprite.PlayAnim("Melee Hit (R)");
				}
				else
				{
					playerHitEffectSprite.PlayAnim("Melee Hit (L)");
				}
				showDamageTaken(damage);
			}
		}
		if(enemy == Enemy.Ranged)
		{
			if(isDefending)
			{
				damage = 0;
				audio.PlayOneShot(sound_defending_ranged);
				if(lastMovement == Direction.Right)
				{
					playerHitEffectSprite.PlayAnim("Ranged Def (R)");
				}
				else
				{
					playerHitEffectSprite.PlayAnim("Ranged Def (L)");
				}
				showDamageDefended(damage);
			}
			else
			{
				gotDamage = true;
				receiveKnockback();
				audio.PlayOneShot(sound_hitted_by_ranged);
				if(lastMovement == Direction.Right)
				{
					playerHitEffectSprite.PlayAnim("Ranged Hit (R)");
				}
				else
				{
					playerHitEffectSprite.PlayAnim("Ranged Hit (L)");
				}
				showDamageTaken(damage);
			}
		}
		if(enemy == Enemy.Mage)
		{
			gotDamage = true;
			receiveKnockback();
			audio.PlayOneShot(sound_hitted_by_mage);
			if(lastMovement == Direction.Right)
			{
				playerHitEffectSprite.PlayAnim("Mage Hit (R)");
			}
			else
			{
				playerHitEffectSprite.PlayAnim("Mage Hit (L)");
			}
			showDamageTaken(damage);
		}
		if(enemy == Enemy.BossIcicle)
		{
			gotDamage = true;
			float knockback_value = 0.2f;
			if(lastMovement == Direction.Right)
				GetComponent<CharacterController>().Move(new Vector3(0,-knockback_value,0));
			else
				GetComponent<CharacterController>().Move(new Vector3(0, knockback_value,0));
			showDamageTaken(damage);
		}
		if(enemy == Enemy.BossBlame)
		{
			gotDamage = true;
			receiveKnockback();
			showDamageTaken(damage);
		}
		if(enemy == Enemy.BossLifeDrain)
		{
			gotDamage = true;
			showDamageTaken(damage);
			if(lastMovement == Direction.Right)
			{
				playerHitEffectSprite.PlayAnim("Melee Hit (R)");
			}
			else
			{
				playerHitEffectSprite.PlayAnim("Melee Hit (L)");
			}
		}
		if(enemy == Enemy.AcidWater)
		{
			audio.PlayOneShot(sound_acid_water);
			showDamageTaken(damage);
		}
		
		GetComponent<PlayerAttributes>().curHP -= damage;
		if(GetComponent<PlayerAttributes>().curHP < 0)
			GetComponent<PlayerAttributes>().curHP = 0;
	}
	
	void showDamageTaken(float damage)
	{
		Vector3 v = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x,transform.position.y+1,transform.position.z));
		GameObject dam = Instantiate(floatingDamage,new Vector3(v.x,v.y,-1),transform.rotation) as GameObject;
		dam.guiText.material.color = Color.red;
		dam.guiText.text = damage.ToString();
	}
	
	void showDamageDefended(float damage)
	{
		Vector3 v = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x,transform.position.y+1,transform.position.z));
		GameObject dam = Instantiate(floatingDamage,new Vector3(v.x,v.y,-1),transform.rotation) as GameObject;
		dam.guiText.material.color = Color.blue;
		dam.guiText.text = damage.ToString();
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "MeleeEnemy_AttackCollision") {
			gotDamaged(Static.MeleeEnemyAttributes.atkDamage, Enemy.Melee);
		}
		if(other.gameObject.tag == "MageEnemy_Thunderball") {
			gotDamaged(Static.MageEnemyAttributes.atkDamage, Enemy.Mage);
			Destroy(other.gameObject);
		}
		if(other.gameObject.tag == "MageEnemy_Hurricane") {
			if(lastMovement == Direction.Right)
				GetComponent<CharacterController>().Move(new Vector3(-0.5f,0.3f,0));
			else
				GetComponent<CharacterController>().Move(new Vector3(0.5f,0.3f,0));
		}
		if(other.gameObject.tag == "MageEnemy_Lightning") {
			gotDamaged(Static.MageEnemyAttributes.lightningDamage, Enemy.Mage);
		}
		if(other.gameObject.tag == "BossSkill_Blame") {
			gotDamaged(BossControl.blameDamage, Enemy.BossBlame);	
		}
		if(other.gameObject.tag == "Floor") {
			isFloor = true;
		}
	}
	
	void OnTriggerStay(Collider other) {
		if(other.gameObject.tag == "MageEnemy_Lightning") {
			gotDamaged(Static.MageEnemyAttributes.lightningDamage, Enemy.Mage);
		}
		if(other.gameObject.tag == "AcidWater") {
			gotDamaged(GetComponent<PlayerAttributes>().maxHP*0.1f, Enemy.AcidWater);
		}
		if(other.gameObject.tag == "StagePortal") {
			loadNextLevel();
		}
		if(other.gameObject.tag == "Checkpoint" && !checkpoint) {
			createCheckpoint();
			checkpoint = true;
		}
		if(other.gameObject.tag == "BossSkill_Icicle") {
			gotDamaged(BossControl.icicleDamage, Enemy.BossIcicle);	
		}
	}
	
	void OnTriggerExit(Collider other) {
		if(other.gameObject.tag == "Floor") {
			isFloor = false;
		}
	}
	
	void loadNextLevel()
	{
		if(Application.loadedLevelName == "Stage1")
			Static.SceneLoader.nextScene = "Stage2";
		else if(Application.loadedLevelName == "Stage2")
		{
			Static.SceneLoader.position = Vector3.zero;
			Static.SceneLoader.nextScene = "Stage3";
		}
		
		savePlayerData();
		Static.SceneLoader.Load();
	}
	
	void savePlayerData()
	{
		var sl = Static.SceneLoader;
		
		sl.experience = GetComponent<LevelUpSystem>().experience;
		sl.level = GetComponent<LevelUpSystem>().level;
		sl.skillPoints = GetComponent<LevelUpSystem>().skillPoints;
		
		sl.skillLvlPassive0 = GetComponent<LevelUpSystem>().passives[0].level;
		sl.skillLvlPassive1 = GetComponent<LevelUpSystem>().passives[1].level;
		
		sl.skillLvlActive0 = GetComponent<LevelUpSystem>().actives[0].level;
		sl.skillLvlActive1 = GetComponent<LevelUpSystem>().actives[1].level;
		sl.skillLvlActive2 = GetComponent<LevelUpSystem>().actives[2].level;
		sl.skillLvlActive3 = GetComponent<LevelUpSystem>().actives[3].level;
	}
	
	void createCheckpoint()
	{
		savePlayerData();
		Static.SceneLoader.position = transform.position;
		Static.SceneLoader.deadEnemies.AddRange(Static.DeadEnemies.deadEnemies);
		Checkpoint.showCheckpointMessage = true;
		if(Application.loadedLevelName == "Stage3")
		{
			Camera.main.GetComponent<FollowCam>().prepareForBossFight();
			var door = GameObject.FindGameObjectWithTag("Door");
			door.GetComponent<BoxCollider>().enabled = true;
		}
	}
	
	void receiveKnockback()
	{
		float knockback_value = 0.2f;
		if(lastMovement == Direction.Right)
			GetComponent<CharacterController>().Move(new Vector3(-knockback_value,0,0));
		else
			GetComponent<CharacterController>().Move(new Vector3(knockback_value,0,0));
	}
	
	void ignoreCollisionWithBarriers()
	{
		List<Collider> barriers = GameObject.FindGameObjectsWithTag("Barrier").ToList()
			.ConvertAll<Collider>(
				(x) => x.GetComponent<Collider>()
				).ToList();
		
		foreach(var barrier in barriers)
		{
			if(barrier != null && playerCollider != barrier)
				Physics.IgnoreCollision(playerCollider, barrier, true);
		}	
	}
	
	void ignoreEnemyCollisions()
	{
		List<Collider> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList()
			.ConvertAll<Collider>(
				(x) => x.GetComponent<Collider>()
				).ToList();
		
		foreach(var enemy in enemies)
		{
			if(enemy != null && playerCollider != enemy)
				Physics.IgnoreCollision(playerCollider, enemy, true);
		}
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
		
		GameObject bossCollider = GameObject.FindGameObjectWithTag("Boss");
		if(bossCollider != null)
			Physics.IgnoreCollision(enemyCollider, bossCollider.collider, true);
	}
}