using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public enum STATE_MOVE{ LEFT, RIGHT }
	public enum STATE_POSITION { LEFT, RIGHT, MID }

    public GameObject head;
	public GameObject[] body = new GameObject[4];

    public STATE_MOVE moveState;
	public STATE_POSITION posState;
    public float accel;
    public float speed;
    public float speed_max;
	public Vector2 prevPos;

	// Use this for initialization
	public void Start () {
        moveState = STATE_MOVE.LEFT;
		posState = STATE_POSITION.MID;
        speed = 0;
		prevPos = head.transform.position;

		gameObject.SetActive(true);
		head.transform.position = new Vector2(240, -20);
		StartCoroutine(StartAnimation());
	}
	
	// Update is called once per frame
	void Update () {

        MoveHead();
		MoveBody();

	}

    public void MoveHead()
    {
        switch(moveState)
        {
            case STATE_MOVE.LEFT:
                if(posState == STATE_POSITION.LEFT && speed < 0)
					speed += 2.0f * accel;
				else
					speed -= accel;
                break;

            case STATE_MOVE.RIGHT:
                if(posState == STATE_POSITION.RIGHT && speed > 0)
					speed -= 2.0f * accel;
				else
					speed += accel;
                break;

        }

        if      (speed < -speed_max)    speed = -speed_max;
        else if (speed > speed_max)     speed = speed_max;
		
    }

	// 머리 뒤에 몸이 따라서 움직이도록 함
	private void MoveBody()
	{
		prevPos = head.transform.position;

		body[3].transform.position = new Vector2(body[2].transform.position.x, body[2].transform.position.y - 8);
		body[2].transform.position = new Vector2(body[1].transform.position.x, body[1].transform.position.y - 8);
		body[1].transform.position = new Vector2(body[0].transform.position.x, body[0].transform.position.y - 8);
		body[0].transform.position = new Vector2(prevPos.x, prevPos.y - 18);

		head.transform.Translate(speed * Time.deltaTime, 0, 0);
	}

    //화면 터치 시 행동. 방향 전환
    public void ChangeDirection()
    {
        switch(moveState)
        {
            case STATE_MOVE.LEFT:
                moveState = STATE_MOVE.RIGHT;
                break;

            case STATE_MOVE.RIGHT:
                moveState = STATE_MOVE.LEFT;
                break;
        }
    }

	// 플레이어 체력에 따른 색 변환
	public void ChangeColor(float health)
	{
		head.GetComponent<SpriteRenderer>().color = new Color(1, health / 100f, health / 100f);
		for (int i = 0; i < 4; i++)
		{
			body[i].GetComponent<SpriteRenderer>().color = new Color(1, health / 100f * (1 + 0.1f * i), health / 100f * (1 + 0.1f * i));
		}
	}

	// BlueStar 획득 시 공격
	public void BlueShot()
	{
		Vector2 firingPos;
		GameObject shot;

		firingPos = head.transform.position;
		shot = (GameObject)Instantiate(Resources.Load("BlueShot"), firingPos, Quaternion.identity);
	}

	IEnumerator StartAnimation()
	{
		while (head.transform.position.y < 45)
		{
			head.transform.Translate(new Vector2(0, 50 * Time.deltaTime));
			yield return null;
		}
	}
}
