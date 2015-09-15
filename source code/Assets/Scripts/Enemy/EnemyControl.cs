using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyControl : MonoBehaviour {
	
	public PackedSprite enemySprite;
	public GameObject player;
	public Transform wayPointA;
	public Transform wayPointB;
	public GameObject floatingDamage;
	
	public float maxHP;
	public float curHP;
	protected float hpRegen = 2;
	public float movementSpd;
	public int atkDamage;
	public int atkCooldown;
	public int sightDistance;
	public int attackRange;
	public int experience;
	
	public Direction currentDirection = Direction.Right;
	public enum Direction{Right, Left};
	public Vector3 currentVelocity;
	public float gravity = -20;
	protected int currentAtkCooldown = 0;
	protected bool wasAttacking = false;
	protected string lastAnimation = "Walking";
	protected int cantMove = 0;
	protected bool gaveExp = false;
	protected int hitCooldown = 0;
	
	protected Vector3 initialPosition;
	
	public AudioClip soundDamaged;
	
	// Use this for initialization
	protected void initialize () {
		ignoreCollisionWithEnemies();
		curHP = maxHP;
		player = GameObject.FindGameObjectWithTag("Player");;
	}
	
	protected void regenHP()
	{
		if(curHP > 0)
		{
			curHP = Mathf.Clamp(curHP+maxHP*hpRegen/100*Time.deltaTime, 0, maxHP);
		}
		else
		{
			curHP = 0;
		}	
	}
	
	protected void decreaseCooldowns()
	{
		if(currentAtkCooldown > 0)
			currentAtkCooldown--;
		
		if(hitCooldown > 0)
			hitCooldown--;
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
		
		GameObject playerCollider = GameObject.FindGameObjectWithTag("Player");
		if(playerCollider != null)
			Physics.IgnoreCollision(enemyCollider, playerCollider.collider, true);
	}
	
	protected void patrol()
	{
		Vector3 waypoint;
		if(currentDirection == Direction.Right)
			waypoint = wayPointB.position;
		else
			waypoint = wayPointA.position;
		
		Vector3 enemyPosition = transform.position;
		Vector3 vecResult = enemyPosition - waypoint;
		
		if(vecResult.x > 0 && currentDirection == Direction.Right)
			currentDirection = Direction.Left;
		if(vecResult.x < 0 && currentDirection == Direction.Left)
			currentDirection = Direction.Right;
		doWalking(currentDirection);
	}
	
	protected void doWalking(Direction direction)
	{
		if(direction == Direction.Right)
		{
			currentVelocity.x = movementSpd;
			currentDirection = Direction.Right;
		}
		else
		{
			currentVelocity.x = -movementSpd;
			currentDirection = Direction.Left;
		}
		
		currentVelocity.y += gravity * Time.deltaTime;
		
		doAnimToDirection("Walking");
		GetComponent<CharacterController>().Move(currentVelocity * Time.deltaTime +Vector3.forward * -transform.position.z);	
	}
	
	protected void doAnimToDirection(string s)
	{
		if(currentDirection == Direction.Right)
			enemySprite.DoAnim(s+" "+"(R)");
		else
			enemySprite.DoAnim(s+" "+"(L)");
		lastAnimation = s;
	}
	
	protected void receiveKnockback()
	{
		float knockback_value = 0.2f;
		if(currentDirection == Direction.Right)
			GetComponent<CharacterController>().Move(new Vector3(-knockback_value,0,0));
		else
			GetComponent<CharacterController>().Move(new Vector3(knockback_value,0,0));
	}
	
	protected bool playerIsvertical()
	{
		Vector3 enemyPosition = transform.position;
		Vector3 playerPosition = player.transform.position;
		Vector3 vecResult = enemyPosition - playerPosition;
		
		if(vecResult.x > -0.1 && vecResult.x < 0.1)
			return true;
		return false;
	}
	
	protected void facePlayer()
	{
		Vector3 enemyPosition = transform.position;
		Vector3 playerPosition = player.transform.position;
		Vector3 vecResult = enemyPosition - playerPosition;
		
		if(vecResult.x > 0 && currentDirection == Direction.Right)
			currentDirection = Direction.Left;
		if(vecResult.x < 0 && currentDirection == Direction.Left)
			currentDirection = Direction.Right;
	}
	
	protected void gotDamaged(float damage, int cooldown)
	{
		if(hitCooldown > 0)
			return;
		hitCooldown = cooldown;
		if(curHP == 0)
			return;
		curHP -= damage;
		showDamageTaken(damage);
		if(soundDamaged != null)
			audio.PlayOneShot(soundDamaged);
		if(curHP < 0)
		{
			curHP = 0;
		}
		if(!wasAttacking)
		{
			cantMove = 30;
			doAnimToDirection("Hitted");
		}
		receiveKnockback();
	}
	
	protected void dead()
	{
		doAnimToDirection("Dead");
		Physics.IgnoreCollision(player.GetComponent<CharacterController>().collider, GetComponent<CharacterController>().collider, true);
		if(gaveExp == false)
		{
			gaveExp = true;
			player.GetComponent<LevelUpSystem>().increase_experience(experience);
			if(Static.DeadEnemies != null)
				Static.DeadEnemies.Add(initialPosition);
		}
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
	
	protected abstract void doAttack();
	protected abstract bool thereIsPaladinInRange();
}
