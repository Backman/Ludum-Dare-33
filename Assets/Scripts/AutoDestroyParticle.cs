using UnityEngine;
using System.Collections;

public class AutoDestroyParticle : MonoBehaviour {
    
    private ParticleSystem[] ps;
 
 
    public void Start() 
    {
        ps = GetComponentsInChildren<ParticleSystem>();
    }
 
    public void Update() 
    {
        bool alive = false;
        for (int i = 0; i < ps.Length; i++)
        {
            if(ps[i].IsAlive(false))
            {
                alive = true;
                break;
            }
        }
        if (alive == false)
        {
            Destroy(gameObject);
        }
    }

}
