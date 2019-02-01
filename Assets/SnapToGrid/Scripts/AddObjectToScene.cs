using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObjectToScene : MonoBehaviour
{
    public static AddObjectToScene instance;
    public Camera m_mainCamera;
    public GameObject m_confirmCanvas;
    public AudioSource m_selectSound;
    public AudioSource m_placeSound;

    [HideInInspector]
    public bool m_isAttached = false;
    [HideInInspector]
    public GameObject m_newObject;
    GameObject m_lastObject;

    void Awake()
    {
        //Enforce the Singleton pattern, there can be only one AddObjectToScene...
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (m_confirmCanvas != null)
            m_confirmCanvas.SetActive(false);

    }

    public void PlaceObject(GameObject g)
    {
        if (m_isAttached)
            return;

        if (m_selectSound != null)
            m_selectSound.Play();

        if (m_mainCamera)
        {
            Vector3 pos = m_mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_mainCamera.transform.position.z));
            m_newObject = g;
            g.transform.position = pos;
            g.SetActive(true);
            //Turn off the colliders so that we can raycast to where we want to place them
            UpdateColliders(g, false);
            m_isAttached = true;
            m_lastObject = m_newObject;
        }
    }

    public void InstantiateNewObject()
    {
        if (m_placeSound != null)
            m_placeSound.Play();

        //If we have a confirm GUI, let's make it active
        if (m_confirmCanvas != null)
        {
            m_confirmCanvas.transform.position = m_lastObject.transform.position;
            m_confirmCanvas.SetActive(true);
        }

        m_lastObject = Instantiate(m_newObject, m_newObject.transform.position, Quaternion.identity);
        m_isAttached = false;
        m_newObject.SetActive(false);
        m_newObject = null;
    }

    public void CancelObject()
    {
        m_lastObject.SetActive(false);
    }

    public void RotateObject()
    {
        m_lastObject.transform.RotateAround(m_lastObject.transform.position, Vector3.up, 45f);
    }

    void ChangeLayer(GameObject g, int layerID)
    {
        Transform[] children = g.GetComponentsInChildren<Transform>();

        foreach (Transform t in children)
        {
            t.gameObject.layer = layerID;
        }
        g.layer = layerID;

    }

    void UpdateColliders(GameObject g, bool isColliderActive)
    {
        Collider[] colliders = g.GetComponentsInChildren<Collider>();
        foreach (Collider mc in colliders)
        {
            mc.enabled = isColliderActive;
        }
    }
}
