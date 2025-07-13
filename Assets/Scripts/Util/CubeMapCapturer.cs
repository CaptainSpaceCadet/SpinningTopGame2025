using UnityEngine;
using UnityEditor;

public class CubeMapCapturer : MonoBehaviour
{
	public string textureName = "cubemap";

	private void Start()
	{
		Capture();
	}
	private void Capture()
	{
		Camera camera = GetComponent<Camera>();
		Cubemap cubemap = new Cubemap(512, TextureFormat.RGBAFloat, false);

		camera.RenderToCubemap(cubemap);
		string path = "Assets/Arts/Textures/HDRI/" + textureName + ".asset";
		AssetDatabase.CreateAsset(cubemap, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Debug.Log("Wrote: " +  path);
	}
}
