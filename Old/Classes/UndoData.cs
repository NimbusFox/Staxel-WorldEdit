using System;
using Plukit.Base;

namespace NimbusFox.WorldEdit.Classes {
    [Serializable]
    public class UndoData {
        public Vector3I Location { get; set; }
        public TileData Tile { get; set; }
    }
}
