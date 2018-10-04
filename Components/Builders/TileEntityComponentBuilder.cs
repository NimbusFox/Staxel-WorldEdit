using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Core;

namespace NimbusFox.WorldEdit.Components.Builders {
    public class TileEntityComponentBuilder : IComponentBuilder {
        public string Kind() {
            return "tileEntity";
        }

        public object Instance(Blob config) {
            return new TileEntityComponent(config);
        }
    }
}
