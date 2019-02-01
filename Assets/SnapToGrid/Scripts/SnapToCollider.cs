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
            AddObjectToScene.instance.m_newObject.transform.position = transform.position + m_offset;
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

        if (AddObjectToScene.instance.m_placeSound != null)
            AddObjectToScene.instance.m_placeSound.Play();

        //Otherwise, as long as we have a confirm GUI, let's make it active
        if (AddObjectToScene.instance.m_confirmCanvas != null)
        {
            AddObjectToScene.instance.m_confirmCanvas.transform.position = transform.position;
            AddObjectToScene.instance.m_confirmCanvas.SetActive(true);
        }

        AddObjectToScene.instance.m_newObject = null;
        this.GetComponent<Collider>().enabled = false;
    }
}
