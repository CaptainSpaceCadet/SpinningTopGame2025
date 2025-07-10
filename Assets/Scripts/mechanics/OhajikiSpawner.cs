using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class OhajikiSpawner : MonoBehaviour
{
	[SerializeField] private GameObject ohajikiPrefab;
	[SerializeField] private int ohajikiAmount = 3; 
	[SerializeField] private float spawnRateInSeconds = 2f;
	[Header("This property is automatically set if a collider is available")]
	[SerializeField] private float spaceBetweenOhajikis = 2.0f;
	private GameObject ohajikiGroup;

	private bool canSpawn;


	private void Start()
	{
		GameManager.instance.OnLevelEnd += OnLevelEnd;
		
		// Instantiate ohajikiPrefab to get the size of a collider attached to the object
		var temp = Instantiate(ohajikiPrefab);
		var collider = temp.GetComponent<Collider>();
		spaceBetweenOhajikis = collider?.bounds.size.x ?? spaceBetweenOhajikis;
		Destroy(temp);

		canSpawn = true;
	}

	private void Update()
	{
		if(canSpawn) SpawnOhajiki();
	}

	// This might need a serious optimization
	// might use object pool if this is too computationally costly
	// but as long as it works on our laptop, I'm just going to leave it 
	private async Task SpawnOhajiki()
	{
		canSpawn = false;
		if (ohajikiPrefab == null || ohajikiAmount <= 0) return;

		float totalLength = (ohajikiAmount - 1) * spaceBetweenOhajikis;
		float startX = -totalLength / 2f;

		for (int i = 0; i < ohajikiAmount; i++)
		{
			Vector3 localOffset = new Vector3(startX + i * spaceBetweenOhajikis, 0f, 0f);
			//Debug.Log(spaceBetweenOhajikis);
			Vector3 spawnPos = transform.position + transform.TransformDirection(localOffset);

			Instantiate(ohajikiPrefab, spawnPos, this.transform.rotation);
		}

		await UniTask.Delay(TimeSpan.FromSeconds(spawnRateInSeconds));
		canSpawn = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 origin = transform.position;
		Vector3 direction = transform.forward * 10.0f;
		Gizmos.DrawRay(origin, direction);
	}

	private void OnLevelEnd()
	{
		gameObject.SetActive(false);
	}
}
