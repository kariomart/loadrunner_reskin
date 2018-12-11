using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomizer : MonoBehaviour
{

	private SpriteRenderer sp;
	
	public Sprite defaultTile;

	public float defaultWeight = 0.75f; // between 0 and 1. How common should the default tile be?
		
	public Sprite[] randTiles;

	// Use this for initialization
	void Start ()
	{
		//get our sprite component
		sp = GetComponent<SpriteRenderer>();
		
		//pick a number.
		float r = Random.value;

		
		//if it's under our weight, let's use the default tile. Otherwise let's pick a random tile from our array.
		if (r < defaultWeight)
		{
			sp.sprite = defaultTile;
		}
		else
		{
			int r2 = (int) Random.Range(0, randTiles.Length);
			sp.sprite = randTiles[r2];
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}