using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(PlatformGenerator))]
public class PlatformGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		PlatformGenerator generator = (PlatformGenerator)target;
		if (GUILayout.Button("Generate Platform"))
		{
			generator.GeneratePlatform();
		}
	}
}