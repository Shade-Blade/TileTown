using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    TMPro.TMP_Text text;

    float updateRate = 0.25f;
    float updateTimer;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateTimer <= 0)
        {
            text.text = "FPS = " + (1 / Time.deltaTime);
            updateTimer = updateRate;
        } else
        {
            updateTimer -= Time.deltaTime;
        }
    }
}
