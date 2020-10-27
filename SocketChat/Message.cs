using System.Text;
using Newtonsoft.Json;

namespace SocketChat
{
    public class Message
    {
        public string Command { get; set; }
        public string User { get; }
        public string Text { get; }
        public string To { get; set; }

        public Message(string command, string user, string to = null, string text = null)
        {
            Command = command;
            User = user;
            To = to;
            Text = text;
        }

        public byte[] Serialize()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}