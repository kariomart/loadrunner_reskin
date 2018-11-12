using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public const float TILE_SIZE = 1f;

	public float walkSpeed = 1f;
	public float fallSpeed = 2f;

	public Hitbox feetHitbox; // To test if we're standing on the ground. 
	public Hitbox upperBodyHitbox; // To test if we're able to grab a horizontal bar.
	public Hitbox leftHitbox;
	public Hitbox rightHitbox;
	public Hitbox bodyHitbox; // To test if we're overlapping a ladder, powerup, etc. 
	public Hitbox headHitbox;

	public float slideSpeed = 3f;
	public float slideTime = 0.75f; // The amount of time our slide lasts (in seconds)

	public float slideCooldownTime = 0.5f; // How much time must pass before we can do another slide. 

	private float _slideTimer = 0f;
	private float _slideCooldownTimer = 0f;
	private float _slideDirection = 1f;


	private Controller _controller;

	public bool dead = false;

	private bool _onGround = false;
	public bool onGround {
		get { return _onGround; }
	}

	private bool _blockedRight = false;
	public bool blockedRight {
		get { return _blockedRight; }
	}

	private bool _blockedLeft = false;
	public bool blockedLeft {
		get { return _blockedLeft; }
	}
	
	private bool _blockedUp = false;
	public bool blockedUp {
		get { return _blockedUp; }
	}

	private bool _onLadder = false;
	public bool onLadder {
		get { return _onLadder; }
	}

	private bool _canClimbUp = false;
	public bool canClimbUp {
		get { return _canClimbUp; }
	}

	private bool _onRope = false;
	public bool onRope {
		get { return _onRope; }
	}

	private bool _droppingFromRope = false;
	public bool droppingFromRope {
		get { return _droppingFromRope; }
	}

	private bool _climbing = false;
	public bool climbing {
		get { return _climbing; }
	}


	private bool _sliding = false;
	public bool sliding {
		get { return _sliding; }
	}

	public bool coolingDownFromSlide {
		get { return !_sliding && _slideCooldownTimer > 0; }
	}


	private float _horizontalVelocity = 0f;
	public float horizontalVelocity {
		get { return _horizontalVelocity; }
	}

	private float _verticalVelocity = 0f;
	public float verticalVelocity {
		get { return _verticalVelocity; }
	}

	void Start () {
		_controller = GetComponent<Controller>();
	}
	
	void Update () {
		
		_horizontalVelocity = 0f;
		_verticalVelocity = 0f;

		// First, determine if we're on solid ground or not. 
		_onGround = feetHitbox.isTouchingAny("Wall");
		if (_onGround) {
			// Try to align vertically when we're on the ground.
			float snapY = Mathf.RoundToInt(transform.position.y / TILE_SIZE)*TILE_SIZE;
			float toSnapY = snapY-transform.position.y;
			_verticalVelocity = Mathf.MoveTowards(0, toSnapY / Time.deltaTime, fallSpeed);
		}

		_blockedLeft = leftHitbox.isTouchingAny("Wall");
		_blockedRight = rightHitbox.isTouchingAny("Wall");
		_blockedUp = headHitbox.isTouchingAny("Wall");
		_onLadder = bodyHitbox.isTouchingAny("Ladder");
		_canClimbUp = upperBodyHitbox.isTouchingAny("Ladder");
		_onRope = upperBodyHitbox.isTouchingAny("Rope");
		if (!_onRope) {
			_droppingFromRope = false;
		}

		// for when we need to correct our x coordinate to move towards the center of a tile.
		float snapX = Mathf.RoundToInt(transform.position.x / TILE_SIZE)*TILE_SIZE;
		float toSnapX = snapX-transform.position.x;

		// Can only move left and right if we're on the ground. 
		if (_onGround || (_onRope && !_droppingFromRope) || _onLadder) {
			if (_controller.moveLeft() && !_blockedLeft) {
				_horizontalVelocity -= walkSpeed;
			}
			if (_controller.moveRight() && !_blockedRight) {
				_horizontalVelocity += walkSpeed;
			}
		}
		else {
			// Otherwise, our vertical velocity brings us down. 
			_verticalVelocity = -fallSpeed;

			// move towards the center of the tile, capping at our normal walk speed
			_horizontalVelocity = Mathf.MoveTowards(0, toSnapX / Time.deltaTime, walkSpeed);
		}

		// If we're sliding, that overrules everything else. 
		if (_sliding) {
			_verticalVelocity = 0f;
			_horizontalVelocity = slideSpeed*_slideDirection;

			_slideTimer -= Time.deltaTime;
			// Slides end when the timer runs out OR we hit a wall. 
			if ((_horizontalVelocity > 0 && _blockedRight)
				|| (_horizontalVelocity < 0 && _blockedLeft)
				|| _slideTimer <= 0) {
				_sliding = false;
			}
			else {
				// Sliding causes us to ignore the rest of our movement. 
				transform.position += Vector3.right*_horizontalVelocity*Time.deltaTime + Vector3.up*_verticalVelocity*Time.deltaTime;
				return;
			}
		}
		else if (_slideCooldownTimer > 0) {
			_slideCooldownTimer -= Time.deltaTime;
		}


		// If we're on a rope, we snap to the rope and we can also press down to drop from it. 
		if (_onRope && !_onLadder && !_onGround && !_droppingFromRope) {
			if (_controller.moveDownOnce()) {
				_droppingFromRope = true;
			}
			else {
				float ropeY = upperBodyHitbox.getColliderWereTouching("Rope").transform.position.y;
				float ropeSnapY = Mathf.RoundToInt(ropeY / TILE_SIZE)*TILE_SIZE;
				float toRopeSnapY = ropeSnapY-transform.position.y;
				_verticalVelocity = Mathf.MoveTowards(0, toRopeSnapY / Time.deltaTime, fallSpeed);
			}
		}

		// If we're on a ladder, we can move up and down.
		_climbing = false;
		if (_onLadder) {
			if (_controller.moveUp() && !_blockedUp) {
				// Normally, we'll walk as normal, but if we're at the top of the ladder, we snap to just above the ladder. 
				if (upperBodyHitbox.isTouchingAny("Ladder")) {
					_verticalVelocity += walkSpeed;
					_climbing = true;
				}
				else {
					float ladderY = bodyHitbox.getColliderWereTouching("Ladder").transform.position.y;
					float ladderSnapY = Mathf.RoundToInt(ladderY / TILE_SIZE)*TILE_SIZE;
					ladderSnapY += TILE_SIZE; // Snap to a tile just above the ladder.
					float toLadderSnapY = ladderSnapY - transform.position.y;
					_verticalVelocity = Mathf.MoveTowards(0, toLadderSnapY / Time.deltaTime, walkSpeed);
					_onGround = true;
					_onLadder = false;
				}
				// move towards the center of the tile, capping at our normal walk speed
				_horizontalVelocity = Mathf.MoveTowards(0, toSnapX / Time.deltaTime, walkSpeed);

			}
			if (_controller.moveDown() && !_onGround) {
				_verticalVelocity -= walkSpeed;
				_climbing = true;
				// move towards the center of the tile, capping at our normal walk speed
				_horizontalVelocity = Mathf.MoveTowards(0, toSnapX / Time.deltaTime, walkSpeed);

			}
		}

		if ( (_onLadder || _onGround || _onRope)
		 && !_droppingFromRope
		 && _slideCooldownTimer <= 0
		 &&_controller.beginSlide()) {
			_sliding = true;
			_slideTimer = slideTime;
			_slideCooldownTimer = slideCooldownTime;
		}


		transform.position += Vector3.right*_horizontalVelocity*Time.deltaTime + Vector3.up*_verticalVelocity*Time.deltaTime;

		// If we initiate a slide, the direction is always based on our previous facing.
		if (_horizontalVelocity != 0) {
			_slideDirection = Mathf.Sign(_horizontalVelocity);
		}
	}


}
