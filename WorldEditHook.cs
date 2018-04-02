using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
using NimbusFox.WorldEdit.Classes;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit {
    public class WorldEditHook : IModHookV2 {
        internal static Dictionary<Guid, List<RenderItem>> ToRender;
        internal static long NextTick;
        internal static Dictionary<Guid, VectorCubeI> ForbidEditing;

        public void Dispose() {
            WorldEditManager.FoxCore.ParticleManager.Dispose();
            WorldEditManager.FoxCore.EntityParticleManager.Dispose();
            WorldEditManager.FoxCore.EntityFollowParticleManager.Dispose();
        }

        public void GameContextInitializeInit() {
            ToRender = new Dictionary<Guid, List<RenderItem>>();
            ForbidEditing = new Dictionary<Guid, VectorCubeI>();
            NextTick = 0;
            WorldEditManager.Init();
        }
        public void GameContextInitializeBefore() { }

        public void GameContextInitializeAfter() {
        }
        public void GameContextDeinitialize() {
            WorldEditManager.RegionManager.Dispose();
        }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            WorldEditManager.FoxCore.ParticleManager.DrawParticles();
            WorldEditManager.FoxCore.EntityParticleManager.DrawParticles();
            WorldEditManager.FoxCore.EntityFollowParticleManager.DrawParticles();

            foreach (var toRender in new Dictionary<Guid, List<RenderItem>>(ToRender)) {
                foreach (var render in new List<RenderItem>(toRender.Value).Take(25)) {
                    if (universe.World.PlaceTile(render.Location, render.Tile, TileAccessFlags.None)) {
                        toRender.Value.Remove(render);
                        if (!toRender.Value.Any()) {
                            ToRender.Remove(toRender.Key);
                            ForbidEditing.Remove(toRender.Key);
                            WorldEditManager.FoxCore.ParticleManager.Remove(toRender.Key);
                        }
                    }
                }
            }
        }

        public static bool CheckIfCanEdit(Vector3I location) {
            return !new Dictionary<Guid, VectorCubeI>(ForbidEditing).Any(x => x.Value.IsInside(location));
        }

        public static bool CheckIfCanEdit(Vector3I start, Vector3I end) {
            var canEdit = true;

            if (!ForbidEditing.Any()) {
                return true;
            }

            Fox_Core.VectorLoop(start, end, (x, y, z) => { canEdit &= !CheckIfCanEdit(new Vector3I(x, y, z)); });

            return canEdit;
        }

        public void UniverseUpdateAfter() { }
        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return CheckIfCanEdit(location);
        }

        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return CheckIfCanEdit(location);
        }

        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
            return CheckIfCanEdit(location);
        }

        public void ClientContextInitializeInit() { }
        public void ClientContextInitializeBefore() { }
        public void ClientContextInitializeAfter() { }
        public void ClientContextDeinitialize() { }
        public void ClientContextReloadBefore() { }
        public void ClientContextReloadAfter() { }
        public void CleanupOldSession() { }
    }
}
