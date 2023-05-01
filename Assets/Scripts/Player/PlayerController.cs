using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PMProblem;
using System;
using System.Linq;

public enum MoveDirection
{
	None, Left, Right, Up, Down
};

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
{
	PlayerControls playerControls = null;

	Player player = null;
	Rigidbody2D playerRigidbody = null;

	[SerializeField]
	float playerMoveSpeed = 0.5f;

	void Awake()
	{
		player = GetComponent<Player>();
		playerRigidbody = GetComponent<Rigidbody2D>();
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

	void OnDisable()
	{
		playerControls.Player.Disable();
	}

	private void FixedUpdate()
	{
		var movement = playerControls.Player.Move.ReadValue<Vector2>();
		movement.Normalize();
		playerRigidbody.velocity = movement * playerMoveSpeed;
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		var inputDir = context.ReadValue<Vector2>();
		if (inputDir.x < 0) 
		{
			player.TurnLeft();
		}
		else if(inputDir.x > 0)
		{
			player.TurnRight();
		}
	}
}
