using UnityEngine;
using System.Collections;

public class Thunderball : MonoBehaviour {

	public Vector3 currentVelocity;
	private int contFrames = 140;
	public AudioClip wave;
	
	// Use this for initialization
	void Start ()
	{
		currentVelocity = transform.forward *5;
		audio.clip = wave;
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
		GetComponentInChildren<PackedSprite>().DoAnim("Thunderball");
		transform.Translate(currentVelocity * Time.deltaTime +Vector3.forward * -transform.position.z, Space.World);
	}
}
