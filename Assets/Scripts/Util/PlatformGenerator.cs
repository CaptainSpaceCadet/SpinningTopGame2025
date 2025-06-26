using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class PlatformGenerator : MonoBehaviour
{
	public int x_size = 30;
	public int z_size = 30;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	public void GeneratePlatform()
	{
		for (int x = -x_size / 2; x < x_size / 2; x++)
		{
			for (int z = -z_size / 2; z < z_size / 2; z++)
			{
				Vector3 position = new Vector3(x, 0, z);
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.position = position;
				cube.transform.SetParent(this.transform);
			}
		}
	}


}
