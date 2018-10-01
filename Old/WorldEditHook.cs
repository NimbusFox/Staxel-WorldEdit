using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
using NimbusFox.WorldEdit.Classes;
using Plukit.Base;
using Staxel.Core;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;
using Color = Microsoft.Xna.Framework.Color;

namespace NimbusFox.WorldEdit {
    public class WorldEditHook : IModHookV2 {
        internal static Dictionary<Guid, List<RenderItem>> ToRender;
        internal static long NextTick;
        internal static Dictionary<Guid, VectorCubeI> ForbidEditing;

        public void Dispose() {
        }

        public void GameContextInitializeInit() {
            ToRender = new Dictionary<Guid, List<RenderItem>>();
            ForbidEditing = new Dictionary<Guid, VectorCubeI>();
            NextTick = 0;
            WorldEditManager.Init();
            if (!WorldEditManager.FoxCore.ModDirectory.FileExists("ignore.flag")) {
                var msgBox = MessageBox.Show(@"Would you like to customise the selection region's colors?", @"Customize World Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (msgBox == DialogResult.Yes) {
                    var defaultPrimary = ColorMath.ParseString("F5C900");
                    var defaultSecondary = Color.Black;

                    var colorPicker = new ColorPicker();

                    Application.Run(colorPicker);

                    var primary = ColorMath.ToString(colorPicker.Primary).Substring(2);
                    var secondary = ColorMath.ToString(colorPicker.Secondary).Substring(2);

                    var blob = BlobAllocator.AcquireAllocator().NewBlob(false);

                    var blob2 = blob.FetchBlob("Custom");

                    blob2.SetString(ColorMath.ToString(defaultPrimary).Substring(2), primary);
                    blob2.SetString(ColorMath.ToString(defaultSecondary).Substring(2), secondary);

                    var wait = true;

                    WorldEditManager.FoxCore.ModDirectory.WriteFile("palettes.json", blob, () => { wait = false; }, true);

                    while (wait) { }

                    colorPicker.Dispose();
                }

                var ms = new MemoryStream();
                var sw = new StreamWriter(ms);

                sw.Write("");
                sw.Flush();

                var streamWait = true;

                WorldEditManager.FoxCore.ModDirectory.WriteFileStream("ignore.flag", ms, () => { streamWait = false; });

                while (streamWait) { }
            }
        }

        public void GameContextInitializeBefore() {
        }

        public void GameContextInitializeAfter() {
            
        }
        public void GameContextDeinitialize() {
            WorldEditManager.RegionManager.Dispose();
        }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {

            foreach (var toRender in new Dictionary<Guid, List<RenderItem>>(ToRender)) {
                foreach (var render in new List<RenderItem>(toRender.Value).Take(25)) {
                    if (universe.World.PlaceTile(render.Location, render.Tile, TileAccessFlags.None)) {
                        toRender.Value.Remove(render);
                        if (!toRender.Value.Any()) {
                            ToRender.Remove(toRender.Key);
                            ForbidEditing.Remove(toRender.Key);
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

            Helpers.VectorLoop(start, end, (x, y, z) => { canEdit &= !CheckIfCanEdit(new Vector3I(x, y, z)); });

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
