using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plukit.Base;

namespace NimbusFox.WorldEdit {
    public class WEConstants {
        public static Vector3D CLNE => new Vector3D(MathHelper.ToRadians(90), 0, 0);
        public static Vector3D CLSE => new Vector3D(MathHelper.ToRadians(270), 0, 0);
        public static Vector3D CLNW => new Vector3D(MathHelper.ToRadians(180), 0, 0);
        public static Vector3D CLSW => new Vector3D(MathHelper.ToRadians(0), 0, 0);
    }
}
