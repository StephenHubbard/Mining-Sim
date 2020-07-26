using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsText : MonoBehaviour
{
    public Text thisText;

    void Start()
    {
        thisText = GetComponent<Text>();
    }

    void Update()
    {
        float fpsInt = 1.0f / Time.deltaTime;
        thisText.text = fpsInt.ToString("F0");
    }
}
