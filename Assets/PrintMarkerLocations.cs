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
    public GameObject boundingBox;
    public int logBufferSize = 2500000;
    private string[] logBuffer;

    // Start is called before the first frame update
    void Start()
    {
        path = string.Format("{0}_markers.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
        logBuffer = new string[logBufferSize];
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        List<OptitrackMarkerState> markerStates = streamingClient.GetLatestMarkerStates();

        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
        foreach (OptitrackMarkerState markerState in markerStates)
        {
            if (boundingBox.GetComponent<Collider>().bounds.Contains(markerState.Position))
            {
                String log = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", timeStamp,
                    logIndex, markerState.Id, markerState.Position.x, markerState.Position.y, markerState.Position.z);

                WriteToBuffer(log);
                logIndex += 1;
            }
        }
    }

    public void WriteToBuffer(string text)
    {
        var thread = new Thread(() => WriteOnSeparateThread(logIndex, text));
        thread.Start();
    }

    private void WriteOnSeparateThread(int logIndex, string logMessage)
    {
        logBuffer[logIndex] = logMessage;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application ended.");
        ExportToFile();
    }

    private void ExportToFile()
    {
        using (TextWriter tw = new StreamWriter(path))
        {
            foreach (String s in logBuffer)
                tw.WriteLine(s);
        }
    }

}
