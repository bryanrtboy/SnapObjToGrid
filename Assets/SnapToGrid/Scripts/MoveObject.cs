using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    AddObjectToScene m;
    // Start is called before the first frame update
    void Start()
    {
        m = AddObjectToScene.instance;
    }

    public void RotateObject()
    {
        if (m.m_selectedItem != null)
        {
            m.m_selectedItem.transform.Rotate(m.m_selectedItem.m_rotationAxis, 45f);
            m.m_selectedItem.ActivateAttachedColliders(true);
        }
    }

    //The speed could be the same as the increment from the GridMaker script. That would move one block at a time.
    //The movement does not take into account a rotated grid
    public void MoveHorizontal(float speed)
    {
        if (m.m_selectedItem != null)
        {
            m.m_selectedItem.transform.position += Vector3.right * speed;
            //m.m_selectedItem.transform.Translate(Vector3.right * speed);
        }
    }

    public void MoveVertical(float speed)
    {
        if (m.m_selectedItem != null)
        {
            if (m.m_selectedItem.m_rotationAxis.y > 0)
                TranslateForward(speed);
            else
                TranslateUp(speed);
        }
    }


    void TranslateForward(float speed)
    {
        m.m_selectedItem.transform.position += Vector3.forward * speed;
        //m.m_selectedItem.transform.Translate(Vector3.forward * speed);
    }

    void TranslateUp(float speed)
    {
        m.m_selectedItem.transform.position += Vector3.up * speed;
        //m.m_selectedItem.transform.Translate(Vector3.up * speed);
    }


}
