using UnityEngine;
using System.Collections;

public class LifeDrainPlayer : MonoBehaviour {
	
	private PackedSprite sprites;
	private bool isActive = false;
	
	// Use this for initialization
	void Start () {
		sprites = GetComponent<PackedSprite>();
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive)
			sprites.DoAnim("Effect");
		else
			sprites.StopAnim();
	}
	
	public void activate()
	{
		isActive = true;	
	}
	
	public void desactivate()
	{
		isActive = false;	
	}
}
