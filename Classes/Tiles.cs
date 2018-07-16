using NimbusFox.Module.ShortCodes;

namespace NimbusFox.WorldEdit.Classes {
    internal static class Tiles {
        internal static FrameTiles Default;
        internal static FrameTiles Custom;

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

            Custom = new FrameTiles(
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.custom.line.XZ"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.custom.line.Y"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.custom.l.Side"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.custom.l.Up"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.custom.l.Down"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.custom.corner.Up"),
                TileShortCodes.GetTile("mods.NimbusFox.WorldEdit.Maxigregrze.frame.custom.corner.down")
            );
        }

        internal static FrameTiles GetTiles() {
            return Custom.Initialized ? Custom : Default;
        }
    }
}
