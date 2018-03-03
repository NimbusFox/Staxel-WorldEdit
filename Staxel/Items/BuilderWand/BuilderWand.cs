using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Staxel.Items.BuilderWand {
    public class BuilderWand : Item {
        private readonly BuilderWandBuilder _builder;
        private Vector3D _pos1;
        private Vector3D _pos2;

        public BuilderWand(BuilderWandBuilder builder) : base(builder.Kind()) {
            _builder = builder;
        }

        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) {

        }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (main.DownClick) {
                _pos1 = entity.Physics.BottomPosition();
            }

            if (alt.DownClick) {
                _pos2 = entity.Physics.BottomPosition();
            }
        }

        protected override void AssignFrom(Item item) {
            Configuration = item.Configuration;
        }

        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        public override bool HasAssociatedToolComponent(Components components) {
            return false;
        }
    }
}
