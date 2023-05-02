using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
	public static void DrawDebugBox2D(Vector2 position, Vector2 size, Color color, float duration = 1.0f)
	{
		var leftTop = new Vector2(position.x - size.x * 0.5f, position.y + size.y * 0.5f);
		var rightTop = new Vector2(position.x + size.x * 0.5f, position.y + size.y * 0.5f);
		var leftBottom = new Vector2(position.x - size.x * 0.5f, position.y - size.y * 0.5f);
		var rightBottom = new Vector2(position.x + size.x * 0.5f, position.y - size.y * 0.5f);


		Debug.DrawLine(leftTop, leftBottom, color, duration);
		Debug.DrawLine(leftBottom, rightBottom, color, duration);
		Debug.DrawLine(rightBottom, rightTop, color, duration);
		Debug.DrawLine(rightTop, leftTop, color, duration);
	}

	public static void DrawNodeLines(IEnumerable<Node> nodeEnumerator, Color color, float duration = 1.0f)
	{
		Node prev = null;
		foreach (var node in nodeEnumerator)
		{
			if(prev != null)
			{
				Debug.DrawLine(prev, node, color, duration);
			}
			prev = node;
		}
	}

}
