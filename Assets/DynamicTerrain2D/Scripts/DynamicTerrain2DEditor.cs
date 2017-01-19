using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DynamicTerrain2D))]
public class DynamicTerrain2DEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		DynamicTerrain2D myScript = (DynamicTerrain2D)target;
		if(GUILayout.Button("Generate"))
		{
			myScript.RefreshTerrain();
		}
	}
}