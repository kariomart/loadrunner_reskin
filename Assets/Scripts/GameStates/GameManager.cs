using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public enum GameState {
		Starting,
		Running,
		GameOver,
		CompletedLevel
	}
	public GameState currentState = GameState.Starting;
	public TextAsset[] allLevels;
	public static int currentLevelIndex = 0;

	public TiledLevelLoader levelLoader;

	public int coinsCollected = 0;
	public int totalCoinsInLevel = 0;

	private static GameManager _instance = null;
	public static GameManager instance {
		get { return _instance; }
	}

	void Awake() {
		_instance = this;
	}


	private PlayerMovement _mainPlayer;
	private GameObject _exitObj;
	
	void Start() {
		TextAsset currentLevelFile = allLevels[currentLevelIndex];
		levelLoader.loadLevel(currentLevelFile.text);
		_mainPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        Camera.main.transform.parent = _mainPlayer.transform;
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
		_exitObj = GameObject.FindGameObjectWithTag("Exit");
		if (_exitObj != null) {
			_exitObj.SetActive(false);
		}

		totalCoinsInLevel = GameObject.FindGameObjectsWithTag("Coin").Length;
		Time.timeScale = 0f;
	}

	// Update is called once per frame
	void Update () {
		
		if (currentState == GameState.Starting) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				currentState = GameState.Running;
				Time.timeScale = 1f;
			}
		}
		else if (currentState == GameState.Running) {
			if (_mainPlayer.bodyHitbox.isTouchingAny("Coin")) {
				coinsCollected++; 
				Destroy(_mainPlayer.bodyHitbox.getColliderWereTouching("Coin").gameObject);
				// If that was the last coin, reveal the exit. 
				if (coinsCollected >= totalCoinsInLevel && _exitObj != null) {
					_exitObj.SetActive(true);
				}
			}
			else if (_mainPlayer.bodyHitbox.isTouchingAny("Enemy")) {
				// Game Over.
				// Do a camera shake
				Camera.main.GetComponent<ObjShake>().shake();
				currentState = GameState.GameOver;
				_mainPlayer.dead = true;
				Time.timeScale = 0f;
			}
			else if (_mainPlayer.bodyHitbox.isTouchingAny("Exit")) {
				currentState = GameState.CompletedLevel;
				Time.timeScale = 0f;
			}
		}
		else if (currentState == GameState.GameOver) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				SceneManager.LoadScene("MainMenuScene");
			}
			else if (Input.GetKeyDown(KeyCode.Return)) {
				// Re-load the scene.
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
		else if (currentState == GameState.CompletedLevel) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				SceneManager.LoadScene("MainMenuScene");
			}
			else if (Input.GetKeyDown(KeyCode.Return)) {
				if (currentLevelIndex >= allLevels.Length-1) {
					SceneManager.LoadScene("MainMenuScene");
				}
				else {
					// Advancing to next level. 
					currentLevelIndex++;
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
			}
		}
	}
}
