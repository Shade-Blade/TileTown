using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpBoxBaseScript : MonoBehaviour
{
    public int currentPage;
    public string[] helpText;

    public bool helpOpen;

    public TMPro.TMP_Text textAsset;

    public GameObject closedObject;
    public GameObject openObject;

    public static HelpBoxBaseScript instance;

    private HelpOpenerScript opener;

    // Start is called before the first frame update
    void Start()
    {
        currentPage = 0;
        helpOpen = false;
        textAsset = GetComponentInChildren<TMPro.TMP_Text>();
        opener = FindObjectOfType<HelpOpenerScript>();

        UpdateGraphics();

        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetText(string[] p_helpText)
    {
        helpText = p_helpText;
        helpOpen = true; //might be reverted back later (this makes help text automatically open if possible)
        UpdateGraphics();
    }

    void UpdateGraphics()
    {
        if (helpText.Length == 0)
        {
            closedObject.SetActive(false);
            openObject.SetActive(false);
            helpOpen = false;
            return;
        }

        if (helpOpen)
        {
            openObject.SetActive(true);
            closedObject.SetActive(false);
        } else
        {
            openObject.SetActive(false);
            closedObject.SetActive(true);
        }

        opener.gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * (helpOpen ? 180 : -90));

        textAsset.text = helpText[currentPage];
    }

    public void SetOpen(bool p_helpOpen)
    {
        helpOpen = p_helpOpen && helpText.Length != 0; //make sure there is text to show
        UpdateGraphics();
    }

    public void TurnPage(int diff)
    {
        currentPage += diff;
        if (currentPage < 0)
        {
            currentPage = 0;
        }
        if (currentPage >= helpText.Length)
        {
            currentPage = helpText.Length-1;
        }

        textAsset.text = helpText[currentPage];

        UpdateGraphics();
    }

    public bool IsPossible(int diff)
    {
        return (currentPage + diff < helpText.Length && currentPage + diff > -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}