using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Dependencies.Harmony;
using NimbusFox.WorldEdit.Components;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Logic;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Entities.Bot {
    public class BotEntityLogic : EntityLogic {

        private enum RotationNum {
            North = 0,
            NW = 45,
            West = 90,
            SW = 135,
            South = 180,
            SE = 225,
            East = 270,
            NE = 315,
            None = -1
        }

        public enum BotMode : byte {
            Idle,
            Waiting,
            Send,
            Receive,
            Storage,
            Copy,
            Cut,
            Paste,
            Delete
        }

        public string BotTile { get; private set; }
        protected Entity Entity { get; }
        protected List<Entity> LinkedEntities { get; }
        protected bool NeedStore { get; private set; }
        public BotComponent BotComponent { get; private set; }
        private Vector3D _destination;
        private bool _remove = false;
        private Blob _constructBlob;
        public int Rotation { get; private set; } = 0;

        // Client variables
        internal bool UpdateColors = true;
        // Client variables

        public IReadOnlyDictionary<Color, Color> ColorReplace { get; private set; }

        public string Owner { get; private set; }
        protected string OwnerUid { get; private set; }

        public string Mode { get; private set; } = "nimbusfox.worldedit.verb.idle";

        public BotEntityLogic(Entity entity) {
            var tile = GameContext.RandomSource.Pick(GameContext.TileDatabase.AllMaterials()
                .Where(x => x.Components.Contains<BotComponent>()).ToArray());

            BotTile = tile.Code;

            BotComponent = tile.Components.Get<BotComponent>();

            Entity = entity;

            entity.Physics.MakePhysicsless();

            var colorReplace = new Dictionary<Color, Color>();

            foreach (var colors in BotComponent.Palettes) {
                if (colors.Value.Count > 0) {
                    colorReplace.Add(colors.Key, GameContext.RandomSource.Pick(colors.Value.ToArray()));
                } else {
                    Color? col = null;

                    while (col == null) {
                        col = new Color(GameContext.RandomSource.Next(0, 255), GameContext.RandomSource.Next(0, 255), GameContext.RandomSource.Next(0, 255));

                        if (BotComponent.IgnoreColors.Contains(col.Value)) {
                            col = null;
                        }
                    }

                    colorReplace.Add(colors.Key, col.Value);
                }
            }

            ColorReplace = colorReplace;

            LinkedEntities = new List<Entity>();
        }

        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (Entity.Removed || _remove) {
                return;
            }

            if (_destination != Entity.Physics.Position) {
                const double increment = 0.005;
                Entity.Physics.ForcedPosition(Entity.Physics.Position + new Vector3D(
                                                  Entity.Physics.Position.X < _destination.X ? increment : Entity.Physics.Position.X > _destination.X ? -increment : 0,
                                                  Entity.Physics.Position.Y < _destination.Y ? increment : Entity.Physics.Position.Y > _destination.Y ? -increment : 0,
                                                  Entity.Physics.Position.Z < _destination.Z ? increment : Entity.Physics.Position.Z > _destination.Z ? -increment : 0));

                if (_destination.X - Entity.Physics.Position.X > -increment && _destination.X - Entity.Physics.Position.X < increment) {
                    Entity.Physics.ForcedPosition(new Vector3D(_destination.X, Entity.Physics.Position.Y, Entity.Physics.Position.Z));
                }

                if (_destination.Y - Entity.Physics.Position.Y > -increment && _destination.Y - Entity.Physics.Position.Y < increment) {
                    Entity.Physics.ForcedPosition(new Vector3D(Entity.Physics.Position.X, _destination.Y, Entity.Physics.Position.Z));
                }

                if (_destination.Z - Entity.Physics.Position.Z > -increment && _destination.Z - Entity.Physics.Position.Z < increment) {
                    Entity.Physics.ForcedPosition(new Vector3D(Entity.Physics.Position.X, Entity.Physics.Position.Y, _destination.Z));
                }
            }
        }

        public override void PostUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (_remove) {
                Entity.SetRemoved();
                entityUniverseFacade.RemoveEntity(Entity.Id);
            }
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            _constructBlob = BlobAllocator.Blob(true);
            _constructBlob.MergeFrom(arguments);
            Entity.Physics.ForcedPosition(arguments.FetchBlob("location").GetVector3I().ToVector3D() + BotComponent.TileOffset);
            Owner = arguments.GetString("owner");
            OwnerUid = arguments.GetString("uid");

            _destination = Entity.Physics.Position;

            NeedsStore();
        }

        public override void Bind() { }
        public override bool Interactable() {
            return true;
        }

        public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (alt.DownClick) {
                Remove();
            }
        }

        public override string AltInteractVerb() {
            return "nimbusfox.worldedit.verb.cancel";
        }

        public override bool CanChangeActiveItem() {
            return false;
        }

        public override Heading Heading() {
            return new Heading();
        }

        public override bool IsPersistent() {
            return true;
        }

        public override void StorePersistenceData(Blob data) {
            data.FetchBlob("position").SetVector3D(Entity.Physics.Position);
            data.FetchBlob("destination").SetVector3D(_destination);
            data.SetLong("rotation", Rotation);
            data.SetString("botTile", BotTile);
            data.SetString("owner", Owner);
            data.SetString("uid", OwnerUid);

            if (ColorReplace.Count > 0) {
                var blob = data.FetchBlob("colorReplace");
                foreach (var color in ColorReplace) {
                    blob.SetString(color.Key.PackedValue.ToString(), color.Value.PackedValue.ToString());
                }
            }
        }

        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) {
            if (data.Contains("construct")) {
                Construct(data.FetchBlob("construct"), facade);
            }

            if (data.Contains("position")) {
                Entity.Physics.ForcedPosition(data.FetchBlob("position").GetVector3D());
            }

            if (data.Contains("destination")) {
                _destination = data.FetchBlob("destination").GetVector3D();
            }

            if (data.Contains("rotation")) {
                Rotation = (int)data.GetLong("rotation");
            }

            if (data.Contains("botTile")) {
                BotTile = data.GetString("botTile");

                BotComponent = GameContext.TileDatabase.GetTileConfiguration(BotTile).Components.Get<BotComponent>();
            }

            if (data.Contains("owner")) {
                Owner = data.GetString("owner");
            }

            if (data.Contains("uid")) {
                OwnerUid = data.GetString("uid");
            }

            if (data.Contains("colorReplace")) {
                var colorReplace = new Dictionary<Color, Color>();

                foreach (var entry in data.FetchBlob("colorReplace").KeyValueIteratable) {
                    if (uint.TryParse(entry.Key, out var colKey)) {
                        if (uint.TryParse(entry.Value.GetString(), out var colVal)) {
                            colorReplace.Add(ColorMath.FromRgba(colKey), ColorMath.FromRgba(colVal));
                        }
                    }
                }

                ColorReplace = colorReplace;
            }
        }

        public override void Store() {
            if (NeedStore) {
                Entity.Blob.SetString("botTile", BotTile);
                Entity.Blob.SetLong("rotation", Rotation);
                Entity.Blob.SetString("owner", Owner); if (ColorReplace.Count > 0) {
                    var blob = Entity.Blob.FetchBlob("colorReplace");
                    foreach (var color in ColorReplace) {
                        blob.SetString(color.Key.PackedValue.ToString(), color.Value.PackedValue.ToString());
                    }
                }
            }
        }

        public override void Restore() {
            if (Entity.Blob.Contains("botTile")) {
                BotTile = Entity.Blob.GetString("botTile");
            }

            if (Entity.Blob.Contains("rotation")) {
                Rotation = (int)Entity.Blob.GetLong("rotation");
            }

            if (Entity.Blob.Contains("owner")) {
                Owner = Entity.Blob.GetString("owner");
            }

            if (Entity.Blob.Contains("colorReplace")) {
                var colorReplace = new Dictionary<Color, Color>();

                foreach (var entry in Entity.Blob.FetchBlob("colorReplace").KeyValueIteratable) {
                    if (uint.TryParse(entry.Key, out var colKey)) {
                        if (uint.TryParse(entry.Value.GetString(), out var colVal)) {
                            colorReplace.Add(ColorMath.FromRgba(colKey), ColorMath.FromRgba(colVal));
                        }
                    }
                }

                ColorReplace = colorReplace;
                UpdateColors = true;
            }
        }

        public override bool IsCollidable() {
            return true;
        }

        protected void NeedsStore() {
            NeedStore = true;
        }

        private void SetRotationZ(RotationNum rotation, Vector3I location, Vector3I pos) {
            if (location.Z > pos.Z) {
                if (rotation == RotationNum.East) {
                    Rotation = (int) RotationNum.NE;
                } else if (rotation == RotationNum.West) {
                    Rotation = (int)RotationNum.NW;
                } else {
                    Rotation = (int) RotationNum.North;
                }
            } else if (location.Z < pos.Z) {
                if (rotation == RotationNum.East) {
                    Rotation = (int)RotationNum.SE;
                } else if (rotation == RotationNum.West) {
                    Rotation = (int)RotationNum.SW;
                } else {
                    Rotation = (int)RotationNum.South;
                }
            } else {
                if (rotation == RotationNum.None) {
                    Rotation = (int)RotationNum.North;
                } else {
                    Rotation = (int) rotation;
                }
            }
        }

        protected void SetRotation(Vector3I location) {
            var pos = new Vector3I((int)Math.Floor(Entity.Physics.Position.X - BotComponent.TileOffset.X),
                (int)Math.Floor(Entity.Physics.Position.Y - BotComponent.TileOffset.Y),
                (int)Math.Floor(Entity.Physics.Position.Z - BotComponent.TileOffset.Z));
            if (location.X > pos.X) {
                SetRotationZ(RotationNum.West, location, pos);
            } else if (location.X < pos.X) {
                SetRotationZ(RotationNum.East, location, pos);
            } else {
                SetRotationZ(RotationNum.None, location, pos);
            }

            NeedsStore();
        }

        public void SetDestination(Vector3I location) {
            SetRotation(location);
            _destination = location.ToVector3D() + BotComponent.TileOffset;
        }

        public void Teleport(Vector3I location) {
            Rotation = (int) RotationNum.North;
            Entity.Physics.ForcedPosition(location.ToVector3D() + BotComponent.TileOffset);
        }

        protected void Remove() {
            _remove = true;
            if (LinkedEntities != null) {
                foreach (var ent in LinkedEntities) {
                    if (ent.Logic is BotEntityLogic logic) {
                        logic.Remove();
                    }
                }
            }
        }

        public void AddLinkedEntity(Entity entity) {
            if (entity.Logic is BotEntityLogic && !LinkedEntities.Contains(entity)) {
                LinkedEntities.Add(entity);
            }
        }

        public void SetMode(BotMode mode) {
            switch (mode) {
                default:
                case BotMode.Idle:
                    Mode = "nimbusfox.worldedit.verb.idle";
                    break;
                case BotMode.Waiting:
                    Mode = "nimbusfox.worldedit.verb.waiting";
                    break;
            }
        }
    }
}
