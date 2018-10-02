using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.WorldEdit.Components {
    public class BotComponent {
        public Vector3D TileOffset { get; }
        public Vector3D BoundingBox { get; }
        public string BladeModel { get; }
        public IReadOnlyList<(Vector3D, float)> BladeLocations { get; }

        public BotComponent(Blob config) {
            TileOffset = config.FetchBlob("tileOffset").GetVector3D();
            var boundingBox = config.FetchBlob("boundingBox");
            BoundingBox = new Vector3D(boundingBox.GetDouble("x", 0.5), boundingBox.GetDouble("y", 0.5), boundingBox.GetDouble("z", 0.5));
            BladeModel = config.GetString("bladeModel");

            var locations = new List<(Vector3D, float)>();

            foreach (var list in config.GetList("bladeLocations")) {
                locations.Add((list.Blob().GetVector3D(), (float)list.Blob().GetDouble("scale")));
            }

            BladeLocations = locations;
        }
    }
}
