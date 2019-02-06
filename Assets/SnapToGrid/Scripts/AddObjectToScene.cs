using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddObjectToScene : MonoBehaviour
{
    public static AddObjectToScene instance;
    public Camera m_mainCamera;

    public GameObject m_confirmCancelCanvas;
    public GameObject m_rotateTrashCanvas;

    public AudioSource m_selectSound;
    public AudioSource m_placeSound;
    [HideInInspector]
    public bool m_isAttachedToMouse = false;
    [HideInInspector]
    public bool m_waitingToConfirm = false;
    [HideInInspector]
    public GameObject m_livePrefab;
    [HideInInspector]
    public Item m_selectedItem;
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
            m_livePrefab.transform.position = pos;
        }
    }

    public void PlaceObject(GameObject g)
    {
        // //Don't do anything if something is still attached to the mouse
        // if (m_isAttachedToMouse)
        //     return;

        if (m_livePrefab != null)
        {
            ConfirmObjectPlacement();
            return;
        }

        if (m_selectedItem != null)
        {
            m_selectedItem.ActivateAttachedColliders(false);
        }

        //Turn off any UI elements if they are open...
        CleanUp();

        if (m_selectSound != null)
            m_selectSound.Play();

        if (m_mainCamera != null)
        {
            Vector3 pos = m_mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_mainCamera.transform.position.z));
            m_livePrefab = g;
            g.transform.position = pos;
            g.SetActive(true);
            //put this object on the ignore Raycast layer
            g.layer = 2;
            m_isAttachedToMouse = true;
        }
    }

    public void SetObjectAndActivateUI(Collider selectedGridUnit, Vector3 rotationDirection)
    {
        m_selectedGridCollider = selectedGridUnit;
        m_rotationAxis = rotationDirection;

        if (m_placeSound != null)
            m_placeSound.Play();

        m_confirmCancelCanvas.transform.position = m_livePrefab.transform.position;
        m_confirmCancelCanvas.gameObject.SetActive(true);

        m_isAttachedToMouse = false;
        m_waitingToConfirm = true;

    }

    public void ConfirmObjectPlacement()
    {
        if (m_placeSound != null)
            m_placeSound.Play();

        Transform g = Instantiate(m_livePrefab.transform);
        //put this on the Default layer
        g.gameObject.layer = 0;
        m_selectedItem = g.GetComponent<Item>();
        m_selectedItem.m_rotationAxis = m_rotationAxis;
        m_selectedItem.AddThisCollider(m_selectedGridCollider);
        m_selectedItem.ActivateAttachedColliders(false);

        if (m_livePrefab != null)
            m_livePrefab.SetActive(false);
        m_livePrefab = null;
        CleanUp();
        SetSelectedObject(g.gameObject);
    }

    public void ConfirmNewPlacement()
    {
        //put this on the Default layer
        m_selectedItem.gameObject.layer = 0;
        m_selectedItem.ActivateAttachedColliders(false);

        CleanUp();
    }


    public void SetSelectedObject(GameObject g)
    {

        m_selectedItem = g.GetComponent<Item>() as Item;
        m_rotateTrashCanvas.transform.position = g.transform.position;
        m_rotateTrashCanvas.gameObject.SetActive(true);
    }

    public void RotateObject()
    {
        if (m_selectedItem != null)
        {
            m_rotationAxis = m_selectedItem.m_rotationAxis;
            m_selectedItem.transform.Rotate(m_rotationAxis, 45f);
            m_selectedItem.ActivateAttachedColliders(true);
        }
    }

    public void DestroyActiveObject()
    {
        if (m_selectedItem != null)
        {
            m_selectedItem.ActivateAttachedColliders(true);
            Destroy(m_selectedItem.gameObject);
        }
        CleanUp();
    }

    public void Cancel()
    {
        if (m_selectedGridCollider != null)
            m_selectedGridCollider.enabled = true;

        if (m_livePrefab != null)
            m_livePrefab.SetActive(false);
        CleanUp();
    }

    public void CleanUp()
    {

        m_livePrefab = null;
        m_selectedItem = null;
        m_waitingToConfirm = false;
        m_selectedGridCollider = null;

        if (m_rotateTrashCanvas != null)
            m_rotateTrashCanvas.gameObject.SetActive(false);
        if (m_confirmCancelCanvas != null)
            m_confirmCancelCanvas.gameObject.SetActive(false);
    }


}
