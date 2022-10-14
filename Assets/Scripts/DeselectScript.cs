using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeselectScript : MonoBehaviour
{
    //this doesn't completely work

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //click
            //checkState = CheckState.Inactive; //debug
            OnClick();
        }
    }

    void OnClick()
    {
        CanvasBaseScript.instance.Deselect();
        HelpBoxBaseScript.instance.SetOpen(false); //close the help
    }
}
