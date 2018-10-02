using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.WorldEdit.Entities.Bot {
    public class BorderBotEntityLogic : BotEntityLogic {
        protected Vector3I Pos1 { get; private set; }
        protected Vector3I Pos2 { get; private set; }
        public string BorderTile { get; private set; }
        public BorderBotEntityLogic(Entity entity) : base(entity) { }

        public void SetPos1(Vector3I pos1) {
            Pos1 = pos1;
        }

        public void SetPos2(Vector3I pos2) {
            Pos2 = pos2;
        }

        public void SetBorderTile(string tile) {
            BorderTile = tile;
        }


    }
}
