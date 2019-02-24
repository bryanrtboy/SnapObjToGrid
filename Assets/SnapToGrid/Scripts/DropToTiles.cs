//Bryan Leister, Feb. 2019
//
//Drop objects into the scene from above. Objects will only be created if a tile is available
//and unoccupied. Tiles are considered occupied if an object is within the tiles bounds
//so, a large object may take up several tiles.
//
//When not snapping to grid objects, the objects need to know the limits for where they can be slid to
//To tell them this, we need to grab the renderer from the Floor used by the GridMaker script
//THe GridMaker script can destroy that object, so we grab it here and store it's size. Later, we pass
//that size to the objects movement script.
//
//Each tile needs to contain a TileStatus script, to let this script know if it's occupied or not.
//
//Instructions:
//1. Create a Canvas and a Panel container for Buttons
//2. Attach ButtonMaker.cs to the panel
//3. Attach this script to the panel
//4. Create a grid container in the Scene, with a GridMaker script attached
//5. Wire up all the dependencies and tag the prefabs as in the example

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropToTiles : MonoBehaviour
{
    [Tooltip("Cube used to make the tiles with the GridMaker script")]
    public Renderer m_tilesBoundingRenderer;
    [Tooltip("Tag that the GridMaker prefab tile is using")]
    public string m_tilesTag = "Floor";
    [Tooltip("Tag that the all objects being placed use, to prevent intersections of objects when not useing snap to center placement")]
    public string m_objectTag = "Furniture";
    [Tooltip("Should the objects snap only to grid squares")]
    public bool m_snapToCenter = false;
    public LayerMask m_tileLayer;
    public Camera m_mainCamera;
    public Vector3 m_spawnPoint = Vector3.up;
    public AudioSource m_placeSound;

    TileStatus[] m_floorTiles;
    bool m_canPlace = false;
    Vector3 m_floorBounds;
    ObjectSlider o;

    void Awake()
    {
        if (m_tilesBoundingRenderer == null)
        {
            Debug.LogError("No Floor!");
            Destroy(this);
        }

        m_floorBounds = new Vector3(m_tilesBoundingRenderer.bounds.max.x, m_tilesBoundingRenderer.bounds.max.y, m_tilesBoundingRenderer.bounds.max.z);

    }

    void Start()
    {
        Invoke("Setup", .1f);
    }

    void Setup()
    {
        m_floorTiles = FindObjectsOfType<TileStatus>(); //Find all elible tiles, must have the TileStatus script attached
        if (m_floorTiles != null)
            m_canPlace = true;
    }

    public void PlaceObject(GameObject g)
    {
        if (!m_canPlace)
            return;

        if (m_placeSound != null)
            m_placeSound.Play();


        List<TileStatus> availableTiles = new List<TileStatus>();

        //Check for unoccupied tiles tagged with our tile tag
        foreach (TileStatus t in m_floorTiles)
        {
            if (t.m_isOccupied == false && t.tag == m_tilesTag)
                availableTiles.Add(t);
        }

        if (availableTiles.Count >= 1)
        {
            TileStatus t = availableTiles[Random.Range(0, availableTiles.Count - 1)];
            m_canPlace = false;

            GameObject go = Instantiate(g, m_spawnPoint, t.transform.rotation, t.transform.parent);
            o = go.AddComponent<ObjectSlider>();
            o.m_otherTag = m_objectTag;
            o.tag = m_objectTag;
            o.m_fixedYDistance = t.transform.position.y;
            o.m_minXYZ = m_floorBounds;
            o.m_ZfudgeFactor = 2f;
            o.m_snapToCenter = m_snapToCenter;
            o.m_tilesTag = m_tilesTag;
            o.m_tileLayer = m_tileLayer;
            o.ToggleCollider(false);
            go.SetActive(true);
            StartCoroutine(FallToTile(t, go));
        }
        else
        {
            Debug.Log("No open spaces for you!");
        }

    }

    IEnumerator FallToTile(TileStatus tile, GameObject go)
    {
        while (Mathf.Abs(go.transform.position.y - tile.transform.position.y) > .01f)
        {
            go.transform.position = Vector3.Lerp(go.transform.position, tile.transform.position, Time.deltaTime * 10f);
            yield return null;
        }
        m_canPlace = true;
        //Debug.Log("Now you can place another." + Time.time);
        ObjectControls.instance.m_selectedObject = go;

        if (o != null)
        {
            o.SetStartPosition(tile.transform.position);
            o.ToggleCollider(true);
            o = null;
        }

        yield return null;
    }

}
