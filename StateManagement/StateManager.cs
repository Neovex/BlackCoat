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
            _Core.Log("Gamestate", _RequestedState, "requested");
        }

        internal void Update(float deltaT)
        {
            if (_CurrentState == _RequestedState)
            {
                _CurrentState.UpdateInternal(deltaT);
            }
            else
            {
                _Core.Log("Changing Gamestate...");

                // Unload old State
                if (_CurrentState != null)
                {
                    _Core.Log("Destroying", _CurrentState);
                    _CurrentState.Destroy();
                    _CurrentState = null;
                    _Core.Log("Old Gamestate destroyed");
                }

                // Set new State
                _Core.Log("Setting new Gamestate", _RequestedState);
                _CurrentState = _RequestedState;

                if (_CurrentState != null)
                {
                    // Load state
                    _Core.Log("Trying to load new State:", _CurrentState);
                    if (_CurrentState.Load())
                    {
                        _Core.Log(_CurrentState, "sucessfully loaded");
                        StateChanged();
                    }
                    else
                    {
                        _Core.Log("Failed to load state", _CurrentState, "resetting state to NULL");
                        _CurrentState = null;
                    }
                }
                _Core.Log("Gamestate change completed. New State:", _CurrentState);
            }
        }

        public virtual void StateChanged() { }

        internal void Draw()
        {
            _CurrentState.Draw();
        }
    }
}