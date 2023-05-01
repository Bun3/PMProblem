using PMProblem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class testplay : MonoBehaviour
{
	PlayerControls playerControls = null;

	Animator animator = null;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void OnEnable()
	{
		if (playerControls == null)
		{
			playerControls = new PlayerControls();
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
		if (movement.x != 0)
			movement.y = 0;

		if (movement != Vector2.zero)
		{
			animator.SetFloat("DirX", movement.x);
			animator.SetFloat("DirY", movement.y);
		}
		animator.SetBool("Walking", movement != Vector2.zero);

	}

}
