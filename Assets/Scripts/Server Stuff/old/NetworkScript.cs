using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;

public class NetworkScript : MonoBehaviour
{
    public string ip = "192.168.254.34";
    public int port = 8080; //change later



    public void Start()
    {
        StartCoroutine(startServer());
    }

    IEnumerator startServer()
    {
        yield return new WaitForSeconds(1);
        TalkToServer();
        Debug.Log("started client!");
        
    }

    public void TalkToServer()
    {
        // This constructor arbitrarily assigns the local port number.
        UdpClient client = new UdpClient(port);
        try
        {
            //udpClient.Connect(ip, port);



            // Sends a message to the host to which you have connected.
            Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");

            client.Send(sendBytes, sendBytes.Length);

            //IPEndPoint object will allow us to read datagrams sent from any source.
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            /*
            // Blocks until a message returns on this socket from a remote host.
            Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
            string returnData = Encoding.ASCII.GetString(receiveBytes);

            // Uses the IPEndPoint object to show info on who responded (you technically already know it, but this will be useful to copy for server code potentially).
            Debug.Log("This is the message you received '" +
                                              returnData.ToString() + "'");

            Debug.Log("This message was sent from " +
                                        RemoteIpEndPoint.Address.ToString() +
                                        " on their port number " +
                                        RemoteIpEndPoint.Port.ToString());

            udpClient.Close();
            */
        }
        catch (Exception e)
        {
            Debug.LogError("problem !! -> " + e.ToString());
        }
    }
}
