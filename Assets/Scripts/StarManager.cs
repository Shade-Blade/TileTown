using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public static StarManager instance;

    public SingleStarScript star1;
    public SingleStarScript star2;
    public SingleStarScript star3;

    public Color activeColor; //you have the star
    public Color inactiveColor; //you don't have the star

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
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (CanvasBaseScript.instance.starTypeA)
        {
            case GridManager.StarType.Normal:
                activeColor = new Color(1, 1, 0);
                inactiveColor = new Color(0.6f, 0.6f, 0f);
                break;
            case GridManager.StarType.AllTypes:
                activeColor = new Color(0, 1, 0);
                inactiveColor = new Color(0f, 0.6f, 0f);
                break;
            case GridManager.StarType.LessNull:
                activeColor = new Color(1, 0, 0);
                inactiveColor = new Color(0.6f, 0f, 0f);
                break;
            case GridManager.StarType.NoNull:
                activeColor = new Color(0, 1, 1);
                inactiveColor = new Color(0f, 0.6f, 0.6f);
                break;
        }
        star1.SetColor(inactiveColor);
        star2.SetColor(inactiveColor);
        star3.SetColor(inactiveColor);
    }

    // Update is called once per frame
    void Update()
    {
        switch (CanvasBaseScript.instance.starTypeA)
        {
            case GridManager.StarType.Normal:
                activeColor = new Color(1, 1, 0);
                inactiveColor = new Color(0.6f, 0.6f, 0f);
                break;
            case GridManager.StarType.AllTypes:
                activeColor = new Color(0, 1, 0);
                inactiveColor = new Color(0f, 0.5f, 0f);
                break;
        }
        if (CanvasBaseScript.instance.starCount == 3)
        {
            star3.SetColor(activeColor);
        } else
        {
            star3.SetColor(inactiveColor);
        }
        if (CanvasBaseScript.instance.starCount >= 2)
        {
            star2.SetColor(activeColor);
        }
        else
        {
            star2.SetColor(inactiveColor);
        }
        if (CanvasBaseScript.instance.starCount >= 1)
        {
            star1.SetColor(activeColor);
        }
        else
        {
            star1.SetColor(inactiveColor);
        }
    }

    public void SetStars(GridManager.StarType starTypeA, int[] starRequirements)
    {
        switch (starRequirements.Length)
        {
            case 3:
                star3.SetEnabled(true);
                star2.SetEnabled(true);
                star1.SetEnabled(true);
                star3.SetNumber(starRequirements[2]);
                star2.SetNumber(starRequirements[1]);
                star1.SetNumber(starRequirements[0]);
                break;
            case 2:
                star3.SetEnabled(true);
                star2.SetEnabled(true);
                star1.SetEnabled(false);
                star3.SetNumber(starRequirements[1]);
                star2.SetNumber(starRequirements[0]);
                break;
            case 1:
                star3.SetEnabled(true);
                star2.SetEnabled(false);
                star1.SetEnabled(false);
                star3.SetNumber(starRequirements[0]);
                break;
            default:
                Debug.LogError("Invalid star count.");
                break;
        }
    }
}
