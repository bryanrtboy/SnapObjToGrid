using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SnapToCollider : MonoBehaviour
{
    public enum Orientation { Xnegative, Xpositive, Ynegative, Ypositive, Znegative, Zpositive };
    [Tooltip("The surface plane that we will snap the object to")]
    public Orientation m_orientation = Orientation.Ypositive;

    Color m_paintColor = Color.white;
    Texture m_texture;
    Vector3 m_orientationOffset = Vector3.zero;

    bool m_isAttached = false;
    MeshRenderer m_mr;
    Collider m_collider;

    void Start()
    {
        m_mr = this.GetComponent<MeshRenderer>();
        m_collider = this.GetComponent<Collider>();
        m_paintColor = m_mr.material.color;

        //set up the surface position where the object pivot point will snap to.
        Vector3 o = m_mr.bounds.extents;
        switch (m_orientation)
        {
            case Orientation.Xnegative:
                m_orientationOffset = new Vector3(-o.x, 0f, 0f);
                break;
            case Orientation.Xpositive:
                m_orientationOffset = new Vector3(o.x, 0f, 0f);
                break;
            case Orientation.Ynegative:
                m_orientationOffset = new Vector3(0f, -o.y, 0f);
                break;
            case Orientation.Ypositive:
                m_orientationOffset = new Vector3(0f, o.y, 0f);
                break;
            case Orientation.Znegative:
                m_orientationOffset = new Vector3(0f, 0f, -o.z);
                break;
            case Orientation.Zpositive:
                m_orientationOffset = new Vector3(0f, 0f, o.z);
                break;
        }

    }
    void OnMouseEnter()
    {

        if (AddObjectToScene.instance.m_newObject != null && !AddObjectToScene.instance.m_waitingToConfirm)
        {
            AddObjectToScene.instance.m_isAttachedToMouse = false;
            AddObjectToScene.instance.m_newObject.transform.position = transform.position + m_orientationOffset;
            AddObjectToScene.instance.m_newObject.transform.rotation = this.transform.rotation;
            m_mr.material.color = Color.red;
        }
    }

    void OnMouseExit()
    {
        m_mr.material.color = m_paintColor;
    }

    void OnMouseDown()
    {
        //If we are waiting for a confirmation, or do not have an object, do nothing.
        if (AddObjectToScene.instance.m_waitingToConfirm || AddObjectToScene.instance.m_newObject == null)
            return;

        AddObjectToScene.instance.SetObjectAndActivateUI(m_collider, m_orientationOffset);
        m_mr.material.color = m_paintColor;
        m_collider.enabled = false;
    }
}
