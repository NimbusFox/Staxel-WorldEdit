using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.WorldEdit.Items.Wands {
    public class SelectionWandItemBuilder : IItemBuilder {
        public static string KindCode => "nimbusfox.worldedit.item.wand.selection";

        public ItemRenderer Renderer { get; private set; }

        public void Dispose() { }

        public void Load() {
            Renderer = new ItemRenderer();
        }
        public Item Build(Blob blob, ItemConfiguration configuration, Item spare) {
            return new SelectionWandItem(this, configuration);
        }

        public string Kind() {
            return KindCode;
        }
    }
}
