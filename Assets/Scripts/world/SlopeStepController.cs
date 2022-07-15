using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeStepController : MonoBehaviour
{
    [Header("Components")]
    public Material material;
    public BoxCollider invisibleWall;

    [Header("Values")]
    public bool isTaken = false;
    public string ownerName;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;

        invisibleWall.enabled = true;
        ownerName = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTaken)
        {
            if (other.name == "Player")
            {
                PlaceBlockPlayer(other);
            }
            //for ai
        }
        else
        {
            if (other.name == "Player")
            {
                if (ownerName == other.name)
                {
                    invisibleWall.enabled = false;
                }
                else 
                { 
                    PlaceBlockPlayer(other); 
                }
            }
            //for ai
        }
    }

    private void PlaceBlockPlayer(Collider other)
    {
        if (other.GetComponent<PlayerLogic>().CheckIfPlayerHasBlocks() > 0)
        {
            other.GetComponent<PlayerLogic>().PlaceBlock(this);
            GetComponent<MeshRenderer>().material = material;
            ownerName = other.name;

            if (other.GetComponent<PlayerLogic>().CheckIfPlayerHasBlocks() >= 1)
            {
                invisibleWall.enabled = false;
            }
            else if (other.GetComponent<PlayerLogic>().CheckIfPlayerHasBlocks() == 0)
            {
                invisibleWall.enabled = true;
            }
        }
        else
        {
            invisibleWall.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
