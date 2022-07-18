using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AstarPathfindingSystem : MonoBehaviour
{
    //G cost - distance from start node
    //H cost distance from end node
    //F cost - G + H

    public PathRequestManager pathRequestManager;
    public Grid grid;

    private void Awake()
    {
        pathRequestManager = GetComponent<PathRequestManager>();
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.GetCurrentNodeFromPosition(startPos);
        Node targetNode = grid.GetCurrentNodeFromPosition(targetPos);


        if (startNode.walkableNode && targetNode.walkableNode)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkableNode || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode); ;
        }
        pathRequestManager.FinishedProcessingPath(wayPoints, pathSuccess);
    }

    public Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        if (currentNode == startNode)
            path.Add(currentNode);

        Vector3[] wayPoints = SimplifyPath(path);
        Array.Reverse(wayPoints);
        return wayPoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        wayPoints.Add(path[0].worldPosition); // add the first waypoint
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                wayPoints.Add(path[i].worldPosition);
                //wayPoints.Add(path[i-1].worldPosition); if something breaks look here ;)
                directionOld = directionNew;
            }
        }
        return wayPoints.ToArray();
    }
    public int GetDistance(Node currentNode, Node targetNode)
    {
        int distX = Mathf.Abs(currentNode.gridX - targetNode.gridX);
        int distY = Mathf.Abs(currentNode.gridY - targetNode.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        else
            return 14 * distX + 10 * (distY - distX);
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    public bool TargetIsInBounds()
    {
        return grid.CheckIfTargetIsInBounds();
    }
}
