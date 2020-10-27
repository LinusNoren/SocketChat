using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace SocketChat
{
    public static class Server
    {
        private static readonly IDictionary<string, Socket> ClientsList = new Dictionary<string, Socket>();

        public static void StartServer(Socket socket)
        {
            try
            {
                socket.Listen(10);

                Console.WriteLine("Chat server started and waiting for a connection...");

                while (true)
                {
                    var listener = socket.Accept();

                    var bytes = new byte[1024];

                    var bytesReceived = listener.Receive(bytes);

                    var byteAsString = Encoding.UTF8.GetString(bytes, 0, bytesReceived);

                    //Can't use System.Text.Json because it can't deserialize without parameterless constructor. Using a parameterized constructor is not supported yet.
                    var message = JsonConvert.DeserializeObject<Message>(byteAsString);

                    SendMessage(message);

                    Console.WriteLine($"{message.User} Joined chat");

                    ClientsList.Add(message.User, listener);

                    var chatThread = new Thread(() => Chat(listener));
                    chatThread.Start();

                    message.Command = Command.LIST.ToString();

                    SendMessage(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void SendMessage(Message message)
        {
            if (message.Command == Command.PRIVATE.ToString())
            {
                if (ClientsList.ContainsKey(message.To ?? throw new ArgumentNullException(nameof(message.To))))
                {
                    Send($"P: {message.User}: {message.Text}", ClientsList[message.To]);
                    Send($"To {message.To}: {message.Text}", ClientsList[message.User]);
                }
                else
                    Send($"User {message.To} does not exist", ClientsList[message.User]);

            }
            else if (message.Command == Command.LIST.ToString())
            {
                var users = $"Connected users: {Environment.NewLine}";

                users = ClientsList.Aggregate(users, (current, item) => $"{current}{item.Key}{Environment.NewLine}");
                
                Send(users, ClientsList[message.User]);
            }
            else
            {
                if (message.Command == Command.DISCONNECT.ToString())
                    ClientsList.Remove(message.User);

                foreach (var item in ClientsList)
                {

                    string broadcastMessage;

                    if (message.Command == Command.JOIN.ToString())
                        broadcastMessage = $"{message.User} Joined!";
                    else if (message.Command == Command.DISCONNECT.ToString())
                        broadcastMessage = $"{message.User} Left!";
                    else
                        broadcastMessage = $"{message.User}: {message.Text}";

                    Send(broadcastMessage, item.Value);
                }
            }
        }

        private static void Chat(Socket clientSocket)
        {
            var privateTo = string.Empty;

            while (true)
            {
                try
                {
                    var bytesFrom = new byte[1024];

                    var bytesReceived = clientSocket.Receive(bytesFrom);

                    var json = Encoding.UTF8.GetString(bytesFrom, 0, bytesReceived);

                    var message = JsonConvert.DeserializeObject<Message>(json);


                    if (message.Command == Command.PRIVATE.ToString())
                    {
                        privateTo = message.To;
                        Console.WriteLine(json);
                        continue;
                    }

                    if (message.Command == Command.ENDPRIVATE.ToString())
                    {
                        privateTo = string.Empty;
                        Console.WriteLine(json);
                        continue;
                    }

                    if (privateTo != string.Empty)
                    {
                        message.To = privateTo;
                        message.Command = Command.PRIVATE.ToString();
                    }

                    SendMessage(message);

                    Console.WriteLine(json);

                    if (message.Command == Command.DISCONNECT.ToString())
                        break;
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionReset)
                        break;

                    Console.WriteLine("SocketException : {0}", se);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            


            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private static void Send(string message, Socket socket) => socket.Send(Encoding.UTF8.GetBytes(message));
    }
}
