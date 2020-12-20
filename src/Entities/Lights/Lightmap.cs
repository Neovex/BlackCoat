using System.IO;

using SFML.System;
using SFML.Graphics;

using BlackCoat.Properties;
using BlackCoat.Entities.Shapes;

namespace BlackCoat.Entities.Lights
{
    /// <summary>
    /// Special Overlay to darken / brighten everything beneath it, creating the illusion of a lit object or area.
    /// </summary>
    /// <seealso cref="BlackCoat.Entities.PrerenderedContainer" />
    public class Lightmap : PrerenderedContainer
    {
        // Variables #######################################################################
        private Container _Lights;
        private Rectangle _AmbientLight;


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the ambient color. Alpha value defines brightness.
        /// </summary>
        public Color Ambient
        {
            get => _AmbientLight.Color;
            set => _AmbientLight.Color = value;
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Lightmap"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="fixedSize">Size of the entire <see cref="Lightmap"/> area.
        public Lightmap(Core core, Vector2f size) : base(core, size)
        {
            BlendMode = BlendMode.Multiply;
            ClearColor = Color.Black;

            var view = new View(size / 2, size);
            Add(_Lights = new Container(_Core) { View = view });
            Add(_AmbientLight = new Rectangle(_Core, size, new Color(50, 50, 50))
            {
                BlendMode = BlendMode.Add,
                View = view
            });
        }


        // Methods #########################################################################        
        /// <summary>
        /// Adds a default point light to the <see cref="Lightmap"/>.
        /// </summary>
        /// <param name="loader">The <see cref="TextureLoader"/> of the current scene.</param>
        /// <param name="position">The position to place the light.</param>
        /// <param name="color">Optional light color.</param>
        /// <param name="scale">Optional light scale.</param>
        /// <param name="rotation">Optional light rotation.</param>
        /// <returns></returns>
        public Graphic AddLight(TextureLoader loader, Vector2f position, Color? color = null, Vector2f? scale = null, float rotation = 0)
        {
            var tex = loader.Load(nameof(Resources.Pointlight), Resources.Pointlight);
            var light = new Graphic(_Core, tex)
            {
                BlendMode = BlendMode.Add,
                Origin = tex.Size.ToVector2f() / 2,
                Position = position,
                Color = color ?? Color.White,
                Scale = scale ?? new Vector2f(1, 1),
                Rotation = rotation
            };
            _Lights.Add(light);
            return light;
        }

        /// <summary>
        /// Adds a custom light based on an entity.
        /// </summary>
        /// <param name="entity">The entity to be utilized as light.</param>
        public void AddCustomLight(IEntity entity) => _Lights.Add(entity);

        /// <summary>
        /// Saves the <see cref="Lightmap" /> definition to a file.
        /// </summary>
        /// <param name="file">The file path to save to.</param>
        /// <remarks>
        /// While lights based on custom entities can be saved they cannot be loaded.
        /// </remarks>
        public void Save(string file)
        {
            using (var stream = new FileStream(file, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                // Ambient
                writer.Write(_AmbientLight.Color.ToInteger());
                // Lights
                foreach (var light in _Lights.GetAll<Graphic>())
                {
                    writer.Write(light.Position.X);
                    writer.Write(light.Position.Y);
                    writer.Write(light.Color.ToInteger());
                    writer.Write(light.Scale.X);
                    writer.Write(light.Scale.Y);
                    writer.Write(light.Rotation);
                }
            }
        }

        /// <summary>
        /// Loads a <see cref="Lightmap" /> definition from a file.
        /// </summary>
        /// <param name="loader">The <see cref="TextureLoader"/> of the current scene.</param>
        /// <param name="file">The file path to load from.</param>
        /// <remarks>
        /// While lights based on custom entities can be saved they cannot be loaded.
        /// </remarks>
        public void Load(TextureLoader loader, string file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                _AmbientLight.Color = new Color(reader.ReadUInt32());
                while (stream.Position != stream.Length)
                {
                    AddLight(loader, new Vector2f(reader.ReadSingle(), reader.ReadSingle()),
                                     new Color(reader.ReadUInt32()),
                                     new Vector2f(reader.ReadSingle(), reader.ReadSingle()),
                                     reader.ReadSingle());
                }
            }
        }
    }
}