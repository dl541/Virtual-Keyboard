﻿using csDelaunay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

public class VoronoiGeneration : MonoBehaviour {

    public GameObject customButtonPrefab;
    public Dictionary<string, Site> buttonSiteDictionary = new Dictionary<string, Site>();

    // The number of polygons/sites we want
    public int polygonNumber = 26;

    //Resolution of phone
    private Vector2 screenSize = new Vector2(1920f, 1020f);
    private float width = 1920f;
    private float height = 1080f;
    public Rectf bounds;


    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;
    private GameObject customButton;

    // Parameters for keyboard
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


    void Awake()
    {
        bounds = new Rectf(0, 0, width, height);

        List<Vector2f> points = GeneratePointsFromFile();

        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points, bounds);

        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);

        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        Debug.Log("Number of sites " + sites.Count);
        edges = voronoi.Edges;


        int entryInd = 0;
        foreach (KeyValuePair<Vector2f, Site> entry in sites)
        {
            buttonSiteDictionary.Add(entryInd.ToString(), entry.Value);
            entryInd += 1;
        }

        DisplayVoronoiDiagram();
    }

    void Start()
    {
        Debug.Log("Generate custom buttons");
        foreach (KeyValuePair<string, Site> entry in buttonSiteDictionary)
        {
            customButton = Instantiate(customButtonPrefab, transform);
            customButton.name = entry.Key;
            
        }
    }

    private List<Vector2f> GeneratePointsFromFile()
    {
        List<Vector2f> points = new List<Vector2f>();
        string line;
        try
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader("D:\\Unity3D\\Virtual keyboard\\keyboard_data.txt");

            //Read the first line of text
            line = sr.ReadLine();

            //Continue to read until you reach end of file
            while (line != null)
            {
                string[] splitString = line.Split('\t');
                points.Add(new Vector2f(float.Parse(splitString[0], CultureInfo.InvariantCulture.NumberFormat), screenSize.y - float.Parse(splitString[1], CultureInfo.InvariantCulture.NumberFormat)));

                //Read the next line
                line = sr.ReadLine();
            }

            //close the file
            sr.Close();
            Console.ReadLine();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            Console.WriteLine("Executing finally block.");
        }
        return points;
    }


    private List<Vector2f> CreateRandomPoint()
    {
        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++)
        {
            points.Add(new Vector2f(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height)));
        }

        return points;
    }

    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram()
    {
        Texture2D tx = new Texture2D(512, 512);
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        this.GetComponent<Renderer>().material.mainTexture = tx;
    }

    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}