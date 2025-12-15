using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AssetData", menuName = "Scriptable Objects/AssetData")]
public class AssetData : ScriptableObject
{
    public List<ItemData> itemData;
}

[System.Serializable]
public struct ItemData
{
    public string _name;
    public Sprite _image;
    public int _price;
}
