using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Classes {
    public class RenderItem {
        public Tile Tile { get; set; }
        public Vector3I Location { get; set; }
        public Guid ParticleGuid { get; set; }
    }
}