using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PMProblem;
using System;
using System.Linq;

public interface IObjectInterface
{

};

public interface IObjectTurn : IObjectInterface
{
	void TurnLeft();
	void TurnRight();
}

public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
{
	PlayerControls playerControls;
	Vector2 moveDirection = Vector2.zero;
	Vector2 prevMoveDir = Vector2.zero;

	IObjectInterface[] objectInterfaces = null;

	SpriteRenderer spriteRenderer;
	Rigidbody2D rigidBody;

	[SerializeField]
	float playerMoveSpeed = 0.5f;

	public void OnMove(InputAction.CallbackContext context)
	{
		moveDirection = context.ReadValue<Vector2>();
	}

	void Awake()
	{
		objectInterfaces = GetComponents<IObjectInterface>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void OnEnable()
	{
		if (playerControls == null)
		{
			playerControls = new PlayerControls();
			playerControls.Player.AddCallbacks(this);
		}
		playerControls.Player.Enable();
	}

	T[] GetObjectInterfaces<T>()
	{
		List<T> result = new List<T>();
        foreach (var item in objectInterfaces)
        {
			if (item is T objectInterface)
			{
				result.Add(objectInterface);
			}
		}
		return result.ToArray();
    }

	void OnDisable()
	{
		playerControls.Player.Disable();
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (moveDirection.x != 0.0f)
		{
			if (moveDirection.x < 0.0f && prevMoveDir.x >= 0.0f)
			{
				spriteRenderer.flipX = false;
				foreach (var turnInterface in GetObjectInterfaces<IObjectTurn>())
				{
					turnInterface.TurnLeft();
				}
			}
			else if (moveDirection.x > 0.0f && prevMoveDir.x <= 0.0f)
			{
				spriteRenderer.flipX = true;
				foreach (var turnInterface in GetObjectInterfaces<IObjectTurn>())
				{
					turnInterface.TurnRight();
				}
			}
		}

		prevMoveDir = moveDirection;
	}

	void FixedUpdate()
	{
		rigidBody.velocity = moveDirection * playerMoveSpeed;
	}
}
