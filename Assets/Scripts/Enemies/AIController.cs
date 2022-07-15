using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, IEnemyController
{
    void Start()
    {

    }
    public void CollectBlock()
    {
        Debug.Log("Block picked");
    }

    public void DropBlocks()
    {
        Debug.Log("Blocks dropped");
    }

    public void PlaceBlock()
    {
        Debug.Log("Block placed");
    }

    void Update()
    {
        
    }
}
