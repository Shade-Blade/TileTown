using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PocketTileData
{
    public TileScript.TileType type;
    public int count;

    public PocketTileData(TileScript.TileType type, int count)
    {
        this.type = type;
        this.count = count;
    }
}

public class LevelData //single level's data
{
    public int levelSize;
    public PocketTileData[] pocketTiles;

    public int[] starRequirements; //usually there are 3 stars, but only 1 star is necessary
    public int[] starRequirementsB; //some levels will have 2 different requirements
        //if a level only has 1 requirement, set the number of stars to 0 or set requirement to none
    public GridManager.StarType starTypeA;
    public GridManager.StarType starTypeB;

    public TileScript.TileType[][] levelLayout;

    public string[] helpText;
}

public class LevelLoaderScript : MonoBehaviour
{
    public static LevelLoaderScript instance;
    private int gridWidth;
    private int gridHeight;

    public static int currLevelID;

    public string levelData { get; private set; }
    public LevelData currLevel { get; private set; }

    //level data format
    //line 1: level size, pocket tiles
    //  level size is one int (rectangular levels are currently not supported, just use unusable tiles to do that)
    //  pocket tiles use enum names, then "*#" to say how many there are
    //line 2: level requirements
    //  level requirement type(s), then the star requirements
    //  if there are 2 sets of star requirements, there must be an even number of them (so each star gets 2 requirements)
    //      separate them into 2 lists (example: (1,2,3) and (2,3,4) written as 1,2,3,2,3,4)
    //line 3+: level data
    //  places level tiles
    //  shortcuts allow you to use ints instead of enum names, though enum names are fine and preferred for anything that isn't empty/unusable

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple level loader scripts found.");
        }

        //currLevelID = 2;
    }

    private void getLevelData()
    {
        levelData = Resources.Load<TextAsset>("LevelData/Level"+currLevelID).text;
        currLevel = new LevelData();

        string[] dataLines = levelData.Split('\n');

        //line 1
        string[] dataPoints = dataLines[0].Split(',');
        currLevel.levelSize = int.Parse(dataPoints[0]);
        currLevel.pocketTiles = new PocketTileData[dataPoints.Length - 1];
        for (int i = 1; i < dataPoints.Length; i++)
        {
            string[] tempString = dataPoints[i].Split('*');
            currLevel.pocketTiles[i - 1] = new PocketTileData((TileScript.TileType)Enum.Parse(typeof(TileScript.TileType), tempString[0]), int.Parse(tempString[1]));
        }

        //line 2
        dataPoints = dataLines[1].Split(',');
        //guaranteed to have at least one star type
        currLevel.starTypeA = (GridManager.StarType)Enum.Parse(typeof(GridManager.StarType), dataPoints[0]);

        GridManager.StarType tempB;
        int tempC;
        if (int.TryParse(dataPoints[1],out tempC))
        {
            //only one star type
            currLevel.starTypeB = GridManager.StarType.None;
            currLevel.starRequirementsB = null;

            //check star requirements
            if (dataPoints.Length > 4)
            {
                Debug.LogError("Too many star requirements (max = 3). Only first 3 will be used.");
                currLevel.starRequirements = new int[3];
            }
            else
            {
                currLevel.starRequirements = new int[dataPoints.Length - 1];
            }

            for (int j = 0; j < currLevel.starRequirements.Length; j++)
            {
                currLevel.starRequirements[j] = int.Parse(dataPoints[j + 1]);
            }
        }
        else
        {
            Enum.TryParse(dataPoints[1], out tempB);
            currLevel.starTypeB = tempB;
            //now try to parse stars
            if (dataPoints.Length > 8)
            {
                Debug.LogError("Too many star requirements (max = 6, or 3 each). Only first 6 will be used.");
                currLevel.starRequirements = new int[3];
                currLevel.starRequirementsB = new int[3];
                int parsePoint;
                for (int j = 0; j < currLevel.starRequirements.Length; j++)
                {
                    parsePoint = j + 2; //starts at 2 and ends at 2 + # of star requirements
                    currLevel.starRequirements[j] = int.Parse(dataPoints[parsePoint]);
                }
                for (int j = 0; j < currLevel.starRequirementsB.Length; j++)
                {
                    parsePoint = j + 2 + currLevel.starRequirements.Length;
                    currLevel.starRequirements[j] = int.Parse(dataPoints[parsePoint]);
                }
            }
            else
            {
                currLevel.starRequirements = new int[(dataPoints.Length - 2) / 2];
                currLevel.starRequirementsB = new int[(dataPoints.Length - 2) / 2];
                int parsePoint;
                for (int j = 0; j < currLevel.starRequirements.Length; j++)
                {
                    parsePoint = j + 2; //starts at 2 and ends at 2 + # of star requirements
                    currLevel.starRequirements[j] = int.Parse(dataPoints[parsePoint]);
                }
                for (int j = 0; j < currLevel.starRequirementsB.Length; j++)
                {
                    parsePoint = j + 2 + currLevel.starRequirements.Length;
                    currLevel.starRequirements[j] = int.Parse(dataPoints[parsePoint]);
                }
            }
        }

        //lines 3+ (level layout data)

        currLevel.levelLayout = new TileScript.TileType[currLevel.levelSize][];
        for (int i = 2; i < 2+currLevel.levelSize; i++)
        {
            dataPoints = dataLines[i].Split(',');
            currLevel.levelLayout[i-2] = new TileScript.TileType[dataPoints.Length];
            for (int j = 0; j < dataPoints.Length; j++)
            {
                //check if it's an int
                int result;
                TileScript.TileType type;

                if (int.TryParse(dataPoints[j],out result))
                {
                    type = (TileScript.TileType)result;
                } else
                {
                    type = (TileScript.TileType)Enum.Parse(typeof(TileScript.TileType), dataPoints[j]);
                }

                //put it into the layout
                //Debug.Log(currLevel.levelLayout[i-2]);
                currLevel.levelLayout[i-2][j] = type;
            }
        }

        //extra lines: help text
        currLevel.helpText = new string[dataLines.Length - (2 + currLevel.levelSize)];
        int k = 0;
        for (int i = 2+currLevel.levelSize; i < dataLines.Length; i++)
        {
            currLevel.helpText[k] = dataLines[i];
            k++;
        }
    }

    private void Start()
    {
        getLevelData();
        StarManager.instance.SetStars(currLevel.starTypeA, currLevel.starRequirements);
        CanvasBaseScript.instance.BuildPocketTiles(currLevel.pocketTiles);
        CanvasBaseScript.instance.SetStarRequirements(currLevel.starTypeA, currLevel.starRequirements);
        GridManager.instance.BuildGrid(currLevel.levelSize, currLevel.levelLayout);
        HelpBoxBaseScript.instance.SetText(currLevel.helpText);
        //HelpBoxBaseScript.instance.SetOpen(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
