using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {

	ParticleSystem fx;
	public ParticleSystem childFx;

	// Use this for initialization
	void Start () {

		fx = GetComponent<ParticleSystem>();
		//childFx = GetComponentInChildren<ParticleSystem>();
		
	}
	
	// Update is called once per frame
	void Update () {

		var shape = fx.shape;
		var childShape = childFx.shape;
		float angleVal = 10 + Mathf.PingPong((Time.time * 30), 20);
//		Debug.Log(angleVal);
		shape.angle = angleVal;
		childShape.angle = angleVal * 2f;
	}

}