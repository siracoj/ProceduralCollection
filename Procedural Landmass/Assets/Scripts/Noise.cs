using UnityEngine;
using System.Collections;

public static class Noise {

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset) {
		// Generationg noise map with a defined width and height
		// Noise maps are generated random 2d arrays based on a seed (time, posistion, etc...)

		float[,] noiseMap = new float[mapWidth, mapHeight];

		// Scale cannot equal 0
		if (scale <= 0) {
			scale = 0.0001f;
		}

		Vector2[] octaveOffsets = generateOctaveOffsets (seed, octaves);

		// Storing min and max values to normalize the noise map later
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2;
		float halfHeight = mapHeight / 2;

		// Map generation loop
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				/*
				* For multiple layers of generation going from "big picture" to small details
				* Each succesive octave will have a wider range of value and a smaller influence
				* The increase in the range of values determined by lacunarity
				* The decrease in influence determined by persistance
				*/
				for (int o = 0; o < octaves; o++) {
					/*
					* Scale to modify value
					* Frequency modifier for the current layer or "octave"
					* The higher the frequency the further apart the values are, so the values will change more rapidly
					*/
					float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[o].x + offset.x;
					float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[o].y + offset.y;

					// Noise value between -1 and 1 generated here
					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;

					noiseHeight += perlinValue * amplitude;

					/* 
					 * Persistance is a value between 0 and 1 and causes the amplitude to decrease each octave
					 *	Reducing the infulence each successive octave has
					 */ 
					amplitude *= persistance;

					// Lacunarity is a value > 1 increasing the frequency of value changes per octave
					frequency *= lacunarity;

				}

				// Keeping track of the max and min values for later normalization
				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				}else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}

				// Setting the value to the appropriate coordinate 
				noiseMap [x, y] = noiseHeight;
			}
		}

		// Normalizing all map values based on the min and max of the values
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
			}
		}

		return noiseMap;
	}

	private static Vector2[] generateOctaveOffsets(int seed, int octaves){
		// Randomizer based on seed
		System.Random prng = new System.Random (seed);

		// Array of offsets to change the generated map based on the seed provided
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			// Anything > 100000 seems to generate the same value in PerlinNoise
			float offsetX = prng.Next (-100000, 100000);
			float offsetY = prng.Next (-100000, 100000);
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}

		return octaveOffsets;
	}
}
