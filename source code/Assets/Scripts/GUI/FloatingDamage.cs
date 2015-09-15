using UnityEngine;
using System.Collections;

public class FloatingDamage : MonoBehaviour {

	public float scroll = 0.1f; // scrolling velocity
	public float duration = 0.8f; // time to die
	private float alpha;
	
	// Use this for initialization
	void Start ()
	{
		alpha = 1;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (alpha>0)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + scroll*Time.deltaTime, transform.position.z);
    		alpha -= Time.deltaTime/duration; 
    		guiText.material.color = new Color(guiText.material.color.r, 
				guiText.material.color.g, guiText.material.color.b, alpha);   
		}
		else
		{
    		Destroy(transform.gameObject);
		}
	}
}
