using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

	public enum PATTERN { ASTEROID_BELT, DEBRIS, SPACESHIP_BOSS }
	public enum GAMESTATE { TITLE, ON, OVER }

	public GAMESTATE gameState;

	public Player playerScript;
	public Spaceship spaceshipScript;

	public GameObject player;
	public GameObject playerHead;

	public GameObject titleUI;
	public GameObject gameOverUI;
	public GameObject touchArea;
	public GameObject scoreText;
	public GameObject finalScore;
	public GameObject finalText;

	public float score;
	public float playerHealth;

	float patternTimer;
	int patternCounter;
	float lastTime;

	public PATTERN currentPattern;

	public GameObject Boss;
	private bool bossSwitch;

	// Use this for initialization
	void Start()
	{
		gameState = GAMESTATE.TITLE;
		currentPattern = PATTERN.ASTEROID_BELT;

		score = 0;
		playerHealth = 100;

		patternTimer = 0;
		patternCounter = 0;
		lastTime = 0;

		Boss.SetActive(false);
		bossSwitch = false;
	}

	// Update is called once per frame
	void Update()
	{

		switch (gameState)
		{
			case GAMESTATE.TITLE:

				break;

			case GAMESTATE.OVER:

				break;

			case GAMESTATE.ON:

				// 점수 관리
				scoreText.GetComponent<Text>().text = score.ToString("0");
				scoreText.transform.position = new Vector2(playerHead.transform.position.x, playerHead.transform.position.y + 50);
				if (score < 0) 
					score = 0;
				score += 20 * Time.deltaTime;
				patternTimer += Time.deltaTime;

				// 체력 관리
				if (playerHealth < 100)
					playerHealth += 7 * Time.deltaTime;
				else
					playerHealth = 100;

				playerScript.ChangeColor(playerHealth);

				if (playerHealth < 0)
					GameOver();


				// 패턴 전환
				RotatePattern();

				// currentPattern에 따른 장애물 생성
				switch (currentPattern)
				{
					// 무작위로 내려오는 패턴
					case PATTERN.ASTEROID_BELT:
						// 480: 스크린 너비, 800 + 200: 스크린 높이 + 여백.

						if (Random.Range(0f, 100f) < 14f && Random.Range(0f, 100f) < 14f) // 확률 조건을 하나만 사용하면 고르게 나오지 않는 것 같아서 두 개를 겹침
						{
							CreateAsteroid(Random.Range(0, 480), 800 + 200, Random.Range(-30, 30), Random.Range(100, 150), Random.Range(-100, 100), Random.Range(50, 100));
						}

						if (Random.Range(0f, 100f) < 2f && Random.Range(0f, 100f) < 2f)
						{
							CreateMoon(Random.Range(0, 480), 800 + 200, Random.Range(-5, 5), Random.Range(70, 100), Random.Range(-10, 10), Random.Range(60, 80));
						}

						if (Random.Range(0f, 100f) < 6f && Random.Range(0f, 100f) < 6f)
						{
							CreateRocket(Random.Range(0, 480), 800 + 200, Random.Range(-5, 5), Random.Range(150, 250), Random.Range(-20, 20), Random.Range(100, 120));
						}

						if (Random.Range(0f, 100f) < 8f && Random.Range(0f, 100f) < 5f)
						{
							spaceshipScript.SpawnBlueStar(new Vector2(Random.Range(0, 480), 800 + 100));
						}
						break;

					// 가로 한 줄로 약 2초마다 내려오는 패턴
					case PATTERN.DEBRIS:

						// 2.2초마다 CreateAsteroidLine
						if (patternTimer - lastTime > 2.2f)
						{
							lastTime = patternTimer;
							spaceshipScript.SpawnBlueStar(new Vector2(Random.Range(0, 480), 800 + 100));
							spaceshipScript.SpawnBlueStar(new Vector2(Random.Range(0, 480), 800 + 100));
							spaceshipScript.SpawnBlueStar(new Vector2(Random.Range(0, 480), 800 + 100));

							CreateAsteroidLine(Random.Range(4, 6));
						}

						if (Random.Range(0f, 100f) < 6f && Random.Range(0f, 100f) < 6f)
						{
							CreateAsteroid(Random.Range(0, 480), 800 + 100, Random.Range(-20, 20), Random.Range(100, 120), Random.Range(-100, 100), Random.Range(50, 100));
						}

						break;

					// 우주선 보스
					case PATTERN.SPACESHIP_BOSS:
						if (!bossSwitch)
						{
							Boss.SetActive(true);
							bossSwitch = true;
						}

						// 보스전에도 무작위 소행성은 나온다
						if (Random.Range(0f, 100f) < 10f && Random.Range(0f, 100f) < 10f)
						{
							CreateAsteroid(Random.Range(0, 480), 800 + 200, Random.Range(-30, 30), Random.Range(100, 150), Random.Range(-100, 100), Random.Range(50, 100));
						}
						if (Random.Range(0f, 100f) < 8f && Random.Range(0f, 100f) < 8f)
						{
							spaceshipScript.SpawnBlueStar(new Vector2(Random.Range(0, 480), 800 + 100));
						}

						break;
				}

				break;
		}
	}

	public void GameOn()
	{
		titleUI.SetActive(false);
		gameOverUI.SetActive(false);

		touchArea.SetActive(true);
		scoreText.SetActive(true);
		player.SetActive(true);

		gameState = GAMESTATE.ON;
	}

	public void GameOver()
	{
		gameOverUI.SetActive(true);

		if(spaceshipScript.shipState == Spaceship.STATE.DESTROYED)
			finalText.GetComponent<Text>().text = "Congratulation!";
		else
			finalText.GetComponent<Text>().text = "Game Over";

		finalScore.GetComponent<Text>().text = score.ToString("score: 0");

		gameState = GAMESTATE.OVER;

		touchArea.SetActive(false);
		scoreText.SetActive(false);
		player.SetActive(false);

	}

	public void GameReset()
	{
		// 이전 게임 정보 초기화
		Start();
		spaceshipScript.Start();
		playerScript.Start();

		GameOn();

		// 아직 남아 있는 모든 장애물, 아이템, 적 제거
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		GameObject[] bossAttacks = GameObject.FindGameObjectsWithTag("BossAttack");
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

		for (int i = 0; i < obstacles.Length; i++)
			Destroy(obstacles[i]);
		for (int i = 0; i < bossAttacks.Length; i++)
			Destroy(bossAttacks[i]);
		for (int i = 0; i < items.Length; i++)
			Destroy(items[i]);
		
	}

	void RotatePattern()
	{
		// 패턴 전환, 보스가 없을 때 10초마다 실행된다
		if (patternTimer > 10 && currentPattern != PATTERN.SPACESHIP_BOSS)
		{
			patternTimer = 0; // 현재 패턴 진행 시간
			lastTime = 0; // 마지막으로 패턴이 바뀐 시간
			patternCounter += 1; // 패턴 전환 횟수

			if (patternCounter % 6 == 0) // Boss 패턴
			{
				currentPattern = PATTERN.SPACESHIP_BOSS;
			}
			else if (currentPattern == PATTERN.ASTEROID_BELT)
			{
				currentPattern = PATTERN.DEBRIS;
			}
			else
			{
				currentPattern = PATTERN.ASTEROID_BELT;
			}
		}

	}

	// 특정 위치에 특정 Asteroid 생성
	// x <= 480, y > 800
	// 위치, 속도, 회전, 크기 순서
	void CreateAsteroid(float x, float y, float dx, float dy, float dr, float size)
	{
		GameObject ast;
		Vector2 pos; // 생성 위치

		pos = new Vector2(x, y);
		ast = (GameObject)Instantiate(Resources.Load("Asteroid"), pos, Quaternion.identity);
		if (Random.Range(0f, 100f) < 50f)
			ast.GetComponent<SpriteRenderer>().flipX = true;
		ast.GetComponent<Asteroid>().SetAsteroid(dx, dy, dr, size);
	}

	// CreateAsteroid와 같지만 대신 Moon을 생성
	void CreateMoon(float x, float y, float dx, float dy, float dr, float size)
	{
		GameObject moon;
		Vector2 pos;

		pos = new Vector2(x, y);
		moon = (GameObject)Instantiate(Resources.Load("Moon"), pos, Quaternion.identity);
		if (Random.Range(0f, 100f) < 50f)
			moon.GetComponent<SpriteRenderer>().flipX = true;
		moon.GetComponent<Asteroid>().SetAsteroid(dx, dy, dr, size);
	}

	// CreateAsteroid와 같지만 대신 Rocket을 생성
	void CreateRocket(float x, float y, float dx, float dy, float dr, float size)
	{
		GameObject Rocket;
		Vector2 pos;

		pos = new Vector2(x, y);
		Rocket = (GameObject)Instantiate(Resources.Load("Rocket"), pos, Quaternion.AngleAxis(132, Vector3.back));

		Rocket.GetComponent<Asteroid>().SetAsteroid(dx, dy, dr, size);
	}

	// 특정 위치에 특정 Satellte 생성
	// x <= 480, y > 800
	// 위치, 속도, 회전, 크기 순서
	void CreateSatellite(float x, float y, float dx, float dy, float dr, float size)
	{
		GameObject sat;
		Vector2 pos; // 생성 위치

		pos = new Vector2(x, y);
		sat = (GameObject)Instantiate(Resources.Load("Satellite"), pos, Quaternion.identity);
		if (Random.Range(0f, 100f) < 50f)
			sat.GetComponent<SpriteRenderer>().flipX = true;
		sat.GetComponent<Satellite>().SetSatellite(dx, dy, dr, size);
	}

	// 가로 한 줄로 내려오는 Asteroid
	// count개가 내려 오고 hole번째 칸에 구멍이 나 있다
	void CreateAsteroidLine(int count)
	{
		int hole = Random.Range(1, count);
		for (int i = 0; i <= count + 1; i++)
		{
			if (i == hole)
				continue;
			if (Random.Range(0, 100) < 50)
				CreateAsteroid((480 / (count + 1)) * i, 1000, Random.Range(-10, 10), Random.Range(130, 150), Random.Range(-80, 80), Random.Range(80, 100));
			else
				CreateSatellite((480 / (count + 1)) * i, 1000, Random.Range(-10, 10), Random.Range(130, 150), Random.Range(-80, 80), Random.Range(80, 100));

		}
	}
}
