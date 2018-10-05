using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.WorldEdit.Items.Wands {
    public abstract class BaseBotSpawnItemBuilder : IItemBuilder {
        public abstract string ItemKind { get; }
        public ItemRenderer Renderer;
        public void Dispose() { }

        public void Load() {
            Renderer = new ItemRenderer();
        }
        public abstract Item Build(Blob blob, ItemConfiguration configuration, Item spare);

        public string Kind() {
            return ItemKind;
        }
    }
}
