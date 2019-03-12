using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Threading;

public class PrintMarkerLocations : MonoBehaviour
{

    //Path for printing logs
    private string path;
    private StreamWriter sw;
    public OptitrackStreamingClient streamingClient;
    private int logIndex = 0;
    private string line_;
    public GameObject boundingBox;

    // Start is called before the first frame update
    void Start()
    {
        path = string.Format("{0}_markers.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        List<OptitrackMarkerState> markerStates = streamingClient.GetLatestMarkerStates();

        foreach (OptitrackMarkerState markerState in markerStates)
        {
            if (boundingBox.GetComponent<Collider>().bounds.Contains(markerState.Position))
            {
                String log = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                    logIndex, markerState.Id, markerState.Position.x, markerState.Position.y, markerState.Position.z);

                PrintTextToFile(log);
                logIndex += 1;
            }
        }
    }

    public void PrintTextToFile(string text)
    {
        line_ = text;

        var thread = new Thread(WriteOnSeparateThread);
        thread.Start();
    }

    private void WriteOnSeparateThread()
    {
        File.AppendAllText(path, line_);
    }
}
