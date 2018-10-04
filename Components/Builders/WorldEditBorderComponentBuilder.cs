using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Core;

namespace NimbusFox.WorldEdit.Components.Builders {
    public class WorldEditBorderComponentBuilder : IComponentBuilder {
        public string Kind() {
            return "worldEditBorder";
        }

        public object Instance(Blob config) {
            return new WorldEditBorderComponent(config);
        }
    }
}
