using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.WorldEdit.Components {
    public class SelectionWandComponent {
        public string BorderLineTile { get; }
        public string BorderCornerTile { get; }
        public SelectionWandComponent(Blob config) {
            BorderLineTile = config.GetString("borderLineTile");
            BorderCornerTile = config.GetString("borderCornerTile");
        }
    }
}
