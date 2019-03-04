using UnityEngine;
using System.Collections;

public class Torpedo : MonoBehaviour {

	public float accel;
	public float maxSpeed;

	private Vector2 movement;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		movement.y -= accel * Time.deltaTime;

		if(movement.y < -maxSpeed)
		{
			accel = 0;
		}

		transform.Translate(movement);

		// 화면 아래로 나간 것 제거
		if (transform.position.y < -100)
		{
			Destroy(this.gameObject);
		}
	}
}
