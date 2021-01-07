using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SFML.System;

namespace Gemmy
{
    public enum BlockType
    {
        Long,
        TBlock,
        SBlock,
        ZBlock,
        QuadBlock,
        JBlock,
        LBlock
    }


    public static class BlockConfig
    {
        private static readonly Dictionary<BlockType, Vector2i[][]> _CONFIG = new Dictionary<BlockType, Vector2i[][]>();

        public static void Initialize(String path)
        {
            if(_CONFIG.Count != 0 ) throw new Exception("Invalid initialization state");
            var cfgRaw = File.ReadAllText(path);
            foreach (var cfg in cfgRaw.Split('+'))
            {
                var split = cfg.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var frames = split.Skip(1)
                                  .Select((s, i) => new { Value = s, Index = i })
                                  .GroupBy(item => item.Index / 4, item => item.Value)
                                  .Select(g => g.ToArray()).ToArray();

                _CONFIG.Add((BlockType)Enum.Parse(typeof(BlockType), split[0], true), ParseFrames(frames));
            }
        }

        private static Vector2i[][] ParseFrames(String[][] frames)
        {
            var vector = new Vector2i[frames.Length][];
            for (int f = 0; f < frames.Length; f++)
            {
                var temp = new List<Vector2i>();
                var lines = frames[f];
                for (int l = 0; l < lines.Length; l++)
                {
                    var line = lines[l];
                    for (int c = 0; c < line.Length; c++)
                    {
                        if (line[c] == '#') temp.Add(new Vector2i(c, l));
                    }
                }
                vector[f] = temp.ToArray();
            }
            return vector;
        }

        public static Vector2i[] GetConfig(BlockType blockType, int frame)
        {
            var cfg = _CONFIG[blockType];
            return cfg[frame % cfg.Length];
        }
    }
}