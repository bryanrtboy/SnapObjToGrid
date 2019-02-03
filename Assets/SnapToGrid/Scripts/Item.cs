using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public string itemname;
    public Texture thumbnail;
    public GameObject prefab;
    public Collider attachedGridCollider;
    public Vector3 m_rotationAxis = Vector3.up;

    void OnMouseDown()
    {
        //Don't select objects while we are placing an object
        if (AddObjectToScene.instance.m_newObject != null)
            return;

        AddObjectToScene.instance.SetSelectedObject(this.gameObject);

    }
}
