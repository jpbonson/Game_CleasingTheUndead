using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FollowCam : MonoBehaviour {

	public Transform target;
	public GameObject boss;
	public AudioClip bossMusic;
	
	public float maxCharScreenSpace = 0.66f;
	public float minCharScreenSpace = 0.33f;
	
	public GameObject destroy1;
	public GameObject destroy2;
	
	public float correctionSpeed = 7; 
	public float startOfLevel;
	public float endOfLevel;
	
	private bool correct1 = false;
	private bool correct2 = false;
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 pos = transform.position;
		
		if(correct1)
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<Controls>().lastAnimation = "Standing";
			startOfLevel = 32;
			correct1 = false;
			correct2 = true;
		}
		
		if(correct2)
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<Controls>().cantMove = 5;
			startOfLevel += 3*Time.deltaTime;
			if(startOfLevel >= endOfLevel)
			{
				startOfLevel = endOfLevel;
				correct2 = false;
				Instantiate(boss);
				Destroy (destroy1);
				Destroy (destroy2);
				var bosshp = GameObject.FindGameObjectWithTag("Boss");
				bosshp.GetComponent<BossHealthBar>().enabled = true;
				Physics.IgnoreCollision(player.collider, bosshp.collider, true);
			}
		}
		
		pos.x = Mathf.Clamp (target.position.x,startOfLevel,endOfLevel);
		
		// Find the extents of the character from the renderers
		var listOfRenderersInTarget = target.GetComponentsInChildren<Renderer>().ToList<Renderer>();
		var minimum = listOfRenderersInTarget.ConvertAll<float>((x)=> x.bounds.min.y).Min();
		var maximum = listOfRenderersInTarget.ConvertAll<float>((x)=> x.bounds.max.y).Max();

		Camera cam = GetComponent<Camera>();
		Vector2 topOfCharacterScreenSpace = cam.WorldToViewportPoint(new Vector3(target.position.x, maximum,target.position.z));
		Vector2 botOfCharacterScreenSpace = cam.WorldToViewportPoint(new Vector3(target.position.x, minimum,target.position.z));
		
		if(topOfCharacterScreenSpace.y > maxCharScreenSpace){
			pos.y += correctionSpeed*Time.deltaTime;
		}
		if(botOfCharacterScreenSpace.y < minCharScreenSpace){
			pos.y -= correctionSpeed* Time.deltaTime;
		}
		
		transform.position = pos;
	}
	
	public void prepareForBossFight()
	{
		correct1 = true;
		var music = GameObject.FindGameObjectWithTag("BackgroundMusic");
		music.GetComponent<AudioSource>().clip = bossMusic;
		music.GetComponent<AudioSource>().Play();
	}
}