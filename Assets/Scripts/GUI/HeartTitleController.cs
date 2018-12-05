using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartTitleController : MonoBehaviour {

	Animation anim;
	Vector2 startPos;
	Vector2 startScale;
	bool mouseOver;
	int counter;
	AudioSource beatSFX;
	public AudioClip beatSound;

	MainMenuScene menu;

	// Use this for initialization
	void Start () {

		anim = GetComponent<Animation>();
		startPos = transform.position;
		startScale = transform.localScale;
		beatSFX = GetComponent<AudioSource>();
		menu = GameObject.Find("_MainMenuScene").GetComponent<MainMenuScene>();
		
	}
	
	// Update is called once per frame
	void Update () {

		if (!mouseOver && transform.localScale.x > startScale.x) {
			transform.localScale /= 1.025f;
		}
		
		
	}

	void OnMouseOver() {

		mouseOver = true;
		anim.Stop();
		Shake();

	}

	void OnMouseExit() {

		mouseOver = false;
		anim.Play();
		//transform.localScale = startScale;
	}

	void OnMouseDown() {

		menu.levelButtonPressed(0);
	}

	void Shake() {

		if(counter % 2 == 0) {
			float randomXOffset = Random.Range(-1f, 1f);
			float randomYOffset = Random.Range(-1f, 1f);
			transform.position = new Vector2(transform.position.x + randomXOffset, transform.position.y + randomYOffset);
		} else {
			transform.position = startPos;
		}

		counter ++;
		transform.localScale *= 1.025f;
		beatSFX.Stop();

	}

	void PlayHeartbeat() {

		beatSFX.clip = beatSound;
		beatSFX.Play();

	}
}
