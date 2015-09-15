using UnityEngine;
using System.Collections;

public class BossHealthBar : MonoBehaviour {

	private BossControl control;
	private float health;
	private float displayedHealth;
	private float maxHealth;
	public Vector3 targetScreenPosition;
	public GUIStyle healthBarStyle;
	public GUIStyle barTextStyle;
	
	// Use this for initialization
	void Start ()
	{
		control = GetComponent<BossControl>();
	}
	
	void Update()
	{
		health = control.curHP;
		maxHealth = control.maxHP;
	}
	
	void OnGUI()
	{
		displayedHealth = Mathf.Lerp(displayedHealth, health, Time.deltaTime * 5);
		DrawBar(new Rect(30,Screen.height-50,Screen.width-60,30), displayedHealth, maxHealth, healthBarStyle, barTextStyle);
	}
	
	void DrawBar(Rect r, float health, float totalHealth, GUIStyle style, GUIStyle textStyle)
	{
		Rect actualHealth = new Rect(r.x,r.y, r.width * (health/totalHealth), r.height);
		GUI.Label(actualHealth,"", style);
	}
}
