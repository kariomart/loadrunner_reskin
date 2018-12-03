using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lodeFX : MonoBehaviour {

	public float lifetime;

	public void Start(){

		//Destroy(gameObject, lifetime);

	}

	public void DestroyMe() {
		Destroy(this.gameObject);
	}
}
