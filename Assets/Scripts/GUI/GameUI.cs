using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	public Text bigUIText;

	public Text coinsCollectedText;

	[TextArea(3, 60)]
	public string startLevelString = "Level {0}\n<size=32>Press Enter to start.</size>";

	[TextArea(3, 60)]
	public string gameOverString = "Game Over!\n<size=32>Press Enter to retry. Press Escape to quit.</size>";
	
	[TextArea(3, 60)]
	public string levelCompleteString = "Level Complete!\n<size=32>Press Enter for next level. Press Escape to Quit.</size>";

	[TextArea(3, 60)]
	public string allLevelsCompleteString = "Game Complete!\n<size=32>Press Enter or Escape to quit.</size>";

	[TextArea(3, 60)]
	public string coinsString = "hearts: {0}/{1}";

	[TextArea(3, 60)]
	public string allCoinsCollectedString = " (go to the exit!)";

	void Update () {
		
		if (GameManager.instance.currentState == GameManager.GameState.Starting) {
			bigUIText.text = string.Format(startLevelString, GameManager.currentLevelIndex+1);
		}
		else if (GameManager.instance.currentState == GameManager.GameState.GameOver) {
			bigUIText.text = gameOverString;
		}
		else if (GameManager.instance.currentState == GameManager.GameState.CompletedLevel) {
			if (GameManager.currentLevelIndex < GameManager.instance.allLevels.Length-1) {
				bigUIText.text = levelCompleteString;
			}
			else {
				bigUIText.text = allLevelsCompleteString;
			}
		}
		else {
			bigUIText.text = "";
		}

		int coinsCollected = GameManager.instance.coinsCollected;
		int maxCoins = GameManager.instance.totalCoinsInLevel;

		coinsCollectedText.text = string.Format(coinsString, coinsCollected, maxCoins);
		if (coinsCollected >= maxCoins) {
			coinsCollectedText.text += allCoinsCollectedString;
		}
	}
}
