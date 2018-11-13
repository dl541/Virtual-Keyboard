using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public enum ButtonAction { PRESS, RELEASE }

public class KeyboardPressingLoggerClient
{


    #region private members 	
    private static TcpClient socketConnection;
    private static Thread clientReceiveThread;
    private static GenerateKeyboard generateKeyboard;
    private static GameObject qButton;
    #endregion

    // Use this for initialization 	
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        Debug.Log("Script is begin run");
        ConnectToTcpServer();
    }

    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private static void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private static void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("localhost", 38300);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);

                        if (serverMessage == "quit")
                        {
                            break;
                        }

                        string[] serverMessageArray = serverMessage.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                        if (serverMessageArray[1] == "0")
                        {
                            Vector2 coordinates = new Vector2(float.Parse(serverMessageArray[serverMessageArray.Length-2]),
                                -float.Parse(serverMessageArray[serverMessageArray.Length - 1]));
                            
                            ButtonAction buttonAction = serverMessageArray[2] == "0"? ButtonAction.PRESS: ButtonAction.RELEASE;

                            UnityMainThreadDispatcher.Instance().Enqueue(sendCoordinatesToMainThread(coordinates, buttonAction));
                        }


                        ////Character message if the first flag is set to 1
                        //if (serverMessageArray[1] == "1")
                        //{
                        //    string buttonName = serverMessageArray[serverMessageArray.Length - 1];

                        //    ButtonAction buttonAction = serverMessageArray[2] == "0" ? ButtonAction.PRESS : ButtonAction.RELEASE;

                        //    Debug.Log("Action on button " + buttonName);
                        //    UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(buttonName, buttonAction));
                        //}

                    }
                    Debug.Log("Connection closed");
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public static IEnumerator sendCoordinatesToMainThread(Vector2 coord, ButtonAction buttonAction)
    {
        Debug.Log(string.Format("Coordinates {0} to main thread.", coord));
        generateKeyboard = GameObject.Find("KeyboardBase").GetComponent<GenerateKeyboard>();


        if (buttonAction == ButtonAction.PRESS)
        {
            generateKeyboard.CoordinateToButton(coord, ButtonState.PRESSING);
        }
        else
        {
            generateKeyboard.CoordinateToButton(coord, ButtonState.RELEASING);
        }
        yield return null;
    }


    public static IEnumerator ThisWillBeExecutedOnTheMainThread(String buttonName, ButtonAction buttonAction)
    {
        Debug.Log("This is executed from the main thread");
        generateKeyboard = GameObject.Find("KeyboardBase").GetComponent<GenerateKeyboard>();
        
        GameObject buttonPressed = generateKeyboard.nameKeyMap[buttonName] as GameObject;

        if (buttonAction == ButtonAction.PRESS)
        {
            buttonPressed.GetComponent<InitializeCollider>().buttonState = ButtonState.PRESSING;
        }
        else
        {
            buttonPressed.GetComponent<InitializeCollider>().buttonState = ButtonState.RELEASING;
        }
        yield return null;
    }
}
