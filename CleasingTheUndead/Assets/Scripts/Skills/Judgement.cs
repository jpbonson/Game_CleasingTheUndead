using UnityEngine;
using System.Collections;

public class Judgement : MonoBehaviour {

	private int contFrames = 0;
	public bool goRight;
	public AudioClip wave;
	private bool test = false;
	
	// Use this for initialization
	void Start () {
		audio.clip = wave;
	}
	
	// Update is called once per frame
	void Update () {
		if(contFrames == 45)
		{
			Destroy(gameObject);
			return;
		}
		if(contFrames > 30)
		{
			var collider = GetComponent<BoxCollider>();
			if(collider.size.x < 5)
			{
				collider.size = new Vector3(collider.size.x+1,collider.size.y+1,0);
			}
			audio.Play();
		}
		contFrames++;
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
		if(goRight)
			GetComponent<PackedSprite>().DoAnim("Effect (R)");
		else
			GetComponent<PackedSprite>().DoAnim("Effect (L)");
	}
}
