using UnityEngine;
using System.Collections;

public enum SwipeDirection {
	None = 0,
	Left = 1,
	Right = 2,
	Up = 4,
	Down = 8,
}

public class SwipeManager : MonoBehaviour {
	public SwipeDirection direction;
	public static SwipeManager instance;

	private Vector3 touchPosition;
	private float swipeResistanceX = 50.0f;
	private float swipeResistanceY = 100.0f;


	void Start () {
		instance = this;
	}

	void Update () {
		direction = SwipeDirection.None;

		if (Input.GetMouseButtonDown(0)) {
			touchPosition = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp (0)) {
			Vector2 deltaSwipe = Input.mousePosition - touchPosition;

			if (Mathf.Abs (deltaSwipe.x) > swipeResistanceX) {
				direction |= deltaSwipe.x < 0 ? SwipeDirection.Left: SwipeDirection.Right;
			}

			if (Mathf.Abs (deltaSwipe.y) > swipeResistanceY) {
				direction |= deltaSwipe.y < 0 ? SwipeDirection.Down: SwipeDirection.Up;
			}
		}
	}

	public bool IsSwiping(SwipeDirection dir)
	{
		return (direction & dir) == dir;
	}﻿
}
