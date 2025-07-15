using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OhajikiSpawner : MonoBehaviour
{
	// Fields shown in the inspector
	[Header("Spawning Settings")]
	[SerializeField] private int ohajikiGroupSize = 3; 
	[SerializeField] private float spawnRateInSeconds = 2f;
	[Tooltip("This property is automatically set if a collider is available")]
	[SerializeField] private float spaceBetweenOhajikis = 2.0f;
	
	[Header("Ohajiki options")]
	[SerializeField] private Collider groundedBounds;

	[Header("Prefab")]
	[SerializeField] private GameObject ohajikiPrefab;
	
	[Header("Initial State")]
	[SerializeField] private GameObject[] initialOhajikis;
	
	// Private members
	private bool canSpawn;

	// References to spawned ohajiki
	private Ohajiki[] initialOhajikisComp;
	private Queue<GameObject> ohajikiQueue;
	private Queue<Ohajiki> ohajikiQueueComp;

	private void Start()
	{
		// Instantiate ohajikiPrefab to get the size of a collider attached to the object
		var temp = Instantiate(ohajikiPrefab);
		var collider = temp.GetComponent<Collider>();
		
		spaceBetweenOhajikis = collider?.bounds.size.x ?? spaceBetweenOhajikis;
		Destroy(temp);
		
		// Store initial ohajiki
		initialOhajikisComp = new Ohajiki[initialOhajikis.Length];
		for (int i = 0; i < initialOhajikis.Length; i++)
		{
			initialOhajikisComp[i] = initialOhajikis[i].GetComponent<Ohajiki>();
			initialOhajikisComp[i].groundedBounds = groundedBounds;
		}

		ohajikiQueue = new Queue<GameObject>(initialOhajikis);
		ohajikiQueueComp = new Queue<Ohajiki>(initialOhajikisComp);
		canSpawn = true;
		
		GameManager.instance.OnLevelStart += OnLevelStarted;
		GameManager.instance.OnLevelEnd += OnLevelEnded;
	}

	private void Update()
	{
		if (canSpawn) SpawnOhajiki();
	}
	
	private async Task SpawnOhajiki()
	{
		canSpawn = false;
		if (ohajikiPrefab == null || ohajikiGroupSize <= 0) return;

		float totalLength = (ohajikiGroupSize - 1) * spaceBetweenOhajikis;
		float startX = -totalLength / 2f;

		for (int i = 0; i < ohajikiGroupSize; i++)
		{
			Vector3 localOffset = new Vector3(startX + i * spaceBetweenOhajikis, 0.15f, 0f);
			Vector3 spawnPos = transform.position + transform.TransformDirection(localOffset);

			if (ohajikiQueue.Count < 1 || ohajikiQueue.Peek().layer != 6)
			{
				GameObject ohajiki = Instantiate(ohajikiPrefab, spawnPos, transform.rotation);
				Ohajiki ohajikiComp = ohajiki.GetComponent<Ohajiki>();
				ohajikiComp.groundedBounds = groundedBounds;
				
				ohajikiQueue.Enqueue(ohajiki);
				ohajikiQueueComp.Enqueue(ohajikiComp);
			}
			else
			{
				Debug.Log("Ohajiki Teleport");
				GameObject ohajiki = ohajikiQueue.Dequeue();
				Ohajiki ohajikiComp = ohajikiQueueComp.Dequeue();
				ohajiki.transform.position = spawnPos;
				ohajiki.transform.rotation = transform.rotation;
				//ohajikiComp.groundedBounds = groundedBounds;
				ohajikiComp.SetGrounded();
			}
		}

		await UniTask.Delay(TimeSpan.FromSeconds(spawnRateInSeconds));
		canSpawn = true;
	}

	// Utility functions
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 origin = transform.position;
		Vector3 direction = transform.forward * 10.0f;
		Gizmos.DrawRay(origin, direction);
	}

	// Event functions
	private void OnLevelStarted()
	{
		gameObject.SetActive(true);
	}
	
	private void OnLevelEnded()
	{
		gameObject.SetActive(false);
	}
}
