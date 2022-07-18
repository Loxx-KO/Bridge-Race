using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FinishUIController : MonoBehaviour
{
    public TMP_Text winnerText;
    public GameObject UI;

    private void Start()
    {
        UI.SetActive(false);
    }

    public void SetWinner(string name)
    {
        winnerText.text += name;
    }

    public void ShowUI()
    {
        UI.SetActive(true);
    }

    public void PlayAgainButton()
    {
        SceneManager.LoadScene("h");
    }

    public void ExitButton()
    {
        Application.Quit();
    }

}
