using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerateKeyboard : MonoBehaviour
{
    public GameObject keyboardBase;
    public GameObject buttonPrefab;
    private int numOfCols = 10;
    private int numOfRows = 3;
    private float buttonSize;
    private float buttonSpacing;
    private int spaceBarLengthInButton = 5;
    private string spaceBarName = "Space";

    // Use this for initialization
    void Start()
    {
        buttonSize = buttonPrefab.GetComponent<RectTransform>().sizeDelta.x;
        buttonSpacing = buttonSize / 10;
        keyboardBase.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonSize * numOfCols, buttonSize * numOfRows);
        generateKeys();
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

        string[] numberRow = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        instantiateRow(numberRow, firstPosInRow + upperCorner);
        firstPosInRow += new Vector3(0f, -buttonSize - buttonSpacing, 0f);


        string[] firstRow = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
        instantiateRow(firstRow, firstPosInRow + upperCorner);
        firstPosInRow += new Vector3(0f, -buttonSize - buttonSpacing, 0f);
        firstPosInRow.x = buttonSize / 2;

        string[] secondRow = { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
        instantiateRow(secondRow, firstPosInRow + upperCorner);
        firstPosInRow += new Vector3(0f, -buttonSize - buttonSpacing, 0f);
        firstPosInRow.x = 0;

        string[] thirdRow = { "Shift", "Z", "X", "C", "V", "B", "N", "M", "<-" };
        instantiateRow(thirdRow, firstPosInRow + upperCorner);

        firstPosInRow += new Vector3(0f, -buttonSize - buttonSpacing, 0f);
        firstPosInRow.x = 0;

        string[] fourthRow = { "123", ",", spaceBarName, ".", "Enter" };
        instantiateRow(fourthRow, firstPosInRow + upperCorner);
    }

    void instantiateRow(string[] row, Vector3 pos)
    {
        foreach (string character in row)
        {
            if (character == spaceBarName)
            {
                instantiateKey(character, pos, spaceBarLengthInButton);
                pos.x += buttonSize * spaceBarLengthInButton + buttonSpacing;
            }
            else
            {
                instantiateKey(character, pos);
                pos.x += buttonSize + buttonSpacing;
            }
        }
    }

    void instantiateKey(string character, Vector3 pos, int buttonWidth = 1)
    {
        GameObject newButton = Instantiate(buttonPrefab) as GameObject;
        Vector2 sizeDelta = newButton.GetComponent<RectTransform>().sizeDelta;
        newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x * buttonWidth, sizeDelta.y);
        newButton.name = string.Format("Button {0}", character);
        newButton.transform.SetParent(keyboardBase.transform, false);

        newButton.transform.localPosition = pos;

        GameObject text = newButton.transform.Find("TextMeshPro Text").gameObject;
        TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
        textMesh.fontSize = 36f;
        textMesh.SetText(character);
    }
}
