using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FinishUIController : MonoBehaviour
{
    public TMP_Text winnerText;
    public GameObject UI;

    public Button butPlayAgain;
    public Button butExit;

    private void Start()
    {
        UI.SetActive(false);
    }

    /*private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }*/

    void OnEnable()
    {
        //Register Button Events
        butPlayAgain.onClick.AddListener(() => ButtonCallBack(butPlayAgain));
        butExit.onClick.AddListener(() => ButtonCallBack(butExit));

    }

    private void ButtonCallBack(Button buttonPressed)
    {
        if (buttonPressed == butPlayAgain)
        {
            PlayAgainButton();
        }

        if (buttonPressed == butExit)
        {
            ExitButton();
        }
    }

    void OnDisable()
    {
        //Un-Register Button Events
        butPlayAgain.onClick.RemoveAllListeners();
        butExit.onClick.RemoveAllListeners();
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
