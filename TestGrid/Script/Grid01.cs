using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid01 : MonoBehaviour {

	public bool onlyDisplayPathGizmos;
	public Transform player;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;
 	
	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Start(){
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	public int MaxSIze {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid(){
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++){
			for (int y = 0; y < gridSizeY; y ++){
				Vector3 worldPoint = worldBottomLeft +  Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius); 
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
				grid[x,y] = new Node(walkable, worldPoint, x, y);

				//Debug.Log("x: " + x + " y: " + y + " walkable: " + walkable + " worldPoint: " + worldPoint);
			}
		}
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY){
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}

	public Node NodeFromWorldPosition(Vector3 worldPosition) {
		float percenX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percenY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percenX = Mathf.Clamp01 (percenX);
		percenY = Mathf.Clamp01 (percenY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percenX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percenY);
		return grid[x,y];
	}

	public List<Node> path;
	void OnDrawGizmos(){

		Gizmos.DrawWireCube (transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
		Node playerNode = null;
		if (onlyDisplayPathGizmos) {
			if (path != null) {
				playerNode = NodeFromWorldPosition(player.position);
				foreach (Node n in path){
					if (playerNode == n) {
						Gizmos.color = Color.green;
					}
					Gizmos.color = Color.blue;
					Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter - .01f));
				}
			}
		} else {

			if (grid != null){
				playerNode = NodeFromWorldPosition(player.position);
				foreach (Node n in grid){
					Gizmos.color = (n.walkable)?Color.white:Color.red;
					if (playerNode == n) {
						Gizmos.color = Color.green;
					}

					if (path != null)
						if (path.Contains(n))
							Gizmos.color = Color.blue;
					//Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
					Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter - .01f));
				}

			}
		
		}

	}
}
