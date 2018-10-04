using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.WorldEdit.Components {
    public class TileEntityComponent {
        public Vector3F TileOffset { get; }
        public float Scale { get; }
        public TileEntityComponent(Blob config) {
            TileOffset = config.FetchBlob("tileOffset").GetVector3F();
            Scale = (float)config.GetDouble("scale");
        }
    }
}
