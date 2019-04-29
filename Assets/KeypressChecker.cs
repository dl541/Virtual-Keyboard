using System.Collections;
using System.Collections.Generic;
using System.IO;
using TensorFlow;
using UnityEngine;

public class KeypressChecker : MonoBehaviour {

    private TFGraph leftNNGraph;
    private TFGraph rightNNGraph;
    private TFSession leftNNSession;
    private TFSession rightNNSession;
    public GameObject topLeftMarker;
    public GameObject topRightMarker;
    public GameObject sideLeftMarker;

	// Use this for initialization
	void Start () {
        leftNNGraph = new TFGraph();
        leftNNGraph.Import(File.ReadAllBytes("Assets/tf_NN_focal_clf_left.pb"));
        leftNNSession = new TFSession(leftNNGraph);

        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i<100; i++)
        {
            //TestTransform();

            var testVector = GetTestVector();
            Debug.Log(IsLeftKeypress(testVector));
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        Debug.Log(string.Format("Time taken for 10 iterations: {0}", elapsedMs));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsLeftKeypress(float[][] features)
    {
        var runner = leftNNSession.GetRunner();
        var tensor = new TFTensor(features);

        runner.AddInput(leftNNGraph["dense_22_input"][0], tensor);
        runner.Fetch(leftNNGraph["dense_23/Sigmoid"][0]);

        var output = runner.Run();
        var result = output[0];
        var prob = (float[,])result.GetValue(jagged: false);
        // Fetch the results from output:
        Debug.Log(string.Format("Left Hand State: {0}", prob[0, 0]));

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
