using System;
using UnityEngine;

public class Balloon : MonoBehaviour
{
	// Initial transform
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Vector3 initialLocalScale;

	private void Start()
	{
		RecordInitialState();
		GameManager.instance.OnLevelStart += OnLevelStarted;
	}
	
	// Reset functions
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

	// Event functions
	private void OnLevelStarted()
	{
		ResetToInitialState();
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