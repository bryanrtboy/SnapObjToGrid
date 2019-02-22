using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSlider : MonoBehaviour
{
    public float fixedYDistance = 2.0f;
    public Vector2 m_minXZ;
    public string m_otherTag = "Furniture";
    public float m_ZfudgeFactor = 0f;

    Plane movePlane;
    float hitDist, t;
    Ray camRay;
    Vector3 startPos, point;
    bool canMove = true;


    void OnMouseDown()
    {
        startPos = transform.position; // save position in case draged to invalid place
        movePlane = new Plane(-Camera.main.transform.forward, transform.position); // find a parallel plane to the camera based on obj start pos;
        if (DropSlideObjectsIntoScene.instance != null)
        {
            DropSlideObjectsIntoScene.instance.m_RotationSlider.gameObject.SetActive(false);
            DropSlideObjectsIntoScene.instance.SetSelectedObject(this.gameObject);
            DropSlideObjectsIntoScene.instance.m_HUDCanvas.transform.position = transform.position;
            DropSlideObjectsIntoScene.instance.ShowHUDCanvas(true);
        }
    }

    void OnMouseUp()
    {
        canMove = true;
    }

    void OnMouseDrag()
    {
        if (!canMove)
        {
            transform.position = startPos;
        }
        else
        {
            camRay = Camera.main.ScreenPointToRay(Input.mousePosition); // shoot a ray at the obj from mouse screen point

            if (movePlane.Raycast(camRay, out hitDist))
            { // finde the collision on movePlane
                point = camRay.GetPoint(hitDist); // define the point on movePlane
                t = -(fixedYDistance - camRay.origin.y) / (camRay.origin.y - point.y); // the x,y or z plane you want to be fixed to
                float x = camRay.origin.x + (point.x - camRay.origin.x) * t; // calculate the new point t futher along the ray
                float y = camRay.origin.y + (point.y - camRay.origin.y) * t;
                float z = camRay.origin.z + (point.z - camRay.origin.z) * t;

                if (x > m_minXZ.x)
                    x = m_minXZ.x;
                else if (x < -m_minXZ.x)
                    x = -m_minXZ.x;

                if (z > m_minXZ.y)
                    z = m_minXZ.y;
                else if (z < -(m_minXZ.y - m_ZfudgeFactor))
                    z = -(m_minXZ.y - m_ZfudgeFactor);

                transform.position = new Vector3(x, y, z);
            }

        }

        if (DropSlideObjectsIntoScene.instance != null)
        {
            DropSlideObjectsIntoScene.instance.m_HUDCanvas.transform.position = transform.position;
            //DropSlideObjectsIntoScene.instance.m_HUDCanvas.transform.Translate(Vector3.back * 3f);
            DropSlideObjectsIntoScene.instance.ResetLastActiveTime();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == m_otherTag)
        {
            canMove = false;
        }
    }

}
