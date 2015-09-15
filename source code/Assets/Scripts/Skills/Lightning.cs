using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {

	public Vector3 currentVelocity;
	private int contFrames = 400;
	public AudioClip soundEffect;
	private bool test = false;
	
	// Use this for initialization
	void Start ()
	{
		currentVelocity.x = 3;
		audio.clip = soundEffect;
		audio.Play();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(contFrames == 0)
		{
			Destroy(gameObject);
			return;
		}
		contFrames--;
		GetComponent<PackedSprite>().DoAnim("Lightning");
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
