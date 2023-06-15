using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class ServerScript : MonoBehaviour
{
    public int serverPort = 8080;

    public IPEndPoint ipep;

    public UdpClient newsock;

    public IPEndPoint sender;

    public List<IPEndPoint> clients = new List<IPEndPoint>();

    public void Awake()
    {
        ipep = new IPEndPoint(IPAddress.Any, serverPort);
        newsock = new UdpClient(ipep);

        Debug.Log("waiting for client...\n", this);

        sender = new IPEndPoint(IPAddress.Any, 0);
    }

    public void Update()
    {
        if (newsock.Available > 0)
        {
            byte[] recvBytes = newsock.Receive(ref sender);

            //add new clients to the list
            if (!clients.Contains(sender))
            {
                clients.Add(sender);
            }

            //Debug.Log("Message from: " + sender.ToString(), this);
            //Debug.Log(Encoding.ASCII.GetString(recvBytes, 0, recvBytes.Length), this);

            //respond to sender (remove later)
            //SendMsg("Hello, this is the server!", sender);

            if (Encoding.ASCII.GetString(recvBytes) == "init")
            {
                return;
            }

            //relay messages from the clients to all other clients
            for(int i = 0; i < clients.Count; i++)
            {
                //don't relay it back to the same client though
                if (clients.IndexOf(sender) == i)
                {
                    continue;
                }

                //send out whatever you received, with a single number amended to the beginning, representing who it came from.
                //(maybe seperate this out at some point if it starts getting messed with more)
                SendMsg(clients.IndexOf(sender).ToString()+Encoding.ASCII.GetString(recvBytes), clients[i]);
            }
        }


    }

    public void SendMsg(string message, IPEndPoint recipient)
    {
        byte[] sendBytes = Encoding.ASCII.GetBytes(message);
        newsock.Send(sendBytes, sendBytes.Length, recipient);
    }

    public void SendMsgBytes(byte[] messageBytes, IPEndPoint recipient)
    {
        newsock.Send(messageBytes, messageBytes.Length, recipient);
    }

}
