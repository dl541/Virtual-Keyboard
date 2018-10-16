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

        string[] firstRow = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
        instantiateRow(firstRow, firstPosInRow + upperCorner);
        firstPosInRow += new Vector3(buttonSize / 2, -buttonSize - buttonSpacing, 0f); 

        string[] secondRow = { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
        instantiateRow(secondRow, firstPosInRow + upperCorner);
        firstPosInRow += new Vector3(buttonSize / 2, -buttonSize - buttonSpacing, 0f);

        string[] thirdRow = { "Z", "X", "C", "V", "B", "N", "M" };
        instantiateRow(thirdRow, firstPosInRow + upperCorner);
    }

    void instantiateRow(string[] row, Vector3 pos)
    {
        foreach (string character in row)
        {
            instantiateKey(character, pos);
            pos.x += buttonSize + buttonSpacing;
        }
    }

    void instantiateKey(string character, Vector3 pos)
    {
        GameObject newButton = Instantiate(buttonPrefab) as GameObject;
        newButton.name = string.Format("Button {0}", character);
        newButton.transform.SetParent(keyboardBase.transform, false);

        newButton.transform.localPosition = pos;

        GameObject text = newButton.transform.Find("TextMeshPro Text").gameObject;
        TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(character);
    }
}
