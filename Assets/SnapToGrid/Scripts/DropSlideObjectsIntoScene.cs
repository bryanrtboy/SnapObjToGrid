using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropSlideObjectsIntoScene : MonoBehaviour
{
    public static DropSlideObjectsIntoScene instance;

    public Renderer m_floor;
    public Camera m_mainCamera;
    public float m_SpawnAtY = 4f;
    public AudioSource m_selectSound;
    public AudioSource m_placeSound;
    public CanvasGroup m_HUDCanvas;
    public Slider m_RotationSlider;
    public float m_fadeHUDDelay = 2f;

    public GameObject m_selectedObject;
    TileStatus[] m_floorTiles;

    bool m_canPlace = false;
    Vector2 m_floorBounds;

    bool m_isFading = false;
    [HideInInspector]
    public float m_lastActiveTime = 0;

    void Awake()
    {
        //Enforce the Singleton pattern, there can be only one AddObjectToScene...
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (m_floor == null)
        {
            Debug.LogError("No Floor!");
            Destroy(this);
        }

        m_floorBounds = new Vector2(m_floor.bounds.max.x, m_floor.bounds.max.z);
        m_RotationSlider.gameObject.SetActive(false);
        m_HUDCanvas.gameObject.SetActive(false);

    }

    void Start()
    {
        Invoke("Setup", .1f);
    }

    void Update()
    {
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

    void Setup()
    {
        m_floorTiles = FindObjectsOfType<TileStatus>();
        if (m_floorTiles != null)
            m_canPlace = true;
    }


    public void PlaceObject(GameObject g)
    {
        if (!m_canPlace)
            return;

        if (m_selectSound != null)
            m_selectSound.Play();

        Vector3 spawnPos = Vector3.zero;
        List<TileStatus> availableTiles = new List<TileStatus>();

        foreach (TileStatus t in m_floorTiles)
        {
            if (!t.m_isOccupied)
                availableTiles.Add(t);
        }

        if (availableTiles.Count >= 1)
        {
            TileStatus t = availableTiles[Random.Range(0, availableTiles.Count - 1)];
            m_canPlace = false;

            spawnPos = new Vector3(t.transform.position.x, m_SpawnAtY, t.transform.position.z);
            GameObject go = Instantiate(g, spawnPos, Quaternion.identity);
            ObjectSlider o = go.AddComponent<ObjectSlider>();
            o.fixedYDistance = t.transform.position.y;
            o.m_minXZ = m_floorBounds;
            go.SetActive(true);
            StartCoroutine(FallToTile(t, go));
        }
        else
        {
            Debug.Log("No open spaces for you!");
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
            DestroyAfterMovingAway(temp);
        }
    }

    IEnumerator DestroyAfterMovingAway(GameObject g)
    {
        yield return new WaitForEndOfFrame();
        ; Destroy(g);
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

    public void RotateSelected(float eulerAngle)
    {
        if (m_selectedObject != null)
            m_selectedObject.transform.localEulerAngles = new Vector3(0, eulerAngle, 0);
    }

    IEnumerator FallToTile(TileStatus tile, GameObject go)
    {
        yield return new WaitForEndOfFrame();

        while (Mathf.Abs(go.transform.position.y - tile.transform.position.y) > .1f)
        {
            go.transform.position = Vector3.Lerp(go.transform.position, tile.transform.position, Time.deltaTime * 10f);
            yield return null;
        }

        m_canPlace = true;
        m_selectedObject = go;
    }

}
