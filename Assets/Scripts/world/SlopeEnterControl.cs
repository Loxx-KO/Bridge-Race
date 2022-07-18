using System;
using System.Collections.Generic;
using UnityEngine;

public class SlopeEnterControl : MonoBehaviour
{
    public bool someoneOnSlope = false;
    private void OnTriggerEnter(Collider other)
    {
        Vector3 direction = other.transform.position - transform.position;

        if (!someoneOnSlope)
        {
            if (Vector3.Dot(transform.forward, direction) < 0)
            {
                someoneOnSlope = true;

                if (other.name == "AI1" || other.name == "AI2" || other.name == "AI3")
                {
                    other.GetComponent<AIController>().StartBuilding();
                }
            }
        }
        else
        {
            if (other.name == "AI1" || other.name == "AI2" || other.name == "AI3")
            {
                other.GetComponent<AIController>().PushBack();
            }
            else if(other.name == "Player")
            {
                other.GetComponent<PlayerLogic>().PushBack();
            }
        }

        if (Vector3.Dot(transform.forward, direction) > 0)
        {
            if (other.name == "AI1" || other.name == "AI2" || other.name == "AI3")
            {
                other.GetComponent<AIController>().StopBuilding();
            }
            someoneOnSlope = false;
        }
    }
}
