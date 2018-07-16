using System;
using System.Collections.Generic;
using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Classes {
    public class UserData {
        public Vector3D Pos1 { get; set; }
        public Vector3D Pos2 { get; set; }
        public Dictionary<Vector3I, Tile> ClipBoard { get; set; }
        public Vector3D ClipBoardOffset { get; set; }
        public Guid RegionGuid { get; set; }

        public UserData() {
            ClipBoard = new Dictionary<Vector3I, Tile>();
            ClipBoardOffset = new Vector3D(0, 0, 0);
            RegionGuid = Guid.Empty;
        }
    }
}
