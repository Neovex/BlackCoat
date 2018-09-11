using System;

namespace BlackCoat.Network
{
    public sealed class Commands<TEnum> where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        public readonly TEnum Handshake;
        public readonly TEnum UserConnected;
        public readonly TEnum UserDisconnected;

        public Commands(TEnum handshake, TEnum userConnected, TEnum userDisconnected)
        {
            Handshake = handshake;
            UserConnected = userConnected;
            UserDisconnected = userDisconnected;
        }
    }
}