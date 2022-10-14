using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBaseScript : MonoBehaviour
{
    public TileScript.TileType selectedType { get; private set; }
    public bool selectedPocket;
    public int pocketIndex;
    public PocketTileScript[] pocketTiles; //this is where the pocket scripts are stored

    public int currentScore { get; private set; } //set in GridManager

    public int starCount;
    public int[] starRequirements; //usually there are 3 stars, but only 1 star is necessary
    public int[] starRequirementsB; //some levels will have 2 different requirements
    public GridManager.StarType starTypeA;
    public GridManager.StarType starTypeB;

    public Sprite[] tileSprites;// = new Sprite[256];
    public GameObject pocketButton;
    private float currYPos;

    public static CanvasBaseScript instance;

    //currently selected tile type and whether it is from the pocket or not
    //"empty" + pocket is treated as "remove", while "empty" + no pocket is treated as a null value
    //  the "inPocket" variable is not necessary, but it's not completely intuitive that placing empty tiles is equal to removing them
    //This is supplemented by GridManager.selected

    //how stuff works
    //  click a pocket tile (with tile count > 1):
    //      selected = [-1,-1] (treated as a null value)
    //      selectedType = (type selected) (clicking remove sets this to empty)
    //      inPocket = true
    //      
    //  click a pocket tile (tile count = 0)
    //      same as pocket with tile count > 1
    //  click a grid tile (not empty)
    //      selected = (position)
    //      selectedType = (type selected)
    //      inPocket = false
    //  click empty grid tile
    //      check that you can place tile
    //      set tile to selectedType if you can

    public void Deselect()
    {
        //revert everything to default values, and use gridmanager deselect
        selectedType = TileScript.TileType.Empty;
        selectedPocket = false;
        GridManager.instance.Deselect();
    }

    public void SelectTile(int gridX, int gridY) //do not call this from GridManager
    {
        selectedType = GridManager.instance.getTile(gridX,gridY).type;
        selectedPocket = false;
        pocketIndex = -2;
        GridManager.instance.Select(gridX, gridY);
    }

    public void SelectPocketTile(TileScript.TileType type)
    {
        GridManager.instance.Deselect();
        selectedPocket = true;
        if (type == TileScript.TileType.Empty)
        {
            pocketIndex = -1; //special case for the remove button
        } else
        {
            for (int i = 0; i < pocketTiles.Length; i++)
            {
                if (type == pocketTiles[i].type)
                {
                    pocketIndex = i;
                }
            }
        }
        selectedType = type;
    }

    public void PlaceTile(int gridX, int gridY, TileScript.TileType type)
    {
        TileScript tile = GridManager.instance.getTile(gridX, gridY);
        if (!tile.mutable || tile.type != TileScript.TileType.Empty) //non-alterable
        {
            return;
        }

        bool success = false;

        //now put in pocket
        foreach (PocketTileScript p in pocketTiles)
        {
            if (p.type == type)
            {
                if (p.numTiles > 0)
                {
                    p.numTiles--;
                    success = true;
                }
                else
                {
                    //Debug.Log("Could not place tile");
                }
            }
        }

        if (!success)
        {
            //Debug.Log("Tile not placed.");
        } else
        {
            GridManager.instance.SetTileType(gridX, gridY, type);
        }
    }

    public void RemoveTile(int gridX, int gridY) //sets tile to empty, puts tile in pocket
    {
        TileScript tile = GridManager.instance.getTile(gridX, gridY);
        if (!tile.mutable || tile.type == TileScript.TileType.Empty || tile.type == TileScript.TileType.Unusable) //non-removable tiles
        {
            return;
        }

        //clear the tile
        TileScript.TileType type = GridManager.instance.Clear(gridX, gridY);

        bool success = false;

        //now put in pocket
        foreach (PocketTileScript p in pocketTiles)
        {
            if (p.type == type)
            {
                if (p.numTiles < p.baseNumTiles)
                {
                    p.numTiles++;
                    success = true;
                } else
                {
                    Debug.LogError("Tile count reached maximum");
                }
            }
        }

        if (!success)
        {
            Debug.LogError("Removed tile could not be returned to pocket.");
        }

        GridManager.instance.GridUpdate();
    }

    //x pos = -405
    //y pos
    //top = 220
    //bottom = -170
    //diff = 390
    //35 canvas units
    //  Space for 11 pocket tiles
    //  ...though a level with that many tile types would be really complicated
    public void BuildPocketTiles(PocketTileData[] data)
    {
        currYPos = 220;
        Vector3 position = new Vector3(-405, currYPos, 0);
        for (int i = 0; i < data.Length; i++)
        {
            CreateCanvasTile(data[i], position);
            currYPos -= 35;
            position = new Vector3(-405, currYPos, 0);
        }

        //could use the loop above to do this but I'm lazy
        pocketTiles = FindObjectsOfType<PocketTileScript>();
    }

    public void CreateCanvasTile(PocketTileData data, Vector3 position) //create a tile with given data struct
    {
        GameObject tile = Instantiate(pocketButton, gameObject.transform);
        //now let's set up the tile
        PocketTileScript script = tile.GetComponent<PocketTileScript>();

        script.baseNumTiles = data.count;
        script.numTiles = data.count;
        script.TileSprite.sprite = getSprite(data.type);
        script.type = data.type;
        //Debug.Log(position);
        tile.transform.localPosition = position;

        //Debug.Log(tile.transform.position);
    }

    public Sprite getSprite(TileScript.TileType type)
    {
        if (tileSprites == null)
        {
            //unity is stupid and this is broken
            tileSprites = Resources.LoadAll<Sprite>("TileGraphics");
        }
        if (type == TileScript.TileType.Empty || type == TileScript.TileType.Unusable)
        {
            return null;
        }
        return tileSprites[(int)type - 1];
    }

    public void SetScore(int i)
    {
        //Debug.Log("set score to " + i);
        instance.currentScore = i; 
    }

    public void SetStarRequirements(GridManager.StarType type, int[] requirements)
    {
        starTypeA = type;
        starRequirements = requirements;
    }

    public void SetStarRequirements(GridManager.StarType typeA, GridManager.StarType typeB, int[] requirementsA, int[] requirementsB)
    {
        starTypeA = typeA;
        starRequirements = requirementsA;
        starTypeB = typeB;
        starRequirementsB = requirementsB;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple canvas scripts found.");
        }

        //if (tileSprites == null)
        //{
        tileSprites = Resources.LoadAll<Sprite>("Sprites/TileGraphics");
        //}
    }

    /*
    private void Start()
    {

    }
    */

    // Update is called once per frame
    void Update()
    {
        instance.starCount = GridManager.instance.getStarCount(starTypeA, starRequirements);
        //Debug.Log(CanvasBaseScript.instance.starCount);
        if (starRequirementsB.Length > 0)
        {
            instance.starCount = Mathf.Min(instance.starCount, GridManager.instance.getStarCount(starTypeB, starRequirementsB));
        }
        //Debug.Log(CanvasBaseScript.instance.starCount);
    }
}
