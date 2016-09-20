using UnityEngine;
using System.Collections;
using UnityEditor;

// Class to create a new button in the editior to run the map generator
[CustomEditor (typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI(){
		MapGenerator mapGen = (MapGenerator)target;

		// If something changes in the editor, and auto update is true
		if (DrawDefaultInspector () && mapGen.autoUpdate) {
			mapGen.GenerateMap ();
		}

		if (GUILayout.Button ("Generate")) {
			mapGen.GenerateMap ();
		}

	}

}
