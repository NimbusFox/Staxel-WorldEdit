﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
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
        private Tile _bladeTile;
        private DateTime _created;

        private MatrixDrawable _bot;

        private bool _up = true;
        private float _add;

        protected NameTag NameTag;

        public BotEntityPainter() {
            _created = DateTime.UtcNow;
        }

        protected override void Dispose(bool disposing) {
            NameTag.Dispose();
        }

        public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController,
            EntityUniverseFacade facade,
            int updateSteps) {
            if (entity.Logic is BotEntityLogic logic) {
                if (_botTileCode != logic.BotTile) {
                    _botTileCode = logic.BotTile;

                    _bot = Helpers.MakeTile(_botTileCode).Configuration.Icon.Matrix();
                }

                if (_bladeTileCode != logic.BotComponent.BladeModel) {
                    _bladeTileCode = logic.BotComponent.BladeModel;
                    _bladeTile = Helpers.MakeTile(_bladeTileCode);
                }

                if (NameTag == null) {
                    NameTag = ClientContext.NameTagRenderer.RegisterNameTag(entity.Id);
                }

                if (logic.UpdateColors) {
                    var botTile = Helpers.MakeTile(_botTileCode);
                    var matrix = botTile.Configuration.Icon as MatrixDrawable;
                    var bot = botTile.Configuration.Icon.GetPrivateFieldValue<CompactVertexDrawable>("_drawable");

                    var list = bot.CompileToVoxelVertexArray();

                    for (var i = 0; i < list.Length; i++) {
                        var colSelected = false;
                        foreach (var color in logic.ColorReplace) {
                            if (colSelected) {
                                break;
                            }
                            for (var r = -logic.BotComponent.PaletteTolerance; r <= logic.BotComponent.PaletteTolerance; r++) {
                                if (colSelected) {
                                    break;
                                }
                                for (var g = -logic.BotComponent.PaletteTolerance; g <= logic.BotComponent.PaletteTolerance; g++) {
                                    if (colSelected) {
                                        break;
                                    }
                                    for (var b = -logic.BotComponent.PaletteTolerance; b <= logic.BotComponent.PaletteTolerance; b++) {
                                        if (colSelected) {
                                            break;
                                        }
                                        var col = list[i].Color;
                                        var curCol = new Color(color.Key.R + r, color.Key.G + g, color.Key.B + b);

                                        if (col.R == curCol.R && col.G == curCol.G && col.B == curCol.B) {
                                            list[i].Color = new Color(color.Value.R + r, color.Value.G + g, color.Value.B + b, col.A);
                                            colSelected = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    _bot = new VertexDrawable(list.ToList()).Matrix(matrix.MatrixValue);

                    logic.UpdateColors = false;
                }
            }
        }

        protected virtual void UpdateNameTag(Entity entity) {
            if (entity.Logic is BotEntityLogic logic) {
                NameTag?.Setup(entity.Physics.Position, Constants.NameTagDefaultOffset,
                    $"{logic.Owner} ({ClientContext.LanguageDatabase.GetTranslationString(logic.Mode)})", false, false,
                    true);
            }
        }

        public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {
            if (entity.Logic is BotEntityLogic logic) {
                entity.Physics.BoundingShape = Shape.MakeCenteredBox(Vector3D.Zero, logic.BotComponent.BoundingBox);
                entity.Physics.CollisionShape = entity.Physics.BoundingShape;
                UpdateNameTag(entity);
            }
        }
        public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }

        public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity, AvatarController avatarController,
            Timestep renderTimestep) { }

        public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode) {
            if (renderMode == RenderMode.Overlay)
                return;

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

                    var botMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(logic.Rotation), 0, 0).ToMatrix4F();

                    botMatrix = Matrix4F.Multiply(botMatrix.Translate(pos.X - (float)renderOrigin.X, pos.Y - (float)renderOrigin.Y, pos.Z - (float)renderOrigin.Z), matrix);

                    _bot.Render(graphics, botMatrix);

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
