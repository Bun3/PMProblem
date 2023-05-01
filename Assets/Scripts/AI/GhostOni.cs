using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GhostOni : Oni, IOnSpawnPlayerHandler
{
	public void OnSpawnPlayer(Player player)
	{
		SetTarget(player);
	}

	// Update is called once per frame
	void Update()
	{
		if(Target != null)
		{
			transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, Time.deltaTime * speed);
		}
	}
}
