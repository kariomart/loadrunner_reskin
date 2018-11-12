using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

	public virtual bool moveLeft() {
		return false;
	}

	public virtual bool moveRight() {
		return false;
	}

	public virtual bool moveUp() {
		return false;
	}

	public virtual bool moveDown() {
		return false;
	}

	public virtual bool moveDownOnce() {
		return false;
	}

	public virtual bool beginSlide() {
		return false;
	}
}
