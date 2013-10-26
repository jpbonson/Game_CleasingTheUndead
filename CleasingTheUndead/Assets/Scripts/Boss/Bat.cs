using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour {
	
	private GameObject player;
	private Transform target;
	private float correctionSpeed = 7;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		target = player.transform;
		transform.position = new Vector3(player.transform.position.x,
			player.transform.position.y+0.7f, player.transform.position.z);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 pos = transform.position;
		if((target.position.x+0.5f) > pos.x){
			pos.x += correctionSpeed*Time.deltaTime;
		}
		if((target.position.x-0.5f) < pos.x){
			pos.x -= correctionSpeed* Time.deltaTime;
		}
		if((target.position.y+0.7f) > pos.y){
			pos.y += correctionSpeed*Time.deltaTime;
		}
		if((target.position.y+0.7f) < pos.y){
			pos.y -= correctionSpeed* Time.deltaTime;
		}
		
		transform.position = pos;
		
		if(player.GetComponent<Controls>().lastMovement == Controls.Direction.Right)
			GetComponent<PackedSprite>().DoAnim("Bat (R)");
		else
			GetComponent<PackedSprite>().DoAnim("Bat (L)");
		
		if(!player.GetComponent<PlayerAttributes>().isCursed)
		{
			Destroy (gameObject);	
		}
	}
}
