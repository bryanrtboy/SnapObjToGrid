﻿//Bryan Leister, Jan. 2019
//
//Generate a grid of objects based on an object in the scene
//Requires that a 3D object 'm_bounds' (something like a Quad) exist in the scene and is placed
//where you want the grid of objects to appear. 
//
//The bounding object works best as a 3D cube
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    [Tooltip("How many objects in X,Y,Z. Example: a 2D horizontal grid that is 5X5 would use 5,1,5 with a Quad rotated 90 on the X axis. A vertical grid would be 5,5,1 and have a Quad with no rotation.")]
    public Vector3Int m_matrix;
    public GameObject m_prefab;
    public Renderer m_bounds;
    public bool m_scalePrefabToFitBounds = false;
    public bool m_destroyBounds = true;

    // Start is called before the first frame update
    void Start()
    {

        if (m_prefab == null || m_bounds == null)
        {
            Debug.LogError("No prefab to make the grid OR no ground to put it on!");
            return;
        }
        //Get the size of the bounding box
        Vector3 max = m_bounds.bounds.size;
        Vector3 pos = new Vector3(-max.x * .5f, -max.y * .5f, -max.z * .5f);

        //Figure out the spacing based on the bounding area
        float x_increment = max.x / m_matrix.x;
        float y_increment = max.y / m_matrix.y;
        float z_increment = max.z / m_matrix.z;

        GameObject gridContainer = new GameObject(this.name + "_container");
        //create a temporary object based on the prefab, we'll destroy this later
        GameObject tempObj = Instantiate(m_prefab, pos, Quaternion.identity);

        if (m_scalePrefabToFitBounds)
        {
            tempObj.transform.localScale = new Vector3(m_prefab.transform.localScale.x * x_increment, m_prefab.transform.localScale.y * y_increment, m_prefab.transform.localScale.z * z_increment);
        }

        //offset based on the pefab's size
        Renderer r = tempObj.GetComponent<Renderer>() as Renderer;
        Vector3 offset = Vector3.zero;
        if (r != null)
        {
            offset = r.bounds.size - (tempObj.transform.localScale * 0.5f);

            if (m_scalePrefabToFitBounds)
                pos = pos + offset;
            else
                pos = pos + r.bounds.size;

        }


        //Make the grid
        for (int z = 0; z < m_matrix.z; z++)
        {
            for (int x = 0; x < m_matrix.x; x++)
            {
                for (int y = 0; y < m_matrix.y; y++)
                {
                    Vector3 p = new Vector3(x_increment * x, y_increment * y, z_increment * z) + pos;
                    Instantiate(tempObj, p, Quaternion.identity, gridContainer.transform);
                }
            }
        }



        //move the grid container to where the bounds object is located
        gridContainer.transform.position = this.transform.position;


        if (m_destroyBounds)
            Destroy(m_bounds.gameObject);
        Destroy(tempObj);

    }
}
