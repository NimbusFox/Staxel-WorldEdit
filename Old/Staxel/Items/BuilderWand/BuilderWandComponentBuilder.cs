using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Core;

namespace NimbusFox.WorldEdit.Staxel.Items.BuilderWand {
    public class BuilderWandComponentBuilder : IComponentBuilder {
        public string Kind() {
            return KindCode;
        }

        public static string KindCode {
            get { return "builderwand"; }
        }

        public object Instance(Blob config) {
            return new BuilderWandComponent(config);
        }
    }
}
