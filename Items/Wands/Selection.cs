using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.WorldEdit.Entities.Bot;
using Plukit.Base;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Items.Wands {
    public class Selection : Item {
        protected SelectionWandBuilder Builder { get; private set; }

        public Selection(SelectionWandBuilder builder, ItemConfiguration config) : base(builder.Kind()) {
            Builder = builder;
            Configuration = config;
        }
        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) { }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (main.DownClick) {
                if (entity.Logic is PlayerEntityLogic logic) {
                    if (logic.LookingAtTile(out _, out var target)) {
                        BotEntityBuilder.Spawn(target, facade);
                    }
                }
            }
        }
        protected override void AssignFrom(Item item) { }
        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        public override bool HasAssociatedToolComponent(Plukit.Base.Components components) {
            return false;
        }

        public override ItemRenderer FetchRenderer() {
            return Builder.Renderer;
        }
    }
}
