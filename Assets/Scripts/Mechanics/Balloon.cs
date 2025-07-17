using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class Balloon : MonoBehaviour
{
	public MMF_Player popingFeedback;
	// Initial transform
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Vector3 initialLocalScale;

	private bool poped = false;

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

	private async void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("player"))
		{
			if (!poped)
			{
				poped = true;
				await popingFeedback?.PlayFeedbacksTask();
				GameManager.instance.IncreaseBalloons();

				gameObject.SetActive(false);
			}
		}
	}
}