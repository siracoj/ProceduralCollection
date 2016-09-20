using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, ColorMap, Mesh}; // Types of maps that can be drawn
	public DrawMode drawMode;

	public int mapWidth;
	public int mapHeight;
	public float scale; // Scale of the map

	[Range(0,1)]
	public float persistence; // Value to change the decrease in influence between layers
	public float lacunarity; // Value to change the increase in frequency between layers
	public int octaves; // number of generation layers

	public int seed; // Seed for noise generation
	public Vector2 offset; // Manual offset to modify the noise

	public float heightMultiplier; // Sets the range of possible height value
	public AnimationCurve meshHeightCurve; // How height values are translated into actual height

	public bool autoUpdate;

	public TerrainType[] regions; // Regions for our map

	public void GenerateMap(){
		float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, seed, octaves, persistence, lacunarity, offset);

		// Using noise map to assign terrainTypes to pixels via height
		Color[] colorMap = new Color[mapWidth*mapHeight];
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						// Found the matching region, setting color
						colorMap [y * mapWidth + x] = regions [i].color;
						break;
					}
				}
			}
		}

		// Finds the instanse of the map display for the attached GameObject?
		MapDisplay display = FindObjectOfType<MapDisplay> ();

		// Draw the texture for the display
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (noiseMap));
		} else if (drawMode == DrawMode.ColorMap) {
			// Draw color map here
			display.DrawTexture (TextureGenerator.TextureFromColorMap (colorMap, mapWidth, mapHeight));
		} else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh (
				MeshGenerator.GenerateTerrainMesh (noiseMap, heightMultiplier, meshHeightCurve),
				TextureGenerator.TextureFromColorMap (colorMap, mapWidth, mapHeight)
			);
		}
	}

	// Validation for all inputs
	void OnValidate(){
		if (mapWidth < 1) {
			mapWidth = 1;
		}
		if (mapHeight < 1) {
			mapHeight = 1;
		}
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
	}
}

// Struct to manage terrain types (Mountain, valley, water, etc...)
// Why Serializable?
[System.Serializable]
public struct TerrainType {
	public string name;
	public float height; // Height value this terrain stops at
	public Color color; // Color of the terrain
}