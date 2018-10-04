using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
using NimbusFox.WorldEdit.Components;
using Plukit.Base;
using Staxel;
using Staxel.Draw;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit {
    internal class WorldEditHook : IFoxModHookV3 {
        internal static WorldEditHook Instance { get; private set; }

        public static Dictionary<string, Dictionary<int, (int r, int g, int b)>> Cache = new Dictionary<string, Dictionary<int, (int r, int g, int b)>>();

        private static bool _cleanCache = true;

        public Fox_Core FxCore { get; }

        public WorldEditHook() {
            Instance = this;
            FxCore = new Fox_Core("NimbusFox", "WorldEdit", "v2");
        }

        public void Dispose() { }
        public void GameContextInitializeInit() { }
        public void GameContextInitializeBefore() { }

        public void GameContextInitializeAfter() {
        }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }

        public void GameContextReloadAfter() {
            _cleanCache = true;
            CreateCache();
        }
        public void UniverseUpdateBefore(Universe universe, Timestep step) { }
        public void UniverseUpdateAfter() { }
        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
            return true;
        }

        public void ClientContextInitializeInit() { }
        public void ClientContextInitializeBefore() { }
        public void ClientContextInitializeAfter() { }
        public void ClientContextDeinitialize() { }
        public void ClientContextReloadBefore() { }
        public void ClientContextReloadAfter() { }
        public void CleanupOldSession() { }
        public bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
            return true;
        }

        public bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
            return true;
        }

        public void OnPlayerLoadAfter(Blob blob) { }
        public void OnPlayerSaveBefore(PlayerEntityLogic logic, out Blob saveBlob) {
            saveBlob = null;
        }

        public void OnPlayerSaveAfter(PlayerEntityLogic logic, out Blob saveBlob) {
            saveBlob = null;
        }

        public void OnPlayerConnect(Entity entity) { }
        public void OnPlayerDisconnect(Entity entity) { }

        public void CreateCache() {
            if (_cleanCache) {
                Cache = new Dictionary<string, Dictionary<int, (int r, int g, int b)>>();

                foreach (var tile in GameContext.TileDatabase.AllMaterials()
                    .Where(x => x.Components.Contains<BotComponent>())) {
                    var data = new Dictionary<int, (int r, int g, int b)>();

                    var component = tile.Components.Get<BotComponent>();

                    var toLook = component.Palettes.Keys;

                    if (tile.Icon == null) {
                        return;
                    }

                    var bot = tile.Icon.GetPrivateFieldValue<CompactVertexDrawable>("_drawable");

                    var list = bot.CompileToVoxelVertexArray();

                    for (var i = 0; i < list.Length; i++) {
                        var colSelected = false;
                        var col = list[i].Color;
                        foreach (var color in toLook) {
                            if (colSelected) {
                                break;
                            }
                            for (var r = -component.PaletteTolerance; r <= component.PaletteTolerance; r++) {
                                if (colSelected) {
                                    break;
                                }
                                for (var g = -component.PaletteTolerance; g <= component.PaletteTolerance; g++) {
                                    if (colSelected) {
                                        break;
                                    }
                                    for (var b = -component.PaletteTolerance; b <= component.PaletteTolerance; b++) {
                                        if (colSelected) {
                                            break;
                                        }

                                        var curCol = new Color(color.R + r, color.G + g, color.B + b);

                                        if (col.R == curCol.R && col.G == curCol.G && col.B == curCol.B) {
                                            data.Add(i, (r, g, b));
                                            colSelected = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Cache.Add(tile.Code, data);
                }

                _cleanCache = false;
            }
        }
    }
}
