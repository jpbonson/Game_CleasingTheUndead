using UnityEngine;
using System.Collections;

public class MageEnemyControl : EnemyControl {
	
	public bool isPatroling = true;
	public Transform thunderball;
	public Transform hurricane;
	public Transform lightning;
	public AudioClip soundCasting;
	
	private bool isDying = false;
	private bool wasCastingHurricane = false;
	private bool wasCastingLightning = false;
	public int hurricaneCooldown;
	public int currentHurricaneCooldown = 0;
	public int lightningCooldown;
	public int currentLightningCooldown = 0;
	
	// Use this for initialization
	void Start () {
		maxHP = Static.MageEnemyAttributes.maxHP;
		movementSpd = Static.MageEnemyAttributes.movementSpd;
		atkDamage = Static.MageEnemyAttributes.atkDamage;
		atkCooldown = Static.MageEnemyAttributes.atkCooldown;
		hurricaneCooldown = Static.MageEnemyAttributes.hurricaneCooldown;
		lightningCooldown = Static.MageEnemyAttributes.lightningCooldown;
		sightDistance = Static.MageEnemyAttributes.sightDistance;
		attackRange = Static.MageEnemyAttributes.attackRange;
		experience = Static.MageEnemyAttributes.experience;
		
		initialize();
		
		initialPosition = transform.position;
		
		if(Static.SceneLoader.deadEnemies.Contains(initialPosition))
			Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		decreaseCooldowns();
		if(currentHurricaneCooldown > 0)
			currentHurricaneCooldown--;
		if(currentLightningCooldown > 0)
			currentLightningCooldown--;
		
		regenHP();
		
		if(cantMove > 0)
		{
			doAnimToDirection(lastAnimation);
			cantMove--;
			if(cantMove == 20 && wasAttacking)
			{
				var rotation = GetComponent<CharacterController>().transform.rotation;
				rotation.SetLookRotation((player.transform.position - transform.position).normalized);
				if(currentDirection == Direction.Right)
				{
					Instantiate(thunderball,
								GetComponent<CharacterController>().transform.position + new Vector3(0.5f,0,0),
								rotation);
				}
				else
				{
					Instantiate(thunderball,
								GetComponent<CharacterController>().transform.position + new Vector3(-0.5f,0,0),
								rotation);
				}
			}
			if(cantMove == 20 && wasCastingHurricane)
			{
				var rotation = GetComponent<CharacterController>().transform.rotation;
				if(currentDirection == Direction.Right)
				{
					rotation.SetLookRotation(new Vector3(0,0,-0.5f));
					Instantiate(hurricane,
								GetComponent<CharacterController>().transform.position + new Vector3(0.5f,0,0),
								rotation);
				}
				else
				{
					rotation.SetLookRotation(new Vector3(0,0,0));
					Instantiate(hurricane,
								GetComponent<CharacterController>().transform.position + new Vector3(-0.5f,0,0),
								rotation);
				}
			}
			if(cantMove == 20 && wasCastingLightning)
			{
				var rotation = GetComponent<CharacterController>().transform.rotation;
				if(currentDirection == Direction.Right)
				{
					rotation.SetLookRotation(new Vector3(0,0,-0.5f));
					Instantiate(lightning,
								GetComponent<CharacterController>().transform.position + new Vector3(1,0,0),
								rotation);
				}
				else
				{
					rotation.SetLookRotation(new Vector3(0,0,0));
					Instantiate(lightning,
								GetComponent<CharacterController>().transform.position + new Vector3(-1,0,0),
								rotation);
				}
			}
			if(cantMove == 0 && wasAttacking)
			{
				wasAttacking = false;	
			}
			if(cantMove == 0 && wasCastingHurricane)
			{
				wasCastingHurricane = false;	
			}
			if(cantMove == 0 && wasCastingLightning)
			{
				wasCastingLightning = false;	
			}
			if(cantMove == 0 && isDying)
			{
				Destroy(gameObject);	
			}
			return;
		}
		
		if(curHP == 0)
		{
			dead();
			cantMove = 40;
			isDying = true;
			return;
		}
		
		if(player.GetComponent<PlayerAttributes>().endGame || !thereIsPaladinInRange())
		{
			if(isPatroling)
				patrol();
			else
				doAnimToDirection("Idle");
		}
	}
	
	protected override bool thereIsPaladinInRange()
	{
		Vector3 enemyPosition = transform.position;
		Vector3 playerPosition = player.transform.position;
		Vector3 vecResult = enemyPosition - playerPosition;
		float mag = vecResult.magnitude;
		
		if(mag < sightDistance)
		{
			if(playerIsvertical()  && currentAtkCooldown == 0)
			{
				doAttack();
				currentAtkCooldown = atkCooldown;
				wasAttacking = true;
				return true;
			}
			else if(mag < attackRange/3 && currentLightningCooldown == 0)
			{
				facePlayer();
				doAttack();
				currentLightningCooldown = lightningCooldown;
				wasCastingLightning = true;
				return true;
			}
			else if(mag < attackRange/2 && currentHurricaneCooldown == 0)
			{
				facePlayer();
				doAttack();
				currentHurricaneCooldown = hurricaneCooldown;
				wasCastingHurricane = true;
				return true;
			}
			else if(mag < attackRange  && currentAtkCooldown == 0)
			{
				facePlayer();
				doAttack();
				currentAtkCooldown = atkCooldown;
				wasAttacking = true;
				return true;
			}
			else if(mag < attackRange)
			{
				doAnimToDirection("Idle");
				return true;
			}
			else
			{
				facePlayer();
				doWalking(currentDirection);
				return true;
			}
		}
		
		return false;
	}
	
	private bool playerAtSameLevel()
	{
		Vector3 enemyPosition = transform.position;
		Vector3 playerPosition = player.transform.position;
		Vector3 vecResult = enemyPosition - playerPosition;

		if(vecResult.y > -0.15 && vecResult.y < -0.05) // same platform level
		{
			return true;
		}
		if(vecResult.y < -0.15 || vecResult.y > -0.05) // different platform level
		{
			return false;
		}
		return false;
	}
	
	protected override void doAttack()
	{
		cantMove = 60;
		doAnimToDirection("Attacking");
		audio.PlayOneShot(soundCasting);
	}
}
