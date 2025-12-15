using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text scoreText;

    private void OnEnable()
    {
        GameHandler.OnScoreAdded += AddScore;
    }
    private void OnDisable()
    {
        GameHandler.OnScoreAdded -= AddScore;
    }
    private void Awake()
    {
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
    }

    public void AddScore(int value)
    {
        scoreText.text = value.ToString();
        CoinClass.AddCoins(value);
    }
}
