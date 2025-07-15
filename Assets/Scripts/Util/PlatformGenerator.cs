using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
	// Ok so this doesn't work intuitively but I'm the only one using it so it is fine for the moment.
	
	[SerializeField] private Vector3 scale;
	
	[SerializeField] private int lengthX = 30;
	[SerializeField] private int widthZ = 30;

	[SerializeField] private Vector2Int[] holesXBound;
	[SerializeField] private Vector2Int[] holesZBound;

	public void GeneratePlatform()
	{
		for (int x = -lengthX / 2; x <= lengthX / 2; x++)
		{
			for (int z = -widthZ / 2; z <= widthZ / 2; z++)
			{
				for (int i = 0; i < holesXBound.Length; i++)
				{
					if (x > holesXBound[i].x && x < holesXBound[i].y
					                         && z > holesZBound[i].x && z < holesZBound[i].y) goto skip_cube;
				}
				
				// Add cube
				Vector3 position = new Vector3(x, 0, z);
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.position = position;
				cube.transform.SetParent(transform);

				skip_cube:;
			}
		}
		
		transform.localScale = scale;
	}
	
	public void DeletePlatform()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			#if UNITY_EDITOR
			DestroyImmediate(transform.GetChild(i).gameObject);
			#else
			Destroy(transform.GetChild(i).gameObject);
			#endif
		}
	}
}
