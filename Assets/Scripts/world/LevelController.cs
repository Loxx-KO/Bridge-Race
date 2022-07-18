using System;
using System.Collections.Generic;
using UnityEngine;


public class LevelController : MonoBehaviour
{
    public string lvlNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Player")
        {
            other.GetComponent<AIController>().ChangeCurrentLevel();
        }
    }
}

