using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;


public class PlayerCamera : BaseBehaviour, IOnMoveMapHandler, IOnSpawnPlayerHandler
{
	Player player = null;

	Vector2 limitMinPos, limitMaxPos, cameraHalfSize;

	[SerializeField]
	Vector2 cameraAspect = new Vector2(4, 3);

	protected override void Awake()
	{
		base.Awake();

		var camera = Camera.main;
		camera.aspect = cameraAspect.x / cameraAspect.y;

		Vector2 ratio = new Vector2(Screen.width / cameraAspect.x, Screen.height / cameraAspect.y);

		Vector2 wh = new Vector2((ratio.y * cameraAspect.x) / Screen.width, (ratio.x * cameraAspect.y) / Screen.height);
		Vector2 xy = new Vector2((1 - wh.x) / 2, (1 - wh.y) / 2);

		wh.x = Mathf.Clamp(wh.x, 0, 1);
		wh.y = Mathf.Clamp(wh.y, 0, 1);
		xy.x = Mathf.Clamp(xy.x, 0, 1);
		xy.y = Mathf.Clamp(xy.y, 0, 1);

		camera.rect = new Rect(xy, wh);

		cameraHalfSize.x = Camera.main.aspect * Camera.main.orthographicSize;
		cameraHalfSize.y = Camera.main.orthographicSize;
	}

	private void LateUpdate()
	{
		if(player == null)
			return;

		var targetPos = player.transform.position;
		targetPos.z = transform.position.z;

		targetPos.x = Mathf.Clamp(targetPos.x, limitMinPos.x + cameraHalfSize.x, limitMaxPos.x - cameraHalfSize.x);
		targetPos.y = Mathf.Clamp(targetPos.y, limitMinPos.y + cameraHalfSize.y, limitMaxPos.y - cameraHalfSize.y);

		transform.position = Vector3.Lerp(transform.position, targetPos, 1.0f);
	}

	public void OnSpawnPlayer(Player player)
	{
		this.player = player;
	}

	public void OnMoveMap(BaseObject moveObject, Portal movedPortal, Map prevMap, Map currentMap)
	{
		if(player != null && moveObject == player)
		{
			limitMinPos = currentMap.NodeDataObject.BoundMinWorldPos;
			limitMaxPos = currentMap.NodeDataObject.BoundMaxWorldPos;
		}
	}
}
