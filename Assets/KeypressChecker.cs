using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TensorFlow;
using UnityEngine;



public class KeypressChecker : MonoBehaviour
{

    enum Hand { LEFT, RIGHT }
    private GameObject keyboardBase;
    private Matrix4x4 keyboardBaseTransform;
    private TFGraph leftNNGraph;
    private TFGraph rightNNGraph;
    private TFSession leftNNSession;
    private TFSession rightNNSession;
    public GameObject leftTipMarker;
    public GameObject leftMidMarker;
    public GameObject leftEndMarker;
    public GameObject rightTipMarker;
    public GameObject rightMidMarker;
    public GameObject rightEndMarker;
    private ThumbStateRecorder leftThumbRecorder = new ThumbStateRecorder();
    private ThumbStateRecorder rightThumbRecorder = new ThumbStateRecorder();
    private string[] NNResultLogs;
    private int logIndex = 0;
    private string path = string.Format("{0}_NN_output.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
    public bool enableNNLogs = false;

    // Use this for initialization
    void Start()
    {

        if (enableNNLogs)
        {
            NNResultLogs = new string[250000];
        }

        keyboardBase = GameObject.Find("KeyboardBase");
        InitialiseTransformMatrix();

        leftNNGraph = new TFGraph();
        leftNNGraph.Import(File.ReadAllBytes("Assets/tf_NN_focal_clf_left.pb"));
        leftNNSession = new TFSession(leftNNGraph);

        rightNNGraph = new TFGraph();
        rightNNGraph.Import(File.ReadAllBytes("Assets/tf_NN_focal_clf_right.pb"));
        rightNNSession = new TFSession(rightNNGraph);
    }

    void InitialiseTransformMatrix()
    {
        Matrix4x4 translationMatrix = Matrix4x4.TRS(-keyboardBase.transform.position, Quaternion.identity, Vector3.one);
        Debug.Log(string.Format("Translation: {0}", translationMatrix));
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, keyboardBase.transform.rotation, Vector3.one).inverse;
        Matrix4x4 screenCoordinateMatrix = new Matrix4x4();
        screenCoordinateMatrix.SetColumn(0, rotationMatrix.GetColumn(0));
        screenCoordinateMatrix.SetColumn(1, rotationMatrix.GetColumn(2));
        screenCoordinateMatrix.SetColumn(2, -rotationMatrix.GetColumn(1));
        keyboardBaseTransform = screenCoordinateMatrix * translationMatrix;
        //keyboardBaseTransform =  Matrix4x4.TRS(translationVector, Quaternion.identity, Vector3.one);
        //Debug.Log(string.Format("Transform: {0}", keyboardBaseTransform));
    }

    private void Update()
    {
        if (rightTipMarker != null && rightMidMarker != null && rightEndMarker != null)
        {
            bool rightKeypress = IsRightKeypress(rightTipMarker.transform.position, rightMidMarker.transform.position, rightEndMarker.transform.position, DateTime.Now);
            Debug.Log(string.Format("Right keypress: {0}", rightKeypress));

            bool majorityVote = rightThumbRecorder.UpdateAndVote(rightKeypress);
            InitiateButtonAnimation(Hand.RIGHT, majorityVote);
        }

        if (leftTipMarker != null && leftMidMarker != null && leftEndMarker != null)
        {
            bool leftKeypress = IsLeftKeypress(leftTipMarker.transform.position, leftMidMarker.transform.position, leftEndMarker.transform.position, DateTime.Now);
            Debug.Log(string.Format("Left keypress: {0}", leftKeypress));

            bool majorityVote = leftThumbRecorder.UpdateAndVote(leftKeypress);
            InitiateButtonAnimation(Hand.LEFT, majorityVote);
        }


    }

    private void InitiateButtonAnimation(Hand hand, bool isKeypress)
    {
        GameObject tipMarker = hand == Hand.LEFT ? leftTipMarker : rightTipMarker;

        // Search for the button closest to the tip marker
        var buttonList = keyboardBase.GetComponent<GenerateKeyboard>().buttonList;
        var minDistance = float.PositiveInfinity;
        GameObject closestButton = buttonList[0];
        foreach (GameObject button in buttonList)
        {
            Vector2 buttonSize = button.GetComponent<RectTransform>().sizeDelta;
            Vector2 buttonScale = button.GetComponentInParent<RectTransform>().lossyScale;
            Vector3 buttonWorldSize = Vector3.Scale(buttonSize / 2, buttonScale);
            Vector3 buttonWorldPosition = button.transform.position + Vector3.Scale(buttonWorldSize, new Vector3(1f, -1f));
            var distance = (buttonWorldPosition - tipMarker.transform.position).magnitude;

            if (distance < minDistance)
            {
                closestButton = button;
                minDistance = distance;
            }
        }

        var currentState = closestButton.GetComponent<InitializeCollider>().buttonState;
        Debug.Log(string.Format("closest button: {0}, hand: {1}", closestButton.name, hand));
        Debug.Log(string.Format("closest button pos: {0}, hand: {1}", closestButton.transform.position.ToString("F7"), hand));

        if (isKeypress)
        {
            closestButton.GetComponent<InitializeCollider>().PressButton();
        }
        else
        {
            closestButton.GetComponent<InitializeCollider>().ReleaseButton();
        }
    }

    public bool IsRightKeypress(Vector3 tipPos, Vector3 midPos, Vector3 endPos, DateTime timestamp)
    {
        float[][] features = GenerateFeatures(keyboardBaseTransform.MultiplyPoint3x4(tipPos),
            keyboardBaseTransform.MultiplyPoint3x4(midPos),
            keyboardBaseTransform.MultiplyPoint3x4(endPos),
            timestamp, rightThumbRecorder);

        if (features == null)
        {
            return false;
        }

        return RunRightNN(features, rightNNSession);
    }

    public bool IsLeftKeypress(Vector3 tipPos, Vector3 midPos, Vector3 endPos, DateTime timestamp)
    {

        float[][] features = GenerateFeatures(keyboardBaseTransform.MultiplyPoint3x4(tipPos),
            keyboardBaseTransform.MultiplyPoint3x4(midPos),
            keyboardBaseTransform.MultiplyPoint3x4(endPos),
            timestamp, leftThumbRecorder);

        if (features == null)
        {
            return false;
        }
        return RunLeftNN(features, leftNNSession);
    }

    private float[][] GenerateFeatures(Vector3 tipPos, Vector3 midPos, Vector3 endPos, DateTime timestamp, ThumbStateRecorder thumbStateRecorder)
    {
        Debug.Log(string.Format("Tip pos: {0}", tipPos.ToString("F7")));
        Vector3[] velocities = thumbStateRecorder.FindVelocityAndUpdate(tipPos, midPos, endPos, timestamp);

        if (velocities == null || velocities.Length == 0)
        {
            Debug.Log("Feature array is null");
            return null;
        }
        Vector3[] positions = new Vector3[] { tipPos, midPos, endPos };

        var features = new float[18];
        int insertIndex = 0;

        // Flatten arrays
        foreach (Vector3 vector in positions)
        {
            features[insertIndex] = vector.x;
            features[insertIndex + 1] = vector.y;
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

    private bool RunLeftNN(float[][] features, TFSession NNSession)
    {
        var runner = NNSession.GetRunner();
        var tensor = new TFTensor(features);

        runner.AddInput(leftNNGraph["dense_22_input"][0], tensor);
        runner.Fetch(leftNNGraph["dense_23/Sigmoid"][0]);

        var output = runner.Run();
        var result = output[0];
        var prob = (float[,])result.GetValue(jagged: false);
        // Fetch the results from output:
        Debug.Log(string.Format("Left hand NN output: {0}", prob[0, 0]));

        if (enableNNLogs)
        {
            string log = string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"), prob[0, 0], "l");
            WriteToBuffer(log);
            logIndex += 1;
        }
        return prob[0, 0] >= 0.5;
    }

    private bool RunRightNN(float[][] features, TFSession NNSession)
    {
        var runner = NNSession.GetRunner();
        var tensor = new TFTensor(features);

        runner.AddInput(rightNNGraph["dense_input"][0], tensor);
        runner.Fetch(rightNNGraph["dense_1/Sigmoid"][0]);

        var output = runner.Run();
        var result = output[0];
        var prob = (float[,])result.GetValue(jagged: false);
        // Fetch the results from output:
        Debug.Log(string.Format("Right hand NN output: {0}", prob[0, 0]));

        if (enableNNLogs)
        {
            string log = string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"), prob[0, 0], "r");
            WriteToBuffer(log);
            logIndex += 1;
        }
        return prob[0, 0] >= 0.5;
    }
    private float[][] GenerateRandomFeatures()
    {

        var rand = new System.Random();
        var rtnlist = new float[18];

        for (int i = 0; i < 18; i++)
        {
            rtnlist[i] = rand.Next() / 10;
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

    private void GetTestPosVectorsAndTimestamps(out Vector3[] posArray, out DateTime[] dateTimeArray)
    {
        var vector1 = new Vector3(-0.07221564f, 0.7727299f, 0.5003643f);
        var vector2 = new Vector3(-0.07268887f, 0.773079f, 0.5000541f);
        var timeStamp1 = new DateTime(2019, 3, 13, 15, 50, 21, 940);
        var timeStamp2 = new DateTime(2019, 3, 13, 15, 50, 21, 941);

        posArray = new Vector3[] { vector1, vector2 };
        dateTimeArray = new DateTime[] { timeStamp1, timeStamp2 };
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
        Vector4 translationVector4 = translationVector;
        translationVector4.w = 1.0f;
        transformMatrix.SetColumn(3, translationVector);

        return transformMatrix;
    }

    public void WriteToBuffer(string text)
    {
        var thread = new Thread(() => WriteOnSeparateThread(logIndex, text));
        thread.Start();
    }

    private void WriteOnSeparateThread(int logIndex, string logMessage)
    {
        NNResultLogs[logIndex] = logMessage;
    }

    private void ExportToFile()
    {
        using (TextWriter tw = new StreamWriter(path))
        {
            foreach (String s in NNResultLogs)
                tw.WriteLine(s);
        }
    }

    private void OnDestroy()
    {
        if (enableNNLogs)
        {
            ExportToFile();
        }
    }
}

public class ThumbStateRecorder
{
    private DateTime? prevTimestamp = null;
    private Vector3 prevTipPos;
    private Vector3 prevMidPos;
    private Vector3 prevEndPos;
    private List<bool> isKeypressHistory = new List<bool>();
    private int isKeypressCount = 0;
    private int keyPressHistoryLength = 5;

    public ThumbStateRecorder()
    {
        for (int index = 0; index < keyPressHistoryLength; index++)
        {
            isKeypressHistory.Add(false);
        }
    }

    public Vector3[] FindVelocityAndUpdate(Vector3 tipPos, Vector3 midPos, Vector3 endPos, DateTime currentTimestamp)
    {


        var velocities = new Vector3[] { };
        if (prevTimestamp != null)
        {
            double timeDiff = (currentTimestamp - prevTimestamp.Value).TotalSeconds;
            Vector3 tipVelocity = (tipPos - prevTipPos) / (float)timeDiff;
            Vector3 midVelocity = (midPos - prevMidPos) / (float)timeDiff;
            Vector3 endVelocity = (endPos - prevEndPos) / (float)timeDiff;
            Debug.Log(string.Format("Tip velocity: {0}", tipVelocity.ToString("F7")));
            velocities = new Vector3[] { tipVelocity, midVelocity, endVelocity };
        }

        prevTimestamp = currentTimestamp;
        prevTipPos = tipPos;
        prevMidPos = midPos;
        prevEndPos = endPos;
        return velocities;
    }

    public bool UpdateAndVote(bool isKeypress)
    {
        int newVote = isKeypress ? 1 : 0;
        int oldestVote = isKeypressHistory[0] ? 1 : 0;
        isKeypressCount += newVote - oldestVote;

        isKeypressHistory.RemoveAt(0);
        isKeypressHistory.Add(isKeypress);
        return isKeypressCount * 2 >= keyPressHistoryLength;
    }
}