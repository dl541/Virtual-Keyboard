using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TensorFlow;
using UnityEngine;

// TODO: working on gettestvectorsandtimestamp, need to test for generatefeatures and thumbstaterecorder
public class KeypressChecker : MonoBehaviour {

    private TFGraph leftNNGraph;
    private TFGraph rightNNGraph;
    private TFSession leftNNSession;
    private TFSession rightNNSession;
    public GameObject topLeftMarker;
    public GameObject topRightMarker;
    public GameObject sideLeftMarker;
    private ThumbStateRecorder leftThumbRecorder = new ThumbStateRecorder();
    private ThumbStateRecorder rightThumbRecorder = new ThumbStateRecorder();

	// Use this for initialization
	void Start () {
        leftNNGraph = new TFGraph();
        leftNNGraph.Import(File.ReadAllBytes("Assets/tf_NN_focal_clf_left.pb"));
        leftNNSession = new TFSession(leftNNGraph);

        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i<100; i++)
        {
            TestTransform();

            var testVector = GetTestVector();
            Debug.Log(RunNN(testVector, leftNNSession));
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        Debug.Log(string.Format("Time taken for 10 iterations: {0}", elapsedMs));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool? IsRightKeypress(Vector3 tipPos, Vector3 midPos, Vector3 endPos, DateTime timestamp)
    {
        float[][] features = GenerateFeatures(tipPos, midPos, endPos, timestamp, rightThumbRecorder);

        if (features == null)
        {
            return null;
        }
        return RunNN(features, rightNNSession);
    }

    public bool? IsLeftKeypress(Vector3 tipPos, Vector3 midPos, Vector3 endPos, DateTime timestamp)
    {
        float[][] features = GenerateFeatures(tipPos, midPos, endPos, timestamp, leftThumbRecorder);

        if (features == null)
        {
            return null;
        }
        return RunNN(features, leftNNSession);
    }

    private float[][] GenerateFeatures(Vector3 tipPos, Vector3 midPos, Vector3 endPos, DateTime timestamp, ThumbStateRecorder thumbStateRecorder)
    {
        Vector3[] velocities = thumbStateRecorder.FindVelocityAndUpdate(tipPos, midPos, endPos, timestamp);

        if (velocities == null||velocities.Length == 0)
        {
            return null;
        }
        Vector3[] positions = new Vector3[] { tipPos, midPos, endPos };

        var features = new float[18];
        int insertIndex = 0;

        // Flatten arrays
        foreach(Vector3 vector in positions)
        {
            features[insertIndex] = vector.x;
            features[insertIndex+1] = vector.y;
            features[insertIndex + 2] = vector.z;
            insertIndex += 3;
        }

        foreach (Vector3 vector in velocities)
        {
            features[insertIndex] = vector.x;
            features[insertIndex + 1] = vector.y;
            features[insertIndex + 2] = vector.z;
            insertIndex += 3;
        }

        return new float[][] { features };
    }

    private bool RunNN(float[][] features, TFSession NNSession)
    {
        var runner = NNSession.GetRunner();
        var tensor = new TFTensor(features);

        runner.AddInput(leftNNGraph["dense_22_input"][0], tensor);
        runner.Fetch(leftNNGraph["dense_23/Sigmoid"][0]);

        var output = runner.Run();
        var result = output[0];
        var prob = (float[,])result.GetValue(jagged: false);
        // Fetch the results from output:
        Debug.Log(string.Format("Hand State: {0}", prob[0, 0]));

        return prob[0,0] >= 0.5; 
    }
    private float[][] GenerateRandomFeatures()
    {

        var rand = new System.Random();
        var rtnlist = new float[18];

        for (int i = 0; i < 18; i++)
        {
            rtnlist[i] = rand.Next()/10;
        }
        return new float[][] { rtnlist };
    }

    private float[][] GetTestVector()
    {
        var testVector = new float[] { 0.036426f, 0.019354f, -0.023583f, 0.041278f, 0.053251f,
            -0.029241f, 0.041664f, 0.083025f, -0.045438f, 0.0f, 0.0f, 0.0f, 0.0f,
            0.00000f,    0.000000f,    0.000000f,   0.000000f,   0.000000f };

        return new float[][] { testVector };
    }

    private (Vector3[], DateTime[]) GetTestPosVectorsAndTimestamps()
    {
        var vector1 = new Vector3(-0.07221564f, 0.7727299f, 0.5003643f);
        var vector2 = new Vector3(-0.07268887f, 0.773079f, 0.5000541f);
        var timeStamp1 = new DateTime(2019, 3, 13, 15, 50, 21, 940);
        var timeStamp2 = new DateTime(2019, 3, 13, 15, 50, 21, 941);

        return (new Vector3[] { vector1, vector2 }, new DateTime[] { timeStamp1, timeStamp2 });
    }
    private void TestTransform()
    {
        var testPos = new Vector3(-0.07221564f, 0.7727299f, 0.5003643f);

        var testTransformMatrix = GetTestTransformMatrix();
        Debug.Log(testTransformMatrix);
        Debug.Log(testTransformMatrix.MultiplyPoint3x4(testPos).ToString("F7"));
    }

    private Matrix4x4 GetTestTransformMatrix()
    {
        var topLeftPos = new Vector3(-0.1130678f, 0.7825412f, 0.5232269f);
        var topRightPos = new Vector3(5.866859e-04f, 7.746999e-01f, 5.192038e-01f);
        var sideLeftPos = new Vector3(-0.1385423f, 0.7663757f, 0.5191758f);

        return FindTransformMatrix(topLeftPos, topRightPos, sideLeftPos);
    }

    private Matrix4x4 FindTransformMatrix(Vector3 topLeftPos, Vector3 topRightPos, Vector3 sideLeftPos)
    {
        var invTransformMatrix = Matrix4x4.identity;

        // x vector points from top left to top right
        // y vector points out of the plane
        // z vector points from top left to bottom left
        Vector3 xColVector = (topRightPos - topLeftPos).normalized;
        Vector3 yColVector = Vector3.Cross(xColVector, sideLeftPos - topLeftPos).normalized;
        Vector3 zColVector = Vector3.Cross(xColVector, yColVector).normalized;

        invTransformMatrix.SetColumn(0, xColVector);
        invTransformMatrix.SetColumn(1, yColVector);
        invTransformMatrix.SetColumn(2, zColVector);

        Matrix4x4 transformMatrix = invTransformMatrix.inverse;
        // Top left marker is the new origin
        Vector3 translationVector = -topLeftPos;
        translationVector = transformMatrix.MultiplyPoint3x4(translationVector);
        Debug.Log(string.Format("translation:{0}",translationVector));
        Vector4 translationVector4 = translationVector;
        translationVector4.w = 1.0f;
        transformMatrix.SetColumn(3, translationVector);

        return transformMatrix;
    }
}

public class ThumbStateRecorder
{
    private DateTime? prevTimestamp = null;
    private Vector3 prevTipPos;
    private Vector3 prevMidPos;
    private Vector3 prevEndPos;

    public Vector3[] FindVelocityAndUpdate(Vector3 tipPos, Vector3 midPos, Vector3 endPos, DateTime currentTimestamp)
    {


        var velocities = new Vector3[] { };
        if (prevTimestamp!= null)
        {
            double timeDiff = (currentTimestamp - prevTimestamp.Value).TotalSeconds;
            Vector3 tipVelocity = (tipPos - prevTipPos) / (float)timeDiff;
            Vector3 midVelocity = (midPos - prevMidPos) / (float)timeDiff;
            Vector3 endVelocity = (endPos - prevEndPos) / (float)timeDiff;
            velocities = new Vector3[]{ tipVelocity, midVelocity, endVelocity};
        }

        prevTimestamp = currentTimestamp;
        prevTipPos = tipPos;
        prevMidPos = midPos;
        prevEndPos = endPos;

        return velocities;
    }
}