using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.WorldEdit.Components {
    public class SelectionWandComponent {
        public string BorderTile { get; }
        public SelectionWandComponent(Blob config) {
            BorderTile = config.GetString("border");
        }
    }
}
