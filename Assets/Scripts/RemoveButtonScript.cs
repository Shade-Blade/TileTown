using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPress()
    {
        CanvasBaseScript.instance.SelectPocketTile(TileScript.TileType.Empty);
        GridManager.instance.Deselect(); //we're not on the grid anymore, so stop selection
    }
}
