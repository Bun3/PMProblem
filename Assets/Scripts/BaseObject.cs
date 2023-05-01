using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : BaseBehaviour
{
	[SerializeField]
	[ReadOnly]
	protected Map currentMap = null;

	public Map CurrentMap { get => currentMap; set => currentMap = value; }

}
