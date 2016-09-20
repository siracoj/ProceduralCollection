using UnityEngine;
using System.Collections;

public static class TextureGenerator{

	public static Texture2D TextureFromColorMap(Color[] ColorMap, int width, int height){
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode = FilterMode.Point; // Fixing blurry rendering
		texture.wrapMode = TextureWrapMode.Clamp; // Getting rid of repeating texture
		texture.SetPixels (ColorMap);
		texture.Apply ();
		return texture;
	}

	public static Texture2D TextureFromHeightMap(float[,] heightMap){
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		// Faster to generate the color map, then set the all of the pixels rather than 
		// generating the color for each pixel then setting the pixel

		// Generate ColorMap
		Color[] colorMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				int colorMapIndex = y * width + x;

				// Setting a color between black and white dependent on the generated noise map value
				colorMap [colorMapIndex] = Color.Lerp (Color.black, Color.white, heightMap [x, y]);
			}
		}

		return TextureFromColorMap (colorMap, width, height);
	}
}
