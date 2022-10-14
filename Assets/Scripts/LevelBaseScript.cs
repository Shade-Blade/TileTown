using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//places level icon tiles and determines level accessibility
//x diff = 0.16
//y diff = 0.35
//6 x 5 grid

public class LevelBaseScript : MonoBehaviour
{
    private static int maxLevels;
    public static int maxPages { get; private set; } //set at time of loading (each page has to most 30 levels)

    public GameObject levelIcon;
    public GameObject baseSprite;

    public GameObject cameraObject;
    private int currentPage;

    public LevelButtonScript[] levelButtons;

    public static LevelBaseScript instance;

    private float xStart = -4.0f;
    private float yStart = 3.0f;
    private float xDiff = 1.6f;
    private float yDiff = -1.5f;
    private float pageDiff = 20f;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        maxLevels = Resources.LoadAll<TextAsset>("LevelData").Length;

        cameraObject = FindObjectOfType<Camera>().gameObject;
        BuildLevelGrid();   
    }

    public static int getMaxLevels() //avoid doing this
    {
        if (maxLevels < 5)
        {
            maxLevels = Resources.LoadAll<TextAsset>("LevelData").Length;
        }
        return maxLevels;
    }

    public void SetPage(int i)
    {
        if (i < 0)
        {
            i = 0;
        }
        if (i > maxPages-1)
        {
            i = maxPages-1;
        }

        currentPage = i;
        cameraObject.transform.position = Vector3.right * currentPage * pageDiff + Vector3.back * 10;
    }

    public void SetPageRel(int j)
    {
        SetPage(currentPage + j);
    }

    void BuildLevelGrid()
    {
        GameObject icon;
        GameObject levelBackground;
        Vector3 posVector;
        int currPage = 0;
        levelButtons = new LevelButtonScript[maxLevels];

        maxPages = Mathf.CeilToInt((float)maxLevels / 30);


        bool done = false;

        while (!done)
        {
            levelBackground = Instantiate(baseSprite, gameObject.transform);
            levelBackground.transform.position = Vector3.right * pageDiff * currPage * gameObject.transform.localScale[0];
            for (int i = 0; i < 5; i++) //column
            {
                for (int j = 0; j < 6; j++) //row
                {
                    if (currPage * 30 + i * 6 + j + 1 > maxLevels)
                    {
                        done = true;
                        break;
                    }

                    icon = Instantiate(levelIcon, levelBackground.gameObject.transform);
                    posVector = Vector3.right * (xStart + xDiff * j) + Vector3.up * (yStart + yDiff * i);
                    icon.transform.localPosition = posVector;
                    LevelButtonScript script = icon.GetComponent<LevelButtonScript>();
                    script.levelID = j + 6 * i + 1 + currPage * 30;
                    script.SetState(CalculateState(j + 6 * i + 1 + currPage * 30));

                }
            }
            currPage++;
        }
    }

    public static LevelButtonScript.LevelState CalculateState(int id)
    {
        if (id > maxLevels) //no accessing nonexistent levels
        {
            return LevelButtonScript.LevelState.Locked;
        }

        int maxLevel = 0;
        int starCount = 0;
        float averageStars;
        for (int i = 0; i < GlobalManager.stars.Count; i++)
        {
            if (GlobalManager.stars[i] != 0)
            {
                starCount += GlobalManager.stars[i];
                maxLevel = i;
                
                if (i+1 == id)
                {
                    return LevelButtonScript.LevelState.Complete;
                }
            }
        }
        averageStars = (float)starCount / (maxLevel+1);

        if (GlobalManager.debug)
        {
            return LevelButtonScript.LevelState.Unlocked;
        }

        return id-1 > maxLevel + Mathf.Clamp((averageStars - 0.5f),0,3) ? LevelButtonScript.LevelState.Locked : LevelButtonScript.LevelState.Unlocked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
