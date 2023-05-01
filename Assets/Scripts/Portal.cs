using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Portal : BaseObject
{
	Collider2D portalCollider = null;

	[SerializeField]
	Portal connectedPortal = null;

	[SerializeField]
	GameObject teleportPointObject = null;

	public Portal ConnectedPortal { get => connectedPortal; }

	void InitCurrentMap()
	{
		currentMap = GetComponentInParent<Map>();
	}

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			base.Awake();
		}

		InitCurrentMap();
		portalCollider = GetComponent<Collider2D>();
		portalCollider.isTrigger = true;
	}

	private void Update()
	{
		if(Application.isPlaying)
		{

		}
		else
		{
			InitCurrentMap();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(connectedPortal != null)
		{
			if(collision.gameObject.GetComponent<Character>() is Character character)
			{
				collision.gameObject.transform.position = connectedPortal.teleportPointObject.transform.position;
				character.CurrentMap = connectedPortal.currentMap;
				GameManager.Instance.ExecuteGameEvent<IOnMoveMapHandler>(null, (x, y) => x.OnMoveMap(character, this, currentMap, character.CurrentMap));
			}
		}
	}
}
