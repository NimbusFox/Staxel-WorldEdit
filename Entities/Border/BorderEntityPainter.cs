using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using NimbusFox.FoxCore;
using NimbusFox.WorldEdit.Components;
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
        }

        public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode) {
            if (entity.Logic is BorderEntityLogic borderLogic) {
                var cube = borderLogic.Cube;

                if (cube != null) {
                    Helpers.VectorLoop(cube, (x, y, z) => {
                        var renderCount = 0;

                        renderCount += y == cube.Start.Y || y == cube.End.Y ? 1 : 0;

                        renderCount += z == cube.Start.Z || z == cube.End.Z ? 1 : 0;

                        renderCount += x == cube.Start.X || x == cube.End.X ? 1 : 0;

                        Matrix4F regionMatrix;
                        MatrixDrawable tile;
                        WorldEditBorderComponent worldEditComponent;

                        if (renderCount == 3) {
                            tile = borderLogic.CornerTile.Icon.Matrix();
                            worldEditComponent = borderLogic.CornerTile.Components.Get<WorldEditBorderComponent>();

                            var yaw = 0;

                            if (y == cube.End.Y) {
                                yaw = 180;
                            }


                        }

                        if (renderCount == 2) {
                            tile = borderLogic.LTile.Icon.Matrix();
                            worldEditComponent = borderLogic.LTile.Components.Get<WorldEditBorderComponent>();

                            var yaw = 0;


                        }

                        if (renderCount == 1) {
                            tile = borderLogic.StraightTile.Icon.Matrix();
                            worldEditComponent =
                                borderLogic.StraightTile.Components.Get<WorldEditBorderComponent>();

                            var scale = worldEditComponent.Scale;

                            if (y == cube.Start.Y) {
                                regionMatrix = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(90), 0).ToMatrix4F();
                                regionMatrix =
                                    Matrix4F.Multiply(
                                        regionMatrix.Translate(
                                            new Vector3F(x, y, z) - renderOrigin.ToVector3F()), matrix).Translate(worldEditComponent.NY);
                                tile.Scale(scale).Render(graphics, regionMatrix);
                                return;
                            }

                            if (y == cube.End.Y) {
                                regionMatrix = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(90), 0).ToMatrix4F();
                                regionMatrix =
                                    Matrix4F.Multiply(
                                            regionMatrix.Translate(
                                                new Vector3F(x, y, z) - renderOrigin.ToVector3F()), matrix)
                                        .Translate(worldEditComponent.PY);
                                tile.Scale(scale).Render(graphics, regionMatrix);
                                return;
                            }

                            if (z == cube.Start.Z) {
                                regionMatrix = Matrix.CreateFromYawPitchRoll(0, 0, 0).ToMatrix4F();
                                regionMatrix =
                                    Matrix4F.Multiply(
                                        regionMatrix.Translate(
                                            new Vector3F(x, y, z) - renderOrigin.ToVector3F()), matrix).Translate(worldEditComponent.NZ);
                                tile.Scale(scale).Render(graphics, regionMatrix);
                                return;
                            }

                            if (z == cube.End.Z) {
                                regionMatrix = Matrix.CreateFromYawPitchRoll(0, 0, 0).ToMatrix4F();
                                regionMatrix =
                                    Matrix4F.Multiply(
                                        regionMatrix.Translate(
                                            new Vector3F(x, y, z) - renderOrigin.ToVector3F()), matrix).Translate(worldEditComponent.PZ);
                                tile.Scale(scale).Render(graphics, regionMatrix);
                                return;
                            }

                            if (x == cube.Start.X) {
                                regionMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(90), 0, 0).ToMatrix4F();
                                regionMatrix =
                                    Matrix4F.Multiply(
                                        regionMatrix.Translate(
                                            new Vector3F(x, y, z) - renderOrigin.ToVector3F()), matrix).Translate(worldEditComponent.NX);
                                tile.Scale(scale).Render(graphics, regionMatrix);
                                return;
                            }

                            if (x == cube.End.X) {
                                regionMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(90), 0, 0).ToMatrix4F();
                                regionMatrix =
                                    Matrix4F.Multiply(
                                        regionMatrix.Translate(
                                            new Vector3F(x, y, z) - renderOrigin.ToVector3F()), matrix).Translate(worldEditComponent.PX);
                                tile.Scale(scale).Render(graphics, regionMatrix);
                                return;
                            }
                        }
                    });

                    var boxMatrix = Matrix.CreateFromYawPitchRoll(0, 0, 0).ToMatrix4F();

                    var boxMatrix1 = Matrix4F.Multiply(
                        boxMatrix.Translate((cube.OrigStart.ToVector3F() - renderOrigin.ToVector3F()) +
                                            borderLogic.Selection1.Components.Get<TileEntityComponent>().TileOffset),
                        matrix);

                    borderLogic.Selection1.Icon.Matrix()
                        .Scale(borderLogic.Selection1.Components.Get<TileEntityComponent>().Scale)
                        .Render(graphics, boxMatrix1);

                    var boxMatrix2 = Matrix4F.Multiply(boxMatrix.Translate(
                        (cube.OrigEnd.ToVector3F() - renderOrigin.ToVector3F()) + borderLogic.Selection2.Components
                            .Get<TileEntityComponent>().TileOffset), matrix);

                    borderLogic.Selection2.Icon.Matrix().Scale(borderLogic.Selection2.Components.Get<TileEntityComponent>().Scale).Render(graphics, boxMatrix2);
                }
            }
        }

        public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) { }
    }
}
