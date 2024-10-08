using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    GameManager gameManager;
    public List<TMP_Text> texts = new List<TMP_Text>();
    public TMP_Text levelText;
    public TMP_Text scoreText;
    public TMP_Text goalText;
    public GameObject reticuleObject;

    private void Start()
    {
        SetLevel(SceneManager.GetActiveScene().buildIndex);
    }

    void SetLevel(int index)
    {
        levelText.gameObject.SetActive(true);
        levelText.text = "LEVEL " + index;
    }

    public void SetGoalText(int goal)
    {
        goalText.gameObject.SetActive(true);
        goalText.text = "GOAL: " + goal;
    }

    public void SetScoreText(int score)
    {
        scoreText.gameObject.SetActive(true);
        scoreText.text = "SCORE: " + score;
    }
}
