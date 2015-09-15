using UnityEngine;
using System.Collections;

public class SkillSummonUndead : MonoBehaviour {
	
	private float fade = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(fade < 1)
		{
			fade += Time.deltaTime*1;
			if(fade >= 1)
			{
				fade = 1;
			}
		}
		GetComponent<MeshRenderer>().material.color = new Color(GetComponent<MeshRenderer>().material.color.r,
			GetComponent<MeshRenderer>().material.color.g, GetComponent<MeshRenderer>().material.color.b, fade);
	}
	
	public void activate()
	{
		GetComponent<MeshRenderer>().enabled = true;
		fade = 0;
	}
	
	public void desactivate()
	{
		GetComponent<MeshRenderer>().enabled = false;
	}
}
