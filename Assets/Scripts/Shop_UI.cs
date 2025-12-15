using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;

public class Shop_UI : MonoBehaviour
{
    [SerializeField]
    private Transform shopcardTemplate;
    [SerializeField]
    private int numberofCard;

    void Awake()
    {
        CoinClass.Load();
        
        if(shopcardTemplate == null)
        {
            shopcardTemplate = transform.Find("Shop_Card").GetComponent<Transform>();
        }
        CreateShopCard(numberofCard);
    }

    private void CreateShopCard(int cardCount)
    {
        int value = cardCount;
        do
        {
            CreateCard(value);
            value--;
        } 
        while(value >= 1);
    }
    private void CreateCard(int cardNumber)
    {
        Transform obj = Instantiate(shopcardTemplate,this.transform);

        Button btn = obj.Find("Buy_Button").GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            Logger($" hey button was pressed on card {cardNumber}");
        });
    }


    private void Logger(string message)
    {
#if UNITY_EDITOR
        Debug.Log($"{message}");
#endif
    }
}
