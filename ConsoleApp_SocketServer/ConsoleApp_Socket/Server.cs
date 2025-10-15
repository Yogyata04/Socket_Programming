using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    static void Main()
    {
        var sets = new Dictionary<string, Dictionary<string, int>>()
        {
            {"SetA", new Dictionary<string, int>{{"One",1},{"Two",2}}},
            {"SetB", new Dictionary<string, int>{{"Three",3},{"Four",4}}},
            {"SetC", new Dictionary<string, int>{{"Five",5},{"Six",6}}},
            {"SetD", new Dictionary<string, int>{{"Seven",7},{"Eight",8}}},
            {"SetE", new Dictionary<string, int>{{"Nine",9},{"Ten",10}}}
        };

        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("----- Server Side -----");
        Console.WriteLine("Server started. Waiting for clients...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread clientThread = new Thread(() => HandleClient(client, sets));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client, Dictionary<string, Dictionary<string, int>> sets)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int read = stream.Read(buffer, 0, buffer.Length);

            if (read == 0)
            {
                client.Close();
                return;
            }

            string received = Encoding.ASCII.GetString(buffer, 0, read).Trim();

            if (string.IsNullOrWhiteSpace(received) || received == "<blank>")
            {
                Console.Write("Empty String from client side....");
                byte[] emptyReply = Encoding.ASCII.GetBytes("Server received empty string. Enter valid set.");
                stream.Write(emptyReply, 0, emptyReply.Length);
                client.Close();
                return;
            }

            if (received.Contains("-"))
            {
                string[] parts = received.Split('-');
                string key = parts[0];
                string subkey = parts[1];

                if (sets.ContainsKey(key) && sets[key].ContainsKey(subkey))
                {
                    int value = sets[key][subkey];
                    Console.WriteLine(value);
                    for (int i = 0; i < value; i++)
                    {
                        string time = DateTime.Now.ToString("HH:mm:ss");
                        byte[] msg = Encoding.ASCII.GetBytes(time + "\n");
                        try { stream.Write(msg, 0, msg.Length); } catch { break; }
                        Console.WriteLine("Sent: " + time);
                        Thread.Sleep(1000);
                    }
                    client.Close();
                    return;
                }
            }

            byte[] fallback = Encoding.ASCII.GetBytes("Invalid input! Please use the format SetX-Subset.");
            stream.Write(fallback, 0, fallback.Length);
            client.Close();
        }
        catch { client.Close(); }
    }
}
