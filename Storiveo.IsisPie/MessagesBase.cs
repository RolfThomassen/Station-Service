namespace Storiveo.IsisPie
{
    public class MessagesBase
    {
        //low level packet message
        public int Type;
        public int Command;
        public int MessageLength;

        public int lType = 1;
        public int lCommand = 1;
        public int lMessageLength = 2;

        public MessagesBase()
        {
            Type = lType;
            Command = Type + lCommand;
            MessageLength = Command + lMessageLength;
        }

    }
}