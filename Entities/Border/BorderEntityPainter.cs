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
                if (!ClientContext.PlayerFacade.IsLocalPlayer(borderLogic.Owner)) { 
                    return;
                }

                var cube = borderLogic.Cube;

                if (cube != null) {
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
