using UnityEngine;
using System.Collections;

public class FloatingHealthBar : MonoBehaviour {

	public EnemyControl control;
	private float health;
	private float displayedHealth;
	private float maxHealth;
	public float healthBarWidthTotal;
	public float targetHeight = 55;
	public Vector3 targetScreenPosition;
	public GUIStyle healthBarStyle;
	public GUIStyle barTextStyle;
	
	// Use this for initialization
	void Start () {
	
		healthBarWidthTotal = 85;
	}
	
	void Update(){
		health = control.curHP;
		maxHealth = control.maxHP;
	}
	
	void OnGUI()
	{
		targetScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
	    targetScreenPosition.y = Screen.height - targetScreenPosition.y;
	
	    displayedHealth = Mathf.Lerp(displayedHealth, health, Time.deltaTime * 5);
		DrawBar(new Rect(targetScreenPosition.x - healthBarWidthTotal/2,targetScreenPosition.y - targetHeight,healthBarWidthTotal,9), displayedHealth, maxHealth, healthBarStyle, barTextStyle);
	}
	
	void DrawBar(Rect r, float health, float totalHealth, GUIStyle style, GUIStyle textStyle)
	{
		Rect actualHealth = new Rect(r.x,r.y, r.width * (health/totalHealth), r.height);
		if(health > totalHealth-1)
			health = totalHealth;
		GUI.Label(actualHealth,"", style);
		GUI.Label(r, string.Format("{0}/{1}", (int)health, (int)totalHealth), textStyle);
	}
}
