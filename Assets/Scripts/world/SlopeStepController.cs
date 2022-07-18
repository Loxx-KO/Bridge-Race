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
    public bool isTheExit = false;

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
            if (other.name == "Player") PlaceBlockPlayer(other);
            else if (other.name == "AI1" || other.name == "AI2" || other.name == "AI3") PlaceBlockAI(other);
        }
        else
        {
            if (other.name == "Player")
            {
                if (ownerName == other.name) invisibleWall.enabled = false;
                else PlaceBlockPlayer(other); 
            }
            else if(other.name == "AI1" || other.name == "AI2" || other.name == "AI3")
            {
                if (ownerName == other.name) invisibleWall.enabled = false;
                else PlaceBlockAI(other);
            }
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
                if(!isTheExit) invisibleWall.enabled = true;
                else invisibleWall.enabled = false;
            }
        }
        /*else if noone on slope
        {
            invisibleWall.enabled = true;
        }*/
    }

    private void PlaceBlockAI(Collider other)
    {
        if (other.GetComponent<AIController>().CheckIfPlayerHasBlocks() > 0)
        {
            other.GetComponent<AIController>().PlaceBlock(this);
            GetComponent<MeshRenderer>().material = material;
            ownerName = other.name;

            if (other.GetComponent<AIController>().CheckIfPlayerHasBlocks() >= 1)
            {
                invisibleWall.enabled = false;
            }
            else if (other.GetComponent<AIController>().CheckIfPlayerHasBlocks() == 0)
            {
                if (!isTheExit) invisibleWall.enabled = true;
                else invisibleWall.enabled = false;

            }
        }
        /*else
        {
            invisibleWall.enabled = true;
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
