using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerateKeyboard : MonoBehaviour
{
    public GameObject keyboardBase;
    public GameObject buttonPrefab;
    private int numOfCols = 10;
    private int numOfRows = 5;
    private float buttonSizeX;
    private float buttonSizeY;
    private float buttonSpacingX;
    private float buttonSpacingY;
    private float spaceBarLengthInButton = 5f;
    private string spaceBarName = " ";
    private float horizontalMargin;
    private float verticalMargin;
    private List<GameObject> buttonList = new List<GameObject>();
    public Dictionary<string, GameObject> nameKeyMap = new Dictionary<string, GameObject>();
    public Dictionary<float, string[]> posToRowMap = new Dictionary<float, string[]>();

    //Resolution of phone
    private Vector2 screenSize = new Vector2(1920f, 1020f);

    // Use this for initialization
    void Start()
    {

        buttonSizeX = screenSize.x / 10;
        buttonSizeY = screenSize.y * 0.23f;
        buttonSpacingX = 0;
        buttonSpacingY = 0;
        horizontalMargin = buttonSizeX / 5;
        verticalMargin = buttonSizeY / 5;
        keyboardBase.GetComponent<RectTransform>().sizeDelta = screenSize;
        generateKeys();

        foreach (GameObject button in buttonList)
        {
            Debug.Log(string.Format("button {0}", button.name));
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void generateKeys()
    {
        Vector3 parentSize = keyboardBase.GetComponent<RectTransform>().sizeDelta;
        Vector3 upperCorner = new Vector3(-parentSize.x / 2, parentSize.y / 2, 0f);
        Vector3 firstPosInRow = Vector3.zero;

        // string[] numberRow = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        // instantiateRow(numberRow, firstPosInRow + upperCorner);
        // firstPosInRow += new Vector3(0f, -buttonSizeY - buttonSpacingY, 0f);


        string[] firstRow = { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p" };
        instantiateRow(firstRow, firstPosInRow + upperCorner);
        firstPosInRow += new Vector3(0f, -buttonSizeY - buttonSpacingY, 0f);
        firstPosInRow.x = screenSize.x * 0.05f;
        UpdateRowMap(firstRow);

        string[] secondRow = { "a", "s", "d", "f", "g", "h", "j", "k", "l" };
        instantiateRow(secondRow, firstPosInRow + upperCorner);
        firstPosInRow += new Vector3(0f, -buttonSizeY - buttonSpacingY, 0f);
        firstPosInRow.x = 0;
        UpdateRowMap(secondRow);

        string[] thirdRow = { "Shift", "z", "x", "c", "v", "b", "n", "m", "<-" };
        instantiateRow(thirdRow, firstPosInRow + upperCorner);
        firstPosInRow += new Vector3(0f, -buttonSizeY - buttonSpacingY, 0f);
        firstPosInRow.x = screenSize.x * 0.25f;
        UpdateRowMap(thirdRow);

        string[] fourthRow = { spaceBarName };
        instantiateRow(fourthRow, firstPosInRow + upperCorner);
        UpdateRowMap(fourthRow);
    }

    void UpdateRowMap(string[] row)
    {
        GameObject firstItem = nameKeyMap[row[0]];
        posToRowMap.Add(firstItem.GetComponent<RectTransform>().anchoredPosition.y, row);
    }

    void instantiateRow(string[] row, Vector3 pos)
    {
        foreach (string character in row)
        {
            if (character == spaceBarName)
            {
                instantiateKey(character, pos, spaceBarLengthInButton);
                pos.x += buttonSizeX * spaceBarLengthInButton + buttonSpacingX;
            }

            else if (character == "Shift" || character == "<-")
            {
                instantiateKey(character, pos, 1.5f);
                pos.x += buttonSizeX * 1.5f + buttonSpacingX;
            }

            else
            {
                instantiateKey(character, pos);
                pos.x += buttonSizeX + buttonSpacingX;
            }
        }
    }

    void instantiateKey(string character, Vector3 pos, float buttonWidth = 1f)
    {
        GameObject newButton = Instantiate(buttonPrefab) as GameObject;
        newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonSizeX * buttonWidth, buttonSizeY);
        newButton.name = string.Format("{0}", character);
        newButton.transform.SetParent(keyboardBase.transform, false);

        newButton.transform.localPosition = pos;

        GameObject text = newButton.transform.Find("TextMeshPro Text").gameObject;
        TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
        textMesh.fontSize = 48f;
        textMesh.SetText(character);

        nameKeyMap.Add(newButton.name, newButton);
        buttonList.Add(newButton);
    }


    public void CoordinateToButton(Vector2 coord, ButtonState buttonState)
    {
        ButtonGridSearcher(coord).GetComponent<InitializeCollider>().buttonState = buttonState;
    }

    private GameObject ButtonGridSearcher(Vector2 coord)
    {
        //introduce offset since the button anchor positions are based on their upper left corners
        coord = coord + new Vector2(-buttonSizeX / 2, buttonSizeY / 2);
        Debug.Log("Corrected coord " + coord);

        float closestYCoord = float.MaxValue;
        foreach (float yCoord in posToRowMap.Keys)
        {
            Debug.Log("Key " + yCoord);
            if (Math.Abs(coord.y - yCoord) < Math.Abs(coord.y - closestYCoord))
            {
                closestYCoord = yCoord;
            }
        }

        GameObject closestGameObject = nameKeyMap[posToRowMap[closestYCoord][0]];
        foreach (string buttonName in posToRowMap[closestYCoord])
        {
            GameObject button = nameKeyMap[buttonName];
            if (Math.Abs(coord.x - button.GetComponent<RectTransform>().anchoredPosition.x) < 
                Math.Abs(coord.x - closestGameObject.GetComponent<RectTransform>().anchoredPosition.x))
            {
                closestGameObject = button;
            }
        }

        Debug.Log(string.Format("Closest object {0}", closestGameObject.name));
        return closestGameObject;
    }
}
