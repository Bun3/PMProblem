using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnMoveMapHandler : IGameEventSystemHandler
{
	public void OnMoveMap(BaseObject moveObject, Portal movedPortal, Map prevMap, Map currentMap);
}
