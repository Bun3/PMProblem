using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Oni : Character
{
	protected BaseObject target = null;

	public BaseObject Target {  get { return target; } }

	[SerializeField]
	protected float speed = 1.0f;

	bool bEnableChaseTarget = false;

	protected override void Awake()
	{
		base.Awake();

		collider2D.isTrigger = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (HasTarget())
		{
			EnableChaseTarget();
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		DisableChaseTarget();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(target is Player && target.gameObject == collision.gameObject)
		{
			GameManager.Instance.OnGameOver();
		}
	}

	private void FixedUpdate()
	{
		if(IsEnableChaseTarget() && CanFindTarget())
		{
			PerformChaseTarget(Time.fixedDeltaTime);
		}
	}

	public void EnableChaseTarget()
	{
		bEnableChaseTarget = true;
	}

	public void DisableChaseTarget()
	{
		bEnableChaseTarget = false;
	}

	public bool IsEnableChaseTarget()
	{
		return bEnableChaseTarget;
	}

	public bool HasTarget()
	{
		return target != null;
	}

	public void SetTarget(BaseObject target)
	{
		if (target == null)
			return;

		this.target = target;
	}

	protected bool CanFindTarget()
	{
		if (currentMap == null)
			return false;
		if (HasTarget() == false)
			return false;
		if (currentMap != target.CurrentMap)
			return false;

		return true;
	}

	protected abstract void PerformChaseTarget(float performDeltaTime);

}
