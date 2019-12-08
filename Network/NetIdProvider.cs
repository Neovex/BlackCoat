using System;

namespace BlackCoat.Network
{
    public static class NetIdProvider
    {
        private static  Int32 _ID_PROVIDER = new Random().Next(Int32.MinValue / 2, 0);
        /// <summary>
        /// Gets the next free/unused net identifier.
        /// </summary>
        public static Int32 NEXT_ID => _ID_PROVIDER++;
    }
}