using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleStarScript : MonoBehaviour
{
    private TMPro.TMP_Text textasset;
    private TMPro.TMP_Text textasset2; //to be implemented later
    private Image starImage;
    private int num1;
    private int num2;

    // Start is called before the first frame update
    void Awake()
    {
        textasset = GetComponentInChildren<TMPro.TMP_Text>();
        starImage = GetComponentInChildren<Image>();
    }

    public void SetNumber(int number) //only one number
    {
        textasset.text = number+"";
    }

    public void SetNumber(int number1, int number2)
    {
        //not implemented
        throw new NotImplementedException();
    }

    public void SetColor( Color color )
    {
        starImage.color = color;
    }

    public void SetColor(Color color, Color color2)
    {
        //not implemented
        throw new NotImplementedException();
    }

    public void SetEnabled(bool input)
    {
        textasset.enabled = input;
        starImage.enabled = input;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
