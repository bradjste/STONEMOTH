using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject swarmPrefab;
    [SerializeField]
    public int swarmCount = 1;
    [SerializeField]
    public List<Swarm> swarms = new List<Swarm>();
    GameObject swarmsObject;

    [SerializeField]
    List<Color> targetColors = new List<Color>();

    [SerializeField]
    public int score = 0;
    [SerializeField]
    public int goal = 10;

    [SerializeField]
    bool gameOver = false;

    public static GameManager Instance;

    [SerializeField]
    int swarmSpawnCount = 30;

    [SerializeField]
    GameObject userInterfaceObject;
    public UserInterface userInterface;
    public List<Goal> goals;

    private void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            /*DontDestroyOnLoad(gameObject);*/ // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }

        if (userInterfaceObject != null)
        {
            userInterface = userInterfaceObject.GetComponent<UserInterface>();
            userInterface.SetGoalText(goal);
            userInterface.SetScoreText(score);
        }

        PlayerStats.CurrentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateSwarms();
        GenerateInitialCreatures();
    }

    public void AddToScore(int points)
    {
        score += points;
        userInterface.SetScoreText(score);

        if (score >= goal && !gameOver)
        {
            WinGame();
        }
    }

    void GenerateSwarms()
    {
        swarmsObject = new GameObject("Swarms");
        swarms.Clear();
        for (int i = 0; i < swarmCount; i++)
        {
            Swarm swarm = Instantiate(swarmPrefab, swarmsObject.transform).GetComponent<Swarm>();
            swarm.gameManager = this;
            swarm.spawnCount = swarmSpawnCount;

            if (userInterface != null)
            {
                swarm.swarmSizeText = userInterface.texts[i];
                swarm.swarmSizeText.gameObject.transform.parent.gameObject.SetActive(true);
            }

            if (i < targetColors.Count)
            {
                swarm.SetTargetColor(targetColors[i]);
            }

            swarms.Add(swarm);
        }
    }

    void GenerateInitialCreatures()
    {
        foreach (Swarm swarm in swarms)
        {
            swarm.GenerateInitialCreatures();
        }
    }

    private void ResetLevel()
    {
        /* foreach (Swarm swarm in swarms)
         {
             swarm.DestroyAllCreatures();
             swarm.SetTargetPosition();
             swarm.GenerateInitialCreatures();
         }

         foreach (Goal goal in goals)
         {
             goal.ResetGoal();
         }

         gameOver = false;
         score = 0;

         PlayerController.ResetCamera();*/

        SceneManager.LoadSceneAsync(PlayerStats.CurrentLevel);
    }

    void WinGame()
    {
        Debug.Log("Win!");
        gameOver = true;

        PlayerStats.CurrentLevel++;

        if (PlayerStats.CurrentLevel > 5)
        {
            PlayerStats.CurrentLevel = 0;
            PlayerStats.IsWinner = true;
        }

        SceneManager.LoadSceneAsync(PlayerStats.CurrentLevel);
    }

    public void LoseGame()
    {
        Debug.Log("Lose!");
        gameOver = true;

        ResetLevel();
    }

    public void CheckIfLostGame()
    {
        int totalCreatures = 0;
        foreach (Swarm swarm in swarms)
        {
            totalCreatures += swarm.creatures.Count;
        }

        if (totalCreatures + score < goal) {
            LoseGame();
        }
    }
}

public static class PlayerStats
{
    public static int CurrentLevel { get; set; }
    public static bool IsWinner { get; set; }
}
