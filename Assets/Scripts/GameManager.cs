using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.ExecuteEvents;

public class GameManager : MonoBehaviour
{
	static GameManager instance = null;
	public static GameManager Instance 
	{ 
		get
		{
			if(instance == null)
			{
				var obj = new GameObject("GameManager");
				instance = obj.AddComponent<GameManager>();

				Assert.IsNotNull(instance.spawnData);

				instance.spawnData.InitSpawnableMap();
			}
			return instance;
		} 
	}

	[SerializeField]
	[ReadOnly]
	SpawnData spawnData = null;

	Player playerInstance = null;

	HashSet<GameObject> gameEventListeners = new HashSet<GameObject>();
	HashSet<GameObject> spawnedObjects = new HashSet<GameObject>();

	GameObject canvas = null;

	void ResetData()
	{
		playerInstance = null;
		spawnedObjects.Clear();
	}

	[ContextMenu("RestartGame")]
	public void RestartGame()
	{
		canvas?.SetActive(false);
		SpawnAll();
	}

	public void OnGameOver()
	{
		DestroyAllSpawnedObject();
		ResetData();
		canvas?.SetActive(true);
	}

	public void AddListener(GameObject listener)
	{
		if (listener == null)
			return;
		if (gameEventListeners.Contains(listener))
			return;

		gameEventListeners.Add(listener);
	}
	public void ExecuteGameEvent<T>(BaseEventData eventData, EventFunction<T> functor) where T : IGameEventSystemHandler
	{
		foreach (var listener in gameEventListeners)
		{
			Execute(listener, eventData, functor);
		}
	}

	protected void Awake()
	{
		DontDestroyOnLoad(gameObject);
		canvas = GameObject.FindGameObjectWithTag("UICanvas");
		canvas?.SetActive(false);
	}

	private void Start()
	{
		SpawnAll();
	}

	void DestroyAllSpawnedObject()
	{
		foreach(var spawnedObject in spawnedObjects)
		{
			gameEventListeners.Remove(spawnedObject);
			Destroy(spawnedObject);
		}
	}

	void SpawnAll()
	{
		SpawnOnis();
		SpawnPlayer();
	}

	void SpawnPlayer()
	{
		var map = spawnData.GetRandomMap();
		var playerInstance = Instantiate(spawnData.playerPrefab, map.NodeDataObject.GetRandomSpawnPosition(), Quaternion.identity);
		if (playerInstance != null)
		{
			this.playerInstance = playerInstance;
			playerInstance.OnSpawn(map);
			spawnedObjects.Add(playerInstance.gameObject);
			ExecuteGameEvent<IOnSpawnPlayerHandler>(null, (x, y) => x.OnSpawnPlayer(playerInstance));
			ExecuteGameEvent<IOnMoveMapHandler>(null, (x, y) => x.OnMoveMap(playerInstance, null, null, map));
		}
	}

	void SpawnOnis()
	{
		foreach (var oni in spawnData.OniPrefabs)
		{
			var randomMap = spawnData.GetRandomMap();
			var oniInstance = Instantiate(oni, randomMap.NodeDataObject.GetRandomSpawnPosition(), Quaternion.identity);
			if (oniInstance != null)
			{
				oniInstance.OnSpawn(randomMap);
				spawnedObjects.Add(oniInstance.gameObject);
				ExecuteGameEvent<IOnMoveMapHandler>(null, (x, y) => x.OnMoveMap(oniInstance, null, null, randomMap));
			}
		}
	}

	public GameObject GetPlayer()
	{
		return playerInstance != null ? playerInstance.gameObject : null;
	}

}
