using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.Module.ShortCodes;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Classes {
    internal static class Tiles {
        internal static FrameTiles Default;

        static Tiles() {
            Default = new FrameTiles(
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.default.line.XZ"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.default.line.Y"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.default.l.Side"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.default.l.Up"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.default.l.Down"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.default.corner.Up"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.default.corner.down")
            );
        }
    }
}
