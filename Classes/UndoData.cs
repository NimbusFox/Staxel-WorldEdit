using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Classes {
    [Serializable]
    public class UndoData {
        public Vector3I Location { get; set; }
        public TileData Tile { get; set; }
    }
}
