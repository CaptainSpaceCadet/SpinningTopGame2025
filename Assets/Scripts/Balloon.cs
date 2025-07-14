using System;
using UnityEngine;

public class Balloon : MonoBehaviour
{
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Vector3 initialLocalScale;
	
	private void RecordInitialState()
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		initialLocalScale = transform.localScale;
	}

	private void ResetToInitialState()
	{
		gameObject.SetActive(true);
		transform.position = initialPosition;
		transform.rotation = initialRotation;
		transform.localScale = initialLocalScale;
	}

	private void OnLevelStarted()
	{
		ResetToInitialState();
	}

	private void Start()
	{
		RecordInitialState();
		GameManager.instance.OnLevelStart += OnLevelStarted;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("player"))
		{
			GameManager.instance.IncreaseBalloons();
			gameObject.SetActive(false);
		}
	}
}