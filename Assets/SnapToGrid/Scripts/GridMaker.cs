//Bryan Leister, Jan. 2019
//
//Generate a grid of objects based on an object in the scene
//to fit the bounding area of a 3D cube gameobject
//
//Instructions: 
//The bounding object works best as a 3D cube
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    [Tooltip("How many objects in X,Y,Z. Example: a 2D horizontal grid that is 5X5 would use 5,1,5 with a Quad rotated 90 on the X axis. A vertical grid would be 5,5,1 and have a Quad with no rotation.")]
    public Vector3Int m_matrix = new Vector3Int(5, 1, 5);
    public GameObject m_prefab;
    public Renderer m_bounds;
    public bool m_scalePrefabToFitBounds = false;
    public float m_spacing = 0f;
    public bool m_destroyBounds = true;

    // Start is called before the first frame update
    void Start()
    {

        if (m_prefab == null || m_bounds == null)
        {
            Debug.LogError("No prefab to make the grid OR no ground to put it on!");
            return;
        }

        //Renderer cannot be rotated or the bounding size is off
        Transform boundingParent = m_bounds.transform.parent;
        Quaternion boundsRotation = m_bounds.transform.rotation;

        //Make the bounding box a root object and then rotate to zero
        m_bounds.transform.parent = null;
        m_bounds.transform.localEulerAngles = Vector3.zero;
        //Get the size of the bounding box
        Vector3 max = m_bounds.bounds.size;
        //get the center position
        Vector3 centerposition = new Vector3(-max.x * .5f, -max.y * .5f, -max.z * .5f);

        //Figure out the spacing based on the bounding area
        float x_increment = max.x / m_matrix.x;
        float y_increment = max.y / m_matrix.y;
        float z_increment = max.z / m_matrix.z;

        GameObject gridContainer = new GameObject(this.name + "_container");
        //create a temporary object based on the prefab, we'll destroy this later
        //GameObject tempObj = Instantiate(m_prefab);
        Vector3 scale = Vector3.one;

        if (m_scalePrefabToFitBounds)
        {
            float x = m_prefab.transform.localScale.x * x_increment;
            if (x > m_spacing)
                x = x - m_spacing;

            float y = m_prefab.transform.localScale.y * y_increment;
            if (y > m_spacing)
                y = y - m_spacing;


            float z = m_prefab.transform.localScale.z * z_increment;
            if (z > m_spacing)
                z = z - m_spacing;

            //tempObj.transform.localScale = new Vector3(x, y, z);
            scale = new Vector3(x, y, z);
        }

        // //offset based on the pefab's size
        // Renderer r = m_prefab.GetComponent<Renderer>() as Renderer;
        // Vector3 offset = Vector3.zero;
        // if (r != null)
        // {
        //     //offset = r.bounds.size - (tempObj.transform.localScale * 0.5f);
        //     offset = r.bounds.size - (scale * 0.5f);

        //     if (m_scalePrefabToFitBounds)
        //         centerposition = centerposition + offset;
        //     else
        //         centerposition = centerposition + r.bounds.size;
        // }


        //Make the grid
        for (int z = 0; z < m_matrix.z; z++)
        {
            for (int x = 0; x < m_matrix.x; x++)
            {
                for (int y = 0; y < m_matrix.y; y++)
                {
                    Vector3 p = new Vector3(x_increment * x, y_increment * y, z_increment * z) + centerposition;
                    GameObject go = Instantiate(m_prefab, p, Quaternion.identity, gridContainer.transform);
                    go.transform.localScale = scale;
                    go.name = m_prefab.name + "_" + x.ToString() + y.ToString() + z.ToString();

                    Renderer r = go.GetComponent<Renderer>() as Renderer;
                    go.transform.localPosition += r.bounds.extents;
                }
            }
        }
        //reset the bounding box rotation to where it was at the start.
        m_bounds.transform.rotation = boundsRotation;

        //move the grid container to where the bounds object is located
        gridContainer.transform.position = m_bounds.transform.position;
        gridContainer.transform.rotation = m_bounds.transform.rotation;


        if (m_destroyBounds)
        {
            Destroy(m_bounds.gameObject);
        }
        else
        {
            m_bounds.transform.parent = boundingParent;
        }
        //Destroy(tempObj);

    }
}
