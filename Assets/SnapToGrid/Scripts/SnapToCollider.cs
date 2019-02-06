using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SnapToCollider : MonoBehaviour
{
    public enum Orientation { Xnegative, Xpositive, Ynegative, Ypositive, Znegative, Zpositive };
    [Tooltip("The surface plane that we will snap the object to")]
    public Orientation m_orientation = Orientation.Ypositive;
    public Color m_isTriggeredColor = Color.yellow;

    Color m_originalSurfaceColor = Color.white;
    Texture m_texture;
    Vector3 m_orientationOffset = Vector3.zero;
    Vector3 m_rotationAxis = Vector3.up;
    bool m_isTriggered = false;

    MeshRenderer m_mr;
    Collider m_collider;

    void Awake()
    {
        m_mr = this.GetComponent<MeshRenderer>();
        m_collider = this.GetComponent<Collider>();
        m_originalSurfaceColor = m_mr.material.color;

        //m_orientationOffset = When placing an object, which face of the grid cube do we want the object
        //pivot point to snap to?
        //m_rotationAxis = When rotating the object, what axis does it rotate around?
        Vector3 o = m_mr.bounds.extents;
        switch (m_orientation)
        {
            case Orientation.Xnegative:
                m_orientationOffset = new Vector3(-o.x, 0f, 0f);
                m_rotationAxis = Vector3.right;
                break;
            case Orientation.Xpositive:
                m_orientationOffset = new Vector3(o.x, 0f, 0f);
                m_rotationAxis = Vector3.right;
                break;
            case Orientation.Ynegative:
                m_orientationOffset = new Vector3(0f, -o.y, 0f);
                m_rotationAxis = Vector3.up;
                break;
            case Orientation.Ypositive:
                m_orientationOffset = new Vector3(0f, o.y, 0f);
                m_rotationAxis = Vector3.up;
                break;
            case Orientation.Znegative:
                m_orientationOffset = new Vector3(0f, 0f, -o.z);
                m_rotationAxis = Vector3.forward;
                break;
            case Orientation.Zpositive:
                m_orientationOffset = new Vector3(0f, 0f, o.z);
                m_rotationAxis = Vector3.forward;
                break;
        }

    }

    void OnMouseEnter()
    {
        if (AddObjectToScene.instance.m_livePrefab != null && !AddObjectToScene.instance.m_waitingToConfirm)
        {
            AddObjectToScene.instance.m_isAttachedToMouse = false;
            AddObjectToScene.instance.m_livePrefab.transform.position = transform.position + m_orientationOffset;
            AddObjectToScene.instance.m_livePrefab.transform.rotation = this.transform.rotation;
        }
    }

    void OnMouseExit()
    {
        if (!m_isTriggered)
            m_mr.material.color = m_originalSurfaceColor;
    }

    void OnMouseDown()
    {
        //If we are waiting for a confirmation, or do not have an object, do nothing.
        if (AddObjectToScene.instance.m_waitingToConfirm || AddObjectToScene.instance.m_livePrefab == null)
            return;

        AddObjectToScene.instance.SetObjectAndActivateUI(m_collider, m_rotationAxis);

    }

    void OnTriggerEnter(Collider other)
    {
        if (!m_isTriggered)
        {
            m_mr.material.color = m_isTriggeredColor;
            m_isTriggered = true;
            //Debug.Log("Sent msg to " + other.name + " at " + Time.time);
            other.SendMessage("AddThisCollider", m_collider);
        }
        //Debug.Log("entered " + other.name + " at " + Time.time);
    }

    void OnTriggerExit(Collider other)
    {
        m_mr.material.color = m_originalSurfaceColor;
        other.SendMessage("RemoveThisCollider", m_collider);
        m_isTriggered = false;
        //Debug.Log("exited " + other.name + Time.time);
    }

    void ResetColor()
    {
        m_mr.material.color = m_originalSurfaceColor;
    }
}
