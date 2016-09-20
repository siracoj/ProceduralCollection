using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {
	
	public Renderer textureRenderer;
	public MeshFilter meshFilter;
	public MeshRenderer MeshRenderer;

	public void DrawTexture(Texture2D texture){
		
		// Using sharedMaterial Rather than Material because it is rendered in the editior
		textureRenderer.sharedMaterial.mainTexture = texture;
		// Setting the object to the size of the noiseMap
		textureRenderer.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

	public void DrawMesh(MeshData meshData, Texture2D texture){
		meshFilter.sharedMesh = meshData.CreateMesh ();
		MeshRenderer.sharedMaterial.mainTexture = texture;
	}
}
