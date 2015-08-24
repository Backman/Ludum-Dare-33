using UnityEngine;
using System.Collections;

public class BlokfoskFollow : MonoBehaviour {

    public float Z;
    FollowCamera _Cam;
	// Use this for initialization
	void Start () {
        _Cam = GetComponent<FollowCamera>();
	}
	
	// Update is called once per frame
	void Update () {
        _Cam.SetTarget(Blokfosk.Instance.transform.position, Vector2.zero, Z, 0, 1000f);
	}
}
