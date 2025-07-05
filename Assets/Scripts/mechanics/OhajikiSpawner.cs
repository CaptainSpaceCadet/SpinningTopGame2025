using UnityEngine;

public class OhajikiSpawner : MonoBehaviour
{
	[SerializeField] private GameObject ohajikiPrefab;
	[SerializeField] private int ohajikiAmount = 3;
	[Header("This property is automatically set if a collider is available")]
	[SerializeField] private float spaceBetweenOhajikis = 2.0f;
	private GameObject ohajikiGroup;


	private void Start()
	{
		var collider = ohajikiPrefab?.GetComponent<Collider>();
		spaceBetweenOhajikis = collider?.bounds.size.x ?? spaceBetweenOhajikis;

		CreateOhajikiGroup();
	}

	private void SpawnOhajiki()
	{
		Instantiate(ohajikiGroup, this.transform.position, this.transform.rotation);
	}


	private void CreateOhajikiGroup()
	{
		if (ohajikiPrefab == null || ohajikiAmount <= 0) return;

		ohajikiGroup = new GameObject("OhajikiGroup");
		ohajikiGroup.transform.SetParent(transform, false);

		float totalLength = (ohajikiAmount - 1) * spaceBetweenOhajikis;
		float startX = -totalLength / 2f;

		for (int i = 0; i < ohajikiAmount; i++)
		{
			Vector3 localOffset = new Vector3(startX + i * spaceBetweenOhajikis, 0f, 0f);
			Vector3 spawnPos = transform.position + transform.TransformDirection(localOffset);

			Instantiate(ohajikiPrefab, spawnPos, Quaternion.identity, ohajikiGroup.transform);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 origin = transform.position;
		Vector3 direction = transform.forward * 10.0f;
		Gizmos.DrawRay(origin, direction);
	}
}
