using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Oni : Character
{
	protected BaseObject target = null;

	public BaseObject Target {  get { return target; } }

	[SerializeField]
	protected float speed = 1.0f;

	protected override void Awake()
	{
		base.Awake();

		collider2D.isTrigger = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(target is Player && target.gameObject == collision.gameObject)
		{
			GameManager.Instance.OnGameOver();
		}
	}

	public bool IsChasingTarget()
	{
		return target != null;
	}

	public void SetTarget(BaseObject target)
	{
		if (target == null)
			return;

		this.target = target;
	}
}
