using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : BaseObject
{
	protected SpriteRenderer spriteRenderer;
	protected new Collider2D collider2D = null;

	protected override void Awake()
	{
		base.Awake();

		spriteRenderer = GetComponent<SpriteRenderer>();
		collider2D = GetComponent<Collider2D>();
	}

	private void LateUpdate()
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

	public void OnSpawn(Map spawnedMap)
	{
		currentMap = spawnedMap;
		GameManager.Instance.ExecuteGameEvent<IOnSpawnCharacterHandler>(null, (x, y) => x.OnSpawn(this));
	}

}
