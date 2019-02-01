using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public int cellWidth = 1;
    public int cellHeight = 1;
    public float price;
    public string priceString;
    public string itemname;
    public int value;
    public Texture thumbnail;
    public bool isTabletop = false;
    public GameObject prefab;
}
