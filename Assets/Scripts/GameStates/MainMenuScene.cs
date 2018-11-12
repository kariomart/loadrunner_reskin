using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScene : MonoBehaviour {
	
	public void levelButtonPressed(int levelIndex) {
		GameManager.currentLevelIndex = levelIndex;
		SceneManager.LoadScene("PlayScene");
	}

	public void quitButtonPressed() {
		Application.Quit();
	}
}
