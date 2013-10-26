using UnityEngine;
using System.Collections;

public class ScaleTexture : MonoBehaviour {
	public float textureUnitsPerWorldUnit_x;
	public float textureUnitsPerWorldUnit_y;
	// Use this for initialization
	void Start () {
		foreach(Renderer r in GetComponentsInChildren<Renderer>()){
			r.material.mainTextureScale = new Vector2(r.transform.lossyScale.x * textureUnitsPerWorldUnit_x,
				r.transform.lossyScale.y * textureUnitsPerWorldUnit_y);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
