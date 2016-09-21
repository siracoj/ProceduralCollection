using UnityEngine;
using System.Collections;

public static class MeshGenerator {

	// Creating a 3D mesh from our height map
	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMulitplier, AnimationCurve heightCurve, int levelOfDetail){
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		// Multipling by 2 to make it a factor of 240
		int meshSimplificationIncrement = levelOfDetail * 2;

		// Has to at least = 1 to work
		if (meshSimplificationIncrement == 0)
			meshSimplificationIncrement = 1;

		// Determining "Vertex width"
		int verticiesPerLine = (width - 1) / meshSimplificationIncrement + 1;

		// making sure that the mesh is centered
		float topLeftX = (width - 1) / -2f; 
		float topLeftZ = (height - 1) / 2f;

		MeshData meshData = new MeshData (verticiesPerLine, verticiesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height; y+=meshSimplificationIncrement) {
			for (int x = 0; x < width; x+=meshSimplificationIncrement) {

				meshData.vertices [vertexIndex] = new Vector3 (
					topLeftX + x, 
					heightCurve.Evaluate(heightMap [x, y])*heightMulitplier, 
					topLeftZ - y
				);

				meshData.uvs[vertexIndex] = new Vector2(x/(float)width, y/(float)height); // % of width and height

				// Ignoring the bottom and right edges of the map (No need to create triangles here)
				if (x < width - 1 && y < height - 1) {
					/*
					 * The map will look something like this:
					 * |     CurrentVertex     |     CurrentVertex + 1     | ...
					 * | CurrentVertex + Width | CurrentVertex + Width + 1 | ...
					 * 
					 * This Function will create 2 triangles from these 4 points for the mesh generation
					 */

					int trianglePointA = vertexIndex; // Current point
					int trianglePointB = trianglePointA + 1; // Point 1 to the right
					int trianglePointC = trianglePointA + verticiesPerLine; // Point 1 down
					int trianglePointD = trianglePointB + verticiesPerLine; // Point 1 to the right and 1 down

					// Adding the two triangles related to the current vertex
					meshData.AddTriangle (trianglePointA, trianglePointD, trianglePointC);
					meshData.AddTriangle (trianglePointD, trianglePointA, trianglePointB);

				}
				vertexIndex++;
			}
		}

		/* 
		 * Returning meshData object here rather than the actual mesh 
		 * because this allows us to generate the mesh data using multi threading, 
		 * then generate the mesh in our main thread (Meshes are not serializable)
		 */
		return meshData;
	}
}

public class MeshData{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs; // to add textures to the mesh?

	private int triangleIndex;

	public MeshData(int meshWidth, int meshHeight){

		int totalVertices = meshWidth * meshHeight;
		int totalTriangles = (meshWidth - 1) * (meshHeight - 1) * 6;

		vertices = new Vector3[totalVertices];
		uvs = new Vector2[totalVertices];
		triangles = new int[totalTriangles];


	}

	public void AddTriangle(int a, int b, int c){
		triangles [triangleIndex] = a;
		triangles [triangleIndex + 1] = b;
		triangles [triangleIndex + 2] = c;

		triangleIndex += 3;
	}

	// Create the mesh from the verticies and triangles
	public Mesh CreateMesh() {
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals (); // Fixing lighting???

		return mesh;
	}
}
