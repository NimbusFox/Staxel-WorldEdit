using System.Collections.Generic;
using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Classes {
    internal class FrameRegion {
        internal class FrameRegionInfo {
            public Vector3I Location { get; set; }
            public Tile Tile { get; set; }
        }

        internal List<FrameRegionInfo> Info { get; set; }

        internal FrameRegion() {
            Info = new List<FrameRegionInfo>();
        }
    }
}
