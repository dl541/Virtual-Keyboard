using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerateKeyboard : MonoBehaviour
{
    public GameObject keyboardBase;
    public GameObject buttonPrefab;

    // Use this for initialization
    void Start()
    {
        GenerateKeys();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateKeys()
    {
        string[] firstRow = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
        instantiateRow(firstRow, Vector3.zero);

        string[] secondRow = { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
        instantiateRow(secondRow, new Vector3(25f, -50f, 0f));

        string[] thirdRow = { "Z", "X", "C", "V", "B", "N", "M" };
        instantiateRow(thirdRow, new Vector3(25f, -100f, 0f));
    }

    void instantiateRow(string[] row, Vector3 pos)
    {
        foreach (string character in row)
        {
            instantiateKey(character, pos);
            pos.x += 50;
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
