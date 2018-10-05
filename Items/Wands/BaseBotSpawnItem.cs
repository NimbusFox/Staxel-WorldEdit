using Plukit.Base;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;
using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Items.Wands {
    public abstract class BaseBotSpawnItem : Item {
        private BaseBotSpawnItemBuilder Builder { get; }

        public BaseBotSpawnItem(BaseBotSpawnItemBuilder builder, ItemConfiguration configuration) : base(builder.Kind()) {
            Builder = builder;
            Configuration = configuration;
        }
        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) { }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (alt.DownClick) {
                if (entity.Logic is PlayerEntityLogic logic) {
                    if (logic.LookingAtTile(out _, out var location)) {
                        SpawnBot(entity, facade, location);
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

        public abstract void SpawnBot(Entity entity, EntityUniverseFacade facade, Vector3I location);

        public override ItemRenderer FetchRenderer() {
            return Builder.Renderer;
        }
    }
}
