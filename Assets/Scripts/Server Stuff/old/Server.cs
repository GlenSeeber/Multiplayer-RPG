/*
C# Network Programming 
by Richard Blum

Publisher: Sybex 
ISBN: 0782141765
*/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server : MonoBehaviour
{
    public void Start()
    {
        byte[] data = new byte[1024];
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 8080);
        UdpClient newsock = new UdpClient(ipep);

        Debug.Log("Waiting for a client...");

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 8080);

        data = newsock.Receive(ref sender);

        Debug.Log("Message received from " + sender.ToString() + " : " + Encoding.ASCII.GetString(data, 0, data.Length));


        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        newsock.Send(data, data.Length, sender);

        while (true)
        {
            data = newsock.Receive(ref sender);

            Debug.Log(Encoding.ASCII.GetString(data, 0, data.Length));
            newsock.Send(data, data.Length, sender);
        }
    }
}