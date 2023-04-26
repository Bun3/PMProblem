using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(NodeGenerator))]
public class AStarSystem : MonoBehaviour
{
	NodeDataObject dataObject = null;

	static UnityAction<AStarSystem> systemInitCallback = null;
	static AStarSystem instance = null;

	public NodeDataObject DataObject { get => dataObject; set => dataObject = value; }

	public static void AddInitSystemCallback(UnityAction<AStarSystem> callback)
	{
		if(instance != null)
		{
			callback(instance);
		}
		else
		{
			systemInitCallback += callback;
		}
	}

	private void Awake()
	{
		dataObject = GetComponent<NodeGenerator>().MakeDataObjectInstance();
		instance = this;

		systemInitCallback?.Invoke(this);
		systemInitCallback = null;
	}

}
