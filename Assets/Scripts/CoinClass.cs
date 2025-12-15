using UnityEngine;

public static class CoinClass
{
    public static int ShoppableCoins { get; private set; }

    public static void AddCoins(int amount)
    {
        ShoppableCoins += amount;
        Save();
    }

    public static bool SpendCoins(int amount)
    {
        if (ShoppableCoins < amount)
            return false;

        ShoppableCoins -= amount;
        Save();
        return true;
    }

    public static void Load()
    {
        ShoppableCoins = PlayerPrefs.GetInt("shoppableCoins", 0);
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("shoppableCoins", ShoppableCoins);
        PlayerPrefs.Save();
    }
}