using System;

namespace BlackCoat
{
    public abstract class BlackCoatBase
    {
        protected Core _Core;

        public BlackCoatBase(Core core)
        {
            _Core = core ?? throw new ArgumentNullException(nameof(core));
        }
    }
}