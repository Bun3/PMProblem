using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.ExecuteEvents;

[RequireComponent(typeof(AISystem))]
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

				instance.InitManager();
			}
			return instance;
		} 
	}

	[SerializeField]
	[ReadOnly]
	SpawnData spawnData = null;

	[SerializeField]
	[ReadOnly]
	GeneralData generalData = null;

	Player playerInstance = null;

	HashSet<GameObject> gameEventListeners = new HashSet<GameObject>();
	HashSet<BaseObject> spawnedObjects = new HashSet<BaseObject>();

	GameObject canvas = null;

	AudioSource BGMSource = null;

	void InitManager()
	{
		Assert.IsNotNull(spawnData);

		AddListener(gameObject);

		spawnData.InitSpawnableMap();

		BGMSource = Camera.main.gameObject.GetComponent<AudioSource>();
		BGMSource.clip = generalData.BGMClip;
		BGMSource.volume = 0.0f;

		Application.targetFrameRate = 60;
	}

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
		ExecuteGameEvent<IOnGameOverHandler>(null, (x, y) => x.OnGameOver());
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
			ExecuteGameEvent<IOnDestroyObjectHandler>(null, (x, y) => x.OnDestroyObject(spawnedObject));
			gameEventListeners.Remove(spawnedObject.gameObject);
			Destroy(spawnedObject.gameObject);
		}
	}

	void SpawnAll()
	{
		SpawnOnis();
		SpawnPlayer();

		ExecuteGameEvent<IOnSpawnPlayerHandler>(null, (x, y) => x.OnSpawnPlayer(playerInstance));
		foreach (var spawnedObject in spawnedObjects)
		{
			ExecuteGameEvent<IOnMoveMapHandler>(null, (x, y) => x.OnMoveMap(spawnedObject, null, null, spawnedObject.CurrentMap));
		}
	}

	void SpawnPlayer()
	{
		var map = spawnData.GetRandomMap();
		var playerInstance = Instantiate(spawnData.playerPrefab, map.NodeDataObject.GetRandomSpawnPosition(), Quaternion.identity);
		if (playerInstance != null)
		{
			this.playerInstance = playerInstance;
			playerInstance.OnSpawn(map);
			spawnedObjects.Add(playerInstance);
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
				spawnedObjects.Add(oniInstance);
			}
		}
	}

	public GameObject GetPlayer()
	{
		return playerInstance != null ? playerInstance.gameObject : null;
	}

	const float minDistance = 5, maxDistance = 15;
	public void SetBGMVolumeByOniDistance(float distance)
	{
		if(BGMSource != null)
		{
			distance = Mathf.Clamp(distance, minDistance, maxDistance);
			float percent = (distance - minDistance) / (maxDistance - minDistance);
			percent = 1 - percent;
			BGMSource.volume = percent;
		}
	}

}
