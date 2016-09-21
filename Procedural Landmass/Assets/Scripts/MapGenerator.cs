using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, ColorMap, Mesh}; // Types of maps that can be drawn
	public DrawMode drawMode; // Editor setting for draw mode

	/*
	 * Determining Chunk Size:
	 * Unity's Vertex limit : 65535 (256^2 - 1)
	 * Width of the largest possible square : 255
	 * Using 241 as the width because the level of detail(LOD) must be determined by a factor of width - 1
	 * Useful Factors of 240: 1, 2, 4, 6, 8, 10, 12 (240 gives us lots of options for LOD)
	 */ 
	public const int mapChunkSize = 241; // Fixed Square size for our mesh chunks
	[Range(0,6)]
	public int levelOfDetail; // # of vertecies to skip over rendering, reducing detail
	public float scale; // Scale of the map

	// Procedural Generation modifiers
	[Range(0,1)]
	public float persistence; // Value to change the decrease in influence between layers
	public float lacunarity; // Value to change the increase in frequency between layers
	public int octaves; // number of generation layers
	public int seed; // Seed for noise generation
	public Vector2 offset; // Manual offset to modify the noise
	public float heightMultiplier; // Sets the range of possible height value
	public AnimationCurve meshHeightCurve; // How height values are translated into actual height

	// Auto update when editor changes are made
	public bool autoUpdate;

	// Regions for our map (Determined by Height)
	public TerrainType[] regions; 

	public void GenerateMap(){
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, scale, seed, octaves, persistence, lacunarity, offset);

		// Using noise map to assign terrainTypes to pixels via height
		Color[] colorMap = new Color[mapChunkSize*mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						// Found the matching region, setting color
						colorMap [y * mapChunkSize + x] = regions [i].color;
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
			display.DrawTexture (TextureGenerator.TextureFromColorMap (colorMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh (
				MeshGenerator.GenerateTerrainMesh (noiseMap, heightMultiplier, meshHeightCurve, levelOfDetail),
				TextureGenerator.TextureFromColorMap (colorMap, mapChunkSize, mapChunkSize)
			);
		}
	}

	// Validation for all inputs
	void OnValidate(){
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