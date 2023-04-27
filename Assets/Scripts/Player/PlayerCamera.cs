using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	GameObject player = null;
	[SerializeField]
	float followSpeed = 1.0f;

	Vector2 limitMinVec, limitMaxVec, cameraHalfSize;

	private void Start()
	{
		var groundTilemap = GameManager.FindGroundTilemap();
		if (groundTilemap != null)
		{
			var cellSize = groundTilemap.layoutGrid.cellSize;
			limitMinVec = -cellSize * 0.5f;
			limitMaxVec = Vector3.Scale(groundTilemap.cellBounds.size, cellSize) - cellSize * 0.5f;

			cameraHalfSize.x = Camera.main.aspect * Camera.main.orthographicSize;
			cameraHalfSize.y = Camera.main.orthographicSize;
		}
	}

	private void LateUpdate()
	{
		if(player == null)
		{
			player = GameManager.FindPlayerObject();
		}

		if(player == null)
			return;

		var targetPos = player.transform.position;
		targetPos.z = transform.position.z;

		targetPos.x = Mathf.Clamp(targetPos.x, limitMinVec.x + cameraHalfSize.x, limitMaxVec.x - cameraHalfSize.x);
		targetPos.y = Mathf.Clamp(targetPos.y, limitMinVec.y + cameraHalfSize.y, limitMaxVec.y - cameraHalfSize.y);

		transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed);
	}

}
