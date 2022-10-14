using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtonScript : MonoBehaviour
{
    public int levelID;
    private int stars;
    private SpriteRenderer sprite;
    public TMPro.TMP_Text textAsset;
    public TMPro.TMP_Text starAsset;
    public LevelState state { get; private set; }

    public Color completeColor;
    public Color unlockedColor;
    public Color lockedColor;

    public enum LevelState
    {
        Complete,
        Unlocked,
        Locked
    }

    //unlocked level formula: (last complete + (average star count of everything up to last complete) - 0.5), min of 1
    //avg of 1.5 to get to next level

    private void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();

        if (GlobalManager.stars == null)
        {
            //failsafe
            stars = 1;
            state = LevelState.Unlocked;
            return;
        }
        if (levelID > GlobalManager.stars.Count)
        {
            stars = 0; //no data
        } else
        {
            stars = GlobalManager.getStars(levelID);
        }

        UpdateGraphics();
    }

    public void UpdateGraphics()
    {
        textAsset.text = "" + levelID;
        starAsset.text = stars + "";
        switch (state)
        {
            case LevelState.Complete:
                sprite.color = completeColor;
                break;
            case LevelState.Unlocked:
                sprite.color = unlockedColor;
                break;
            case LevelState.Locked:
                sprite.color = lockedColor;
                break;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //click
            //checkState = CheckState.Inactive; //debug
            OnClick();
        }
    }

    public void SetState(LevelState p_state)
    {
        state = p_state;
    }

    private void OnClick()
    {
        if (state != LevelState.Locked)
        {
            LevelLoaderScript.currLevelID = levelID;
            SceneManager.LoadScene("LevelScene");
        }
    }
}
