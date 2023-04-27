using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Oni : Character
{
	new Collider2D collider2D = null;

	const string c_playerTag = "Player";

	protected override void Awake()
	{
		base.Awake();

		collider2D = GetComponent<Collider2D>();
		collider2D.isTrigger = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag(c_playerTag))
		{
			Debug.Log("¿‚æ“¥Á");
		}
	}

}
