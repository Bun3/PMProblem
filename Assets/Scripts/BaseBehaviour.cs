using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBehaviour : MonoBehaviour
{
	protected virtual void Awake()
	{
		if(this as IGameEventSystemHandler != null)
		{
			GameManager.Instance.AddListener(gameObject);
		}
	}
}
