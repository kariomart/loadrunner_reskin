using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

	public flashlight flashlight;
	public MainMenuScene menuScene;
	public int level;
	AudioSource sound;

	// Use this for initialization
	void Start () {

		sound = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseOver() {

		if (flashlight.moving) {
			illuminate();
		}

	}

	void OnMouseDown() {

		if (Mathf.Abs(transform.position.x - flashlight.transform.position.x) < .5f) {
			menuScene.levelButtonPressed(level);
		}

	}

	public void illuminate() {

		flashlight.desiredPos = transform.position + Vector3.down * 4.5f;
		if (Mathf.Abs(transform.position.x - flashlight.transform.position.x) < .5f) {
			sound.Play();
		}

	}


}
