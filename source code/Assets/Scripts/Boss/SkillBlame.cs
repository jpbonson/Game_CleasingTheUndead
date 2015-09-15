using UnityEngine;
using System.Collections;

public class SkillBlame : MonoBehaviour {
	
	private float startTime;
	private PackedSprite sprites;
	private bool test = false;
	public AudioClip soundEffect;
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		sprites = GetComponent<PackedSprite>();
		audio.clip = soundEffect;
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - startTime < sprites.GetAnim("Effect").GetDuration())
		{
			sprites.DoAnim("Effect");
		}
		else if(Time.time - startTime > sprites.GetAnim("Effect").GetDuration())
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
