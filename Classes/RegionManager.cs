using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
using NimbusFox.Module.ShortCodes;
using Plukit.Base;
using Staxel.Items;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Classes {
    internal class RegionManager : IDisposable {
        private Dictionary<Guid, FrameRegion> Regions;

        internal RegionManager() {
            Regions = new Dictionary<Guid, FrameRegion>();
        }

        private Dictionary<Guid, FrameRegion> CloneRegions() {
            return new Dictionary<Guid, FrameRegion>(Regions);
        }

        private void Add(Guid key, FrameRegion.FrameRegionInfo item) {
            if (!Regions.ContainsKey(key)) {
                Regions.Add(key, new FrameRegion());
            }

            if (!Regions[key].Info.Any(x => x.Location == item.Location)) {
                Regions[key].Info.Add(item);
            }

            var loop = true;

            while (loop) {
                loop = !WorldEditManager.FoxCore.WorldManager.World.PlaceTile(item.Location, item.Tile, TileAccessFlags.SynchronousWait);
            }
        }

        //internal void RenderRegions() {
        //    foreach (var toRender in CloneRegions().Where(x => !x.Value.Rendered || x.Value.Remove)) {
        //        foreach (var item in toRender.Value.Info.Where(x => !x.Rendered || (toRender.Value.Remove && !x.Removed)).Take(25)) {
        //            if (!item.Rendered) {
        //                toRender.Value.Rendered =
        //                    WorldEditManager.FoxCore.WorldManager.World.PlaceTile(item.Location, item.Tile,
        //                        TileAccessFlags.None);
        //            } else {
        //                if (toRender.Value.Remove) {
        //                    if (WorldEditManager.FoxCore.WorldManager.World.PlaceTile(item.Location,
        //                        TileShortCodes.GetTile("staxel.tile.Sky"), TileAccessFlags.SynchronousWait)) {
        //                        item.Removed = true;
        //                    }
        //                }
        //            }
        //        }

        //        if (!toRender.Value.Info.Any(x => !x.Rendered)) {
        //            toRender.Value.Rendered = true;
        //        }

        //        if (!toRender.Value.Info.Any(x => !x.Removed)) {
        //            Regions.Remove(toRender.Key);
        //        }
        //    }
        //}

        internal Guid AddCube(Vector3I start, Vector3I end, FrameTiles tiles) {
            var guid = Guid.NewGuid();

            var cube = new VectorCubeI(start, end).GetOuterRegions();

            Helpers.VectorLoop(cube.Start, cube.End, (x, y, z) => {
                var renderCount = 0;

                renderCount += y == cube.Start.Y || y == cube.End.Y ? 1 : 0;

                renderCount += z == cube.Start.Z || z == cube.End.Z ? 1 : 0;

                renderCount += x == cube.Start.X || x == cube.End.X ? 1 : 0;

                if (renderCount > 1) {
                    Tile tile;
                    var current = new Vector3I(x, y, z);
                    loop:
                    if (WorldEditManager.FoxCore.WorldManager.Universe.ReadTile(current, TileAccessFlags.SynchronousWait, out tile)) {
                        if (tile.Configuration.Code.ToLower() == "staxel.tile.sky") {
                            Tile? cT = null;

                            if ((x == cube.Start.X || x == cube.End.X) && (z == cube.Start.Z || z == cube.End.Z)) {
                                cT = tiles.Line.Y;
                            }

                            if (z == cube.Start.Z || z == cube.End.Z) {
                                if (cT == null) {
                                    cT = tiles.Line.Z;
                                } else {
                                    if (z == cube.Start.Z) {
                                        if (y == cube.Start.Y) {
                                            cT = tiles.L.Up.Z;
                                        } else if (y == cube.End.Y) {
                                            cT = tiles.L.Down.Z;
                                        }
                                    } else if (z == cube.End.Z) {
                                        if (y == cube.Start.Y) {
                                            cT = tiles.L.Up.NZ;
                                        } else if (y == cube.End.Y) {
                                            cT = tiles.L.Down.NZ;
                                        }
                                    }
                                }
                            }

                            if (x == cube.Start.X || x == cube.End.X) {
                                if (cT == null) {
                                    cT = tiles.Line.X;
                                } else {
                                    if (x == cube.Start.X) {
                                        if (cT == tiles.Line.Y) {
                                            if (y == cube.Start.Y) {
                                                cT = tiles.L.Up.X;
                                            } else if (y == cube.End.Y) {
                                                cT = tiles.L.Down.X;
                                            }
                                        } else if (cT == tiles.Line.Z) {
                                            if (z == cube.Start.Z) {
                                                cT = tiles.L.Side.NE;
                                            } else if (z == cube.End.Z) {
                                                cT = tiles.L.Side.SW;
                                            }
                                        } else if (cT == tiles.L.Up.Z) {
                                            cT = tiles.Corner.Up.NE;
                                        } else if (cT == tiles.L.Down.Z) {
                                            cT = tiles.Corner.Down.NE;
                                        } else if (cT == tiles.L.Up.NZ) {
                                            cT = tiles.Corner.Up.WN;
                                        } else if (cT == tiles.L.Down.NZ) {
                                            cT = tiles.Corner.Down.WN;
                                        }
                                    } else if (x == cube.End.X) {
                                        if (cT == tiles.Line.Y) {
                                            if (y == cube.Start.Y) {
                                                cT = tiles.L.Up.NX;
                                            } else if (y == cube.End.Y) {
                                                cT = tiles.L.Down.NX;
                                            }
                                        } else if (cT == tiles.Line.Z) {
                                            if (z == cube.Start.Z) {
                                                cT = tiles.L.Side.ES;
                                            } else if (z == cube.End.Z) {
                                                cT = tiles.L.Side.WN;
                                            }
                                        } else if (cT == tiles.L.Up.Z) {
                                            cT = tiles.Corner.Up.ES;
                                        } else if (cT == tiles.L.Down.Z) {
                                            cT = tiles.Corner.Down.ES;
                                        } else if (cT == tiles.L.Up.NZ) {
                                            cT = tiles.Corner.Up.SW;
                                        } else if (cT == tiles.L.Down.NZ) {
                                            cT = tiles.Corner.Down.SW;
                                        }
                                    }
                                }
                            }

                            if (cT != null) {
                                Add(guid, new FrameRegion.FrameRegionInfo {
                                    Tile = cT.Value,
                                    Location = current
                                });
                            }

                        }
                    } else {
                        goto loop;
                    }
                }
            });

            return guid;
        }

        internal Guid Add(Vector3I location, FrameTiles tiles) {
            return AddCube(location, location, tiles);
        }

        internal void Remove(Guid key) {
            if (Regions.ContainsKey(key)) {
                while (CloneRegions()[key].Info.Any()) {
                    foreach (var region in new List<FrameRegion.FrameRegionInfo>(CloneRegions()[key].Info)) {
                        if (WorldEditManager.FoxCore.WorldManager.World.PlaceTile(region.Location,
                            TileShortCodes.GetTile("staxel.tile.Sky"), TileAccessFlags.None)) {
                            Regions[key].Info.Remove(region);
                        }
                    }
                }

                Regions.Remove(key);
            }
        }

        public void Dispose() {
            foreach (var key in CloneRegions().Keys) {
                Remove(key);
            }
        }
    }
}
