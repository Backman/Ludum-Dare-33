using UnityEngine;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{
    struct CloudState
    {
        public Cloud Obj;
        
    }


    public float LayerDepth;
    List<CloudState> _Clouds = new List<CloudState>();
    void Update()
    {
        Vector3 centerPoint = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).GetPoint(LayerDepth);
        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        for (int i =0; i < planes.Length; i++)
        {
            Vector3 localNormal = Camera.main.worldToCameraMatrix * planes[i].normal;
            float distance = planes[i].GetDistanceToPoint(centerPoint);
            if (localNormal.x > 0)
            {
                maxX = centerPoint.x + distance;
            }
            else if (localNormal.x < 0)
            {
                minX = centerPoint.x - distance;
            }
            else if (localNormal.y > 0)
            {
                maxY = centerPoint.y + distance;
            }
            else if (localNormal.y < 0)
            {
                minY = centerPoint.y - distance;
            }

            //Debug.Log("point " + centerPoint + " distance " + distance + " from " + planes[i].normal + " -- " + planes[i].distance );
        }
        Debug.Log("minX" + minX + "maxX" + maxX + "minY" + minY + " maxY" + maxY);



        int minVisibleX = Mathf.RoundToInt(minX - 0.5f);
        int minVisibleY = Mathf.RoundToInt(minY - 0.5f);
        int maxVisibleX = Mathf.RoundToInt(maxX + 0.5f);
        int maxVisibleY = Mathf.RoundToInt(maxY + 0.5f);
    }
}
