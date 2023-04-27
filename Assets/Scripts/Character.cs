using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	SpriteRenderer spriteRenderer;

	protected virtual void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void TurnLeft()
	{
		if (spriteRenderer == null)
			return;

		spriteRenderer.flipX = false;
	}

	public void TurnRight()
	{
		if (spriteRenderer == null)
			return;

		spriteRenderer.flipX = true;
	}

}
