using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnSpawnObjectHandler : IGameEventSystemHandler
{
	public void OnSpawnObject(BaseObject baseObject);
}