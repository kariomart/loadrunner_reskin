using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	private Animator _anim;
	private SpriteRenderer _spriteRenderer;

	private PlayerMovement _movement;

	private bool _wasSliding = false;
	public float slideCooldownFlashPeriod = 0.1f;
	private float _slideCooldownFlashFrequency;

	public lodeFX slideFX;

	// Use this for initialization
	void Start () {
		_anim = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_movement = GetComponent<PlayerMovement>();

		_slideCooldownFlashFrequency = Mathf.PI*2/slideCooldownFlashPeriod;
	}
	
	// Update is called once per frame
	void Update () {

		// To ensure we don't do anything when the game is paused.
		if (!_movement.dead && Time.timeScale == 0) {
			_anim.enabled = false;
			return;
		}
		else {
			_anim.enabled = true;
		}

		_anim.SetBool("Walking", _movement.horizontalVelocity != 0 && (_movement.onGround || _movement.onRope));
		_anim.SetBool("OnRope", _movement.onRope && !_movement.droppingFromRope);
		_anim.SetBool("Climbing", _movement.climbing);
		_anim.SetBool("OnLadder", _movement.onLadder);
		_anim.SetBool("OnGround", _movement.onGround);
		_anim.SetBool("Sliding", _movement.sliding);
		_anim.SetBool("Dead", _movement.dead);
		if (_movement.sliding && !_wasSliding) {
			_anim.SetTrigger("BeginSlide");
				//SpawnFX(slideFX);
		}



		if (_movement.horizontalVelocity < 0) {
			_spriteRenderer.flipX = true;
		}
		else if (_movement.horizontalVelocity > 0) {
			_spriteRenderer.flipX = false;
		}

		// Do a flash to indicate that we're cooling down from a slide
		if (_movement.coolingDownFromSlide && !_movement.dead) {
			_spriteRenderer.enabled = Mathf.Sin(Time.time*_slideCooldownFlashFrequency) >= 0;
		}
		else {
			_spriteRenderer.enabled = true;
		}
	}

	public void SpawnFX(lodeFX fx) {

		if (fx) {
			GameObject newFX = Instantiate(fx, transform.position + Vector3.down * .5f, Quaternion.identity).gameObject;
			newFX.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;	
		}

	}
}
