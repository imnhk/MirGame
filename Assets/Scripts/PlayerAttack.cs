using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(new Vector2(0, speed * Time.deltaTime));
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		// Asteroid 충돌
		if (coll.tag == "Obstacle")
		{
			if (coll.gameObject.name == "MoonCollider")
				Destroy(transform.parent.gameObject);

			else
			{
				Destroy(coll.transform.parent.gameObject);
				Destroy(transform.parent.gameObject);
			}
		}

		// 보스 공격
		if (coll.tag == "BossAttack")
		{
			if (coll.gameObject.name == "TorpedoCollider")
			{
				Destroy(coll.transform.parent.gameObject);
				Destroy(transform.parent.gameObject);
			}
		}
	}
}
