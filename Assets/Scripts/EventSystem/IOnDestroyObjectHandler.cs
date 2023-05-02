using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnDestroyObjectHandler : IGameEventSystemHandler
{
	public void OnDestroyObject(BaseObject baseObject);
}
