using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Classes;
using NimbusFox.WorldEdit.Components;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Logic;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Entities.Border {
    public class BorderEntityLogic : EntityLogic {

        public TileConfiguration StraightTile { get; private set; }
        public TileConfiguration CornerTile { get; private set; }
        public TileConfiguration LTile { get; private set; }
        public TileConfiguration Selection1 { get; private set; }
        public TileConfiguration Selection2 { get; private set; }

        private Blob _constructBlob;
        protected bool NeedStore { get; private set; }

        public Vector3I? Pos1 { get; private set; }
        public Vector3I? Pos2 { get; private set; }

        public VectorCubeI Cube {
            get {
                if (Pos1 == null || Pos2 == null) {
                    return null;
                }

                return new VectorCubeI(Pos1.Value, Pos2.Value);
            }
        }

        protected Entity Entity { get; }
        protected string Owner { get; private set; }

        public BorderEntityLogic(Entity entity) {
            Entity = entity;
            entity.Physics.MakePhysicsless();
        }

        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }
        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }

        public override void PostUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (!WorldEditHook.Instance.FxCore.UserManager.IsUserOnline(Owner)) {
                Entity.SetRemoved();
                entityUniverseFacade.RemoveEntity(Entity.Id);
            }
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            _constructBlob = BlobAllocator.Blob(true);
            _constructBlob.MergeFrom(arguments);
            Entity.Physics.ForcedPosition(arguments.FetchBlob("location").GetVector3I().ToVector3D());
            Owner = arguments.GetString("owner");

            NeedsStore();
        }
        public override void Bind() { }
        public override bool Interactable() {
            return false;
        }

        public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) { }
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

        public override void Store() {
            if (NeedStore) {
                if (Pos1 != null) {
                    Entity.Blob.FetchBlob("pos1").SetVector3I(Pos1.Value);
                }

                if (Pos2 != null) {
                    Entity.Blob.FetchBlob("pos2").SetVector3I(Pos2.Value);
                }

                if (StraightTile != null) {
                    Entity.Blob.SetString("straightTile", StraightTile.Code);
                }

                if (CornerTile != null) {
                    Entity.Blob.SetString("cornerTile", CornerTile.Code);
                }

                if (LTile != null) {
                    Entity.Blob.SetString("lTile", LTile.Code);
                }

                if (Selection1 != null) {
                    Entity.Blob.SetString("selection1", Selection1.Code);
                }

                if (Selection2 != null) {
                    Entity.Blob.SetString("selection2", Selection2.Code);
                }

                NeedStore = false;
            }
        }

        public override void Restore() {
            if (Entity.Blob.Contains("pos1")) {
                Pos1 = Entity.Blob.FetchBlob("pos1").GetVector3I();
            }

            if (Entity.Blob.Contains("pos2")) {
                Pos2 = Entity.Blob.FetchBlob("pos2").GetVector3I();
            }

            if (Entity.Blob.Contains("straightTile")) {
                StraightTile = GameContext.TileDatabase.GetTileConfiguration(Entity.Blob.GetString("straightTile"));
            }
            
            if (Entity.Blob.Contains("cornerTile")) {
                CornerTile = GameContext.TileDatabase.GetTileConfiguration(Entity.Blob.GetString("cornerTile"));
            }

            if (Entity.Blob.Contains("lTile")) {
                LTile = GameContext.TileDatabase.GetTileConfiguration(Entity.Blob.GetString("lTile"));
            }

            if (Entity.Blob.Contains("selection1")) {
                Selection1 = GameContext.TileDatabase.GetTileConfiguration(Entity.Blob.GetString("selection1"));
            }

            if (Entity.Blob.Contains("selection2")) {
                Selection2 = GameContext.TileDatabase.GetTileConfiguration(Entity.Blob.GetString("selection2"));
            }
        }

        public override bool IsCollidable() {
            return false;
        }

        public void SetPos1(Vector3I pos1) {
            Pos1 = pos1;
            if (Pos2 == null) {
                Pos2 = pos1;
            }
            MoveEntity();
            NeedsStore();
        }

        public void SetPos2(Vector3I pos2) {
            Pos2 = pos2;
            if (Pos1 == null) {
                Pos1 = pos2;
            }
            MoveEntity();
            NeedsStore();
        }

        private void MoveEntity() {
            Entity.Physics.ForcedPosition(Cube.Start.ToTileCenterVector3D());
        }

        public void SetBorderTiles(SelectionWandComponent component) {
            StraightTile = GameContext.TileDatabase.GetTileConfiguration(component.SelectionStraight);
            LTile = GameContext.TileDatabase.GetTileConfiguration(component.SelectionL);
            CornerTile = GameContext.TileDatabase.GetTileConfiguration(component.SelectionCorner);
            Selection1 = GameContext.TileDatabase.GetTileConfiguration(component.SelectionPoint1);
            Selection2 = GameContext.TileDatabase.GetTileConfiguration(component.SelectionPoint2);
        }

        protected void NeedsStore() {
            NeedStore = true;
        }
    }
}
