using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

	private HashSet<Collider2D> _collidersWereTouching = new HashSet<Collider2D>();

	private Collider2D _collider;

	void Start() {
		_collider = GetComponent<Collider2D>();
	}

	void OnTriggerEnter2D(Collider2D otherCollider) {
		if (otherCollider.transform.parent != transform.parent) {
			_collidersWereTouching.Add(otherCollider);
		}
	}

	void OnTriggerExit2D(Collider2D otherCollider) {
		_collidersWereTouching.Remove(otherCollider);
	}
	
	public bool isTouchingAny(string tagName) {
		foreach (Collider2D colliderWereTouching in _collidersWereTouching) {
			if (colliderWereTouching.gameObject.tag == tagName) {
				return true;
			}
		}
		return false;
	}

	public Collider2D getColliderWereTouching(string tagName) {
		foreach (Collider2D colliderWereTouching in _collidersWereTouching) {
			if (colliderWereTouching.gameObject.tag == tagName) {
				return colliderWereTouching;
			}
		}
		return null;
	}

	public void alignToTop(string tagName) {
		
		float maxOverlapDelta = 0;
		bool foundCollider = false;
		float ourBottom = _collider.bounds.min.y;

		foreach (Collider2D colliderWereTouching in _collidersWereTouching) {
			if (colliderWereTouching.gameObject.tag == tagName) {
				float theirTop = colliderWereTouching.bounds.max.y;
				float overlapDelta = theirTop-ourBottom;
				if (!foundCollider || overlapDelta > maxOverlapDelta) {
					maxOverlapDelta = overlapDelta;
				}
				foundCollider = true;
			}
		}

		if (foundCollider) {
			// NOTE: we move our parent (since we expect hitboxes to be children of a parent object);.
			transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y+maxOverlapDelta, transform.parent.position.z);
		}
	}	

}
