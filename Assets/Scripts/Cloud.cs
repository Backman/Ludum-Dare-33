using UnityEngine;

public class Cloud : MonoBehaviour
{
    public bool Visible;
    public float InvisibleTime;
    
    void OnBecameVisible()
    {
        Visible = true;
    }

    void OnBecameInvisible()
    {
        Visible = false;
        InvisibleTime = Time.time;
    }
}
