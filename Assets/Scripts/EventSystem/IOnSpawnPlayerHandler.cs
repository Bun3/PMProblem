using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnSpawnPlayerHandler : IGameEventSystemHandler
{
	public void OnSpawnPlayer(Player player);
}
