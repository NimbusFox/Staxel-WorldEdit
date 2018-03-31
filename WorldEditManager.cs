using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel.Collections;
using Staxel.Core;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.Voxel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
using NimbusFox.Module.ShortCodes;
using NimbusFox.WorldEdit.Classes;
using NimbusFox.WorldEdit.Enums;
using Staxel;
using Staxel.FoxCore;
using Staxel.FoxCore.Enums;
using Staxel.LivingWorld;
using Staxel.Steam;
using Color = Microsoft.Xna.Framework.Color;

namespace NimbusFox.WorldEdit {
    public class WorldEditManager {
        internal static Fox_Core FoxCore;
        private static Dictionary<Entity, UserData> _positions;
        private static Dictionary<string, RedoUndo> _undoData;

        private static Dictionary<Entity, UserData> PositionClone() {
            return new Dictionary<Entity, UserData>(_positions);
        }

        private static Dictionary<string, RedoUndo> UndoClone() {
            return new Dictionary<string, RedoUndo>(_undoData);
        }

        internal static void Init() {
            FoxCore = new Fox_Core("NimbusFox", "WorldEdit", "V0.1");
            _positions = new Dictionary<Entity, UserData>();
            _undoData = new Dictionary<string, RedoUndo>();
        }

        internal static void Flush() {
            foreach (var item in UndoClone()) {
                FoxCore.DirectoryManager.WriteFile($"{item.Key}.dat", item.Value);
            }
        }

        private static void EntityCheck(Entity entity) {
            if (!_positions.ContainsKey(entity)) {
                _positions.Add(entity, new UserData());
            }

            if (!_undoData.ContainsKey(entity.PlayerEntityLogic.Uid())) {
                var data = new RedoUndo();

                var pause = false;

                if (FoxCore.DirectoryManager.FileExists($"{entity.PlayerEntityLogic.Uid()}.dat")) {
                    pause = true;
                    FoxCore.DirectoryManager.ReadFile<RedoUndo>($"{entity.PlayerEntityLogic.Uid()}.dat", data1 => {
                        data = data1;

                        var dir = FoxCore.DirectoryManager.FetchDirectory(entity.PlayerEntityLogic.Uid());

                        var files = dir.Files;

                        foreach (var id in new List<Guid>(data.ID.Keys)) {
                            if (!files.Contains(id.ToString())) {
                                data.ID.Remove(id);

                                if (data.Redo.Contains(id)) {
                                    data.Redo.Remove(id);
                                }

                                if (data.Undo.Remove(id)) {
                                    data.Undo.Remove(id);
                                }
                            }
                        }

                        if (!UndoClone().ContainsKey(entity.PlayerEntityLogic.Uid())) {
                            _undoData.Add(entity.PlayerEntityLogic.Uid(), data);
                        }

                        pause = false;
                    });
                } else {
                    _undoData.Add(entity.PlayerEntityLogic.Uid(), new RedoUndo());
                }

                while(pause) { }
            }
        }

        private static void CreateRegion(Entity entity) {
            var target = PositionClone()[entity];

            var start = target.Pos1 != default(Vector3D) ? target.Pos1 : target.Pos2;
            var end = target.Pos2 != default(Vector3D) ? target.Pos2 : target.Pos1;

            if (target.RegionGuid != Guid.Empty) {
                FoxCore.ParticleManager.Remove(target.RegionGuid);
            }

            target.RegionGuid = FoxCore.ParticleManager.Add(start, end, "mods.nimbusfox.worldedit.particles.region");
        }

        internal static void AddPos1(Entity entity) {
            EntityCheck(entity);

            PositionClone()[entity].Pos1 = entity.Physics.BottomPosition();

            CreateRegion(entity);
        }

        internal static void AddPos2(Entity entity) {
            EntityCheck(entity);

            PositionClone()[entity].Pos2 = entity.Physics.BottomPosition();

            CreateRegion(entity);
        }

        internal static long Copy(Entity entity) {
            EntityCheck(entity);
            var target = PositionClone()[entity];

            if (target.Pos1 == default(Vector3D) || target.Pos2 == default(Vector3D)) {
                return 0;
            }

            var region = new VectorCubeI(target.Pos1.From3Dto3I(), target.Pos2.From3Dto3I());

            target.ClipBoard.Clear();

            Fox_Core.VectorLoop(target.Pos1.From3Dto3I(), target.Pos2.From3Dto3I(), (x, y, z) => {
                Tile tile;
                if (FoxCore.WorldManager.Universe.World.ReadTile(new Vector3I(x, y, z), TileAccessFlags.SynchronousWait,
                    out tile)) {
                    target.ClipBoard.Add(new Vector3I(x - region.Start.X, y - region.Start.Y, z - region.Start.Z), tile);
                }
            });

            target.ClipBoardOffset = new Vector3D(region.Start.X - entity.Physics.Position.X,
                region.Start.Y - entity.Physics.Position.Y, region.Start.Z - entity.Physics.Position.Z);

            return region.GetTileCount();
        }

        internal static long Paste(Entity entity, bool vectorOverride = false, Vector3I vectorO = default(Vector3I)) {
            EntityCheck(entity);

            var target = PositionClone()[entity];

            if (target.ClipBoard.Any()) {
                var entityVector = entity.Physics.BottomPosition().From3Dto3I();

                if (vectorOverride) {
                    entityVector = vectorO;
                }

                var clipboardVector = target.ClipBoardOffset.From3Dto3I();

                var first = target.ClipBoard.First();
                var last = target.ClipBoard.Last();

                var min = new Vector3I(first.Key.X + clipboardVector.X + entityVector.X,
                    first.Key.Y + clipboardVector.Y + entityVector.Y,
                    first.Key.Z + clipboardVector.Z + entityVector.Z);
                var max = new Vector3I(last.Key.X + clipboardVector.X + entityVector.X,
                    last.Key.Y + clipboardVector.Y + entityVector.Y,
                    last.Key.Z + clipboardVector.Z + entityVector.Z);

                var vc = new VectorCubeI(min, max);

                SaveUndo(entity, vc.Start, vc.End);

                var current = FoxCore.ParticleManager.Add(min, max, "mods.nimbusfox.worldedit.particles.region");

                foreach (var item in target.ClipBoard) {
                    var vector = new Vector3I(entityVector.X + clipboardVector.X + item.Key.X,
                        entityVector.Y + clipboardVector.Y + item.Key.Y,
                        entityVector.Z + clipboardVector.Z + item.Key.Z);

                    var render = new RenderItem {
                        ParticleGuid = current,
                        Location = vector,
                        Tile = item.Value
                    };

                    if (!WorldEditHook.ToRender.ContainsKey(current)) {
                        WorldEditHook.ToRender.Add(current, new List<RenderItem>());
                    }

                    WorldEditHook.ToRender[current].Add(render);

                }

                return target.ClipBoard.Count;
            }

            return 0;
        }

        internal static void Clear(Entity entity) {
            EntityCheck(entity);

            var target = PositionClone()[entity];

            if (target.RegionGuid != Guid.Empty) {
                FoxCore.ParticleManager.Remove(target.RegionGuid);
            }

            target.RegionGuid = Guid.Empty;

            target.Pos1 = default(Vector3D);
            target.Pos2 = default(Vector3D);
        }

        internal static void Replace(Entity entity, string newKindCode, string oldKindCode, out long replacedTiles, out ReplaceResult result) {
            replacedTiles = 0;
            result = ReplaceResult.Success;

            EntityCheck(entity);

            var target = PositionClone()[entity];

            if (target.Pos1 == default(Vector3D) || target.Pos2 == default(Vector3D)) {
                result = ReplaceResult.InvalidPositions;
                return;
            }

            var newTileConfig = GameContext.TileDatabase.AllMaterials().FirstOrDefault(x =>
                string.Equals(x.Code, newKindCode, StringComparison.CurrentCultureIgnoreCase));

            if (newTileConfig == null) {
                result = ReplaceResult.InvalidTile;
                return;
            }

            var newTile = newTileConfig.MakeTile();

            var vc = new VectorCubeI(target.Pos1.From3Dto3I(), target.Pos2.From3Dto3I());

            SaveUndo(entity, vc.Start, vc.End, true, newKindCode);

            var count = 0;

            var current = FoxCore.ParticleManager.Add(target.Pos1, target.Pos2, "mods.nimbusfox.worldedit.particles.region");

            Fox_Core.VectorLoop(target.Pos1, target.Pos2, (x, y, z) => {
                Tile tile;

                if (!WorldEditHook.ToRender.ContainsKey(current)) {
                    WorldEditHook.ToRender.Add(current, new List<RenderItem>());
                }

                var location = new Vector3I(x, y, z);
                if (FoxCore.WorldManager.Universe.ReadTile(location, TileAccessFlags.SynchronousWait,
                    out tile)) {
                    if (!string.IsNullOrWhiteSpace(oldKindCode)) {
                        if (string.Equals(tile.Configuration.Code, oldKindCode,
                            StringComparison.CurrentCultureIgnoreCase)) {
                            WorldEditHook.ToRender[current].Add(new RenderItem {
                                Location = location,
                                ParticleGuid = current,
                                Tile = newTile
                            });
                            count++;
                        }
                    } else {
                        if (tile.Configuration.Code.ToLower() != "staxel.tile.sky") {
                            WorldEditHook.ToRender[current].Add(new RenderItem {
                                Location = location,
                                ParticleGuid = current,
                                Tile = newTile
                            });
                            count++;
                        }
                    }
                }
            });

            replacedTiles = count;
        }

        internal static void Set(Entity entity, string kindCode, out long replacedTiles, out ReplaceResult result) {
            replacedTiles = 0;
            result = ReplaceResult.Success;

            EntityCheck(entity);

            var target = PositionClone()[entity];

            if (target.Pos1 == default(Vector3D) || target.Pos2 == default(Vector3D)) {
                result = ReplaceResult.InvalidPositions;
                return;
            }

            var newTileConfig = GameContext.TileDatabase.AllMaterials().FirstOrDefault(x =>
                string.Equals(x.Code, kindCode, StringComparison.CurrentCultureIgnoreCase));

            if (newTileConfig == null) {
                result = ReplaceResult.InvalidTile;
                return;
            }

            var newTile = newTileConfig.MakeTile();

            var vc = new VectorCubeI(target.Pos1.From3Dto3I(), target.Pos2.From3Dto3I());

            SaveUndo(entity, vc.Start, vc.End, true, kindCode);

            var count = 0;

            var current = FoxCore.ParticleManager.Add(target.Pos1, target.Pos2, "mods.nimbusfox.worldedit.particles.region");

            var list = new List<RenderItem>();

            Fox_Core.VectorLoop(target.Pos1, target.Pos2, (x, y, z) => {
                Tile tile;
                if (FoxCore.WorldManager.Universe.ReadTile(new Vector3I(x, y, z), TileAccessFlags.SynchronousWait,
                    out tile)) {
                    if (!string.Equals(tile.Configuration.Code, kindCode, StringComparison.CurrentCultureIgnoreCase))
                    list.Add(new RenderItem {
                        Location = new Vector3I(x, y, z),
                        ParticleGuid = current,
                        Tile = newTile
                    });
                    count++;
                }
            });

            if (!WorldEditHook.ToRender.ContainsKey(current)) {
                WorldEditHook.ToRender.Add(current, new List<RenderItem>());
            }

            WorldEditHook.ToRender[current].AddRange(list);

            replacedTiles = count;
        }

        internal static void Import(Entity entity, string schematic) {
            EntityCheck(entity);

            var target = PositionClone()[entity];

            var stream = FoxCore.FileManager.ReadFileStream(schematic + ".qb");
            var json = FoxCore.FileManager.ReadFile<FileData>(schematic + ".json", true);

            var qb = VoxelLoader.LoadQb(stream, schematic + ".qb", Vector3I.Zero, json.Size);

            target.ClipBoardOffset = Vector3D.Zero;
            target.ClipBoard.Clear();



            Fox_Core.VectorLoop(new Vector3I(0, 0, 0), qb.Size, (x, y, z) => {
                var color = ColorMath.ToString(qb.Read(x, y, z));

                var mapping = json.Mappings[color];

                if (!GameContext.TileDatabase.AllMaterials().Any(v => string.Equals(mapping.Code, v.Code))) {
                    target.ClipBoard.Clear();
                    throw new Exception("The schematic contains an invalid tile");
                }

                var tileConfig = GameContext.TileDatabase.GetTileConfiguration(mapping.Code);

                target.ClipBoard.Add(new Vector3I(x, y, z), tileConfig.MakeTile(tileConfig.BuildRotationVariant(mapping.Rotation)));
            });
        }

        internal static void Export(Entity entity, string schematic) {
            EntityCheck(entity);

            var target = PositionClone()[entity];

            var cube = new VectorCubeI(target.Pos1.From3Dto3I(), target.Pos2.From3Dto3I());
            var cube0 = Vector3I.Min(new Vector3I(cube.Start.X, cube.Start.Y, cube.Start.Z), new Vector3I(cube.End.X, cube.End.Y, cube.End.Z));
            var cube1 = Vector3I.Max(new Vector3I(cube.Start.X, cube.Start.Y, cube.Start.Z),
                            new Vector3I(cube.End.X, cube.End.Y, cube.End.Z)) - cube0 + Vector3I.One;

            var data = new Dictionary<int, Color>();
            var colorSet = new HashSet<Color>
            {
                Color.Transparent
            };

            var dictionary = new Dictionary<CodeEntry, Color> {
                {
                    new CodeEntry(Constants.SkyCode + "+0", 0U),
                    Color.Transparent
                }, {
                    new CodeEntry(Constants.CompoundCode + "+0", 0U),
                    Color.Transparent
                }, {
                    new CodeEntry(Constants.CompoundCollisionCode + "+0", 0U),
                    Color.Transparent
                }
            };

            var fileData = new FileData { Size = cube1 };

            fileData.Mappings.Add("00000000", new VectorData {
                Code = Constants.SkyCode,
                Rotation = 0
            });

            fileData.Mappings.Add("FF000000", new VectorData {
                Code = Constants.SkyCode,
                Rotation = 0
            });

            var randomSource = GameContext.RandomSource;

            Fox_Core.VectorLoop(new Vector3I(0, 0, 0), cube1, (x, y, z) => {
                var index = x + z * cube1.X + y * cube1.X * cube1.Z;
                Tile tile;
                if (!FoxCore.WorldManager.Universe.ReadTile(cube0 + new Vector3I(x, y, z),
                    TileAccessFlags.SynchronousWait, out tile)) {
                    throw new Exception("Failed to read tile at: " + (cube0 + new Vector3I(x, y, z)) +
                                        ". Tileflags: " + TileAccessFlags.SynchronousWait);
                }

                var rotation = tile.Configuration.Rotation(tile.Variant());
                if (tile.Configuration.CompoundFiller) {
                    rotation = 0U;
                }

                var key = new CodeEntry(tile.Configuration.Code + '+' + rotation, rotation);

                Color color;
                if (!dictionary.TryGetValue(key, out color)) {
                    color = tile.Configuration.ExportColor;
                    var loop = true;
                    while (loop) {
                        while (colorSet.Contains(color)) {
                            color = new Color(randomSource.Next(0, 256), randomSource.Next(0, 256), randomSource.Next(0, 256));
                        }
                        colorSet.Add(color);
                        dictionary.Add(key, color);

                        if (!fileData.Mappings.ContainsKey(ColorMath.ToString(color))) {
                            fileData.Mappings.Add(ColorMath.ToString(color), new VectorData {
                                Code = tile.Configuration.Code,
                                Rotation = rotation
                            });
                            loop = false;
                        }
                    }
                }

                if (!data.ContainsKey(index)) {
                    data.Add(index, color);
                }
            });

            FoxCore.FileManager.WriteFile(schematic + ".json", fileData, true);

            var stream = new MemoryStream();

            var bw = new BinaryWriter(stream);

            bw.Write((byte)1);
            bw.Write((byte)1);
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write(0U);
            bw.Write(0U);
            bw.Write(1U);
            bw.Write(0U);
            bw.Write(1U);
            bw.Write("main");
            bw.Write(cube1.X);
            bw.Write(cube1.Y);
            bw.Write(cube1.Z);
            bw.Write(0);
            bw.Write(0);
            bw.Write(0);
            Action<Color, int> action = (Action<Color, int>)((color, run) => {
                if (run <= 0)
                    return;
                int num = 1;
                if (run > 2) {
                    bw.Write(2U);
                    bw.Write((uint)run);
                } else
                    num = run;
                for (int index = 0; index < num; ++index)
                    bw.Write(color.PackedValue);
            });
            int num1 = 0;
            Color color1 = Color.White;
            int num2 = 0;
            for (; num1 < cube1.Z; ++num1) {
                for (int index = 0; index < cube1.X * cube1.Y; ++index) {
                    int num3 = index % cube1.X;
                    int num4 = index / cube1.X;
                    Color transparent = data[num3 + num1 * cube1.X + num4 * cube1.X * cube1.Z];
                    if (((int)transparent.PackedValue & -16777216) == 0)
                        transparent = Color.Transparent;
                    else
                        transparent.PackedValue |= 4278190080U;
                    if (transparent != color1) {
                        action(color1, num2);
                        num2 = 0;
                    }
                    color1 = transparent;
                    ++num2;
                }
                action(color1, num2);
                num2 = 0;
                bw.Write(6U);
            }

            bw.Flush();
            stream.Seek(0L, SeekOrigin.Begin);

            FoxCore.FileManager.WriteFileStream(schematic + ".qb", stream);
        }

        internal static bool Platform(Entity entity, int size, out long tileCount, string tile = "staxel.tile.dirt.dirt") {
            var parseSize = size <= 0 ? 0 : size;
            tileCount = 0;

            if (!TileShortCodes.IsValidTile(tile)) {
                return false;
            }

            var newTile = TileShortCodes.GetTile(tile);

            var current = entity.Physics.BottomPosition().From3Dto3I();

            var start = new Vector3I(current.X - parseSize, current.Y - 1, current.Z - parseSize);
            var end = new Vector3I(current.X + parseSize, current.Y - 1, current.Z + parseSize);

            var count = 0;

            var vc = new VectorCubeI(start, end);

            SaveUndo(entity, vc.Start, vc.End, true, tile);

            Fox_Core.VectorLoop(start, end, (x, y, z) => {
                Tile currentTile;
                if (FoxCore.WorldManager.World.ReadTile(new Vector3I(x, y, z), TileAccessFlags.SynchronousWait,
                    out currentTile)) {
                    if (string.Equals(currentTile.Configuration.Code, "staxel.tile.Sky",
                        StringComparison.CurrentCultureIgnoreCase)) {
                        if (FoxCore.WorldManager.World.PlaceTile(new Vector3I(x, y, z), newTile,
                            TileAccessFlags.SynchronousWait)) {
                            count++;
                        }
                    }
                }
            });

            tileCount = count;

            return true;
        }

        internal static bool Wall(Entity entity, int width, int height, out long tileCount, string tile = "staxel.tile.dirt.dirt") {
            var parseWidth = width <= 0 ? 0 : width;
            var parseHeight = height <= 0 ? 0 : height;
            tileCount = 0;

            if (!TileShortCodes.IsValidTile(tile)) {
                return false;
            }

            var newTile = TileShortCodes.GetTile(tile);

            var facing = entity.PlayerEntityLogic.Heading().GetDirection();

            Vector3I center;
            Vector3I start;
            Vector3I end;

            var position = entity.Physics.BottomPosition().From3Dto3I();

            if (facing == Compass.NORTH || facing == Compass.SOUTH) {
                center = facing == Compass.NORTH ? new Vector3I(position.X, position.Y, position.Z - 1) : new Vector3I(position.X, position.Y, position.Z + 1);
            } else {
                center = facing == Compass.EAST ? new Vector3I(position.X + 1, position.Y, position.Z) : new Vector3I(position.X - 1, position.Y, position.Z);
            }

            var div = (float)parseWidth / 2;

            var side1 = (int)Math.Floor(div);
            var side2 = (int)Math.Floor(div);

            if (facing == Compass.NORTH || facing == Compass.SOUTH) {
                start = new Vector3I(center.X - side1, center.Y, center.Z);
                end = new Vector3I(center.X + side2, center.Y + parseHeight, center.Z);
            } else {
                start = new Vector3I(center.X, center.Y, center.Z - side1);
                end = new Vector3I(center.X, center.Y + parseHeight, center.Z + side2);
            }

            var vc = new VectorCubeI(start, end);

            SaveUndo(entity, vc.Start, vc.End, true, tile);

            var current = FoxCore.ParticleManager.Add(start, end, "mods.nimbusfox.worldedit.particles.region");

            var count = 0;

            var list = new List<RenderItem>();

            Fox_Core.VectorLoop(start, end, (x, y, z) => {
                list.Add(new RenderItem {
                    Location = new Vector3I(x, y, z),
                    ParticleGuid = current,
                    Tile = newTile
                });
                count++;
            });

            if (!WorldEditHook.ToRender.ContainsKey(current)) {
                WorldEditHook.ToRender.Add(current, new List<RenderItem>());
            }

            WorldEditHook.ToRender[current].AddRange(list);

            tileCount = count;

            return true;
        }

        internal static UndoRedoResult Undo(Entity entity) {
            EntityCheck(entity);

            var files = FoxCore.DirectoryManager.FetchDirectory(entity.PlayerEntityLogic.Uid());

            var undoRedo = UndoClone()[entity.PlayerEntityLogic.Uid()];

            if (!undoRedo.Undo.Any()) {
                return UndoRedoResult.NoUndoRedo;
            }

            var last = undoRedo.Undo.Last();

            var id = undoRedo.ID[last];

            if (!WorldEditHook.CheckIfCanEdit(id.Start, id.End)) {
                return UndoRedoResult.NotFinished;
            }

            var current = FoxCore.ParticleManager.Add(id.Start, id.End,
                "mods.nimbusfox.worldedit.particles.region");

            if (!files.FileExists(last.ToString())) {

            }

            WorldEditHook.ForbidEditing.Add(current, id);

            SaveUndo(entity, id.Start, id.End, false);

            files.ReadFile<List<UndoData>>(last.ToString(), data => {

                var list = new List<RenderItem>();
                foreach (var item in data) {
                    list.Add(new RenderItem {
                        Location = item.Location,
                        ParticleGuid = current,
                        Tile = GameContext.TileDatabase.GetTileConfiguration(item.Tile.TileCode).MakeTile(item.Tile.Variant)
                    });
                    Tile oldTile;
                    if (FoxCore.WorldManager.World.ReadTile(item.Location, TileAccessFlags.SynchronousWait, out oldTile)) {
                        item.Tile.TileCode = oldTile.Configuration.Code;
                        item.Tile.Variant = oldTile.Variant();
                    }
                }

                if (!WorldEditHook.ToRender.ContainsKey(current)) {
                    WorldEditHook.ToRender.Add(current, list);
                } else {
                    WorldEditHook.ToRender[current].AddRange(list);
                }

                files.DeleteFile(last.ToString());
                undoRedo.ID.Remove(last);
                undoRedo.Undo.Remove(last);

                Flush();
            });

            return UndoRedoResult.Success;
        }

        internal static UndoRedoResult Redo(Entity entity) {
            EntityCheck(entity);

            var files = FoxCore.DirectoryManager.FetchDirectory(entity.PlayerEntityLogic.Uid());

            var undoRedo = UndoClone()[entity.PlayerEntityLogic.Uid()];

            if (!undoRedo.Redo.Any()) {
                return UndoRedoResult.NoUndoRedo;
            }

            var last = undoRedo.Redo.Last();

            var id = undoRedo.ID[last];

            if (!WorldEditHook.CheckIfCanEdit(id.Start, id.End)) {
                return UndoRedoResult.NotFinished;
            }

            var current = FoxCore.ParticleManager.Add(id.Start, id.End,
                "mods.nimbusfox.worldedit.particles.region");

            if (!files.FileExists(last.ToString())) {

            }

            WorldEditHook.ForbidEditing.Add(current, id);

            SaveUndo(entity, id.Start, id.End);

            files.ReadFile<List<UndoData>>(last.ToString(), data => {

                var list = new List<RenderItem>();

                foreach (var item in data) {
                    list.Add(new RenderItem {
                        Location = item.Location,
                        ParticleGuid = current,
                        Tile = GameContext.TileDatabase.GetTileConfiguration(item.Tile.TileCode).MakeTile(item.Tile.Variant)
                    });
                    Tile oldTile;
                    if (FoxCore.WorldManager.World.ReadTile(item.Location, TileAccessFlags.SynchronousWait, out oldTile)) {
                        item.Tile.TileCode = oldTile.Configuration.Code;
                        item.Tile.Variant = oldTile.Variant();
                    }
                }

                if (!WorldEditHook.ToRender.ContainsKey(current)) {
                    WorldEditHook.ToRender.Add(current, list);
                } else {
                    WorldEditHook.ToRender[current].AddRange(list);
                }

                files.DeleteFile(last.ToString());
                undoRedo.ID.Remove(last);
                undoRedo.Redo.Remove(last);

                Flush();
            });

            return UndoRedoResult.Success;
        }

        internal static void Stack(Entity entity, int repeat, string direction = "forwards") {
            EntityCheck(entity);

            var target = PositionClone()[entity];

            if (target.Pos1 == default(Vector3D) || target.Pos2 == default(Vector3D)) {
                return;
            }

            var targetcube = new VectorCubeI(target.Pos1.From3Dto3I(), target.Pos2.From3Dto3I());

            var facing = entity.Logic.Heading().GetDirection();

            switch (direction.ToLower()) {
                case "forwards":
                    break;
                case "left":
                    if (facing == Compass.EAST) {
                        facing = Compass.NORTH;
                    } else if (facing == Compass.NORTH) {
                        facing = Compass.WEST;
                    } else if (facing == Compass.WEST) {
                        facing = Compass.SOUTH;
                    } else if (facing == Compass.SOUTH) {
                        facing = Compass.EAST;
                    }

                    break;
                case "right":
                    if (facing == Compass.EAST) {
                        facing = Compass.SOUTH;
                    } else if (facing == Compass.SOUTH) {
                        facing = Compass.WEST;
                    } else if (facing == Compass.WEST) {
                        facing = Compass.NORTH;
                    } else if (facing == Compass.NORTH) {
                        facing = Compass.EAST;
                    }

                    break;
                case "backwards":
                    if (facing == Compass.EAST) {
                        facing = Compass.WEST;
                    } else if (facing == Compass.WEST) {
                        facing = Compass.EAST;
                    } else if (facing == Compass.NORTH) {
                        facing = Compass.SOUTH;
                    } else if (facing == Compass.SOUTH) {
                        facing = Compass.NORTH;
                    }

                    break;
                case "up":
                    facing = Compass.UP;
                    break;
                case "down":
                    facing = Compass.DOWN;
                    break;
                default:
                    return;
            }

            var oldClipboard = target.ClipBoard;

            Copy(entity);

            var cube = new VectorCubeI(target.ClipBoard.Keys.First(), target.ClipBoard.Keys.Last());

            var oldOffset = target.ClipBoardOffset;

            var x = cube.End.X - cube.Start.X;
            var y = cube.End.Y - cube.Start.Y;
            var z = cube.End.Z - cube.Start.Z;

            for (var i = 1; i <= repeat; i++) {
                if (facing == Compass.NORTH) {
                    target.ClipBoardOffset = new Vector3D(0, 0, -z * i);
                } else if (facing == Compass.EAST) {
                    target.ClipBoardOffset = new Vector3D(x * i, 0, 0);
                } else if (facing == Compass.SOUTH) {
                    target.ClipBoardOffset = new Vector3D(0, 0, z * i);
                } else if (facing == Compass.WEST) {
                    target.ClipBoardOffset = new Vector3D(-x * i, 0, 0);
                } else if (facing == Compass.UP) {
                    target.ClipBoardOffset = new Vector3D(0, y * i, 0);
                } else if (facing == Compass.DOWN) {
                    target.ClipBoardOffset = new Vector3D(0, -y * i, 0);
                }

                Paste(entity, true, targetcube.Start);
            }

            target.ClipBoardOffset = oldOffset;
            target.ClipBoard = oldClipboard;
        }

        public static void SaveUndo(Entity entity, Vector3I start, Vector3I end, bool undo = true, string exclude = null) {
            EntityCheck(entity);
            var undoRedo = UndoClone()[entity.PlayerEntityLogic.Uid()];

            undoRedo.Redo.Clear();

            var old = new List<UndoData>();

            Fox_Core.VectorLoop(start, end, (x, y, z) => {
                Tile tile;
                var vector = new Vector3I(x, y, z);
                if (FoxCore.WorldManager.World.ReadTile(vector, TileAccessFlags.None, out tile)) {
                    if (tile.Configuration.Code.ToLower() != exclude?.ToLower())
                    old.Add(new UndoData {
                        Location = vector,
                        Tile = new TileData {
                            TileCode = tile.Configuration.Code,
                            Variant = tile.Variant()
                        }
                    });
                }
            });

            var dir = FoxCore.DirectoryManager.FetchDirectory(entity.PlayerEntityLogic.Uid());

            var key = Guid.NewGuid();

            dir.WriteFile(key.ToString(), old, () => {
                if (undo) {
                    undoRedo.Undo.Add(key);
                } else {
                    undoRedo.Redo.Add(key);
                }

                undoRedo.ID.Add(key, new VectorCubeI(start, end));

                Flush();
            });
        }
    }
}
