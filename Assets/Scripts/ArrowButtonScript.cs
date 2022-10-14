using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowButtonScript : MonoBehaviour
{
    public int distance; //how much to scroll by (scrolling too far makes you go to the end)

    public void OnButtonPress()
    {
        LevelBaseScript.instance.SetPageRel(distance);
    }
}
