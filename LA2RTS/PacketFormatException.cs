using System;

namespace LA2RTS
{
    class PacketFormatException : Exception
    {
        public PacketFormatException() : base("Wrong data format in the incoming packet.")
        {
        }

        public PacketFormatException(string message) : base(message)
        {
        }
    }
}
