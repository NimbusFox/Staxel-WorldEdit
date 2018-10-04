using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.WorldEdit.Components {
    public class SelectionWandComponent {
        public string SelectionPoint1 { get; }
        public string SelectionPoint2 { get; }
        public string SelectionStraight { get; }
        public string SelectionL { get; }
        public string SelectionCorner { get; }
        public SelectionWandComponent(Blob config) {
            SelectionPoint1 = config.GetString("selectionPoint1");
            SelectionPoint2 = config.GetString("selectionPoint2");
            SelectionStraight = config.GetString("selectionStraight");
            SelectionL = config.GetString("selectionL");
            SelectionCorner = config.GetString("selectionCorner");
        }
    }
}
