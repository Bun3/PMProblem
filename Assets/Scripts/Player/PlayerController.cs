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

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
{
	PlayerControls playerControls;
	Vector2 moveDirection = Vector2.zero;
	Vector2 prevMoveDir = Vector2.zero;

	IObjectInterface[] objectInterfaces = null;

	Rigidbody2D rigidBody;

	Player player = null;

	[SerializeField]
	float playerMoveSpeed = 0.5f;

	public void OnMove(InputAction.CallbackContext context)
	{
		moveDirection = context.ReadValue<Vector2>();
	}

	void Awake()
	{
		objectInterfaces = GetComponents<IObjectInterface>();
		rigidBody = GetComponent<Rigidbody2D>();
		player = GetComponent<Player>();
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
				player.TurnLeft();
			}
			else if (moveDirection.x > 0.0f && prevMoveDir.x <= 0.0f)
			{
				player.TurnRight();
			}
		}

		prevMoveDir = moveDirection;
	}

	void FixedUpdate()
	{
		rigidBody.velocity = moveDirection * playerMoveSpeed;
	}
}
