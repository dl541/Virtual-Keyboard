using csDelaunay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    float width = 100;
    float height = 100;
    bool rendered = false;


	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update()
    {
        if (rendered == false)
        {
            Site buttonSite = GetComponentInParent<VoronoiGeneration>().buttonSiteDictionary[name];
            Rectf bounds = GetComponentInParent<VoronoiGeneration>().bounds;

            Mesh mesh = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            foreach (Vector2f vector2f in buttonSite.Region(bounds))
            {
                Vector3 vertex = new Vector3(vector2f.x, vector2f.y, 0f);
                vertices.Add(vertex);
                Debug.Log("Vertex " + vertex);
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

            rendered = true;
        }
    }
}
