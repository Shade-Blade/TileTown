using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//y pos
//top = 220
//bottom = -170
//diff = 390
//35 canvas units
//  Space for 11 pocket tiles
//  ...though a level with that many tile types would be really complicated

public class PocketTileScript : MonoBehaviour
{
    public TileScript.TileType type;
    public int baseNumTiles;
    public int numTiles;
    public Image TileSprite;
    public TMPro.TMP_Text NumberText;
    //to do: add a description text box

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //I don't have to do this stuff every frame but it's probably not going to make the game slow
    void Update()
    {
        NumberText.text = numTiles + "";
    }

    public void OnButtonPress()
    {
        CanvasBaseScript.instance.SelectPocketTile(type);
        GridManager.instance.Deselect(); //we're not on the grid anymore, so stop selection
    }
}
