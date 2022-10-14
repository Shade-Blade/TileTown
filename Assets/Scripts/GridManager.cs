using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    public GameObject tilePrefab;

    public float tileWidth { get; private set; }
    public float interTileDistance;
    private int gridTileWidth;
    private int gridTileHeight;
    public float gridDistanceWidth;

    public int[] selected = { -1, -1 };

    //same objects but this way should save time later
    public GameObject[][] tileGridObjects;
    public TileScript[][] tileGridScripts;
    public bool[][] dotArray; //array of dots

    public enum StarType //type of star (some levels can have up to 2)
    {
        None,
        Normal,   //get N points to win
        AllTypes, //each tile type tracks points separately, get all point counts above N points
        LessNull, //get below N nullified tiles
        NoNull,   //same as normal, but no nullified tiles are allowed (sets your points to 0)
        LowTile   //lowest tile count
    }

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None; //so you can click on things :)
        selected = new int[2];
        selected[0] = -1;
        selected[1] = -1;

        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogWarning("Multiple grid creator scripts found.");
        }
    }

    public void BuildGrid(int width)
    {
        BuildGrid(width, width);
    }

    private void BuildGrid(int width, int height) //different widths and heights are not supported right now
    {
        /*
            may need to prevent race conditions
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //Debug.LogWarning("Multiple grid creator scripts found.");
        }
        */

        gridTileWidth = width;
        gridTileHeight = height;

        //calculate tile width
        //interTileDistance * (gridTileWidth + 1) + tileWidth * (gridTileWidth) = gridDistanceWidth
        //gridDistanceWidth - interTileDistance * (gridTileWidth + 1) / gridTileWidth
        tileWidth = (gridDistanceWidth - interTileDistance * (gridTileWidth + 1)) / gridTileWidth;

        //generate some tiles
        float currXPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2;
        float currYPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2;

        tileGridObjects = new GameObject[gridTileWidth][]; //hope this is right
        tileGridScripts = new TileScript[gridTileWidth][]; //hope this is right

        Vector3 scaleVector = new Vector3(tileWidth, tileWidth, tileWidth);

        for (int i = 0; i < gridTileWidth; i++)
        {
            tileGridObjects[i] = new GameObject[gridTileHeight];
            tileGridScripts[i] = new TileScript[gridTileHeight];
            for (int j = 0; j < gridTileHeight; j++)
            {
                tileGridObjects[i][j] = Instantiate(tilePrefab, new Vector3(currXPos, currYPos), Quaternion.identity);
                tileGridObjects[i][j].transform.localScale = scaleVector;
                tileGridObjects[i][j].transform.parent = gameObject.transform;
                tileGridScripts[i][j] = tileGridObjects[i][j].GetComponent<TileScript>();
                currYPos += tileWidth + interTileDistance; //tiles are square, so width = height
                tileGridScripts[i][j].TileStart();
                tileGridScripts[i][j].gridX = i;
                tileGridScripts[i][j].gridY = j;
            }
            //Debug.Log(currYPos - (-gridDistanceWidth / 2));
            currYPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2;
            currXPos += tileWidth + interTileDistance;
        }

        dotArray = new bool[tileGridScripts.Length][];
        for (int i = 0; i < tileGridScripts.Length; i++)
        {
            dotArray[i] = new bool[tileGridScripts.Length];
        }

        GridUpdate();
    }

    public void BuildGrid(int width,TileScript.TileType[][] layout)
    {
        BuildGrid(width, width, layout);
    }

    public void BuildGrid(int width, int height, TileScript.TileType[][] layout)
    {
        /*
            may need to prevent race conditions
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //Debug.LogWarning("Multiple grid creator scripts found.");
        }
        */

        gridTileWidth = width;
        gridTileHeight = height;

        //calculate tile width
        //interTileDistance * (gridTileWidth + 1) + tileWidth * (gridTileWidth) = gridDistanceWidth
        //gridDistanceWidth - interTileDistance * (gridTileWidth + 1) / gridTileWidth
        tileWidth = (gridDistanceWidth - interTileDistance * (gridTileWidth + 1)) / gridTileWidth;

        //generate some tiles
        tileGridObjects = new GameObject[gridTileWidth][]; 
        tileGridScripts = new TileScript[gridTileWidth][];

        Vector3 scaleVector = new Vector3(tileWidth, tileWidth, tileWidth);

        /*
        float currXPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2;
        float currYPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2;
        
        for (int i = 0; i < gridTileWidth; i++) //x
        {
            tileGridObjects[i] = new GameObject[gridTileHeight];
            tileGridScripts[i] = new TileScript[gridTileHeight];
            for (int j = 0; j < gridTileHeight; j++) //y
            {
                tileGridObjects[i][j] = Instantiate(tilePrefab, new Vector3(currXPos, currYPos), Quaternion.identity);
                tileGridObjects[i][j].transform.localScale = scaleVector;
                tileGridObjects[i][j].transform.parent = gameObject.transform;
                tileGridScripts[i][j] = tileGridObjects[i][j].GetComponent<TileScript>();
                currYPos += tileWidth + interTileDistance; //tiles are square, so width = height
                tileGridScripts[i][j].TileStart();
                tileGridScripts[i][j].gridX = i;
                tileGridScripts[i][j].gridY = j;
            }
            //Debug.Log(currYPos - (-gridDistanceWidth / 2));
            currYPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2;
            currXPos += tileWidth + interTileDistance;
        }
        */

        float currXPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2;
        float currYPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2 + (tileWidth + interTileDistance) * (gridTileHeight-1); //I want to start at the top

        for (int i = 0; i < gridTileHeight; i++)
        {
            tileGridObjects[i] = new GameObject[gridTileWidth];
            tileGridScripts[i] = new TileScript[gridTileWidth];
            for (int j = 0; j < gridTileWidth; j++)
            {
                tileGridObjects[i][j] = Instantiate(tilePrefab, new Vector3(currXPos, currYPos), Quaternion.identity);
                tileGridObjects[i][j].transform.localScale = scaleVector;
                tileGridObjects[i][j].transform.parent = gameObject.transform;
                tileGridScripts[i][j] = tileGridObjects[i][j].GetComponent<TileScript>();
                currXPos += tileWidth + interTileDistance; 
                tileGridScripts[i][j].TileStart();
                tileGridScripts[i][j].gridX = i;
                tileGridScripts[i][j].gridY = j;
            }
            currXPos = -gridDistanceWidth / 2 + interTileDistance + tileWidth / 2;
            currYPos -= tileWidth + interTileDistance;
        }

        //now set all the tiles properly
        //note that the arrays are not set up the same way ( :( )
        //Grid objects:
        //  i = x (0 = left)
        //  j = y (0 = bottom)

        //layout
        //  i = y (0 = top)
        //  j = x (0 = left)

        for (int i = 0; i < layout.Length; i++) //y
        {
            for (int j = 0; j < layout[i].Length; j++) //x
            {
                SetTileTypeBuild(i, j, layout[i][j]);
            }
        }

        dotArray = new bool[tileGridScripts.Length][];
        for (int i = 0; i < tileGridScripts.Length; i++)
        {
            dotArray[i] = new bool[tileGridScripts.Length];
        }

        GridUpdate();
    }

    public bool DotActive(int gridX, int gridY) //is dot active at this point?
    {
        if (selected[0] > -1 && selected[1] > -1)
        {
            return instance.dotArray[gridX][gridY];
        }
        return false;
    }

    public void Deselect()
    {
        selected[0] = -1;
        selected[1] = -1;

        DotArrayClear();
        //dotArray values default to false I think

        GridUpdate(); //not necessary, but it doesn't break anything
    }

    public void Select(int gridX, int gridY) //avoid using this, use CanvasBaseScript.instance.SelectTile
    {
        selected[0] = gridX;
        selected[1] = gridY;

        DotArrayClear();

        getTile(gridX, gridY).TileSelect();

        GridUpdate(); //not necessary, but it doesn't break anything
    }

    public TileScript.TileType Clear(int gridX, int gridY) //returns type of cleared tile
    {
        TileScript.TileType returnType = getTile(gridX, gridY).type;
        getTile(gridX, gridY).type = TileScript.TileType.Empty;
        getTile(gridX, gridY).graphics.RemoveSprite();
        GridUpdate(); //required (this method changes grid layout)
        return returnType;
    }

    public void SetTileType(int gridX, int gridY, TileScript.TileType type, bool update = true)
    {
        getTile(gridX, gridY).type = type;
        getTile(gridX, gridY).graphics.AddSprite(type);
        if (type == TileScript.TileType.Unusable)
        {
            //make tile transparent
            getTile(gridX, gridY).Hide();
        }
        if (update)
        {
            GridUpdate();
        }
    }

    public void SetTileTypeBuild(int gridX, int gridY, TileScript.TileType type) //used for grid builder
    {
        getTile(gridX, gridY).type = type;
        //Debug.Log(getTile(gridX, gridY).name);
        //Debug.Log(getTile(gridX, gridY).graphics);
        getTile(gridX, gridY).graphics.AddSprite(type);
        if (type == TileScript.TileType.Unusable)
        {
            //make tile transparent
            getTile(gridX, gridY).Hide();
        }
        if (type != TileScript.TileType.Empty && type != TileScript.TileType.Unusable)
        {
            getTile(gridX, gridY).mutable = false;
        }
    }

    public TileScript getTile(int gridX, int gridY)
    {
        return tileGridScripts[gridX][gridY];
    }

    public GameObject getTileObject(int gridX, int gridY)
    {
        return tileGridObjects[gridX][gridY];
    }

    public TileScript getRelative(int gridX, int gridY, int dx, int dy) //helper method for getting tile scripts around a given tile
    {
        if (gridX + dx < 0 || gridX + dx > gridTileWidth - 1 || gridY + dy < 0 || gridY + dy > gridTileHeight - 1)
        {
            return null; //out of bounds
        }
        return tileGridScripts[gridX + dx][gridY + dy];
    }

    public bool checkRelative(int gridX, int gridY, int dx, int dy, TileScript.TileType type) //true if tile at space == given type
    {
        if (getRelative(gridX, gridY, dx, dy) == null)
        {
            return false; //OOB = not the tile given
        }
        return getRelative(gridX, gridY, dx, dy).type == type;
    }

    public void DotArrayOffsetSet(int gridX, int gridY, int dx, int dy, bool value)
    {
        if (gridX + dx < 0 || gridX + dx > gridTileWidth - 1 || gridY + dy < 0 || gridY + dy > gridTileHeight - 1)
        {
            return; //out of bounds
        }
        if (tileGridScripts[gridX + dx][gridY + dy].type == TileScript.TileType.Unusable)
        {
            return; //don't put dots on unusable tiles
        }
        dotArray[gridX + dx][gridY + dy] = value;
    }

    public void DotArrayOffsetSetMulti(int gridX, int gridY, int[][] offsets, bool value)
    {
        for (int i = 0; i < offsets.Length; i++)
        {
            DotArrayOffsetSet(gridX, gridY, offsets[i][0], offsets[i][1], value);
        }
    }

    public void DotArrayClear()
    {
        for (int i = 0; i < dotArray.Length; i++)
        {
            for (int j = 0; j < dotArray[i].Length; j++)
            {
                dotArray[i][j] = false;
            }
        }
    }

    //do the two points have an offset of (dx,dy)?
    public bool checkOffset(int gridX, int gridY, int inputX, int inputY, int dx, int dy)
    {
        int dx2 = inputX-gridX;
        int dy2 = inputY-gridY;

        int[][] offsetList = createRelativeList(dx, dy);

        for (int i = 0; i < offsetList.Length; i++)
        {
            if (offsetList[i][0] == dx2 && offsetList[i][1] == dy2)
            {
                return true;
            }
        }
        return false;
    }

    //hard to explain method
    //it makes symmetry

    //(0,1) -> (0,1),(0,-1)(1,0)(-1,0)
    //(1,1) -> (1,1),(-1,1),(1,-1),(-1,-1)
    //(1,2) -> (1,2),(1,-2),(-1,2),(-1,-2),(2,1),(2,-1),(-2,1),(-2,-1)
    public int[][] createRelativeList(int offsetA, int offsetB) //helper method
    {
        int[][] output;
        //check offset symmetry
        //determines how big the output is
        if (offsetA == offsetB && offsetA == 0)
        {
            //degenerate case
            output = new int[1][];
            output[0] = new int[2];
            output[0][0] = 0;
            output[0][1] = 0;
        } else if (offsetA == offsetB)
        {
            output = new int[4][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = new int[2];
            }
            output[0][0] = offsetA;
            output[0][1] = offsetB;

            output[1][0] = -offsetA;
            output[1][1] = offsetB;

            output[2][0] = -offsetA;
            output[2][1] = -offsetB;

            output[3][0] = offsetA;
            output[3][1] = -offsetB;
        }
        else if (offsetA == 0 || offsetB == 0)
        {
            output = new int[4][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = new int[2];
            }
            output[0][0] = offsetA;
            output[0][1] = offsetB;

            output[1][0] = -offsetA;
            output[1][1] = -offsetB;

            output[2][0] = offsetB;
            output[2][1] = offsetA;

            output[3][0] = -offsetB;
            output[3][1] = -offsetA;
        }
        else
        {
            output = new int[8][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = new int[2];
            }
            output[0][0] = offsetA;
            output[0][1] = offsetB;

            output[1][0] = offsetA;
            output[1][1] = -offsetB;

            output[2][0] = -offsetA;
            output[2][1] = offsetB;

            output[3][0] = -offsetA;
            output[3][1] = -offsetB;

            output[4][0] = offsetB;
            output[4][1] = offsetA;

            output[5][0] = offsetB;
            output[5][1] = -offsetA;

            output[6][0] = -offsetB;
            output[6][1] = offsetA;

            output[7][0] = -offsetB;
            output[7][1] = -offsetA;
        }

        return output;
    }

    public bool checkAllRelative(int gridX, int gridY, int offsetA, int offsetB, TileScript.TileType type) //check spaces with given offset (symmetry is enforced)
    {
        //Returns true if at least one is equal to type

        int[][] offsetList = createRelativeList(offsetA, offsetB);

        for (int i = 0; i < offsetList.Length; i++)
        {
            if (checkRelative(gridX,gridY,offsetList[i][0],offsetList[i][1],type))
            {
                return true;
            }
        }

        return false;
    }

    public int countNeighbors(int gridX, int gridY, TileScript.TileType type)
    {
        int output = 0;

        //orthogonal neighbors
        int[][] offsetListA = createRelativeList(0, 1);
        for (int i = 0; i < offsetListA.Length; i++)
        {
            if (checkRelative(gridX, gridY, offsetListA[i][0], offsetListA[i][1], type))
            {
                output++;
            }
        }

        //diagonal neighbors
        offsetListA = createRelativeList(1, 1);
        for (int i = 0; i < offsetListA.Length; i++)
        {
            if (checkRelative(gridX, gridY, offsetListA[i][0], offsetListA[i][1], type))
            {
                output++;
            }
        }

        return output;
    }

    public int countOrthoNeighbors(int gridX, int gridY, TileScript.TileType type)
    {
        int output = 0;

        //orthogonal neighbors
        int[][] offsetListA = createRelativeList(0, 1);
        for (int i = 0; i < offsetListA.Length; i++)
        {
            if (checkRelative(gridX, gridY, offsetListA[i][0], offsetListA[i][1], type))
            {
                output++;
            }
        }

        return output;
    }

    public int countScore()
    {
        int output = 0;
        for (int u = 0; u < gridTileWidth; u++)
        {
            for (int v = 0; v < gridTileHeight; v++)
            {
                output += tileGridScripts[u][v].getScore();
            }
        }
        //Debug.Log(countGreenScore());

        return output;
    }

    public int countGreenScore()
    {
        //counts score for all tile types in the grid and pocket

        List<int> scores = new List<int>();
        scores.Add(1000); //value for type 0 (empty) (this gets sorted to the end of the list)
        for (int u = 0; u < gridTileWidth; u++)
        {
            for (int v = 0; v < gridTileHeight; v++)
            {
                if (tileGridScripts[u][v].type <= 0)
                {
                    continue;
                }

                //Debug.Log((int)tileGridScripts[u][v].type);

                while ((int)tileGridScripts[u][v].type > scores.Count-1) //need more space
                {
                    scores.Add(1000);
                }
                
                if (scores[(int)tileGridScripts[u][v].type] > 999)
                {
                    scores[(int)tileGridScripts[u][v].type] = tileGridScripts[u][v].getScore();
                } else
                {
                    scores[(int)tileGridScripts[u][v].type] += tileGridScripts[u][v].getScore();
                }
            }
        }

        //check pocket tiles
        for (int i = 0; i < CanvasBaseScript.instance.pocketTiles.Length; i++)
        {
            int index = (int)CanvasBaseScript.instance.pocketTiles[i].type;

            while (index > scores.Count - 1) //need more space
            {
                scores.Add(1000);
            }

            if (scores[index] > 999)
            {
                scores[index] = 0;                
            }
        }

        scores.Sort();
        if (scores.Count == 1 || scores[0] > 999)
        {
            return 0;
        }
        return scores[0];
    }

    public int countNull()
    {
        int output = 0;
        for (int u = 0; u < gridTileWidth; u++)
        {
            for (int v = 0; v < gridTileHeight; v++)
            {
                if (tileGridScripts[u][v].checkState == TileScript.CheckState.Inactive)
                {
                    output++;
                }
            }
        }
        //Debug.Log(countGreenScore());

        return output;
    }

    public int countScoreNoNull()
    {
        int output = 0;
        for (int u = 0; u < gridTileWidth; u++)
        {
            for (int v = 0; v < gridTileHeight; v++)
            {
                if (tileGridScripts[u][v].checkState == TileScript.CheckState.Inactive)
                {
                    return 0;
                }
                output += tileGridScripts[u][v].getScore();
            }
        }
        //Debug.Log(countGreenScore());

        return output;
    }

    public int getStarCount(StarType type, int[] requirements)
    {
        int output = 0;
        int score;
        switch (type)
        {
            case StarType.Normal:
                score = countScore();
                for (int i = 0; i < requirements.Length; i++)
                {
                    if (score >= requirements[i])
                    {
                        output++;
                    }
                }
                if (output != 0) //a level with only 1 star will have the one star act like 3 stars, a level with 2 stars acts like 2 or 3 stars
                {
                    output += (3 - requirements.Length);
                }
                break;
            case StarType.AllTypes:
                score = countGreenScore();
                for (int i = 0; i < requirements.Length; i++)
                {
                    if (score >= requirements[i])
                    {
                        output++;
                    }
                }
                if (output != 0) //a level with only 1 star will have the one star act like 3 stars, a level with 2 stars acts like 2 or 3 stars
                {
                    output += (3 - requirements.Length);
                }
                break;
            case StarType.LessNull:
                score = countNull();
                for (int i = 0; i < requirements.Length; i++)
                {
                    if (score <= requirements[i])
                    {
                        output++;
                    }
                }
                if (output != 0) //a level with only 1 star will have the one star act like 3 stars, a level with 2 stars acts like 2 or 3 stars
                {
                    output += (3 - requirements.Length);
                }
                break;
            case StarType.NoNull:
                score = countScoreNoNull();
                for (int i = 0; i < requirements.Length; i++)
                {
                    if (score >= requirements[i])
                    {
                        output++;
                    }
                }
                if (output != 0) //a level with only 1 star will have the one star act like 3 stars, a level with 2 stars acts like 2 or 3 stars
                {
                    output += (3 - requirements.Length);
                }
                break;
        }
        //Debug.Log(score + ", "+requirements[0]);
        return output;
    }

    public void GridUpdate() //call this any time when grid layout is updated
    {
        for (int i = 0; i < gridTileWidth; i++)
        {
            for (int j = 0; j < gridTileHeight; j++)
            {
                tileGridScripts[i][j].TileUpdate();
            }
        }

        switch (CanvasBaseScript.instance.starTypeA)
        {
            case StarType.Normal:
                CanvasBaseScript.instance.SetScore(countScore());
                break;
            case StarType.AllTypes:
                CanvasBaseScript.instance.SetScore(countGreenScore());
                break;
            case StarType.LessNull:
                CanvasBaseScript.instance.SetScore(countNull());
                break;
            case StarType.NoNull:
                CanvasBaseScript.instance.SetScore(countScoreNoNull());
                break;
        }
    }
}
