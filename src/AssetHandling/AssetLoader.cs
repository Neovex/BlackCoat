using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace BlackCoat
{
    /// <summary>
    /// Asset management class. Handles loading/unloading of assets located in a specified root folder.
    /// Multiple instances of the AssetManager class can be used to simplify asset memory management.
    /// </summary>
    public class AssetLoader<T> : IDisposable where T : class, IDisposable
    {
        // Variables #######################################################################
        protected String[] _FileEndings;
        protected Dictionary<String, T> _Assets = new Dictionary<String, T>();


        // Properties ######################################################################
        /// <summary>
        /// The supported file endings by this <see cref="AssetLoader{T}"/> instance
        /// </summary>
        public IEnumerable<String> FileEndings { get { return _FileEndings; } }

        /// <summary>
        /// Root-folder to look for Assets
        /// </summary>
        public String RootFolder { get; set; }

        /// <summary>
        /// Determines whether this asset loader should operate in debug mode.
        /// Setting this value to <c>true</c> will cause loading exceptions to be thrown instead of being handled internally.
        /// </summary>
        public Boolean Debug { get; set; }

        /// <summary>
        /// Gets a value indicating whether this AssetManager is disposed.
        /// </summary>
        public bool Disposed { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the AssetManager class.
        /// </summary>
        /// <param name="supportedFileEndings">List of File Endings this manager is supposed to support (i.e: .jpg)</param>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        internal AssetLoader(IEnumerable<String> supportedFileEndings, String assetRoot = "")
        {
            RootFolder = assetRoot;
            _FileEndings = new[] { String.Empty }.Concat(supportedFileEndings).Distinct().ToArray();
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="AssetLoader{T}"/> class.
        /// </summary>
        ~AssetLoader()
        {
            Dispose(false);
        }


        // Methods #########################################################################
        /// <summary>
        /// Loads or retrieves an already loaded instance of an Asset from a File or Raw Data Source
        /// </summary>
        /// <param name="name">Name of the Resource</param>
        /// <param name="rawData">Optional byte array containing the raw data of the asset</param>
        /// <returns>The managed Asset</returns>
        public virtual T Load(String name, Byte[] rawData = null)
        {
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (name == null) throw new ArgumentNullException("name");
            if (_Assets.ContainsKey(name)) return _Assets[name];

            Object param = null;
            try
            {
                param = rawData as Object ?? ResolveFileEndings(name);
                var asset = (T)Activator.CreateInstance(typeof(T), new object[] { param });
                _Assets.Add(name, asset);
                return asset;
            }
            catch (Exception e)
            {
                Log.Error("Could not load asset", name, "from", param, "because of", e);
                if (Debug) throw;
            }
            return null;
        }

        /// <summary>
        /// Loads all compatible files in the root directory.
        /// </summary>
        /// <param name="logErrors">Determines if errors should be logged</param>
        /// <returns>Array containing the names of all successfully loaded files</returns>
        public virtual String[] LoadAllFilesInDirectory(bool logErrors = false)
        {
            var assetNames = new List<String>();
            foreach (var file in Directory.EnumerateFiles(RootFolder))
            {
                try
                {
                    var asset = (T)Activator.CreateInstance(typeof(T), new object[] { file });
                    var name = Path.GetFileNameWithoutExtension(file);
                    _Assets.Add(name, asset);
                    assetNames.Add(name);
                }
                catch (Exception e)
                {
                    if(logErrors) Log.Error("Could not load", file, "because of", e);
                    if (Debug) throw;
                }
            }
            return assetNames.ToArray();
        }

        private String ResolveFileEndings(string name)
        {
            var path = _FileEndings.Select(f => Path.Combine(RootFolder, String.Concat(name, f))).FirstOrDefault(f => File.Exists(f));
            if (path != null) return path;
            Log.Warning("Could not find a matching file for", name, "with any of the registered file endings");
            return Path.Combine(RootFolder, name);
        }

        /// <summary>
        /// Unloads the Asset with the given name
        /// </summary>
        /// <param name="name">Name of the Asset</param>
        public void Release(String name)
        {
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (!_Assets.ContainsKey(name)) return;
            _Assets[name].Dispose();
            _Assets.Remove(name);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            Disposed = true;

            foreach (var kvp in _Assets)
            {
                try
                {
                    kvp.Value.Dispose();
                }
                catch (Exception e)
                {
                    Log.Error("Failed to dispose", kvp.Key, "Reason:", e);
                }
            }
            _Assets.Clear();
        }
    }
}