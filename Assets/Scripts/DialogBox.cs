using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogBox : MonoBehaviour
{
    public Text Text;
    public Graphic[] Graphics;
    void Awake()
    {
        Graphics = GetComponentsInChildren<Graphic>();
    }
}
