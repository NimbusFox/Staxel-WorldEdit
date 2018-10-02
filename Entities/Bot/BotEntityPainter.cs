using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NimbusFox.FoxCore;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Rendering;
using Staxel.Tiles;
using Staxel.Voxel;

namespace NimbusFox.WorldEdit.Entities.Bot {
    public class BotEntityPainter : EntityPainter {
        private string _botTileCode;
        private string _bladeTileCode;
        private Tile _botTile;
        private Tile _bladeTile;
        private DateTime _created;

        private bool _up = true;
        private float _add;

        public BotEntityPainter() {
            _created = DateTime.UtcNow;
        }

        protected override void Dispose(bool disposing) { }

        public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController,
            EntityUniverseFacade facade,
            int updateSteps) {
            if (entity.Logic is BotEntityLogic logic) {
                if (_botTileCode != logic.BotTile) {
                    _botTileCode = logic.BotTile;
                    _botTile = Helpers.MakeTile(_botTileCode);
                }

                if (_bladeTileCode != logic.BotComponent.BladeModel) {
                    _bladeTileCode = logic.BotComponent.BladeModel;
                    _bladeTile = Helpers.MakeTile(_bladeTileCode);
                }
            }
        }

        public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {
            if (entity.Logic is BotEntityLogic logic) {
                entity.Physics.BoundingShape = Shape.MakeCenteredBox(Vector3D.Zero, logic.BotComponent.BoundingBox);
                entity.Physics.CollisionShape = entity.Physics.BoundingShape;
            }
        }
        public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }

        public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity, AvatarController avatarController,
            Timestep renderTimestep) { }

        public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode) {
            if (!_botTileCode.IsNullOrEmpty()) {
                if (entity.Logic is BotEntityLogic logic) {
                    var pos = entity.Physics.Position.ToVector3F();
                    if (pos.Y + _add >= pos.Y + 0.125) {
                        _up = false;
                    }

                    if (pos.Y + _add <= pos.Y + 0) {
                        _up = true;
                    }

                    _add = _up ? _add + 0.0015F : _add - 0.0015F;
                    pos.Y += _add;

                    var botMatrix = Matrix.CreateFromYawPitchRoll(0, 0, 0).ToMatrix4F();

                    botMatrix = Matrix4F.Multiply(botMatrix.Translate(pos.X - (float)renderOrigin.X, pos.Y - (float)renderOrigin.Y, pos.Z - (float)renderOrigin.Z), matrix);

                    _botTile.Configuration.Icon.Matrix().Render(graphics, botMatrix);

                    var bladeMatrix = Matrix
                        .CreateFromYawPitchRoll(
                            (float)((DateTime.UtcNow - _created).TotalMilliseconds * 0.003 % (2 * Math.PI)), 0, 0)
                        .ToMatrix4F();

                    for (var i = 0; i < logic.BotComponent.BladeLocations.Count; i++) {
                        var currentPosition = pos + logic.BotComponent.BladeLocations[i].Item1.ToVector3F();
                        var currentBladeMatrix = Matrix4F.Multiply(bladeMatrix.Translate(
                            currentPosition.X - (float) renderOrigin.X, currentPosition.Y - (float) renderOrigin.Y,
                            currentPosition.Z - (float) renderOrigin.Z), matrix);

                        _bladeTile.Configuration.Icon.Matrix().Scale(logic.BotComponent.BladeLocations[i].Item2).Render(graphics, currentBladeMatrix);
                    }
                }
            }
        }

        public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) {}
    }
}
