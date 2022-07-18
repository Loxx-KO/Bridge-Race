using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FinishLineControl : MonoBehaviour
{
    public bool gameFinished = false;
    public string winnerName = "";
    public Transform winnerPos;
    public List<Transform> loserPos = new List<Transform>();
    public List<Transform> players = new List<Transform>();
    public FinishUIController finishUIController;

    private void OnTriggerEnter(Collider other)
    {
        gameFinished = true;
        winnerName = other.name;

        if (other.name == "AI1" || other.name == "AI2" || other.name == "AI3")
        {
            other.GetComponent<AIController>().FinishGame(winnerPos.position);
        }
        else if (other.name == "Player")
        {
            other.GetComponent<PlayerLogic>().FinishGame(winnerPos.position);
        }

        int i = 0;
        foreach(Transform player in players)
        {
            if(player.name != winnerName)
            {
                if (other.name == "AI1" || other.name == "AI2" || other.name == "AI3")
                {
                    other.GetComponent<AIController>().FinishGame(loserPos[i].position);
                }
                else if (other.name == "Player")
                {
                    other.GetComponent<PlayerLogic>().FinishGame(loserPos[i].position);
                }
                i++;
            }
        }
        SetWinnerName();
    }

    public void SetWinnerName()
    {
        finishUIController.SetWinner(winnerName);
        finishUIController.ShowUI();
    }
}

