using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowFactor = 0.8f;
    public float slowLength = 2f;

    void Start()
    {
        Time.timeScale = slowFactor;

        Time.fixedDeltaTime = Time.timeScale * 0.2f;

    }
    void Update()
    {

        Time.timeScale = (1f / slowLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);

        Time.fixedDeltaTime = Time.timeScale * 0.2f;
    }
}
