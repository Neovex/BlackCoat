using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Entities.Shapes;
using BlackCoat.Properties;

namespace BlackCoat.Entities.Lights
{
    /// <summary>
    /// Special Overlay to darken / brighten everything beneath it creating th illusion of a lit object / area
    /// </summary>
    /// <seealso cref="BlackCoat.Entities.PrerenderedContainer" />
    public class Lightmap : PrerenderedContainer
    {
        private Container _Lights;
        private Rectangle _AmbientLight;

        /// <summary>
        /// Gets or sets the ambient color. Alpha value defines brightness.
        /// </summary>
        public Color Ambient
        {
            get => _AmbientLight.Color;
            set => _AmbientLight.Color = value;
        }


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

        public void AddCustomLight(IEntity entity) => _Lights.Add(entity);

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