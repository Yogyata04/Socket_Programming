using System;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main()
    {
        Console.WriteLine("---- Client Side ----");

        while (true)
        {
            Console.Write("Enter message (For example -> SetA-Two or press Enter to send empty): ");
            string message = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(message))
                message = "<blank>";

            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 5000);
                NetworkStream stream = client.GetStream();

                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.Write(response);
                    if (!stream.DataAvailable)
                        break;
                }

                Console.WriteLine();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
