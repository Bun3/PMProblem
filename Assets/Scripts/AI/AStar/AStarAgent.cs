using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AStarNode : Node
{
	public AStarNode(Vector2 pos, Vector2Int index, bool bIsBlock = false)
		: base(pos, index, bIsBlock)
	{
	}

	public AStarNode(Node node)
		: base(node.Pos,node.Index,node.bIsBlock)
	{

	}
	public void Clear()
	{
		G = 0;
		H = 0;
	}

	//G: 시작부터 현재 노드까지 거리, H: 현재 노드부터 타겟까지의 거리(장애물 무시)
	public int G = 0, H = 0;
	//F: G + H
	public int F { get => G + H; }

}

public class AStarAgent : MonoBehaviour
{
	AStarNode startNode, targetNode, curNode;
	List<AStarNode> openList, closeList, finalList;

	AStarSystem systemInstace = null;

	void Awake()
	{
		openList = new List<AStarNode>();
		closeList = new List<AStarNode>();
		finalList = new List<AStarNode>();

		AStarSystem.AddInitSystemCallback(OnInitSystem);
	}

	void OnInitSystem(AStarSystem system)
	{
		systemInstace = system;
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	bool IsValidAgent()
	{
		if (systemInstace == null)
			return false;

		return true;
	}

}
