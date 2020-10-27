using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketChat
{
    public static class Client
    {
        public static void StartClient(Socket socket)
        {
            try
            {
                Console.WriteLine("Chat Name?");

                var name = Console.ReadLine();

                var message = new Message(Command.JOIN.ToString(), name);

                socket.Send(message.Serialize());

                Console.WriteLine("Start chatting! (Type HELP if you want to list available commands)");


                var receiveThread = new Thread(() => Receive(socket));
                receiveThread.Start();


                while (true)
                {
                    var userInput = Console.ReadLine();

                    if (userInput == Command.DISCONNECT.ToString())
                    {
                        message = new Message(Command.DISCONNECT.ToString(), name);
                        socket.Send(message.Serialize());
                        break;
                    }

                    if (userInput != null && userInput.StartsWith(Command.PRIVATE.ToString()))
                    {
                        var to = userInput.Remove(0, Command.PRIVATE.ToString().Length).Trim();
                        message = new Message(Command.PRIVATE.ToString(), name, to);
                        socket.Send(message.Serialize());
                    }
                    else if (userInput == Command.LIST.ToString())
                    {
                        message = new Message(Command.LIST.ToString(), name);
                        socket.Send(message.Serialize());
                    }
                    else if (userInput == Command.HELP.ToString())
                    {
                        Console.WriteLine("Write a message and press enter to chat.");
                        Console.WriteLine($"Type {Command.LIST} to list all connected users.");
                        Console.WriteLine($"Type {Command.DISCONNECT} to end the chat.");
                        Console.WriteLine($"Type {Command.PRIVATE} followed by username to start a private chat");
                        Console.WriteLine($"Type {Command.ENDPRIVATE} to end a private chat");

                    }
                    else if (userInput == Command.ENDPRIVATE.ToString())
                    {
                        message = new Message(Command.ENDPRIVATE.ToString(), name);
                        socket.Send(message.Serialize());
                    }
                    else
                    {
                        message = new Message(Command.CHAT.ToString(), name,null,userInput);
                        socket.Send(message.Serialize());
                    }
                }
            
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e);
            }
        }

        private static void Receive(Socket socket)
        {
            try
            {
                var bytes = new byte[1024];

                while (socket.Connected)
                {
                    var bytesRecived = socket.Receive(bytes);
                    Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, bytesRecived));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}