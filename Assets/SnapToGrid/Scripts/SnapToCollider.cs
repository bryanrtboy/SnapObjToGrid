using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToCollider : MonoBehaviour
{
    [Tooltip("When placing, how much should the pivot be offset from the center of this object?")]
    public Vector3 m_offset = Vector3.zero;
    Color m_paintColor = Color.white;
    Texture m_texture;

    bool m_isAttached = false;
    MeshRenderer m_mr;

    void Start()
    {
        m_mr = this.GetComponent<MeshRenderer>();
        m_paintColor = m_mr.material.color;
    }
    void OnMouseEnter()
    {
        m_mr.material.color = Color.red;
        if (AddObjectToScene.instance.m_newObject != null)
        {
            AddObjectToScene.instance.m_newObject.transform.position = transform.position + m_offset;
        }
    }

    void OnMouseExit()
    {
        m_mr.material.color = m_paintColor;
    }

    void OnMouseDown()
    {
        //If no object is currently in play, do nothing. 
        if (AddObjectToScene.instance.m_newObject == null)
            return;

        AddObjectToScene.instance.InstantiateNewObject();

        //De-activate this slot since it already has something on it.
        this.GetComponent<Collider>().enabled = false;
    }
}
