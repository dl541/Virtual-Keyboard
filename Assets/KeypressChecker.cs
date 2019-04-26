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

	// Use this for initialization
	void Start () {
        leftNNGraph = new TFGraph();
        leftNNGraph.Import(File.ReadAllBytes("Assets/tf_NN_focal_clf_left.pb"));
        leftNNSession = new TFSession(leftNNGraph);
        
        for (int i = 0; i<10; i++)
        {
            Debug.Log(IsLeftKeypress(GetTestVector()));
        }
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
}
