using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //click
            //checkState = CheckState.Inactive; //debug
            OnClick();
        }
    }

    public void OnClick()
    {
        SceneManager.LoadScene("SelectScene");
    }
}
