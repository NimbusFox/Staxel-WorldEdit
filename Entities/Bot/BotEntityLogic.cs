using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore;
using NimbusFox.WorldEdit.Components;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Logic;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Entities.Bot {
    public class BotEntityLogic : EntityLogic {

        public string BotTile { get; private set; } = "";
        protected Entity Entity { get; }
        protected Entity LinkedEntity { get; }
        protected bool NeedStore { get; private set; }
        public BotComponent BotComponent { get; }
        private Vector3D _location;
        private bool _remove = false;

        public BotEntityLogic(Entity entity) {
            var tile = GameContext.RandomSource.Pick(GameContext.TileDatabase.AllMaterials()
                .Where(x => x.Components.Contains<BotComponent>()).ToArray());

            BotTile = tile.Code;

            BotComponent = tile.Components.Get<BotComponent>();

            Entity = entity;
        }

        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (Entity.Removed || _remove) {
                return;
            }

            if (_location != Entity.Physics.Position) {
                const double increment = 0.005;
                Entity.Physics.ForcedPosition(Entity.Physics.Position + new Vector3D(
                                                  Entity.Physics.Position.X < _location.X ? increment : Entity.Physics.Position.X > _location.X ? -increment : 0,
                                                  Entity.Physics.Position.Y < _location.Y ? increment : Entity.Physics.Position.Y > _location.Y ? -increment : 0,
                                                  Entity.Physics.Position.Z < _location.Z ? increment : Entity.Physics.Position.Z > _location.Z ? -increment : 0));

                if (_location.X - Entity.Physics.Position.X > -increment && _location.X - Entity.Physics.Position.X < increment) {
                    Entity.Physics.ForcedPosition(new Vector3D(_location.X, Entity.Physics.Position.Y, Entity.Physics.Position.Z));
                }

                if (_location.Y - Entity.Physics.Position.Y > -increment && _location.Y - Entity.Physics.Position.Y < increment) {
                    Entity.Physics.ForcedPosition(new Vector3D(Entity.Physics.Position.X, _location.Y, Entity.Physics.Position.Z));
                }

                if (_location.Z - Entity.Physics.Position.Z > -increment && _location.Z - Entity.Physics.Position.Z < increment) {
                    Entity.Physics.ForcedPosition(new Vector3D(Entity.Physics.Position.X, Entity.Physics.Position.Y, _location.Z));
                }

                NeedsStore();
            } else {

            }
        }

        public override void PostUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (_remove) {
                entityUniverseFacade.RemoveEntity(Entity.Id);
            }
        }

        public override void Store() {
            if (NeedStore) {
                Entity.Blob.SetString("botTile", BotTile);
                Entity.Blob.FetchBlob("position").SetVector3D(Entity.Physics.Position);
            }
        }

        public override void Restore() {
            if (Entity.Blob.Contains("botTile")) {
                BotTile = Entity.Blob.GetString("botTile");
            }

            if (Entity.Blob.Contains("position")) {
                Entity.Physics.ForcedPosition(Entity.Blob.FetchBlob("position").GetVector3D());
            }
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            Entity.Physics.ForcedPosition(arguments.FetchBlob("location").GetVector3I().ToVector3D() + BotComponent.TileOffset);

            _location = Entity.Physics.Position;

            NeedsStore();
        }

        public override void Bind() { }
        public override bool Interactable() {
            return true;
        }

        public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (alt.DownClick) {
                facade.RemoveEntity(Entity.Id);

                if (LinkedEntity != null) {
                    if (LinkedEntity.Logic is BotEntityLogic logic) {
                        logic.Remove();
                    }
                }
            }
        }
        public override bool CanChangeActiveItem() {
            return false;
        }

        public override Heading Heading() {
            return new Heading();
        }

        public override bool IsPersistent() {
            return false;
        }

        public override void StorePersistenceData(Blob data) { }
        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) { }
        public override bool IsCollidable() {
            return true;
        }

        protected void NeedsStore() {
            NeedStore = true;
        }

        public void SetDestination(Vector3I location) {
            _location = location.ToVector3D() + BotComponent.TileOffset;
        }

        private void Remove() {

        }
    }
}
