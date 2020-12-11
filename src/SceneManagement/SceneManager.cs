using System;

namespace BlackCoat
{
    /// <summary>
    /// Manages the transitions between Scenes
    /// </summary>
    public sealed class SceneManager : BlackCoatBase
    {
        /// <summary>
        /// Occurs when a new Scene has successfully finished loading an becomes the new active Scene.
        /// </summary>
        public event Action<Scene> SceneChanged = s => { };
        /// <summary>
        /// Occurs when a new Scene has failed to load.
        /// </summary>
        public event Action<Scene> SceneChangeFailed = s => { };

        
        private Scene _CurrentScene;
        private Scene _RequestedScene;


        /// <summary>
        /// Name of the currently active Scene. NULL if none active.
        /// </summary>
        public String CurrentSceneName => _CurrentScene?.Name;


        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="core">The Engine Core.</param>
        internal SceneManager(Core core) : base(core)
        {
            Log.Debug(nameof(SceneManager), "created");
        }


        /// <summary>
        /// Begins a Scene change. Note: the new Scene usually becomes active in the next frame.
        /// </summary>
        /// <param name="Scene">The new Scene.</param>
        public void ChangeScene(Scene scene)
        {
            _RequestedScene = scene;
            Log.Debug("---------------------------------------");
            Log.Debug("Scene", _RequestedScene, "requested");
        }

        /// <summary>
        /// Updates the current Scene or executes a Scene change.
        /// </summary>
        /// <param name="deltaT">Current frame time</param>
        internal void Update(float deltaT)
        {
            if (_CurrentScene == _RequestedScene)
            {
                _CurrentScene?.UpdateInternal(deltaT);
            }
            else
            {
                Log.Debug("Changing Scene...");

                // Unload old Scene
                if (_CurrentScene != null)
                {
                    try
                    {
                        Log.Debug("Destroying", _CurrentScene);
                        _CurrentScene.DestroyInternal();
                        Log.Debug("Old Scene destroyed");
                    }
                    catch (Exception e)
                    {
                        Log.Error("Failed to destroy old Scene. Reason:", e);
                    }
                    _CurrentScene = null;
                }

                // Set new Scene
                Log.Debug("Setting new Scene", _RequestedScene);
                _CurrentScene = _RequestedScene;

                if (_CurrentScene != null)
                {
                    try
                    {
                        // Load Scene
                        Log.Debug("Trying to load new Scene:", _CurrentScene);
                        if (_CurrentScene.LoadInternal())
                        {
                            Log.Debug(_CurrentScene, "successfully loaded");
                            SceneChanged.Invoke(_CurrentScene);
                        }
                        else
                        {
                            Log.Error("Failed to load Scene", _CurrentScene);
                            var failedScene = _CurrentScene;
                            _CurrentScene = _RequestedScene = null;
                            SceneChangeFailed.Invoke(failedScene);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Failed to load Scene", _CurrentScene, "Reason:", e);
                        var failedScene = _CurrentScene;
                        _CurrentScene = _RequestedScene = null;
                        SceneChangeFailed.Invoke(failedScene);
                        if (_Core.Debug) throw;
                    }
                }
                Log.Info("Scene change completed. New Scene:", _CurrentScene);
            }
        }

        /// <summary>
        /// Draws the current Scene if any.
        /// </summary>
        internal void Draw()
        {
            _CurrentScene?.Draw();
        }

        /// <summary>
        /// Destroys this instance and the current Scene if any.
        /// </summary>
        internal void Destroy()
        {
            _CurrentScene?.DestroyInternal();
            _CurrentScene = _RequestedScene = null;
            Log.Debug(nameof(SceneManager), "destroyed");
        }
    }
}