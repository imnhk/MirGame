using UnityEngine;
using System.Collections;

public class Spaceship : MonoBehaviour {

	public enum STATE { ENGAGE, IDLE, BURST, CANNON, TORPEDO, DESTROYED }
	public STATE shipState;

	/* ENGAGE:	시작 패턴, 200px만큼 아래로 천천히 이동해 와 자리를 잡는다.
	 * IDLE:	공격과 공격 사이. 상하좌우로 조금씩 천천히 이동한다.
	 * 
	 * BURST:	가느다란 레이저 세 개를 플레이어의 방금 전 위치에 발사한다. 플레이어가 방향을 바꾸지 않으면 피할 수 있다.
	 * CANNON:	화면 가운데를 덮는 두꺼운 레이저를 발사한다. 모서리에 있으면 피할 수 있다.
	 * TORPEDO:	많은 어뢰를 줄지어 발사한다.
	 * 
	 * DESTROYED: 플레이어 승리 애니메이션. 색이 바뀌고 흔들리면서 화면 밖으로 나간다.
	 */

	public GameObject player;
	public GameObject playerHead;
	public GameManager gameManager;

	public float battleTimer;
	public float patternTimer;

	private Vector2 defaultPosition;
	private float movingSpeed;

	// 우주선 체력
	public float shipHealth;

	// Burst pattern에 쓰임
	public GameObject[] burstLaser = new GameObject[3];
	private GameObject[] burstSight = new GameObject[3];
	private GameObject[] burstBeam = new GameObject[3];

	// Cannon pattern에 쓰임
	public GameObject laserCannon;
	private GameObject laserSight;
	private GameObject laserBeam;

	// Laser 발사는 한 번 씩만
	private bool isAiming;
	private bool isFiring;

	// Use this for initialization
	public void Start () {

		shipState = STATE.ENGAGE;
		battleTimer = 0;
		patternTimer = 0;
		shipHealth = 100;

		defaultPosition = new Vector2(240, 700);
		transform.position = new Vector2(240, 900);
		gameObject.SetActive(false);
		movingSpeed = 100;

		isAiming = false;

	}
	
	// Update is called once per frame
	void Update () {
		if(shipHealth <= 0)
			shipState = STATE.DESTROYED;

		ShipAct();
	}


	void ShipAct()
	{
		battleTimer += Time.deltaTime;
		patternTimer += Time.deltaTime;

		switch (shipState)
		{
			case STATE.ENGAGE:
				if (patternTimer < 4)
				{
					transform.position = Vector2.MoveTowards(transform.position, defaultPosition, movingSpeed * Time.deltaTime);
					movingSpeed -= 25 * Time.deltaTime;
				}
				if(battleTimer > 5)
				{
					shipState = STATE.IDLE;
					movingSpeed = 25;
					patternTimer = 0;
				}
				break;

			case STATE.IDLE:
				// 3초가 지나면 세 가지 중 하나로 패턴을 바꾼다.
				if (patternTimer > 3)
				{
					switch ((int)Random.Range(0, 4))
					{
						case 0:
						case 1:
							shipState = STATE.BURST;
							patternTimer = 0;
							break;

						case 2:
							shipState = STATE.CANNON;
							patternTimer = 0;
							break;

						case 3:
							shipState = STATE.TORPEDO;
							patternTimer = 0;
							break;

						default:
							Debug.Log("Idle pattern error");
							break;
					}
				}
				break;

			case STATE.BURST:
				if (!isFiring)
				{
					StartCoroutine(BurstLasetShot());
				}
				if (patternTimer > 8)
				{
					patternTimer = 0;
					shipState = STATE.IDLE;
				}
				break;

			case STATE.CANNON:
				Vector2 pos = laserCannon.transform.position;

				if (!isAiming)
				{
					laserSight = (GameObject)Instantiate(Resources.Load("LaserSight"), pos, Quaternion.identity);
					laserSight.transform.localScale = new Vector2(143, 1000);
					isAiming = true;

					Destroy(laserSight, 1f);
				}

				// 조준 후 1.5초 뒤 발사
				if (patternTimer > 1.5f && !isFiring)
				{
					laserBeam = (GameObject)Instantiate(Resources.Load("LaserBeam"), pos, Quaternion.identity);
					laserBeam.transform.localScale = new Vector2(22, 100);
					isFiring = true;

					Destroy(laserBeam, 1.5f);
				}

				// 3초 뒤 다시 Idle상태로 
				else if (patternTimer > 3)
				{
					for (int i = 0; i < 6; i++)
						SpawnBlueStar(new Vector2(Random.Range(pos.x - 50, pos.x + 50), pos.y));

					isAiming = false;
					isFiring = false;
					patternTimer = 0;
					shipState = STATE.IDLE;
				}
				break;

			case STATE.TORPEDO:
				if (!isFiring)
				{
					StartCoroutine(TorpedoShot());
				}

				if (patternTimer > 8)
				{
					isFiring = false;
					shipState = STATE.IDLE;
				}
				break;

			case STATE.DESTROYED:
				if (patternTimer < 8)
				{
					ShakingAnimation(1);
					transform.position = Vector2.MoveTowards(transform.position, new Vector2(240, 900), movingSpeed * Time.deltaTime);
				}
				else
				{
					gameManager.score += 5000;
					gameManager.GameOver();
					gameObject.SetActive(false);
				}
				break;
		}
	}

	IEnumerator BurstLasetShot()
	{
		Vector2 firingPos;
		Vector2 targetPos;
		float firingAngle;

		// Coroutine은 한 번씩만 실행되도록
		isFiring = true;

		for (int i = 0; i < 3; i++)
		{
			firingPos = burstLaser[i].transform.position;
			targetPos = playerHead.transform.position;
			//firingAngle = (targetPos - firingPos).normalized;
			firingAngle = Vector2.Angle(Vector2.up, (firingPos-targetPos));

			// Vector2.Angle은 0~180도 까지의 값만 반환하는 것 같다. 레이저는 아래를 향해 쏘므로 0 미만의 각도를 나타낼 필요가 있었음.
			if (firingPos.x > targetPos.x)
				firingAngle = -firingAngle;

			// 조준선은 1초 유지
			burstSight[i] = (GameObject)Instantiate(Resources.Load("LaserSight"), firingPos, Quaternion.AngleAxis(firingAngle, Vector3.forward));
			Destroy(burstSight[i], 1f);
			yield return new WaitForSeconds(1f);

			// 레이저는 0.5초 유지
			burstBeam[i] = (GameObject)Instantiate(Resources.Load("LaserBeam"), firingPos, Quaternion.AngleAxis(firingAngle, Vector3.forward));
			Destroy(burstBeam[i], 0.5f);
			SpawnBlueStar(firingPos);

		}
		isFiring = false;
	}

	IEnumerator TorpedoShot()
	{
		Vector2 firingPos;
		GameObject torpedo = null;

		isFiring = true;

		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				firingPos = new Vector2((480 / 3) * j + (i * 20), 700);
				torpedo = (GameObject)Instantiate(Resources.Load("Torpedo"), firingPos, Quaternion.identity);
				
				if (Random.Range(0f, 100f) < 50f && Random.Range(0f, 100f) < 50f) 
					SpawnBlueStar(firingPos);
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds(0.3f);
		}
	}

	// 주로 공격 위치에서 BlueStar 생성
	public void SpawnBlueStar(Vector2 spawnPos)
	{
		GameObject star;

		star = (GameObject)Instantiate(Resources.Load("BlueStar"), spawnPos, Quaternion.identity);
		star.GetComponent<Asteroid>().SetAsteroid(Random.Range(-10, 10), Random.Range(100, 120), Random.Range(-100, 100), 120);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		// 플레이어 공격
		if (coll.tag == "PlayerAttack")
		{
			StartCoroutine(BlueShotHit());
			Destroy(coll.transform.parent.gameObject);
		}
	}

	private void ShakingAnimation(float intensity)
	{
		Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		transform.Translate(randomDir * intensity);
	}

	IEnumerator BlueShotHit()
	{
		shipHealth -= 10;

		gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0.5f);
		yield return new WaitForSeconds(0.1f);
		gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

	}

}
