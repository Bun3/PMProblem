using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AISystem : MonoBehaviour, IOnSpawnObjectHandler, IOnDestroyObjectHandler, IOnSpawnPlayerHandler
{
	HashSet<Oni> onis = new HashSet<Oni>();

	Player player = null;

	public void OnDestroyObject(BaseObject baseObject)
	{
		if (baseObject is Oni oni)
		{
			onis.Remove(oni);
		}
	}

	public void OnSpawnObject(BaseObject baseObject)
	{
		if (baseObject is Oni oni)
		{
			onis.Add(oni);
		}
	}

	private void LateUpdate()
	{
		float nearestOniDist = float.MaxValue;
		foreach (var oni in onis)
		{
			float oniDist = Vector3.Distance(player.transform.position, oni.transform.position);
			if (oniDist < nearestOniDist)
			{
				nearestOniDist = oniDist;
			}
		}
		GameManager.Instance.SetBGMVolumeByOniDistance(nearestOniDist);
	}

	public void OnDestroy()
	{
		onis.Clear();
	}

	public void OnSpawnPlayer(Player player)
	{
		this.player = player;
	}
}
