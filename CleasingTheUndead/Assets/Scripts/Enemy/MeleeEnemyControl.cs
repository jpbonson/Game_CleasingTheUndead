using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MeleeEnemyControl : EnemyControl {
	
	public GameObject AttackCollision_Left;
	public GameObject AttackCollision_Right;
		
	private int contWaitPatrol = 100;
	
	public AudioClip soundAttacking;
	
	private bool isDying = false;
	
	// Use this for initialization
	void Start ()
	{
		maxHP = Static.MeleeEnemyAttributes.maxHP;
		movementSpd = Static.MeleeEnemyAttributes.movementSpd;
		atkDamage = Static.MeleeEnemyAttributes.atkDamage;
		atkCooldown = Static.MeleeEnemyAttributes.atkCooldown;
		sightDistance = Static.MeleeEnemyAttributes.sightDistance;
		attackRange = Static.MeleeEnemyAttributes.attackRange;
		experience = Static.MeleeEnemyAttributes.experience;
		
		initialize();
		
		initialPosition = transform.position;
		
		if(Static.SceneLoader.deadEnemies.Contains(initialPosition))
			Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		decreaseCooldowns();
		
		regenHP();
		
		if(cantMove > 0)
		{
			doAnimToDirection(lastAnimation);
			cantMove--;
			if(cantMove == 0 && wasAttacking)
			{
				stopAttacking();
				wasAttacking = false;	
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
		
		Vector3 enemyPosition = transform.position;
		Vector3 playerPosition = player.transform.position;
		Vector3 vecResult = enemyPosition - playerPosition;

		if(vecResult.y > -0.15 && vecResult.y < -0.05)// && vecResult.y < -0.1) // same platform level
		{
			contWaitPatrol = 100;
		}
		if(vecResult.y < -0.15 || vecResult.y > -0.05)// || vecResult.y > -0.1) // different platform level
		{
			if(contWaitPatrol > 0)
				contWaitPatrol--;
			else
				patrol();
		}
		else if(player.GetComponent<PlayerAttributes>().endGame || !thereIsPaladinInRange())
		{
			patrol();
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
			if(playerIsvertical())
			{
				doAnimToDirection("Standing");
				return true;	
			}
			if(mag < attackRange)
			{
				facePlayer();
				doAttack();
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
	
	protected override void doAttack()
	{
		if(currentAtkCooldown != 0)
			return;
		if(currentDirection == Direction.Right)
		{
			AttackCollision_Right.GetComponent<BoxCollider>().enabled = true;
		}
		else
		{
			AttackCollision_Left.GetComponent<BoxCollider>().enabled = true;
		}
		cantMove = 50;
		currentAtkCooldown = atkCooldown;
		wasAttacking = true;
		doAnimToDirection("Attacking");
		audio.PlayOneShot(soundAttacking);
	}
	
	void stopAttacking()
	{
		AttackCollision_Right.GetComponent<BoxCollider>().enabled = false;
		AttackCollision_Left.GetComponent<BoxCollider>().enabled = false;
	}
	
	
}
