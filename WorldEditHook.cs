using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void Dispose() { }

        public void GameContextInitializeInit() {
            ToRender = new Dictionary<Guid, List<RenderItem>>();
            NextTick = 0;
        }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() {
            WorldEditManager.Init();
        }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            WorldEditManager.FoxCore.ParticleManager.DrawParticles();
            WorldEditManager.FoxCore.EntityParticleManager.DrawParticles();
            WorldEditManager.FoxCore.EntityFollowParticleManager.DrawParticles();

            if (NextTick <= DateTime.Now.Ticks) {
                foreach (var toRender in new Dictionary<Guid, List<RenderItem>>(ToRender)) {
                    foreach (var render in new List<RenderItem>(toRender.Value).Take(250)) {
                        if (universe.World.PlaceTile(render.Location, render.Tile, TileAccessFlags.SynchronousWait)) {
                            toRender.Value.Remove(render);
                            if (!toRender.Value.Any()) {
                                ToRender.Remove(toRender.Key);
                                WorldEditManager.FoxCore.ParticleManager.Remove(toRender.Key);
                            }
                        }
                    }
                }
                NextTick = DateTime.Now.Ticks;
            }
        }
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
    }
}
