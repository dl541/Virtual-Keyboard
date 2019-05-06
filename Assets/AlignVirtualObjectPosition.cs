using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class AlignVirtualObjectPosition : MonoBehaviour
{
    public OptitrackStreamingClient streamingClient;
    public int rigidBodyId;
    private string path;
    private StreamWriter sw;

    // Start is called before the first frame update
    void Start()
    {
        path = string.Format("{0}_rigidBody{1}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"), rigidBodyId);

        OptitrackRigidBodyState rigidBodyState = streamingClient.GetLatestRigidBodyState(rigidBodyId);

        if (rigidBodyState != null)

        {
            Debug.Log(string.Format("Align position of rigid body with id {0}", rigidBodyId));
            gameObject.transform.SetPositionAndRotation(rigidBodyState.Pose.Position, rigidBodyState.Pose.Orientation);


            sw = new StreamWriter(path, true);
            Debug.Log("Print rigid body position to file");
            sw.WriteLine(string.Format("{0}\t{1}", rigidBodyState.Pose.Position, rigidBodyState.Pose.Orientation));
            sw.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        OptitrackRigidBodyState rigidBodyState = streamingClient.GetLatestRigidBodyState(rigidBodyId);

        if (rigidBodyState != null)

        {
            Debug.Log(string.Format("Align position of rigid body with id {0}", rigidBodyId));
            gameObject.transform.SetPositionAndRotation(rigidBodyState.Pose.Position, rigidBodyState.Pose.Orientation);


            //sw = new StreamWriter(path, true);
            //Debug.Log("Print rigid body position to file");
            //sw.WriteLine(string.Format("{0}\t{1}", rigidBodyState.Pose.Position, rigidBodyState.Pose.Orientation));
            //sw.Close();
        }
    }
}
