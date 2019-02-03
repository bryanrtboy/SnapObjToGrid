using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddObjectToScene : MonoBehaviour
{
    public static AddObjectToScene instance;
    public Camera m_mainCamera;

    public Canvas m_confirmCancelCanvas;
    public Canvas m_rotateTrashCanvas;

    public AudioSource m_selectSound;
    public AudioSource m_placeSound;
    [HideInInspector]
    public bool m_isAttachedToMouse = false;
    [HideInInspector]
    public bool m_waitingToConfirm = false;
    [HideInInspector]
    public GameObject m_newObject;
    [HideInInspector]
    public GameObject m_selectedObject;
    Collider m_selectedGridCollider;
    Vector3 m_rotationAxis = Vector3.up;

    void Awake()
    {
        //Enforce the Singleton pattern, there can be only one AddObjectToScene...
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (m_confirmCancelCanvas != null)
            m_confirmCancelCanvas.gameObject.SetActive(false);

        if (m_rotateTrashCanvas != null)
            m_rotateTrashCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (m_isAttachedToMouse)
        {
            Vector3 pos = m_mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -m_mainCamera.transform.position.z));
            m_newObject.transform.position = pos;
        }
    }

    public void PlaceObject(GameObject g)
    {
        if (m_isAttachedToMouse || m_newObject != null)
            return;

        //Turn off any UI elements if they are open...
        CleanUp();

        if (m_selectSound != null)
            m_selectSound.Play();

        if (m_mainCamera != null)
        {
            Vector3 pos = m_mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_mainCamera.transform.position.z));
            m_newObject = g;
            g.transform.position = pos;
            g.SetActive(true);
            //Turn off the colliders so that we can raycast to where we want to place them
            UpdateColliders(g, false);
            m_isAttachedToMouse = true;
        }
    }

    public void SetObjectAndActivateUI(Collider selectedGridUnit, Vector3 rotationDirection)
    {
        m_selectedGridCollider = selectedGridUnit;
        m_rotationAxis = rotationDirection;

        if (m_placeSound != null)
            m_placeSound.Play();

        m_confirmCancelCanvas.transform.position = m_newObject.transform.position;
        m_confirmCancelCanvas.gameObject.SetActive(true);

        m_isAttachedToMouse = false;
        m_waitingToConfirm = true;

    }

    public void ConfirmObjectPlacement()
    {
        Transform g = Instantiate(m_newObject.transform);
        Item i = g.GetComponent<Item>();
        if (i != null)
        {
            i.attachedGridCollider = m_selectedGridCollider;
            i.m_rotationAxis = m_rotationAxis;
        }

        UpdateColliders(g.gameObject, true);
        CleanUp();
    }

    public void SetSelectedObject(GameObject g)
    {
        m_selectedObject = g;
        m_rotateTrashCanvas.transform.position = g.transform.position;
        m_rotateTrashCanvas.gameObject.SetActive(true);
    }

    public void RotateObject()
    {
        if (m_selectedObject != null)
        {
            Item i = m_selectedObject.GetComponent<Item>();
            if (i != null)
            {
                m_rotationAxis = i.m_rotationAxis;
            }
            m_selectedObject.transform.RotateAround(m_selectedObject.transform.position, m_rotationAxis, 45f);
        }
    }

    public void DestroyActiveObject()
    {
        if (m_selectedObject != null)
        {
            Item i = m_selectedObject.GetComponent<Item>();
            if (i != null && i.attachedGridCollider != null)
                i.attachedGridCollider.enabled = true;

            Destroy(m_selectedObject);
        }
        CleanUp();
    }

    void UpdateColliders(GameObject g, bool isColliderActive)
    {
        Collider[] colliders = g.GetComponentsInChildren<Collider>();
        foreach (Collider mc in colliders)
        {
            mc.enabled = isColliderActive;
        }
    }

    public void Cancel()
    {
        if (m_selectedGridCollider != null)
            m_selectedGridCollider.enabled = true;

        CleanUp();
    }

    public void CleanUp()
    {
        if (m_newObject != null)
            m_newObject.SetActive(false);

        m_newObject = null;
        m_selectedObject = null;
        m_waitingToConfirm = false;
        m_selectedGridCollider = null;

        if (m_rotateTrashCanvas != null)
            m_rotateTrashCanvas.gameObject.SetActive(false);
        if (m_confirmCancelCanvas != null)
            m_confirmCancelCanvas.gameObject.SetActive(false);
    }


}
