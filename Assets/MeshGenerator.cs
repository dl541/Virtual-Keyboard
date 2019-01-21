using csDelaunay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeshGenerator : MonoBehaviour {
    public GameObject buttonTextPrefab;
    public GameObject buttonText;
    public Site buttonSite;
    float width = 100;
    float height = 100;
    public bool rendered = false;

    private float maxColorValue = 0.9f;
    private float minColorValue = 0.5f;


	// Use this for initialization
	void Start () {
        setColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (rendered == false)
        {
            buttonSite = GetComponentInParent<VoronoiGeneration>().buttonSiteDictionary[name];
            RenderMesh();
            attachText();
            rendered = true;
        }

        updateColor();
    }

    private void RenderMesh()
    {
        Rectf bounds = GetComponentInParent<VoronoiGeneration>().bounds;

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        foreach (Vector2f vector2f in buttonSite.Region(bounds))
        {
            Vector3 vertex = new Vector3(vector2f.x, vector2f.y, 0f);
            vertices.Add(vertex);
        }
        vertices.Reverse();

        mesh.vertices = vertices.ToArray();

        List<int> triangles = new List<int>();
        for (int ind = 1; ind < mesh.vertices.Length - 1; ind++)
        {
            triangles.Add(0);
            triangles.Add(ind);
            triangles.Add(ind + 1);
        }

        mesh.triangles = triangles.ToArray();

        gameObject.GetComponent<MeshFilter>().mesh = mesh;


    }

    private void attachText()
    {
        buttonText = Instantiate(buttonTextPrefab) as GameObject;
        buttonText.transform.SetParent(gameObject.transform, false);
        buttonText.transform.localPosition = new Vector3(buttonSite.x, buttonSite.y, -0.01f);
        buttonText.transform.localRotation = Quaternion.identity;
        buttonText.GetComponentInChildren<TextMeshProUGUI>().text = name;
    }

    private void setColor()
    {

        int ascii = gameObject.name.ToCharArray()[0];
        float hue = (ascii - 97) / 27f;
        Color randomColor = Color.HSVToRGB(Random.value, Random.value, 0.9f);
        gameObject.GetComponent<Renderer>().material.color = randomColor;

    }

    private void updateColor()
    {
        if (transform.position.z != 0f)
        {
            float newValue = maxColorValue - transform.position.z / SpringAnimation.maxDepth * (maxColorValue - minColorValue);
            float H, S, V;
            Color.RGBToHSV(gameObject.GetComponent<Renderer>().material.color, out H, out S, out V);
            gameObject.GetComponent<Renderer>().material.color = Color.HSVToRGB(H, S, newValue);
        }
    }
}
