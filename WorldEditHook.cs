﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit {
    internal class WorldEditHook : IFoxModHookV3 {
        internal static WorldEditHook Instance { get; private set; }

        public Fox_Core FxCore { get; }

        public WorldEditHook() {
            Instance = this;
            FxCore = new Fox_Core("NimbusFox", "WorldEdit", "v2");
        }

        public void Dispose() { }
        public void GameContextInitializeInit() { }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() { }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }
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
    }
}
