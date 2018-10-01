using System;
using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Classes {
    public class RenderItem {
        public Tile Tile { get; set; }
        public Vector3I Location { get; set; }
        public Guid ParticleGuid { get; set; }
        public string UserUID { get; set; }
    }
}