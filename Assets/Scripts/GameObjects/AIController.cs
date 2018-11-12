using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller {

	public enum MoveDirection {
		Up,
		Right,
		Down,
		Left,
		None
	}

	public MoveDirection currentDirection = MoveDirection.None;

	public override bool moveRight() {
		return currentDirection == MoveDirection.Right;
	}

	public override bool moveLeft() {
		return currentDirection == MoveDirection.Left;
	}

	public override bool moveUp() {
		return currentDirection == MoveDirection.Up;
	}

	public override bool moveDown() {
		return currentDirection == MoveDirection.Down;
	}

	public override bool moveDownOnce() {
		// Make sure if we're saying to drop from a rope that we KNEW we were on a rope before the decision was made
		return currentDirection == MoveDirection.Down && _wasOnRope;
	}

	public const float SNAP_THRESHOLD = 0.1f;

	private Transform _target;
	private PlayerMovement _movement;

	private bool _wasOnRope = false;

	private bool _canMoveUp = false;
	private bool _canMoveRight = false;
	private bool _canMoveDown = false;
	private bool _canMoveLeft = false;


	// The three variables used to determine:
	// a. where we are right now (in terms of grid snapped coordinates)
	public Vector2 ourSnapPosition;
	// b. Where we're moving towards
	public Vector2 nextSnapPosition;
	// c. Where we were last. 
	public Vector2 lastSnapPosition;

	// Use this for initialization
	void Start () {
		_target = GameObject.FindGameObjectWithTag("Player").transform;
		_movement = GetComponent<PlayerMovement>();
		ourSnapPosition = snapPosition(transform.position);
		lastSnapPosition = ourSnapPosition;
		nextSnapPosition = ourSnapPosition;
	}
	
	// Update is called once per frame
	void Update () {
		// Start by updating information about what we can do, based on the data PlayerMovement gives us.
		_canMoveRight = !_movement.blockedRight && (_movement.onGround || _movement.onLadder || _movement.onRope);
		_canMoveLeft = !_movement.blockedLeft && (_movement.onGround || _movement.onLadder || _movement.onRope);
		_canMoveDown = !_movement.onGround && (_movement.onLadder || _movement.onRope);
		_canMoveUp = _movement.onLadder && _movement.canClimbUp;
		_wasOnRope = _movement.onRope;

		// If we're close enough to our target, take a new step. 
		if (Vector2.Distance(nextSnapPosition, transform.position) < SNAP_THRESHOLD || currentDirection == MoveDirection.None) {
			takeStep();
		}

		// Now, assuming our target snap position is accurate, pick a direction to move towards it. 
		bool verticalMove = Mathf.Abs(transform.position.y - nextSnapPosition.y) >= Mathf.Abs(transform.position.x - nextSnapPosition.x);
		if (transform.position.y < nextSnapPosition.y && verticalMove) {
			currentDirection = MoveDirection.Up;
		}
		else if (transform.position.x < nextSnapPosition.x && !verticalMove) {
			currentDirection = MoveDirection.Right;
		}
		else if (transform.position.y > nextSnapPosition.y && verticalMove) {
			currentDirection = MoveDirection.Down;
		}
		else if (transform.position.x > nextSnapPosition.x && !verticalMove) {
			currentDirection = MoveDirection.Left;
		}
		else {
			currentDirection = MoveDirection.None;
		}

		// Now, if we're overlapping another enemy, wait for them to get out of the way before we move. 
		// Using instance ids to ensure that only one of the two overlapping enemies waits.
		if (_movement.bodyHitbox.isTouchingAny("Enemy")) {
			GameObject enemyWereTouching = _movement.bodyHitbox.getColliderWereTouching("Enemy").transform.parent.gameObject;
			if (enemyWereTouching.GetInstanceID() < gameObject.GetInstanceID()) {
				currentDirection = MoveDirection.None;
			}
		}

		// Finally, if we can't continue towards our target position for whatever reason, change our snap and think again next frame
		if (!canMoveInDirection(currentDirection)) {
			currentDirection = MoveDirection.None;
		}
	}


	// The actual AI routine. This is a greedy AI that does NOT use pathfinding. 
	// The basic idea is to look at the four points next to us, rule out the ones we can't move to
	// rule out a few special exceptions, and then pick the one that brings us closest to our target (the player)
	private void takeStep() {
		ourSnapPosition = snapPosition(transform.position);		
		Vector2 targetSnapPosition = snapPosition(_target.position);

		// Keep a list of all the neighbors we can move to.
		List<Vector2> possibleNextPositions = new List<Vector2>();

		// We place vertical directions in the list first so we can prioritize taking those over horzintal directions in case of a tie.
		// (because it's much harder to move vertically, so we should try to do it when we can).
		Vector2 upSnapPosition = ourSnapPosition+Vector2.up*PlayerMovement.TILE_SIZE;
		if (_canMoveUp) {
			possibleNextPositions.Add(upSnapPosition);
		}
		Vector2 downSnapPosition = ourSnapPosition-Vector2.up*PlayerMovement.TILE_SIZE;
		if (_canMoveDown) {
			possibleNextPositions.Add(downSnapPosition);
		}
		Vector2 rightSnapPosition = ourSnapPosition+Vector2.right*PlayerMovement.TILE_SIZE;
		if (_canMoveRight) {
			possibleNextPositions.Add(rightSnapPosition);
		}
		Vector2 leftSnapPosition = ourSnapPosition-Vector2.right*PlayerMovement.TILE_SIZE;
		if (_canMoveLeft) {
			possibleNextPositions.Add(leftSnapPosition);
		}
		// Now some special cases.
		// Special Case #1: Never return to our previous position. 
		// This ensures a few things. First it means we will always keep moving if we're able to (no stopping).
		// Second, If we need to move further away from our target to be able to get closer (i.e. we need to find a ladder)
		// This ensures that we won't just keep moving back and forth on the same position.
		possibleNextPositions.Remove(lastSnapPosition);


		// Special Case #2: If our opponent is above us, never move down.
		// Becomes important because sometimes moving down technically keeps us closer to an opponent above us than moving to the left or right.
		if (targetSnapPosition.y > ourSnapPosition.y) {
			possibleNextPositions.Remove(downSnapPosition);
		}

		// Special Case #3: If our opponent is above us and we CAN move up, do it. 
		if (targetSnapPosition.y > ourSnapPosition.y && possibleNextPositions.Contains(upSnapPosition)) {
			nextSnapPosition = upSnapPosition;
		}
		else {
			// Otherwise, default is to choose a position that's closest to our target. 
			float closestTargetDistance = -1;
			Vector2 closestSnapPosition = ourSnapPosition;
			foreach (Vector2 neighborPosition in possibleNextPositions) {
				float neighborDistance = Mathf.Abs(neighborPosition.x-targetSnapPosition.x) + Mathf.Abs(neighborPosition.y-targetSnapPosition.y);
				if (neighborDistance < closestTargetDistance || closestTargetDistance < 0) {
					closestSnapPosition = neighborPosition;
					closestTargetDistance = neighborDistance;
				}
			}
			nextSnapPosition = closestSnapPosition;
		}
		
		lastSnapPosition = ourSnapPosition;
	}

	private Vector2 snapPosition(Vector2 unsnappedPosition) {
		return new Vector2(Mathf.RoundToInt(unsnappedPosition.x / PlayerMovement.TILE_SIZE)*PlayerMovement.TILE_SIZE, Mathf.RoundToInt(unsnappedPosition.y / PlayerMovement.TILE_SIZE)*PlayerMovement.TILE_SIZE);
	}


	private bool canMoveInDirection(MoveDirection direction) {
		if (direction == MoveDirection.Up) {
			return _canMoveUp;
		}
		else if (direction == MoveDirection.Right) {
			return _canMoveRight;
		}
		else if (direction == MoveDirection.Down) {
			return _canMoveDown;
		}
		else if (direction == MoveDirection.Left) {
			return _canMoveLeft;
		}
		return true;
	}

}
