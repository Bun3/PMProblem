using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Data", menuName = "ScriptableObject/SpawnData")]
public class SpawnData : ScriptableObject
{
	//Runtime Data
	Map[] SpawnableMap = null;

	public List<Oni> OniPrefabs = new List<Oni>();

	public Player playerPrefab = null;

	public void InitSpawnableMap()
	{
		SpawnableMap = FindObjectsOfType<Map>();
	}

	public Map GetRandomMap()
	{
		if (SpawnableMap == null)
			return null;

		return SpawnableMap[Random.Range(0, SpawnableMap.Length)];
	}

	public Map GetMap(int idx)
	{
		if(SpawnableMap == null || idx < 0 || idx >= SpawnableMap.Length)
			return null;

		return SpawnableMap[idx];
	}

}
