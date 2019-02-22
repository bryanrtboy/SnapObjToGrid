using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TileStatus : MonoBehaviour
{

    public bool m_isOccupied;
    public Color m_isTriggeredColor = Color.yellow;
    [Tooltip("The object that we are checking for should have this tag and a collider")]
    public string m_otherTag = "Furniture";

    Color m_originalSurfaceColor = Color.white;


    MeshRenderer m_mr;
    Collider m_collider;


    // Start is called before the first frame update
    void Awake()
    {
        m_mr = this.GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!m_isOccupied && other.tag == m_otherTag)
        {
            m_isOccupied = true;
            m_mr.material.color = m_isTriggeredColor;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (m_isOccupied && other.tag == m_otherTag)
        {
            m_mr.material.color = m_originalSurfaceColor;
            m_isOccupied = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!m_isOccupied && other.tag == m_otherTag)
        {
            m_isOccupied = true;
            m_mr.material.color = m_isTriggeredColor;
        }
    }

}
