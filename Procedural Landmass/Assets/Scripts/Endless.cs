using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Endless : MonoBehaviour {

	public const float maxViewDistance = 300; // How far the player can see(Fixed value)
	public Transform viewer; // The player object
	public Transform parentMesh; // Meshes stored here

	public static Vector2 viewerPosition;
	private int chunkSize; // How big the chunks are
	private int chunksVisableInViewDistance; // Number of chunks the player can see

	// Keeping track of rendered terrain chunks
	private Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>(); 
	private List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	void Start() {
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisableInViewDistance = Mathf.RoundToInt (maxViewDistance / chunkSize);
	}

	void Update(){
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);
		UpdateVisableChunks ();
	}

	void UpdateVisableChunks(){

		// Set all
		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();

		// Which chuck the player is currently in
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);

		// Need to update chunks in a range of 
		// (-chunksVisableInViewDistance, -chunksVisableInViewDistance) 
		// to (chunksVisableInViewDistance, chunksVisableInViewDistance) 
		// Relative to the players current chunk
		for (int yOffset = -chunksVisableInViewDistance; yOffset <= chunksVisableInViewDistance; yOffset++) {
			for (int xOffset = -chunksVisableInViewDistance; xOffset <= chunksVisableInViewDistance; xOffset++) {
				Vector2 chunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (terrainChunks.ContainsKey (chunkCoord)) {
					terrainChunks [chunkCoord].UpdateTerrainChunk();

					// If visible post update, add it in
					if (terrainChunks [chunkCoord].IsVisible ())
						terrainChunksVisibleLastUpdate.Add (terrainChunks [chunkCoord]);
					
				} else {
					terrainChunks.Add (chunkCoord, new TerrainChunk (chunkCoord, chunkSize, parentMesh));

				}
			}
		}
	}

	public class TerrainChunk {

		private Vector2 position;
		private GameObject mesh;
		private Bounds bounds;

		public TerrainChunk(Vector2 coord, int size, Transform parent){
			position = coord * size;
			bounds = new Bounds(position, Vector2.one*size);
			Vector3 positionV3 = new Vector3 (position.x, 0, position.y); // transforms requre a vector 3 for positioning

			mesh = GameObject.CreatePrimitive (PrimitiveType.Plane); // create blank mesh
			mesh.transform.position = positionV3; // Setting the position according to the provided coords
			mesh.transform.transform.localScale = Vector3.one * size/10f; // resize the mesh
			mesh.transform.parent = parent; // Keeping the meshes organized

			SetVisible (false); // Default hidden

		}

		public void UpdateTerrainChunk(){
			// Find the point on its perimeter closest to the viewer
			float distanceToViewer = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));
			// Check if that point is within the viewer's view distance
			bool visible = distanceToViewer <= maxViewDistance;
			// If it is it will enable the mesh (Show it) else Disable it (Hide it)
			SetVisible (visible);


		}

		public void SetVisible(bool visible){
			mesh.SetActive (visible);
		}

		public bool IsVisible(){
			return mesh.activeSelf;
		}

	}
}
