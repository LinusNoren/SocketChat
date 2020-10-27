using System;
using System.Net;
using System.Net.Sockets;

namespace SocketChat
{
    public static class Start
    {
        public static void Main()
        {
            try
            {
                var host = Dns.GetHostEntry("localhost");
                var ipAddress = host.AddressList[0];
                var endPoint = new IPEndPoint(ipAddress, 1337);
                var socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(endPoint);

                    Console.WriteLine($"Socket connected to {endPoint}");

                    Client.StartClient(socket);
                }

                catch (ArgumentNullException ae)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ae);
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        Console.WriteLine("No server running, starting one!");

                        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        socket.Bind(endPoint);

                        Server.StartServer(socket);
                    }
                    else
                        Console.WriteLine("SocketException : {0}", se);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}