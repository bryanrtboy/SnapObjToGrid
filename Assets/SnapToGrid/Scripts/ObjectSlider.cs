using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSlider : MonoBehaviour
{
    public float m_fixedYDistance = 2.0f;
    public Vector3 m_minXYZ;
    public string m_otherTag = "Furniture";
    public string m_tilesTag = "Wall";
    public LayerMask m_tileLayer;
    public float m_ZfudgeFactor = 0f;
    public bool m_snapToCenter = false;

    Plane m_movePlane;
    float m_hitDist, m_t;
    Ray m_camRay;
    Vector3 m_startPos, m_point;
    public bool m_isDragging = false;
    Collider[] m_colliders;


    void OnMouseDown()
    {
        if (ObjectControls.instance.m_selectedObject != this.gameObject)
            ObjectControls.instance.SetSelectedObject(this.gameObject);

        m_isDragging = true; //We are in the dragging state

        if (m_snapToCenter && m_tileLayer.value == 0)
            Debug.LogWarning("Tile layer is not set!");

        SetStartPosition(transform.position);

        if (!m_snapToCenter)
            m_movePlane = new Plane(-Camera.main.transform.forward, transform.position); // find a parallel plane to the camera based on obj start pos;

    }

    void OnMouseUp()
    {
        m_isDragging = false;
    }

    void OnMouseDrag()
    {
        if (m_snapToCenter)
            UseRaycastToCenter();
        else
            UseToMovePlane();

        if (ObjectControls.instance != null)
        {
            //ObjectControls.instance.m_HUDCanvas.transform.position = transform.position;
            ObjectControls.instance.ResetLastActiveTime();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!m_snapToCenter && this.gameObject == ObjectControls.instance.m_selectedObject && !m_isDragging)
        {
            if (other.transform.tag == m_otherTag)
                transform.position = m_startPos;
        }
    }

    public void SetStartPosition(Vector3 position)
    {
        m_startPos = position; // save position in case draged to invalid place
    }

    void UseToMovePlane()
    {

        m_camRay = Camera.main.ScreenPointToRay(Input.mousePosition); // shoot a ray at the obj from mouse screen point

        if (m_movePlane.Raycast(m_camRay, out m_hitDist))
        { // finde the collision on movePlane
            m_point = m_camRay.GetPoint(m_hitDist); // define the point on movePlane
            m_t = -(m_fixedYDistance - m_camRay.origin.y) / (m_camRay.origin.y - m_point.y); // the x,y or z plane you want to be fixed to

            float x = m_camRay.origin.x + (m_point.x - m_camRay.origin.x) * m_t; // calculate the new point t futher along the ray
            float y = m_camRay.origin.y + (m_point.y - m_camRay.origin.y) * m_t;
            float z = m_camRay.origin.z + (m_point.z - m_camRay.origin.z) * m_t;

            if (x > m_minXYZ.x)
                x = m_minXYZ.x;
            else if (x < -m_minXYZ.x)
                x = -m_minXYZ.x;

            if (z > m_minXYZ.z)
                z = m_minXYZ.z;
            else if (z < -(m_minXYZ.z - m_ZfudgeFactor))
                z = -(m_minXYZ.z - m_ZfudgeFactor);

            transform.position = new Vector3(x, y, z);
        }

    }

    void UseRaycastToCenter()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, m_tileLayer))
        {
            TileStatus t = hit.transform.GetComponent<TileStatus>();
            if (!t.m_isOccupied)
                transform.position = hit.transform.position;
        }
    }

    public void ToggleCollider(bool isActive)
    {
        if (m_colliders == null)
            m_colliders = this.GetComponentsInChildren<Collider>();

        foreach (Collider c in m_colliders)
            c.enabled = isActive;

    }
}
