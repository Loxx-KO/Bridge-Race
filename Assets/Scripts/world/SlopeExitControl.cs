using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeExitControl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Vector3 direction = other.transform.position - transform.position;

        if (Vector3.Dot(transform.forward, direction) > 0)
        {
            if (other.name == "AI1" || other.name == "AI2" || other.name == "AI3")
            {
                other.GetComponent<AIController>().PushBack();
            }
            else if (other.name == "Player")
            {
                other.GetComponent<PlayerLogic>().PushBack();
            }
        }
        else if (Vector3.Dot(transform.forward, direction) < 0)
        {
            if (other.name == "AI1" || other.name == "AI2" || other.name == "AI3")
            {
                other.GetComponent<AIController>().ChangeCurrentLevel();
            }
        }
    }
}
