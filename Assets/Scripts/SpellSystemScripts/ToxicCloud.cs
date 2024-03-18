using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicCloud : MonoBehaviour
{
    public float targetScale;
    public float duration;
    public float stayDuration;

    private Vector3 initialScale;
    private float startTime;

    void Start()
    {
        initialScale = transform.localScale;
        startTime = Time.time;
    }

    void Update()
    {
        float progress = (Time.time - startTime) / duration;
        progress = Mathf.Clamp01(progress);
        Vector3 newScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, targetScale), progress);

        transform.localScale = newScale;

        if (progress >= 1f)
        {
            Destroy(gameObject, stayDuration);
        }
    }
}
