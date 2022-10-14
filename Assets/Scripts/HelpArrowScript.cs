using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpArrowScript : MonoBehaviour, MouseTarget
{
    public int pageDiff;
    public Color onColor;
    public Color offColor;

    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (HelpBoxBaseScript.instance.IsPossible(pageDiff))
        {
            sprite.color = onColor;
        }
        else
        {
            sprite.color = offColor;
        }
    }

    public void OnClick()
    {
        HelpBoxBaseScript.instance.TurnPage(pageDiff);
    }
}
