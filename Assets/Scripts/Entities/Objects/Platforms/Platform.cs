using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float viewBuffer = 2f;
    public int platformID;

    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        bool inCamera = viewportPos.x > -viewBuffer && viewportPos.x < 1 + viewBuffer * 10 &&
                        viewportPos.y > -viewBuffer && viewportPos.y < 1 + viewBuffer * 10;

        if (!inCamera)
        {
            Destroy(gameObject);
        }
    }
}
