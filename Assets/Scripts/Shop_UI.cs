using UnityEngine;
using UnityEngine.UI;

public class Shop_UI : MonoBehaviour
{
    [SerializeField]
    private AssetData shopData;
    [SerializeField]
    private GameObject prefabTemplate;

    void Awake()
    {
        CoinClass.Load();
        CreateShopCard(shopData.itemData.Count);
    }

    private void CreateShopCard(int cardCount)
    {
        int value = cardCount;
        do
        {
            CreateCard(value);
            value--;
        } 
        while(value < 0);
    }
    private void CreateCard(int cardNumber)
    {
        Transform obj = Instantiate(prefabTemplate,this.transform).transform;

        if(obj.TryGetComponent<AssetCardScript>(out AssetCardScript cardData))
        {
            string _name = shopData.itemData[cardNumber]._name;
            int _price = shopData.itemData[cardNumber]._price;
            Sprite _image = shopData.itemData[cardNumber]._image;

            cardData.CardValues(_name,_price.ToString(),_image);
        }
    }


    private void Logger(string message)
    {
#if UNITY_EDITOR
        Debug.Log($"{message}");
#endif
    }
}
