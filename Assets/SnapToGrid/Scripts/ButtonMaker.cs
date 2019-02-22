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
    public List<Item> buttons;
    public string m_path = "FloorObjects";
    public bool m_makeSnapButtons = true;

    private void Start()
    {
        buttons = new List<Item>();
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


            Item i = go.GetComponent<Item>();
            if (i == null)
                i = go.AddComponent(typeof(Item)) as Item;

            Texture t = RuntimePreviewGenerator.GenerateModelPreview(go.transform, 256, 256);

            //Set the image to use for a thumbnail on the button if it does not already exist
            if (i.thumbnail == null && t != null)
                i.thumbnail = t;
            string str = g.name.Replace("(Clone)", "");
            i.itemname = str;
            i.prefab = go;

            Bounds bounds = new Bounds(go.transform.position, Vector3.one);
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                //Debug.Log("growing...");
                bounds.Encapsulate(renderer.bounds);
            }
            //Debug.Log("Size is " + bounds.size);
            i.m_size = bounds.size;

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

            if (m_makeSnapButtons)
            {

                but.onClick.AddListener(() => { AddObjectToScene.instance.PlaceObject(b.prefab); });
            }
            else
            {
                but.onClick.AddListener(() => { DropSlideObjectsIntoScene.instance.PlaceObject(b.prefab); });
            }
        }
    }

}
