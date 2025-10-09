using CodeMonkey;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {
    private static GameHandler instance;
    private static int score;
    [SerializeField] private Snake snake;
    private LevelGrid levelGrade;
    private void Awake()
    {
        instance = this;
        InitializeStatic();
    }
    private void Start() {
        Debug.Log("GameHandler.Start");
        levelGrade = new LevelGrid(Camera.main);
        snake.Setup(levelGrade);
        levelGrade.Setup(snake);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused())
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public static int GetScore()
    {
        return score;
    }

    public static void AddScore()
    {
        score += 100;
    }

    private static void InitializeStatic()
    {
        score = 0;
    }
    public static void SnakeDied()
    {
        GameOverWindow.ShowStatic();
    }

    public static void ResumeGame()
    {
        PauseWindow.HideStatic();
        Time.timeScale = 1f;
    }
    public static void PauseGame()
    {
        PauseWindow.ShowStatic();
        Time.timeScale = 0f;
    }
    public static bool isGamePaused()
    {
        return Time.timeScale == 0f;
    }
}
