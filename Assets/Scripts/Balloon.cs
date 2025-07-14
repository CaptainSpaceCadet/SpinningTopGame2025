using System;
using UnityEngine;

public class Balloon : MonoBehaviour
{
	Transform initialTransform;
	
	private void RecordInitialState()
	{
		initialTransform.position = transform.position;
		initialTransform.rotation = transform.rotation;
		initialTransform.localScale = transform.localScale;
	}

	private void ResetToInitialState()
	{
		gameObject.SetActive(true);
		transform.position = initialTransform.position;
		transform.rotation = initialTransform.rotation;
		transform.localScale = initialTransform.localScale;
	}

	private void OnLevelStarted()
	{
		ResetToInitialState();
	}

	private void Start()
	{
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