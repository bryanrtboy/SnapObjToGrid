using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLGenerateGrid : MonoBehaviour
{
    public int m_rows = 5;
    public int m_columns = 5;
    public GameObject m_prefab;


    // Start is called before the first frame update
    void Start()
    {

        if (m_prefab != null)
        {
            int num = m_rows * m_columns;
            for (int y = 0; y < m_rows; y++)
            {
                for (int x = 0; x < m_columns; x++)
                {
                    Vector3 pos = new Vector3(x, 0, y);
                    Instantiate(m_prefab, pos, Quaternion.identity);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
