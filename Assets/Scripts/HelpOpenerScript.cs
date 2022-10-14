using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpOpenerScript : MonoBehaviour, MouseTarget
{
    public void OnClick()
    {
        HelpBoxBaseScript.instance.SetOpen(!HelpBoxBaseScript.instance.helpOpen);
    }
}
