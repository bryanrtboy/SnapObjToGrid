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
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        if (m_confirmCanvas != null)
            m_confirmCanvas.SetActive(false);

    }
    void Update()
    {
        if (m_isAttached)
        {
            Vector3 pos = m_mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -m_mainCamera.transform.position.z));
            m_newObject.transform.position = pos;

            bool canRelease = false;
            RaycastHit hit;
            Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("Ray hit " + hit.transform.name + " at " + Time.time);
                if (hit.collider.tag == "Sketch")
                {
                    canRelease = true;
                }
            }

            if (Input.GetMouseButtonDown(0) && canRelease)
            {
                pos = m_mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -m_mainCamera.transform.position.z));
                m_newObject.transform.position = pos;
                m_isAttached = false;
                m_newObject = null;
            }
        }

    }

    public void AddObject(GameObject g)
    {
        if (m_isAttached)
            return;

        if (m_selectSound != null)
            m_selectSound.Play();

        if (m_mainCamera)
        {
            Vector3 pos = m_mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_mainCamera.transform.position.z));
            //m_newObject = Instantiate(g, pos, Quaternion.identity);
            m_newObject = g;
            g.transform.position = pos;
            g.SetActive(true);
            g.transform.Rotate(new Vector3(0, 0, 0));
            //Turn off the colliders so that we can raycast to where we want to place them
            UpdateColliders(g, false);
            m_isAttached = true;
            m_lastObject = m_newObject;
        }
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
