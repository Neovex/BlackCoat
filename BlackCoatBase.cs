﻿using System;

namespace BlackCoat
{
    /// <summary>
    /// Abstract base class of all standalone and core-dependent systems and entities.
    /// </summary>
    public abstract class BlackCoatBase
    {
        /// <summary>
        /// The Engine Core
        /// </summary>
        protected Core _Core;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackCoatBase"/> class.
        /// </summary>
        /// <param name="core">The Engine core.</param>
        /// <exception cref="ArgumentNullException">core</exception>
        public BlackCoatBase(Core core)
        {
            _Core = core ?? throw new ArgumentNullException(nameof(core));
        }
    }
}