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

public class ButtonMaker : MonoBehaviour
{
    public GameObject m_buttonPrefab;
    public Transform m_contentContainer;
    public List<ItemButton> buttons;
    public string m_path = "FloorObjects";

    private void Start()
    {
        buttons = new List<ItemButton>();
        GetButtonAtPath(m_path, 1, 1, false);

        MakeButtons();
    }
    void GetButtonAtPath(string s, int w, int h, bool isTopper)
    {
        GameObject[] prefabs;
        prefabs = Resources.LoadAll<GameObject>(s + "/");
        foreach (GameObject g in prefabs)
        {
            GameObject go = Instantiate(g, Vector3.zero, Quaternion.identity);


            ItemButton i = go.AddComponent(typeof(ItemButton)) as ItemButton;
            Texture t = RuntimePreviewGenerator.GenerateModelPreview(go.transform, 60, 60);
            if (t != null)
                i.thumbnail = t;
            i.cellHeight = 1;
            i.cellWidth = 1;
            string str = g.name.Replace("(Clone)", "");
            i.itemname = str;
            i.price = 15.99f;
            i.priceString = i.price.ToString("#.##");
            i.value = 15;
            i.isTabletop = isTopper;
            i.prefab = go;
            go.SetActive(false);
            buttons.Add(i);
        }

    }

    void MakeButtons()
    {
        foreach (ItemButton b in buttons)
        {
            GameObject go = Instantiate(m_buttonPrefab, Vector3.zero, Quaternion.identity, m_contentContainer);
            Button but = go.GetComponent<Button>() as Button;
            RawImage ri = go.GetComponentInChildren<RawImage>();
            if (ri != null)
                ri.texture = b.thumbnail;
            Text[] tm = go.GetComponentsInChildren<Text>();

            foreach (Text tmp in tm)
            {
                tmp.text = b.itemname;
            }

            but.onClick.AddListener(() => { AddObjectToScene.instance.AddObject(b.prefab); });
        }
    }

}
