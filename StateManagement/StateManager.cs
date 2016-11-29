using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat;

namespace BlackCoat
{
    /// <summary>
    /// Manages the transistions between Gamestates
    /// </summary>
    public sealed class StateManager
    {
        private Core _Core;
        private BaseGamestate _CurrentState;
        private BaseGamestate _RequestedState;

        /// <summary>
        /// Name of the currently active Gamestate. NULL if none active.
        /// </summary>
        public String CurrentState { get { return _CurrentState?.Name; } }


        /// <summary>
        /// Occurs when a new Gamestate has successfully finished loading an becomes the new acive state.
        /// </summary>
        public event Action<BaseGamestate> StateChanged = s => { };


        /// <summary>
        /// Initializes a new instance of the <see cref="StateManager"/> class.
        /// </summary>
        /// <param name="core">The Engine Core.</param>
        internal StateManager(Core core)
        {
            _Core = core;
        }

        /// <summary>
        /// Beginns a state chane. Note: the new state usually becomes active in the next frame.
        /// </summary>
        /// <param name="state">The new state.</param>
        public void ChangeState(BaseGamestate state)
        {
            _RequestedState = state;
            Log.Debug("---------------------------------------");
            Log.Debug("Gamestate", _RequestedState, "requested");
        }

        /// <summary>
        /// Updates the current state or executes a state change.
        /// </summary>
        /// <param name="deltaT">Current Frametime</param>
        internal void Update(float deltaT)
        {
            if (_CurrentState == _RequestedState)
            {
                _CurrentState?.UpdateInternal(deltaT);
            }
            else
            {
                Log.Debug("Changing Gamestate...");

                // Unload old State
                if (_CurrentState != null)
                {
                    try
                    {
                        Log.Debug("Destroying", _CurrentState);
                        _CurrentState.DestroyInternal();
                        Log.Debug("Old Gamestate destroyed");
                    }
                    catch (Exception e)
                    {
                        Log.Error("Failed to destroy old Gamestate. Reason:", e);
                    }
                    _CurrentState = null;
                }

                // Set new State
                Log.Debug("Setting new Gamestate", _RequestedState);
                _CurrentState = _RequestedState;

                if (_CurrentState != null)
                {
                    try
                    {
                        // Load state
                        Log.Debug("Trying to load new State:", _CurrentState);
                        if (_CurrentState.LoadInternal())
                        {
                            Log.Debug(_CurrentState, "sucessfully loaded");
                            StateChanged.Invoke(_CurrentState);
                        }
                        else
                        {
                            Log.Error("Failed to load state", _CurrentState, "resetting state to NULL");
                            _CurrentState = _RequestedState = null;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Failed to load state", _CurrentState, "Reason:", e);
                        _CurrentState = _RequestedState = null;
                    }
                }
                Log.Info("Gamestate change completed. New State:", _CurrentState);
            }
        }

        /// <summary>
        /// Draws the current state if any.
        /// </summary>
        internal void Draw()
        {
            _CurrentState?.Draw();
        }

        /// <summary>
        /// Destroys this instance and the current state if any.
        /// </summary>
        internal void Destroy()
        {
            _CurrentState?.DestroyInternal();
        }
    }

