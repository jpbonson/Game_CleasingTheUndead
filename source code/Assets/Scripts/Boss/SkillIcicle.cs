using UnityEngine;
using System.Collections;

public class SkillIcicle : MonoBehaviour {

	private float startTime;
	private PackedSprite sprites;
	private float duration = 5;
	private bool test = false;
	private bool control = false;
	public AudioClip soundEffect;
	public AudioClip soundEffectDestroy;
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		sprites = GetComponent<PackedSprite>();
		audio.clip = soundEffect;
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - startTime < sprites.GetAnim("Start").GetDuration())
		{
			sprites.DoAnim("Start");
		}
		else if(Time.time - startTime > sprites.GetAnim("Start").GetDuration() + duration &&
				Time.time - startTime < sprites.GetAnim("Start").GetDuration() + duration + 
											sprites.GetAnim("End").GetDuration())
		{
			sprites.DoAnim("End");
			GetComponent<BoxCollider>().enabled = false;
			if(!control)
			{
				audio.clip = soundEffectDestroy;
				audio.volume = 0.7f;
				audio.Play();
				control = true;
			}
		}
		else if(Time.time - startTime > sprites.GetAnim("Start").GetDuration() + duration + 
											sprites.GetAnim("End").GetDuration()+1)
		{
			Destroy (gameObject);	
		}
		
		if(test)
		{
			transform.position = transform.position + new Vector3(0,0,0.1f);
			test = false;
		}
		else
		{
			transform.position = transform.position + new Vector3(0,0,-0.1f);
			test = true;
		}
	}
}
