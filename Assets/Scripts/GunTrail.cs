using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTrail : MonoBehaviour
{
    public LineRenderer line;

    public Vector3 startPos;
    public Vector3 endPos;

    public const float FadeTime = 0.5f;
    private float FadeTimeLeft = FadeTime;

    private void Update()
    {
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);

        line.material.SetColor("_Color", new Color(1, 1, 1, FadeTimeLeft / FadeTime));

        if (FadeTimeLeft <= 0)
        {
            Destroy(gameObject);
        }

        FadeTimeLeft = Mathf.Max(FadeTimeLeft - Time.deltaTime, 0);
    }
}
