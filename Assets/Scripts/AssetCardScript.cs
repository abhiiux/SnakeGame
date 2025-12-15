using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AssetCardScript : MonoBehaviour
{
    private TextMeshProUGUI itemName;
    private Image itemImage;
    private TextMeshProUGUI itemPrice;
    private Button itemButton;

    private void Awake()
    {
        if(itemName == null || itemImage == null || itemPrice == null)
        {
            itemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            itemImage = transform.Find("ItemImage").GetComponent<Image>();
            itemPrice = transform.Find("Buy_Button/Coins").GetComponent<TextMeshProUGUI>();
            itemButton = transform.Find("Buy_Button").GetComponent<Button>();
        }
    }

    public void CardValues(string _name,string _price, Sprite _sprite)
    {
        itemName.text = _name;
        itemImage.sprite = _sprite;
        itemPrice.text = _price;

        itemButton.onClick.AddListener(() =>
        {
            Logger($" hey button was pressed on card {itemName}");
        });
    }

    private void Logger(string message)
    {
#if UNITY_EDITOR
        Debug.Log($"{message}");
#endif
    }
}
