using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectControls : MonoBehaviour
{
    public static ObjectControls instance;

    public AudioSource m_selectSound;
    public CanvasGroup m_HUDCanvas;
    public Slider m_RotationSlider;
    public float m_fadeHUDDelay = 2f;


    public GameObject m_selectedObject;
    bool m_isFading = false;
    [HideInInspector]
    public float m_lastActiveTime = 0;

    private void Awake()
    {
        //Enforce the Singleton pattern, there can be only one AddObjectToScene...
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (m_RotationSlider != null)
            m_RotationSlider.gameObject.SetActive(false);
        if (m_HUDCanvas != null)
            m_HUDCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (m_HUDCanvas == null)
            return;

        if (Time.time - m_lastActiveTime >= m_fadeHUDDelay && !m_isFading && m_HUDCanvas.gameObject.activeSelf)
        {
            //Debug.Log("Fading at " + (Time.time - m_lastActiveTime).ToString("##.#####"));
            m_isFading = true;
        }

        if (m_isFading)
        {
            m_HUDCanvas.alpha = Mathf.Lerp(m_HUDCanvas.alpha, 0f, Time.deltaTime);
            if (m_HUDCanvas.alpha == 0.0f)
            {
                m_HUDCanvas.gameObject.SetActive(false);
                m_isFading = false;
            }
        }

    }

    public void SetSelectedObject(GameObject g)
    {
        m_selectedObject = g;
        m_lastActiveTime = Time.time;
        m_RotationSlider.value = g.transform.localEulerAngles.y;
    }

    public void DeselectObject()
    {
        m_selectedObject = null;
    }

    public void DestroySelected()
    {
        if (m_selectedObject != null)
        {
            GameObject temp = m_selectedObject;
            temp.transform.Translate(Vector3.forward * 10000f);
            m_selectedObject = null;
            StartCoroutine(DestroyAfterMovingAway(temp));
        }
    }

    IEnumerator DestroyAfterMovingAway(GameObject g)
    {
        yield return new WaitForSeconds(1);
        Destroy(g);
    }

    public void ShowHUDCanvas(bool isVisible)
    {
        if (isVisible)
        {
            ResetLastActiveTime();
            m_HUDCanvas.gameObject.SetActive(isVisible);
        }
        else
        {
            m_isFading = true;
        }
    }

    public void ResetLastActiveTime()
    {
        m_lastActiveTime = Time.time;
        m_isFading = false;
        m_HUDCanvas.alpha = 1;
    }

    //Due to not understanding Quaternions, this only works because the selectedObject is parented to the grid parent
    //The grid parent is rotated, so the objects up Y axis is oriented up at all times, the grid is what's rotated
    //I could get Y and X rotation to work without doing this, but trying to rotate on the Z axis never worked as desired
    public void RotateSelected(float angle)
    {
        if (m_selectedObject != null)
            m_selectedObject.transform.localEulerAngles = new Vector3(0f, angle, 0f);

    }

}
