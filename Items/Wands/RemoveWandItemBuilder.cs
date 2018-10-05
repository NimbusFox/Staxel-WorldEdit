using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.WorldEdit.Items.Wands {
    public class RemoveWandItemBuilder : BaseBotSpawnItemBuilder {
        public override string ItemKind { get; } = "nimbusfox.worldedit.item.wand.remove";
        public override Item Build(Blob blob, ItemConfiguration configuration, Item spare) {
            return new RemoveWandItem(this, configuration);
        }
    }
}
