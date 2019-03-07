//Bryan Leister, Jan. 2019
//
//This script was created during Global Game Jam 2019! The script creates buttons based on the contents of a folder located
//in a Resources folder. The script creates a thumbnail using Yasirkula's excellent Preview generator:
//https://github.com/yasirkula/UnityRuntimePreviewGenerator
//
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ButtonMaker : MonoBehaviour
{
    public GameObject m_buttonPrefab;
    public Transform m_contentContainer;
    public int m_iconSize = 64;
    public string m_path = "Objects";
    public DropToTiles m_dropScript;

    [HideInInspector]
    public List<Item> buttons;

    private void Start()
    {
        buttons = new List<Item>();
        GetButtonAtPath(m_path);
        MakeButtons();
    }
    void GetButtonAtPath(string s)
    {
        GameObject[] prefabs;
        prefabs = Resources.LoadAll<GameObject>(s + "/");
        foreach (GameObject g in prefabs)
        {
            //Make a template object
            GameObject go = Instantiate(g, Vector3.zero, Quaternion.identity);

            //Create the item properties if it does not exist on the prefab
            Item i = go.GetComponent<Item>();
            if (i == null)
                i = go.AddComponent(typeof(Item)) as Item;


            //If the prefab had an item, check for a thumbnail to use
            if (i.thumbnail == null)
            {
                //Build the thumbnail for the button
                Texture2D t = RuntimePreviewGenerator.GenerateModelPreview(go.transform, m_iconSize, m_iconSize);
                i.thumbnail = t;
            }
            //Rename the cloned prefab
            string str = g.name.Replace("(Clone)", "");
            i.itemname = str;
            i.prefab = go;
            go.SetActive(false);
            buttons.Add(i);


        }

    }

    void MakeButtons()
    {
        foreach (Item b in buttons)
        {
            Transform go = Instantiate(m_buttonPrefab.transform, m_contentContainer);
            Button but = go.GetComponent<Button>() as Button;
            RawImage ri = go.GetComponentInChildren<RawImage>();

            if (ri != null)
                ri.texture = b.thumbnail;

            Text[] tm = go.GetComponentsInChildren<Text>();

            foreach (Text tmp in tm)
            {
                tmp.text = b.itemname;
            }

            but.onClick.AddListener(() => { m_dropScript.PlaceObject(b.prefab); });

        }
    }

}
