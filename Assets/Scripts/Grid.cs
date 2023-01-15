// Grid.cs
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Grid
{
    // The size of the grid cells
    private float cellSize;
    // The grid as a dictionary
    private Dictionary<Vector2, List<NavMeshAgent>> grid;

    public Grid(float cellSize)
    {
        this.cellSize = cellSize;
        this.grid = new Dictionary<Vector2, List<NavMeshAgent>>();
    }

    public void AddObject(Transform transform, Vector3 velocity, float radius)
    {
        // Calculate the grid cell the object is in
        Vector2 gridCell = WorldToGrid(transform.position);

        // Add the object to the grid cell
        if (!grid.ContainsKey(gridCell))
        {
            grid[gridCell] = new List<NavMeshAgent>();
        }
        grid[gridCell].Add(transform.GetComponent<NavMeshAgent>());
    }

    public List<NavMeshAgent> GetNearbyObjects(Vector3 position, float radius)
    {
        List<NavMeshAgent> nearbyObjects = new List<NavMeshAgent>();

        // Calculate the grid cell the object is in
        Vector2 gridCell = WorldToGrid(position);

        // Check the 9 neighboring grid cells
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2 neighborCell = gridCell + new Vector2(x, y);
                if (grid.ContainsKey(neighborCell))
                {
                    nearbyObjects.AddRange(grid[neighborCell]);
                }
            }
        }
        return nearbyObjects;
    }

    private Vector2 WorldToGrid(Vector3 worldPosition)
    {
        Vector2 gridPosition = new Vector2(Mathf.FloorToInt(worldPosition.x / cellSize),
                                          Mathf.FloorToInt(worldPosition.z / cellSize));
        return gridPosition;
    }
}

