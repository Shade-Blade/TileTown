using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextButtonScript : MonoBehaviour
{

    public Image TileSprite;
    public TMPro.TMP_Text NextText;

    private bool active; //is next level button active?

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        active = CanvasBaseScript.instance.starCount > 0;
        if (TileSprite.enabled != active)
        {
            if (active)
            {
                TileSprite.enabled = true;
                NextText.enabled = true;
            } else
            {
                TileSprite.enabled = false;
                NextText.enabled = false;
            }
        }
    }

    public void OnButtonPress()
    {
        if (active)
        {
            //Set star count if higher than star count saved


            if (GlobalManager.getStars(LevelLoaderScript.currLevelID) < CanvasBaseScript.instance.starCount)
            {
                GlobalManager.setStars(LevelLoaderScript.currLevelID, CanvasBaseScript.instance.starCount);
            }
 
            if (LevelBaseScript.CalculateState(LevelLoaderScript.currLevelID + 1) == LevelButtonScript.LevelState.Locked)
            {
                SceneManager.LoadScene("SelectScene"); //go back to menu
            }
            else
            {
                LevelLoaderScript.currLevelID++;
                SceneManager.LoadScene("LevelScene");
            }
        }
    }
}