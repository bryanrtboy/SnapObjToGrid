using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public string itemname;
    public Texture thumbnail;
    public GameObject prefab;
    public Vector3 m_rotationAxis = Vector3.up;

    public List<Collider> m_attachedColliders;

    void OnDisable()
    {
        ActivateAttachedColliders(true);
        ClearColliderList();
    }

    void OnMouseDown()
    {
        //Don't select objects while we are placing an object
        if (AddObjectToScene.instance.m_livePrefab != null)
            return;

        AddObjectToScene.instance.SetSelectedObject(this.gameObject);
        //make all of these colliders active again, clearing the list
        ActivateAttachedColliders(true);

    }

    public void ActivateAttachedColliders(bool isActive)
    {
        if (m_attachedColliders == null)
            return;

        foreach (Collider c in m_attachedColliders)
        {
            //check this because when the game ends in case we have lost track of a collider
            if (c != null)
            {
                c.enabled = isActive;
                c.SendMessage("ResetColor");
            }
        }
    }

    public void ClearColliderList()
    {
        if (m_attachedColliders != null)
            m_attachedColliders.Clear();
    }

    public void AddThisCollider(Collider c)
    {
        if (m_attachedColliders == null)
            m_attachedColliders = new List<Collider>();

        if (!m_attachedColliders.Contains(c))
            m_attachedColliders.Add(c);
    }

    public void RemoveThisCollider(Collider c)
    {
        if (m_attachedColliders == null)
            return;

        if (m_attachedColliders.Contains(c))
            m_attachedColliders.Remove(c);
    }
}
