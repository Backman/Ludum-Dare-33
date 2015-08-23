using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WrekFaceHandler : MonoBehaviour
{
    public float RekDuration = 0.08f;
    public Color BlinkColor;
    public float BlinkDuration;
    bool _HasFreezeTime = false;
    void Awake()
    {
        GameLogic.Instance.OnRekFace += OnRekFace;

    }

    void OnRekFace(GameObject obj)
    {
        BlinkManager.Instance.AddBlink(obj, BlinkColor, BlinkDuration);
        if(_HasFreezeTime == false)
        {
            StartCoroutine(FreezeTime(obj));
            _HasFreezeTime = true;
        }
    }

    IEnumerator FreezeTime(GameObject obj)
    {
        float startTime = Time.unscaledTime;
        Time.timeScale = 0f;
        while (startTime + RekDuration > Time.unscaledTime)
        {
            var followCam = Camera.main.GetComponent<FollowCamera>();
            Vector3 pos = obj.transform.position;
            followCam.SetTarget(pos, Vector2.zero, 0, 1);
            yield return null;
        }
        Time.timeScale = 1f;
        _HasFreezeTime = false;
    }

    void Update()
    {

    }
}
