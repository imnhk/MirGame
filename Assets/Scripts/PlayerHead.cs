using UnityEngine;
using System.Collections;

// 플레이어의 충돌을 이 스크립트에서 확인한다.

public class PlayerHead : MonoBehaviour {

    public Player player;
	public GameManager gameManager;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.name == "EdgeLeft")
        {
			player.posState = Player.STATE_POSITION.LEFT;			      
        }
        else if (coll.gameObject.name == "EdgeRight")
		{
			player.posState = Player.STATE_POSITION.RIGHT;
        }

		if (coll.tag == "BossAttack")
		{
			if (coll.gameObject.name == "LaserBeamCollider")
				gameManager.playerHealth -= 600 * Time.deltaTime;
				gameManager.score -= 1000 * Time.deltaTime;
		}
    }

	void OnTriggerExit2D(Collider2D coll)
	{
		player.posState = Player.STATE_POSITION.MID;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		// Asteroid 충돌
		if(coll.tag == "Obstacle")
		{
			if (coll.gameObject.name == "MoonCollider")
				gameManager.playerHealth -= 50;
			else
			{
				Destroy(coll.transform.parent.gameObject);
				gameManager.playerHealth -= 60;
				gameManager.score -= 100;
			}
		}

		// 보스 공격
		if (coll.tag == "BossAttack")
		{
			if (coll.gameObject.name == "TorpedoCollider")
			{
				gameManager.playerHealth -= 90;
				gameManager.score -= 200;
			}
		}

		// 아이템 획득
		if(coll.tag == "Item")
		{
			Destroy(coll.transform.parent.gameObject);

			if (coll.gameObject.name == "BlueStarCollider")
			{
				gameManager.score += 50;
				player.BlueShot();
			}
		}
	}
}