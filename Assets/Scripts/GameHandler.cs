using CodeMonkey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour 
{
    private static GameHandler instance;
    private static int score;
    public float width, height;
    public static event Action<int> OnScoreAdded;
    [SerializeField] private Snake snake;
    [SerializeField] private Button myButton;
    [SerializeField] private float cellSize = 0.5f; 
    private LevelGrid levelGrade;
    
    private void Awake()
    {
        instance = this;
        InitializeStatic();
    }
    
    private void Start() 
    {
        Logger("GameHandler.Start");
        myButton.onClick.AddListener(OnButtonClick);
        // width = GameAssets.instance.GetScreenWidth();
        // height = GameAssets.instance.GetScreenHeight();
        // Pass cellSize to LevelGrid
        // levelGrade = new LevelGrid(width,height,cellSize);
        levelGrade = new LevelGrid(Camera.main,cellSize);
        snake.Setup(levelGrade);
        levelGrade.Setup(snake);
    }
    private void OnDestroy()
    {
        CoinClass.Save();
    }
    private void OnButtonClick()
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
    
    public static int GetScore()
    {
        return score;
    }

    public static void AddScore()
    {
        score += 10;
        OnScoreAdded?.Invoke(score);
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
    private void Logger(string message)
    {
#if UNITY_EDITOR
        Debug.Log($"{message}");
#endif
    }
}
