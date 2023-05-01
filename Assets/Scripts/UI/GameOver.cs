using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	public void OnClickRestartButton()
	{
		GameManager.Instance.RestartGame();
	}

	public void OnClickQuitButton()
	{
		Application.Quit();
	}

}
