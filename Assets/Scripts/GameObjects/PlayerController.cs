using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller {

	public KeyCode walkRightKey = KeyCode.RightArrow;
	public KeyCode walkLeftKey = KeyCode.LeftArrow;
	public KeyCode climbUpKey = KeyCode.UpArrow;
	public KeyCode climbDownKey = KeyCode.DownArrow;
	public KeyCode slideKey = KeyCode.Space;

	public override bool moveLeft() {
		return Input.GetKey(walkLeftKey);
	}

	public override bool moveRight() {
		return Input.GetKey(walkRightKey);
	}

	public override bool moveUp() {
		return Input.GetKey(climbUpKey);
	}

	public override bool moveDown() {
		return Input.GetKey(climbDownKey);
	}

	public override bool moveDownOnce() {
		return Input.GetKeyDown(climbDownKey);
	}

	public override bool beginSlide() {
		return Input.GetKeyDown(slideKey);
	}
	
}
