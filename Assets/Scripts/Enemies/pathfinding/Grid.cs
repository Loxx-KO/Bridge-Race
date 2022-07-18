using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Components")]
    public Transform target;
    public LayerMask unwalkableMask;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private bool walkable;
    private Node[,] grid;

    [Header("Grid variables")]
    public Vector2 gridWorldSize;
    public float nodeRadius;

    [Header("Other")]
    public bool displayGizmos;
    public Vector3 gridBottomLeft;
    public Vector3 gridBottomRight;
    public Vector3 gridTopLeft;
    public Vector3 gridTopRight;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        //grid bounds
        gridBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        gridBottomRight = transform.position + Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        gridTopLeft = transform.position - Vector3.right * gridWorldSize.x / 2 + Vector3.up * gridWorldSize.y / 2;
        gridTopRight = transform.position + Vector3.right * gridWorldSize.x / 2 + Vector3.up * gridWorldSize.y / 2;

        CreateGrid();
    }

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        gridBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        Vector3 worldPoint;
        
        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                worldPoint = gridBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node mainNode)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = mainNode.gridX + x;
                int checkY = mainNode.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node GetCurrentNodeFromPosition(Vector3 targetPosition)
    {
        float procentX = (targetPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float procentY = (targetPosition.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;
        procentX = Mathf.Clamp01(procentX);
        procentY = Mathf.Clamp01(procentY);

        int x = Mathf.CeilToInt(gridSizeX * procentX) - 1;
        int y = Mathf.CeilToInt(gridSizeY * procentY) - 1;
        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);

        return grid[x, y];
    }
    public bool CheckIfTargetIsInBounds()
    {
        if (target.position.x > gridBottomLeft.x && target.position.x < gridBottomRight.x
            && target.position.y > gridBottomLeft.y && target.position.y < gridTopLeft.y)
            return true;
        else
            return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0f));

        if(grid != null && displayGizmos)
        {
            Node targetNode = GetCurrentNodeFromPosition(target.position);
            foreach (Node n in grid)
            {
                if (n.walkableNode) Gizmos.color = Color.white;
                else Gizmos.color = Color.red;
                if (targetNode == n) Gizmos.color = Color.cyan;

                Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.02f));
            }
        }
    }
}
