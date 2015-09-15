using UnityEngine;
using System.Collections;

public class VengeanceWave : MonoBehaviour {
	
	public Vector3 currentVelocity;
	public bool goRight;
	private int contFrames = 100;
	public AudioClip wave;
	
	// Use this for initialization
	void Start () {
		currentVelocity.x = 5;
		audio.clip = wave;
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(contFrames == 0)
		{
			Destroy(gameObject);
			return;
		}

		contFrames--;
		GetComponent<PackedSprite>().DoAnim("Wave");
		if(goRight)
			transform.Translate(currentVelocity * Time.deltaTime +Vector3.forward * -transform.position.z);
		else
			transform.Translate(-currentVelocity * Time.deltaTime +Vector3.forward * -transform.position.z);
	}
	
	void OnTriggerEnter(Collider other)
	{
    	if(other.gameObject.tag == "Barrier") {
			Destroy(gameObject);
		}
	}
}
