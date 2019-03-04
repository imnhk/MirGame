using UnityEngine;
using System.Collections;

public class ObstacleCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "BossAttack")
		{
			Destroy(transform.parent.gameObject);
		}

	}
}
