using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PrintMarkerLocations : MonoBehaviour
{

    //Path for printing logs
    private string path;
    private StreamWriter sw;
    public OptitrackStreamingClient streamingClient;
    private int logIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        path = string.Format("{0}_markers.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
    }

    // Update is called once per frame
    void Update()
    {

        List<OptitrackMarkerState> markerStates = streamingClient.GetLatestMarkerStates();

        foreach (OptitrackMarkerState markerState in markerStates)
        {
            String log = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"), 
                logIndex, markerState.Id, markerState.Position.x, markerState.Position.y, markerState.Position.z);

            PrintTextToFile(log);
        }
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

        logIndex += 1;
    }
}
