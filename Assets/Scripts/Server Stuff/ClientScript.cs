using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

public class ClientScript : MonoBehaviour
{
    public bool host = false;

    public int clientPort = 5600;

    public string serverIP = "127.0.0.1";
    public int serverPort = 8080;

    private string connectionMessage = "init";

    public UdpClient client;
    public IPEndPoint remoteEndPoint;

    public TextMeshProUGUI clientInputDisplay;

    private PlayerInputScript playerInputs;

    public Vector2 clientDirection;

    private int packetBuffSize = 10;
    private List<Vector2> sendPacketBuff = new List<Vector2>();

    public int maxBuffLists = 5;   //how many lists are allowed to be contained within the array (recvPacketBuff) !!REMBER TO CHANGE `recvPacketBuff` SIZE AS WELL!!
    public List<Vector2>[] recvPacketBuff = new List<Vector2>[5];


    private int renderBuff = 0;         //the buff we are currently using, and running the game with.
    private int activeBuffIndex = 0;    //the way we index through each item within the current list
    private int transmissionBuff = 0;        //the buff we are waiting to receive from the server

    private bool buffReady = false;

    void Start()
    {
        //
        playerInputs = GetComponent<PlayerInputScript>();

        //network initialization
        client = new UdpClient(clientPort);
        try
        {
            client.Connect(serverIP, serverPort);
            byte[] sendBytes = Encoding.ASCII.GetBytes(connectionMessage);
            client.Send(sendBytes, sendBytes.Length);
            //Debug.Log("message sent!", this);

            remoteEndPoint = new IPEndPoint(IPAddress.Any, serverPort);

            
            
        }
        catch (Exception e)
        {
            Debug.Log("error: " + e.Message);
        }
        
    }

    public void FixedUpdate()
    {
        if (client.Available > 0)
        {
            //read from server
            byte[] receiveBytes = client.Receive(ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receiveBytes);

            //Debug.Log(waitingBuff);
            //add the receivedString to the buff array, and increase the index
            recvPacketBuff[transmissionBuff] = StringToList(receivedString.Substring(1));
            //Debug.Log("size: "+recvPacketBuff.Length);
            //Debug.Log("nested size: " + recvPacketBuff[0].Count);
            //Debug.Log(StringToList(receivedString.Substring(1))[0]);
            //Debug.Log(listToString(recvPacketBuff[transmissionBuff]));
            //this implimentation (above) might be cpu intensive, since it is probably the same as doing `list = new List<T>();` instead of `list.Clear();`
            transmissionBuff++;

            //if the index exceeds the max amount we are allowed to have, repeat at 0.
            if (transmissionBuff > maxBuffLists - 1)
            {
                transmissionBuff = 0;
            }

            
            
            //clientDirection = StringToList(receivedString.Substring(1));

        }
        //START READING THE BUFF
        //only start working through the buff after it has had a moment to fill up, then go as fast as you like.
        if (renderBuff + 1 < transmissionBuff || buffReady)
        {
            Debug.Log("READING THE BUFF!");
            //we only have to wait for the buff at the start of the program, after that we're allowed to eat into it.
            buffReady = true;

            //every Physics frame, iterate through the current buffer, and set clientDirection based on that Vector2
            Debug.Log("recvPacketBuff["+renderBuff+"]["+activeBuffIndex+"]");
            //Debug.Log(recvPacketBuff[0][0].ToString());
            clientDirection = recvPacketBuff[renderBuff][activeBuffIndex];
            activeBuffIndex++;

            //once we read every Vector2 in the list, start over at the begginning of the next list
            if (activeBuffIndex >= packetBuffSize)
            {
                activeBuffIndex = 0;    //start over
                renderBuff++;           //next list
                //if we were at the last list, begin again at the first list
                if (renderBuff >= maxBuffLists)
                {
                    renderBuff = 0;
                }
            }

            //do stuff with the inputs

            //update GUI                    //this will always pass
            if (/* receivedString[0] == '0' ||*/ true)
            {
                clientInputDisplay.text = "Client: " + clientDirection.ToString();
            }
        }

        //get movement from playerInputScript, then add it to the buff, eventually sending it to the server
        Vector2 direction = playerInputs.move;
        sendPacketBuff.Add(direction);

        //send the packet off and clear the buff once we hit the size limit.
        if (sendPacketBuff.Count == packetBuffSize)
        {
            SendMsg(listToString(sendPacketBuff));
            sendPacketBuff.Clear();
        }
    }


    public void SendMsg(string msg)
    {
        byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
        client.Send(sendBytes, sendBytes.Length);
    }

    //convert a string back to a Vector3
    public Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    //Convert string back to Vector2
    public Vector2 StringToVector2(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector2
        Vector2 result = new Vector2(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));

        return result;
    }

    public string listToString(List<Vector2> myList)
    {
        string output = "";
        //iterate through each list item, adding it to the string, appended with a newline
        for(int i = 0; i < myList.Count; i++)
        {
            output += myList[i].ToString();

            //only print newline if this isn't the last entry
            if (i < myList.Count - 1)
            {
                output += "\n";
            }
        }
        return output;
    }

    public List<Vector2> StringToList(string message)
    {
        //split the string into several substrings, between each newline
        string[] sArray = message.Split("\n");

        //create our list for the output
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < sArray.Length; i++)
        {
            //take each substring, convert it to a Vector2, then add that to the List
            result.Add(StringToVector2(sArray[i]));
        }

        Debug.Log("items: "+result.Count);

        return result;
    }

}
