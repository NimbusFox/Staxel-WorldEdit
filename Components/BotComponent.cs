using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel.Core;

namespace NimbusFox.WorldEdit.Components {
    public class BotComponent {
        public Vector3D TileOffset { get; }
        public Vector3D BoundingBox { get; }
        public string BladeModel { get; }
        public IReadOnlyList<(Vector3D location, float scale)> BladeLocations { get; }
        public IReadOnlyDictionary<Color, IReadOnlyList<Color>> Palettes { get; }
        public IReadOnlyList<Color> IgnoreColors { get; }
        public int PaletteTolerance { get; }

        public BotComponent(Blob config) {
            TileOffset = config.FetchBlob("tileOffset").GetVector3D();
            var boundingBox = config.FetchBlob("boundingBox");
            BoundingBox = new Vector3D(boundingBox.GetDouble("x", 0.5), boundingBox.GetDouble("y", 0.5), boundingBox.GetDouble("z", 0.5));
            BladeModel = config.GetString("bladeModel");

            var locations = new List<(Vector3D location, float scale)>();

            foreach (var list in config.GetList("bladeLocations")) {
                locations.Add((list.Blob().GetVector3D(), (float)list.Blob().GetDouble("scale")));
            }

            var palettes = new Dictionary<Color, IReadOnlyList<Color>>();

            var paletteBlob = config.FetchBlob("palettes");

            foreach (var entry in paletteBlob.KeyValueIteratable) {
                try {
                    var col = ColorMath.ParseString(entry.Key);
                    if (!palettes.ContainsKey(col)) {
                        palettes.Add(col, new List<Color>());
                    }

                    foreach (var colString in entry.Value.List().Select(x => x.GetString())) {
                        try {
                            var coll = ColorMath.ParseString(colString);
                            if (!palettes[col].Contains(coll)) {
                                ((List<Color>)palettes[col]).Add(coll);
                            }
                        } catch {
                            // ignore
                        }
                    }
                } catch {
                    // ignore
                }
            }

            Palettes = palettes;

            IgnoreColors = config.GetStringList("ignoreColors").Select(ColorMath.ParseString).ToList();

            PaletteTolerance = (int) config.GetLong("paletteTolerance", 4);

            BladeLocations = locations;
        }
    }
}
