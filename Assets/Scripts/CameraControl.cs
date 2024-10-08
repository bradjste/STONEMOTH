using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float worldMinX = -100f;
    public float worldMaxX = 100f;
    public float worldMinY = -100f;
    public float worldMaxY = 100f;
    public float minY = 20f;
    public float maxY = 80f;

    float panX = 0f;
    float panY = 0f;

    float zoom = 0f;

    public float panSensitivity = 0.1f;
    public float scrollSensitivity = 10f;

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 mouseScroll = Input.mouseScrollDelta;
        if((mousePosition.x > 0.75f && mousePosition.x < 1f) && transform.position.x < worldMaxX)
        {
            panX = Mathf.Clamp((mousePosition.x - 0.5f) * panSensitivity * 1 / (1f - mousePosition.x), 0f, 1.5f);
        }
        if ((mousePosition.x > 0 && mousePosition.x < 0.25f) && transform.position.x > worldMinX)
        {
            panX = Mathf.Clamp((mousePosition.x - 0.5f) * panSensitivity * 1 / mousePosition.x, -1.5f, 0f);
        }
        if ((mousePosition.y > 0.75f && mousePosition.y < 1f) && transform.position.z < worldMaxY)
        {
            panY = Mathf.Clamp((mousePosition.y - 0.5f) * panSensitivity * 1 / (1f - mousePosition.y), 0f, 1.5f);
        }
        if ((mousePosition.y > 0 && mousePosition.y < 0.25f) && transform.position.z > worldMinY)
        {
            panY = Mathf.Clamp((mousePosition.y - 0.5f) * panSensitivity * 1 / mousePosition.y, -1.5f, 0f);
        }
        /*       if(transform.position.y >= 10 && mouseScroll.y > 0)
               {
                   zoom = mouseScroll.y * sensitivity;
               }
               if (transform.position.y <= worldSize && mouseScroll.y < 0)
               {
                   zoom = mouseScroll.y * sensitivity;
               }
       */
        zoom = -mouseScroll.y * scrollSensitivity;

        transform.position += new Vector3(panX, zoom, panY);

        if (transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }
        if (transform.position.y < minY)
        {
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        }

        panX = 0;
        panY = 0;
        zoom = 0;
    }
}
