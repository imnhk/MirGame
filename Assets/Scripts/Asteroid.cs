using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour
{

	public float speed_y;
	public float speed_x;
	public float speed_rotate;
	public float scale;

	Vector2 moveVector;

	// Start 대신 이걸로 GameManager에서 초기화 함
	public void SetAsteroid(float dx, float dy, float drotate, float size)
	{
		speed_x = dx;
		speed_y = dy;
		speed_rotate = drotate;
		scale = size;
		moveVector = new Vector2(speed_x, -speed_y);
		transform.localScale = new Vector2(scale, scale);
	}

	// Use this for initialization
	void Start()
	{
		moveVector = new Vector2(speed_x, -speed_y);

	}

	// Update is called once per frame
	void Update()
	{

		// 이동
		MoveObject();

		// 밑으로 나간 것 제거
		if (transform.position.y < -100)
		{
			Destroy(this.gameObject);
		}
	}

	void MoveObject()
	{
		transform.Translate(moveVector * Time.deltaTime, Space.World);
		transform.Rotate(Vector3.forward, speed_rotate * Time.deltaTime);
	}

}
