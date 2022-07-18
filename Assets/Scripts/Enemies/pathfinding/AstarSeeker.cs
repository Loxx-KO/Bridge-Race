using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarSeeker : MonoBehaviour
{
    public AstarPathfindingSystem pathfinding;
    public Transform target;
    float speed = 2f;
    Vector3[] path;
    int targetIndex = 0;

    private void Start()
    {
        //PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        InvokeRepeating("UpdatePath", 0f, 1f);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccesful)
    {
        if(pathSuccesful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        targetIndex = 0;
        Vector3 currentWayPoint = path[0];
        while (true)
        {
            if (transform.position == currentWayPoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    targetIndex = 0;
                    path = new Vector3[0];
                    yield break;
                }
                currentWayPoint = path[targetIndex];
            }
            //movement
            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void UpdatePath()
    {
        if (pathfinding.TargetIsInBounds())
        {
            targetIndex = 0;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }
        else
            path = null;
    }

    private void OnDrawGizmos()
    {
        if(path != null)
        {
            for(int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}
