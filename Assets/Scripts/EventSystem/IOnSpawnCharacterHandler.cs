using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnSpawnCharacterHandler : IGameEventSystemHandler
{
	public void OnSpawn(BaseObject baseObject);
}