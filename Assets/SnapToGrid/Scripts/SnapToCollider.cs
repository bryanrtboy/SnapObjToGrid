using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToCollider : MonoBehaviour
{
    public float m_yOffset = .5f;
    Color m_paintColor = Color.white;
    Texture m_texture;

    bool m_isAttached = false;
    MeshRenderer m_mr;

    void Start()
    {
        if (AddObjectToScene.instance == null)
        {
            Debug.LogError("No cursor tex available!");
            Destroy(this);
        }
        m_mr = this.GetComponent<MeshRenderer>();
        m_paintColor = m_mr.material.color;
    }
    void OnMouseEnter()
    {
        AddObjectToScene.instance.m_isAttached = false;
        m_mr.material.color = Color.red;
        if (AddObjectToScene.instance.m_newObject != null)
        {
            AddObjectToScene.instance.m_newObject.transform.position = new Vector3(transform.position.x, transform.position.y + m_yOffset, transform.position.z);
        }
    }

    void OnMouseExit()
    {
        //AddObjectToScene.instance.m_isAttached = true;
        m_mr.material.color = m_paintColor;
    }

    void OnMouseDown()
    {
        //If no object is currently in play, do nothing. 
        if (AddObjectToScene.instance.m_newObject == null)
            return;


        //Otherwise, as long as we have a confirm GUI, let's make it active
        if (AddObjectToScene.instance.m_confirmButton != null)
        {
            Vector3 pos = new Vector3(transform.position.x + 1, transform.position.y + m_yOffset, transform.position.z);
            AddObjectToScene.instance.m_confirmButton.transform.position = pos;
            AddObjectToScene.instance.m_confirmButton.SetActive(true);
        }

        AddObjectToScene.instance.m_newObject = null;
        this.GetComponent<Collider>().enabled = false;
    }
}
