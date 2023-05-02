using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnGameOverHandler : IGameEventSystemHandler
{
	public void OnGameOver();
}
