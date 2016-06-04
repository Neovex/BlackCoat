using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat;

namespace BlackCoat
{
    public class StateManager
    {
        protected Core _Core;
        private BaseGameState _CurrentState;
        private BaseGameState _RequestedState;
        
        protected internal StateManager(Core core)
        {
            _Core = core;
        }

        public void ChangeState(BaseGameState state)
        {
            _RequestedState = state;
            Log.Debug("---------------------------------------");
            Log.Debug("Gamestate", _RequestedState, "requested");
        }

        internal void Update(float deltaT)
        {
            if (_CurrentState == _RequestedState)
            {
                if (_CurrentState != null) _CurrentState.UpdateInternal(deltaT);
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
                        _CurrentState.Destroy();
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
                        if (_CurrentState.Load())
                        {
                            Log.Debug(_CurrentState, "sucessfully loaded");
                            StateChanged();
                        }
                        else
                        {
                            Log.Error("Failed to load state", _CurrentState, "resetting state to NULL");
                            _CurrentState = null;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Failed to load state", _CurrentState, "Reason:", e);
                        _CurrentState = null;
                    }
                }
                Log.Info("Gamestate change completed. New State:", _CurrentState);
            }
        }

        public virtual void StateChanged() { }

        internal void Draw()
        {
            if (_CurrentState != null) _CurrentState.Draw();
        }
    }
}