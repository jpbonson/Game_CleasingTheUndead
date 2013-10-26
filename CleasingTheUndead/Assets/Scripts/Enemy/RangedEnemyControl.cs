using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RangedEnemyControl : EnemyControl {
	
	public bool isPatroling = true;
	public AudioClip soundShooting;
	public AudioClip soundDying;
	
	private bool isDying = false;
	
	// Use this for initialization
	void Start ()
	{
		maxHP = Static.RangedEnemyAttributes.maxHP;
		movementSpd = Static.RangedEnemyAttributes.movementSpd;
		atkDamage = Static.RangedEnemyAttributes.atkDamage;
		atkCooldown = Static.RangedEnemyAttributes.atkCooldown;
		sightDistance = Static.RangedEnemyAttributes.sightDistance;
		attackRange = Static.RangedEnemyAttributes.attackRange;
		experience = Static.RangedEnemyAttributes.experience;
		
		initialize();
		
		initialPosition = transform.position;
		
		if(Static.SceneLoader.deadEnemies.Contains(initialPosition))
			Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetComponent<CharacterController>().Move(new Vector3(0,0,0));
		
		decreaseCooldowns();
		
		regenHP();
		
		if(cantMove > 0)
		{
			doAnimToDirection(lastAnimation);
			cantMove--;
			if(cantMove == 20 && wasAttacking)
			{
				player.GetComponent<Controls>().gotDamaged(atkDamage, Controls.Enemy.Ranged);
			}
			if(cantMove == 0 && wasAttacking)
			{
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
			audio.volume = 0.5f;
			audio.PlayOneShot(soundDying);
			cantMove = 40;
			isDying = true;
			return;
		}
		
		if(player.GetComponent<PlayerAttributes>().endGame || !thereIsPaladinInRange())
		{
			if(isPatroling)
				patrol();
			else
				doAnimToDirection("Walking");
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
				doAttack();
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
		{
			doAnimToDirection("Walking");
			return;
		}

		cantMove = 60;
		currentAtkCooldown = atkCooldown;
		wasAttacking = true;
		doAnimToDirection("Attacking");
		audio.PlayOneShot(soundShooting);
	}
}
