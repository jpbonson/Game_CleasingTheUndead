using UnityEngine;
using System.Collections;

public class Hurricane : MonoBehaviour {

	public Vector3 currentVelocity;
	private int contFrames = 100;
	public AudioClip soundEffect;
	
	// Use this for initialization
	void Start ()
	{
		currentVelocity.x = 3;
		audio.volume = 0.5f;
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
		GetComponent<PackedSprite>().DoAnim("Hurricane");
		transform.Translate(-currentVelocity * Time.deltaTime +Vector3.forward * -transform.position.z);
	}
}
