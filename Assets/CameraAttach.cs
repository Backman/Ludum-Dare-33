using UnityEngine;
using System.Collections;

public class CameraAttach : MonoBehaviour
{
    void LateUpdate()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 currentPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        currentPos.x = cameraPos.x;
        currentPos.y = cameraPos.y;
        transform.position = currentPos;
        float frustumHeight = 2.0f * Mathf.Abs(transform.position.z - Camera.main.transform.position.z) * Mathf.Tan((float)(Camera.main.fieldOfView * 0.5 * Mathf.Deg2Rad));
        float frustumWidth = frustumHeight * Camera.main.aspect;
        rect.sizeDelta = new Vector2(frustumWidth, frustumHeight) * 6;

    }
}
