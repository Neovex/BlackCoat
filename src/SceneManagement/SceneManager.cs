using System;

namespace BlackCoat
{
    /// <summary>
    /// Manages the transitions between Scenes
    /// </summary>
    public sealed class SceneManager : BlackCoatBase
    {
        // Events ##########################################################################
        /// <summary>
        /// Occurs when a new Scene has successfully finished loading an becomes the new active Scene.
        /// </summary>
        public event Action<Scene> SceneChanged = s => { };

        /// <summary>
        /// Occurs when a new Scene has failed to load.
        /// </summary>
        public event Action<Scene> SceneChangeFailed = s => { };


        // Variables #######################################################################
        private Scene _CurrentScene;
        private Scene _RequestedScene;


        // Properties ######################################################################
        /// <summary>
        /// Name of the currently active Scene. NULL if none is active.
        /// </summary>
        public String CurrentSceneName => _CurrentScene?.Name;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="core">The Engine Core.</param>
        internal SceneManager(Core core) : base(core)
        {
            Log.Debug(nameof(SceneManager), "created");
        }


        // Methods #########################################################################
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
                var success = false;
                if (_RequestedScene != null)
                {
                    // Load Scene
                    try
                    {
                        Log.Debug("Trying to load new Scene:", _RequestedScene);
                        if (_RequestedScene.LoadInternal())
                        {
                            Log.Debug(_RequestedScene, "successfully loaded");
                            success = true;
                        }
                        else throw new Exception("Manual Failure");
                    }
                    catch (Exception e)
                    {
                        Log.Error("Failed to load Scene", _CurrentScene, "Reason:", e);
                        SceneChangeFailed.Invoke(_RequestedScene);
                        _RequestedScene = null;
                        if (_Core.Debug) throw;
                    }
                }
                if (!success) return;

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
                SceneChanged.Invoke(_CurrentScene);

                Log.Info("Scene change completed. New Scene:", _CurrentScene);
            }
        }

        /// <summary>
        /// Draws the current Scene if any.
        /// </summary>
        internal void Draw() => _CurrentScene?.Draw();

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