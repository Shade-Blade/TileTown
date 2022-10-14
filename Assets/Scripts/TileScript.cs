using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour, MouseTarget
{
    public bool mutable; //can you change what's on this tile?
    public TileType type;
    public CheckState checkState; //does this tile give points?
                                  //inactive = no
                                  //active = yes
                                  //indeterminate = to be calculated later
    public int score { get; private set; }
    public bool hidden { get; private set; }

    public int gridX;
    public int gridY;

    public SpriteRenderer TileSprite;
    public SpriteRenderer XSprite;
    public SpriteRenderer SelectCursor;
    public SpriteRenderer TileDot; //dot for tiles affected by selected tile

    public TileGraphicScript graphics;

    private bool started = false;

    public enum TileType
    {
        Unusable = -1, //cannot place other tiles (usually these tiles are invisible)
        Empty = 0, //can place other tiles
        CarrotFarm = 1,
        PotatoFarm = 2,
        BeanFarm = 3,
        MaizeFarm = 4,
        WheatFarm = 5,
        House0 = 6,
        House2 = 7,
        House3 = 8,
        House5 = 9,
        GuardTower = 10,
        Store = 11
    }

    public enum CheckState
    {
        Inactive,
        Active,
        Indeterminate //some tiles require other tiles to calculate their active state
    }

    // Start is called before the first frame update
    void Start()
    {
        TileStart();
    }


    public void OnClick()
    {
        //  click a grid tile (not empty) or can't place tile (but not removed)
        //      place tile
        //  click a grid tile (not empty) with the remove button
        //      remove tile
        //  click empty grid tile
        //      select tile

        if (hidden)
        {
            return; //ignore
        }

        if (mutable && !hidden && CanvasBaseScript.instance.selectedPocket && CanvasBaseScript.instance.selectedType != TileType.Empty && type == TileType.Empty)
        {
            CanvasBaseScript.instance.PlaceTile(gridX, gridY, CanvasBaseScript.instance.pocketTiles[CanvasBaseScript.instance.pocketIndex].type);
        } else if (mutable && CanvasBaseScript.instance.selectedPocket && CanvasBaseScript.instance.selectedType == TileType.Empty)
        {
            CanvasBaseScript.instance.RemoveTile(gridX, gridY);
        } else
        {
            CanvasBaseScript.instance.SelectTile(gridX, gridY);
        }
    }

    public void Hide() //make transparent (currently, it also makes tile unselectable and unusable)
    {
        hidden = true;
        mutable = false;
        type = TileType.Unusable;
        TileSprite.enabled = false;
        XSprite.enabled = false;
        SelectCursor.enabled = false;
        TileDot.enabled = false;
    }

    public void TileStart() //called while the tile grid is being built
    {
        if (started)
        {
            return;
        }
        started = true;
        checkState = CheckState.Active;
        XSprite.enabled = false;
        SelectCursor.enabled = false;
        TileDot.enabled = false;
        graphics = GetComponent<TileGraphicScript>();
    }

    public void TileSelect()
    {
        GridManager.instance.DotArrayClear();

        switch (type)
        {
            case TileType.CarrotFarm:
            case TileType.PotatoFarm:
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(0, 1), true);
                break;
            case TileType.BeanFarm:
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(1, 1), true);
                break;
            case TileType.MaizeFarm:
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(2, 2), true);
                break;
            case TileType.WheatFarm:
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(0, 1), true);
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(0, 2), true);
                break;
            case TileType.GuardTower:
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(0, 1), true);
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(2, 2), true);
                break;
            case TileType.House0:
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(0, 1), true);
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(1, 1), true);
                break;
            case TileType.House2:
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(0, 1), true);
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(1, 1), true);
                break;
            case TileType.Store:
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(1, 1), true);
                GridManager.instance.DotArrayOffsetSetMulti(gridX, gridY, GridManager.instance.createRelativeList(0, 2), true);
                break;
        }
    }

    public void TileUpdate() //activated when the map is updated, after calculating active state
    {
        //could just use different script files for each tile type
        //but then I would have to replace the script every time you place or remove a tile

        switch (type)
        {
            case TileType.CarrotFarm:
                checkState = CheckState.Active;
                if (GridManager.instance.checkAllRelative(gridX,gridY,1,0,type))
                {
                    checkState = CheckState.Inactive;
                }
                break;
            case TileType.PotatoFarm:
                checkState = CheckState.Active;
                if (GridManager.instance.checkAllRelative(gridX, gridY, 1, 0, type))
                {
                    checkState = CheckState.Inactive;
                }
                break;
            case TileType.BeanFarm:
                checkState = CheckState.Active;
                if (GridManager.instance.checkAllRelative(gridX, gridY, 1, 1, type))
                {
                    checkState = CheckState.Inactive;
                }
                break;
            case TileType.MaizeFarm:
                checkState = CheckState.Active;
                if (GridManager.instance.checkAllRelative(gridX, gridY, 2, 2, type))
                {
                    checkState = CheckState.Inactive;
                }
                break;
            case TileType.WheatFarm:
                checkState = CheckState.Active;
                if (GridManager.instance.checkAllRelative(gridX, gridY, 0, 1, type))
                {
                    checkState = CheckState.Inactive;
                }
                if (GridManager.instance.checkAllRelative(gridX, gridY, 0, 2, type))
                {
                    checkState = CheckState.Inactive;
                }
                break;
            case TileType.GuardTower:
                checkState = CheckState.Active;
                if (GridManager.instance.checkAllRelative(gridX, gridY, 1, 0, type))
                {
                    checkState = CheckState.Inactive;
                }
                if (GridManager.instance.checkAllRelative(gridX, gridY, 2, 2, type))
                {
                    checkState = CheckState.Inactive;
                }
                break;
            case TileType.House0:
                checkState = CheckState.Active;
                if (GridManager.instance.checkAllRelative(gridX, gridY, 1, 0, type))
                {
                    checkState = CheckState.Inactive;
                }
                if (GridManager.instance.checkAllRelative(gridX, gridY, 1, 1, type))
                {
                    checkState = CheckState.Inactive;
                }
                break;
            case TileType.House2:
                checkState = CheckState.Active;
                if (GridManager.instance.countOrthoNeighbors(gridX,gridY,type) != 2)
                {
                    checkState = CheckState.Inactive;
                }
                break;
            case TileType.Store:
                checkState = CheckState.Active;
                if (GridManager.instance.checkAllRelative(gridX, gridY, 1, 1, type))
                {
                    checkState = CheckState.Inactive;
                }
                if (GridManager.instance.checkAllRelative(gridX, gridY, 2, 0, type))
                {
                    checkState = CheckState.Inactive;
                }
                break;
            default:
                //nothing
                //but make sure the graphics are correct
                checkState = CheckState.Active;
                break;
        }

        XSprite.enabled = checkState == CheckState.Inactive;
        //Debug.Log(XSprite.enabled);

        //Debug.Log(XSprite.enabled);

        SelectCursor.enabled = GridManager.instance.selected[0] == gridX && GridManager.instance.selected[1] == gridY;
        if (!CanvasBaseScript.instance.selectedPocket)
        {
            TileDot.enabled = GridManager.instance.DotActive(gridX, gridY);
        } else
        {
            TileDot.enabled = false;
        }
    }

    public int getScore()
    {
        switch (type)
        {
            case TileType.Unusable:
            case TileType.Empty:
                score = 0;
                break;
            default:
                score = 1;
                break;
        }

        return (checkState == CheckState.Active) ? score : 0;
    }
}
