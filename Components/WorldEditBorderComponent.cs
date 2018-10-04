using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.WorldEdit.Components {
    public class WorldEditBorderComponent {
        public Vector3F PXPZPY { get; }
        public Vector3F PXPZNY { get; }
        public Vector3F PXNZPY { get; }
        public Vector3F PXNZNY { get; }
        public Vector3F NXPZPY { get; }
        public Vector3F NXPZNY { get; }
        public Vector3F NXNZPY { get; }
        public Vector3F NXNZNY { get; }

        public Vector3F PXPZ { get; }
        public Vector3F PXNZ { get; }
        public Vector3F NXPZ { get; }
        public Vector3F NXNZ { get; }

        public Vector3F PXPY { get; }
        public Vector3F PXNY { get; }
        public Vector3F NXPY { get; }
        public Vector3F NXNY { get; }

        public Vector3F PZPY { get; }
        public Vector3F PZNY { get; }
        public Vector3F NZPY { get; }
        public Vector3F NZNY { get; }

        public Vector3F PX { get; }
        public Vector3F NX { get; }

        public Vector3F PZ { get; }
        public Vector3F NZ { get; }

        public Vector3F PY { get; }
        public Vector3F NY { get; }

        public WorldEditBorderComponent(Blob config) {
            PXPZPY = config.Contains("x+z+y+") ? config.FetchBlob("x+z+y+").GetVector3F() : Vector3F.Zero;
            PXPZNY = config.Contains("x+z+y-") ? config.FetchBlob("x+z+y-").GetVector3F() : Vector3F.Zero;
            PXNZPY = config.Contains("x+z-y+") ? config.FetchBlob("x+z-y+").GetVector3F() : Vector3F.Zero;
            PXNZNY = config.Contains("x+z-y-") ? config.FetchBlob("x+z-y-").GetVector3F() : Vector3F.Zero;
            NXPZPY = config.Contains("x-z+y+") ? config.FetchBlob("x-z+y+").GetVector3F() : Vector3F.Zero;
            NXPZNY = config.Contains("x-z+y-") ? config.FetchBlob("x-z+y-").GetVector3F() : Vector3F.Zero;
            NXNZPY = config.Contains("x-z-y+") ? config.FetchBlob("x-z-y+").GetVector3F() : Vector3F.Zero;
            NXNZNY = config.Contains("x-z-y-") ? config.FetchBlob("x-z-y-").GetVector3F() : Vector3F.Zero;

            PXPZ = config.Contains("x+z+") ? config.FetchBlob("x+z+").GetVector3F() : Vector3F.Zero;
            PXNZ = config.Contains("x+z-") ? config.FetchBlob("x+z-").GetVector3F() : Vector3F.Zero;
            NXPZ = config.Contains("x-z+") ? config.FetchBlob("x-z+").GetVector3F() : Vector3F.Zero;
            NXNZ = config.Contains("x-z-") ? config.FetchBlob("x-z-").GetVector3F() : Vector3F.Zero;

            PXPY = config.Contains("x+y+") ? config.FetchBlob("x+y+").GetVector3F() : Vector3F.Zero;
            PXNY = config.Contains("x+y-") ? config.FetchBlob("x+y-").GetVector3F() : Vector3F.Zero;
            NXPY = config.Contains("x-y+") ? config.FetchBlob("x-y+").GetVector3F() : Vector3F.Zero;
            NXNY = config.Contains("x-y-") ? config.FetchBlob("x-y-").GetVector3F() : Vector3F.Zero;

            PZPY = config.Contains("z+y+") ? config.FetchBlob("z+y+").GetVector3F() : Vector3F.Zero;
            PZNY = config.Contains("z+y-") ? config.FetchBlob("z+y-").GetVector3F() : Vector3F.Zero;
            NZPY = config.Contains("z-y+") ? config.FetchBlob("z-y+").GetVector3F() : Vector3F.Zero;
            NZNY = config.Contains("z-y-") ? config.FetchBlob("z-y-").GetVector3F() : Vector3F.Zero;

            PX = config.Contains("x+") ? config.FetchBlob("x+").GetVector3F() : Vector3F.Zero;
            NX = config.Contains("x-") ? config.FetchBlob("x-").GetVector3F() : Vector3F.Zero;

            PZ = config.Contains("z+") ? config.FetchBlob("z+").GetVector3F() : Vector3F.Zero;
            NZ = config.Contains("z-") ? config.FetchBlob("z-").GetVector3F() : Vector3F.Zero;

            PY = config.Contains("y+") ? config.FetchBlob("y+").GetVector3F() : Vector3F.Zero;
            NY = config.Contains("y-") ? config.FetchBlob("y-").GetVector3F() : Vector3F.Zero;
        }
    }
}
