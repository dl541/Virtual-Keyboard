using csDelaunay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Globalization;
using System;

public class VoronoiGeneration : MonoBehaviour {

    public GameObject customButtonPrefab;
    public Dictionary<string, Site> buttonSiteDictionary = new Dictionary<string, Site>();

    // The number of polygons/sites we want
    public int polygonNumber = 26;

    //Resolution of phone
    private Vector2 screenSize = new Vector2(1920f, 1020f);
    private float widthPortion = 1f;
    private float heightPortion = 0.75f;
    public Rectf bounds;


    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;
    private GameObject customButton;
    private List<GameObject> buttonList = new List<GameObject>();
    private Dictionary<string, Vector2f> points;

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

    // Parameters for Lloyd's relaxation
    private int numOfIterations = 15;
    //Path for printing logs
    private string path;
    private StreamWriter sw;

    public string SpaceBarName
    {
        get
        {
            return spaceBarName;
        }

    }

    void Awake()
    {
        bounds = new Rectf(0, 0, screenSize.x * widthPortion, screenSize.y * heightPortion);

        points = GeneratePointsFromFile();

        Voronoi voronoi = new Voronoi(points.Values.ToList(), bounds);

        sites = voronoi.SitesIndexedByLocation;
        Debug.Log("Number of sites " + sites.Count);

        foreach (KeyValuePair<string, Vector2f> entry in points)
        {
            buttonSiteDictionary[entry.Key] = sites[entry.Value];
        }
/**
        // Measure how far away the current positions are from the original positions
        var positionDiffList = new SortedList<float, KeyValuePair<string, Site>>();
        foreach (KeyValuePair<Vector2f, Site> entry in sites)
        {
            Vector2f sitePosition = entry.Key;
            foreach (KeyValuePair<string, Vector2f> record in points)
            {
                Debug.Log(record.Key);
                Vector2f originalPosition = record.Value;
                Vector2f difference = originalPosition - sitePosition;
                Debug.Log(difference);
                positionDiffList.Add(difference.magnitude, new KeyValuePair<string, Site>(record.Key, entry.Value));
            }
        }

        // Select combinations with the smallest difference
        foreach (var entry in positionDiffList)
        {
            var charSitePair = entry.Value;
            if (!buttonSiteDictionary.ContainsKey(charSitePair.Key) && !buttonSiteDictionary.ContainsValue(charSitePair.Value))
            {
                buttonSiteDictionary[charSitePair.Key] = charSitePair.Value;
            }
        }
    **/
    }

    void Start()
    {
        Debug.Log("Generate custom buttons");
        foreach (KeyValuePair<string, Site> entry in buttonSiteDictionary)
        {
           
            customButton = Instantiate(customButtonPrefab, transform);
            customButton.name = entry.Key;
            buttonList.Add(customButton);
        }
        path = string.Format("{0}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"));

    }

    private Dictionary<string,Vector2f> GeneratePointsFromFile()
    {
        Dictionary<string, Vector2f> points = new Dictionary<string, Vector2f>();
        string line;
        try
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(string.Format("{0}\\{1}",Environment.CurrentDirectory,"voronoi_keyboard_data.txt"));

            //Read the first line of text
            line = sr.ReadLine();

            //Continue to read until you reach end of file
            while (line != null)
            {
                string[] splitString = line.Split('\t');
                
                points[splitString[2]] = new Vector2f(float.Parse(splitString[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(splitString[1], CultureInfo.InvariantCulture.NumberFormat));

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

    public void CoordinateToButton(Vector2 coord, ButtonState buttonState)
    {
        GameObject closestButton = ClosestMeanSearcher(coord);
        closestButton.GetComponent<InitializeCollider>().buttonState = buttonState;

        
    }

    private void UpdateMeanPosition(string key, Vector2f coord)
    {
        float adaptiveRate = 0.01f;
        float deltaX = (coord.x - points[key].x) * adaptiveRate;
        float deltaY = (coord.y - points[key].y) * adaptiveRate;
        points[key] = points[key] + new Vector2f(deltaX, deltaY);

        Debug.Log(string.Format("Key: {0}, Position: {1}", key, points[key]));
    }

    private GameObject ClosestMeanSearcher(Vector2 coord)
    {
        GameObject closestButton = buttonList[0];
        double closestDistance = double.MaxValue;

        foreach (GameObject button in buttonList)
        {
            Vector2f meanPos = button.GetComponent<MeshGenerator>().buttonSite.Coord;
            double norm = Math.Pow(meanPos.x - coord.x, 2) + Math.Pow(meanPos.y - coord.y, 2);
            if (norm < closestDistance)
            {
                closestButton = button;
                closestDistance = norm;
            }

        }

        return closestButton;
    }

    public void PrintTextToFile(string text)
    {
        if (sw == null)
        {
            Debug.Log(String.Format("Printing logs to {0}", path));
        }
        sw = new StreamWriter(path, true);
        Debug.Log("Print logs to file");
        sw.WriteLine(text);
        sw.Close();
    }

    public void closeStreamWriter()
    {
        if (sw != null)
        {
            Debug.Log("Close streamswriter");
            sw.Close();
        }
    }
}