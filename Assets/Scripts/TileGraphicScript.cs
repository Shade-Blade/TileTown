using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles all the graphics on top of the empty tiles

//how to add a tile type:
//  add to the CanvasBaseScript's list
//  add to the tiletype enum in TileScript
//  add a score calculation in the TileUpdate method
//  add a dot array calculation in the TileSelect method

//re design- remove sprite objects (no TileSprites/Field folder anymore

public class TileGraphicScript : MonoBehaviour
{
    public SpriteRenderer sprite;

    public void AddSprite(TileScript.TileType type)
    {
        sprite.enabled = true;
        sprite.sprite = CanvasBaseScript.instance.getSprite(type);
    }

    public void RemoveSprite()
    {
        sprite.enabled = false;
    }
}
