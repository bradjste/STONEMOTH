using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject winnerObject;

    private void Start()
    {
        winnerObject.SetActive(PlayerStats.IsWinner);
    }

    public void PlayGame()
    {
        PlayerStats.CurrentLevel = 1;
        SceneManager.LoadSceneAsync(PlayerStats.CurrentLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
