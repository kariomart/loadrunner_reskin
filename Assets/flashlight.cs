using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashlight : MonoBehaviour {


	public Vector2 desiredPos;
	GameObject light;
	bool on;
	int counter;
	public bool moving;


	// Use this for initialization
	void Start () {
		
		//desiredPos = new Vector2(-4.14f, -4.26f);
		light = transform.GetChild(0).gameObject;

	}
	
	// Update is called once per frame
	void Update () {

		if (on && counter >= 3) {
			transform.position = Vector2.MoveTowards(transform.position, desiredPos, .125f);
		}

		if (Input.GetMouseButtonDown(0)) {
			counter ++;
		}

		if (counter == 2) {
			//GetComponent<SpriteRenderer>().enabled = true;
			on = true;
			//GetComponent<AudioSource>().Play();
			light.SetActive(true);
		}

		if (counter == 3) {
			moving = true;
			transform.eulerAngles = new Vector3(0,0,90);
			transform.position = new Vector2(-4.14f, -4.26f);
			counter ++;
		}
		
	}
}
