using UnityEngine;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{
    struct CloudState
    {
        public GameObject Obj;
        public int X;
        public int Y;
    }

    struct IntRect
    {
        public int MinX;
        public int MinY;
        public int MaxX;
        public int MaxY;
    }

    IntRect _VisibleRect;


    public GameObject Prefab;
    public float LayerDepth;
    public float SpawnThreshold;
    public float NoiseResolution = 100f;
    public float GridSize = 2f;
    public float MinY = 1;
    public bool HasMinY;
    public float MaxY = 1;
    public bool HasMaxY;
    public bool RandomizedPos;
    List<CloudState> _Clouds = new List<CloudState>();
    static Dictionary<GameObject, Stack<GameObject>> _Pool = new Dictionary<GameObject, Stack<GameObject>>();



    void Update()
    {
        Vector3 centerPoint = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).GetPoint(LayerDepth);
        float frustumHeight = 2.0f * LayerDepth * Mathf.Tan((float)(Camera.main.fieldOfView * 0.5 * Mathf.Deg2Rad));
        float frustumWidth = frustumHeight * Camera.main.aspect;
        float minX = centerPoint.x - frustumWidth;
        float maxX = centerPoint.x + frustumWidth;
        float minY = centerPoint.y - frustumHeight;
        float maxY = centerPoint.y + frustumHeight;
//        Debug.Log(frustumWidth + " h " + frustumHeight);
        //Debug.Log("minX" + minX + "maxX" + maxX + "minY" + minY + " maxY" + maxY);
        //Debug.Log(centerPoint);


        IntRect newVisible;
        newVisible.MinX = Mathf.RoundToInt(GridSize * minX - 0.5f);
        newVisible.MinY = Mathf.RoundToInt(GridSize * minY - 0.5f);
        newVisible.MaxX = Mathf.RoundToInt(GridSize * maxX + 0.5f);
        newVisible.MaxY = Mathf.RoundToInt(GridSize * maxY + 0.5f);

        int xDeltaSign = Mathf.Abs(newVisible.MinX - _VisibleRect.MinX);
        if (xDeltaSign != 0)
        {
            for (int x = Mathf.Min(_VisibleRect.MinX, newVisible.MinX); x < Mathf.Max(newVisible.MinX, _VisibleRect.MinX); x += xDeltaSign)
            {
                for (int y = newVisible.MinY; y < newVisible.MaxY; y++)
                {
   //                 Debug.Log("new min x coords " + x + "," + y);
                    CoordVisible(x, y);
                }
            }

            for (int x = Mathf.Min(_VisibleRect.MaxX, newVisible.MaxX); x < Mathf.Max(newVisible.MaxX, _VisibleRect.MaxX); x += xDeltaSign)
            {
                for (int y = newVisible.MinY; y < newVisible.MaxY; y++)
                {
  //                  Debug.Log("new max x coords " + x + " , " + y);
                    CoordVisible(x, y);
                }
            }
        }

        _VisibleRect.MinX = newVisible.MinX;
        _VisibleRect.MaxX = newVisible.MaxX;
        int yDeltaSign = Mathf.Abs(newVisible.MinY - _VisibleRect.MinY);
        if (yDeltaSign != 0)
        {
            for (int y = Mathf.Min(_VisibleRect.MinY, newVisible.MinY); y < Mathf.Max(newVisible.MinY, _VisibleRect.MinY); y += yDeltaSign)
            {
                for (int x = newVisible.MinX; x < newVisible.MaxX; x++)
                {
 //                   Debug.Log("new min y coords " + y + " , " + x);
                    CoordVisible(x, y);
                }
            }

            for (int y = Mathf.Min(_VisibleRect.MaxY, newVisible.MaxY); y < Mathf.Max(newVisible.MaxY, _VisibleRect.MaxY); y += yDeltaSign)
            {
                for (int x = newVisible.MinX; x < newVisible.MaxX; x++)
                {
//                    Debug.Log("new max y coords" + y + " , " + x);
                    CoordVisible(x, y);
                }
            }
        }
        _VisibleRect.MinY = newVisible.MinY;
        _VisibleRect.MaxY = newVisible.MaxY;
        for (int i = _Clouds.Count - 1; i >= 0; i--)
        {
            var cloud = _Clouds[i];
            if (cloud.X < _VisibleRect.MinX
                || cloud.X > _VisibleRect.MaxX
                || cloud.Y > _VisibleRect.MaxY
                || cloud.Y < _VisibleRect.MinY)
            {
                cloud.Obj.SetActive(false);
                Stack<GameObject> objects;
                if (_Pool.TryGetValue(Prefab, out objects) == false)
                    objects = _Pool[Prefab] = new Stack<GameObject>();
                objects.Push(cloud.Obj);
                _Clouds.RemoveAt(i);
            }
        }
    }

    void CoordVisible(int x, int y)
    {
        float t = Mathf.PerlinNoise((float)x / NoiseResolution, (float)y / NoiseResolution);
        //Debug.Log("Spawn at " + x + ", " + y + ": " + (t > SpawnThreshold) + " " + t);
        if (t > SpawnThreshold)
        {
            Vector3 pos = new Vector3(x / GridSize, y / GridSize, LayerDepth);
            if (RandomizedPos)
            {
                pos.x += (Mathf.PerlinNoise(pos.y, pos.x) - 0.5f) / GridSize;
                pos.y += (Mathf.PerlinNoise(pos.x, pos.y) - 0.5f) / GridSize;
            }
            if ((HasMinY == false || pos.y > MinY) && (HasMaxY == false || pos.y < MaxY))
            {
                Stack<GameObject> objects;
                GameObject go;
                if (_Pool.TryGetValue(Prefab, out objects) == false || objects.Count == 0)
                    go = GameObject.Instantiate(Prefab) as GameObject;
                else
                    go = objects.Pop();
                go.SetActive(true);
                go.transform.position = pos;

                _Clouds.Add(new CloudState()
                {
                    Obj = go,
                    X = x,
                    Y = y,
                });
            }
        }
    }
}
