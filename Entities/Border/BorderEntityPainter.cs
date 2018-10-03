using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using NimbusFox.FoxCore;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Logic;
using Staxel.Rendering;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Entities.Border {
    public class BorderEntityPainter : EntityPainter {
        private VertexDrawable _cube;

        protected override void Dispose(bool disposing) {
        }

        public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade,
            int updateSteps) { }

        public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController,
            EntityUniverseFacade facade) {
        }
        public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }

        public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController,
            Timestep renderTimestep) {
            if (_cube == null) {
                CreateCubeColored();
            }
        }

        public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode) {
            if (entity.Logic is BorderEntityLogic borderLogic) {
                var cube = borderLogic.Cube;

                if (cube != null) {
                    cube = cube.GetOuterRegions();
                    GameContext.DebugGraphics.Enabled = true;
                    var cubeMatrix = Matrix.CreateFromYawPitchRoll(0, 0, 0).ToMatrix4F();
                    Helpers.VectorLoop(cube, (x, y, z) => {
                        var renderCount = 0;

                        renderCount += y == cube.Start.Y || y == cube.End.Y ? 1 : 0;

                        renderCount += z == cube.Start.Z || z == cube.End.Z ? 1 : 0;

                        renderCount += x == cube.Start.X || x == cube.End.X ? 1 : 0;

                        if (renderCount > 1) {
                            var target = new Vector3D(x, y, z);
                            var max = new Vector3D(x + 1, y + 1, z + 1);
                            Helpers.VectorLoop(target, max, (x2, y2, z2) => {
                                var renderCount2 = 0;

                                renderCount2 += y2 == target.Y || y == max.Y ? 1 : 0;

                                renderCount2 += z2 == target.Z || z == max.Z ? 1 : 0;

                                renderCount2 += x2 == target.X || x == max.X ? 1 : 0;

                                if (renderCount2 > 1) {
                                    var ccubeMatrix =
                                        Matrix4F.Multiply(
                                            cubeMatrix.Translate(new Vector3F((float)x2, (float)y2, (float)z2) - renderOrigin.ToVector3F()),
                                            matrix);
                                    _cube.Matrix().Render(graphics, ccubeMatrix);
                                }
                            }, 1D / 16D);
                        }
                    });
                }

            }
        }

        public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) { }

        private void CreateCubeColored() {
            var cube = new CubeDrawable();

            var list = ((CompactVertexDrawable)cube.Compile()).CompileToVoxelVertexArray();

            for (var i = 0; i < list.Length; i++) {
                list[i].Color = new Color(Color.Red, 0.25f);
            }

            _cube = new VertexDrawable(list.ToList());
        }
    }
}
