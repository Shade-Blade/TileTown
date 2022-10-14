using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTrackerScript : MonoBehaviour
{
    public Image Background;
    public TMPro.TMP_Text NumberText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NumberText.text = CanvasBaseScript.instance.currentScore + "";   
    }
}
