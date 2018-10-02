using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.WorldEdit.Components;
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
        protected SelectionWandBuilder Builder { get; }
        protected SelectionWandComponent WandComponent { get; }

        private Entity _selectionEntity;

        public Selection(SelectionWandBuilder builder, ItemConfiguration config) : base(builder.Kind()) {
            Builder = builder;
            Configuration = config;
            WandComponent = config.Components.Get<SelectionWandComponent>();
        }
        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) { }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (main.DownClick || alt.DownClick) {
                if (entity.Logic is PlayerEntityLogic logic) {
                    if (logic.LookingAtTile(out var target, out var location)) {
                        if (_selectionEntity?.Removed != false) {
                            CreateSelectionEntity(entity, facade, location);
                        }

                        if (_selectionEntity?.Logic is BorderBotEntityLogic borderLogic) {
                            if (main.DownClick) {
                                borderLogic.SetPos1(target);
                            }

                            if (alt.DownClick) {
                                borderLogic.SetPos2(target);
                            }

                            borderLogic.SetBorderTile(WandComponent.BorderTile);
                        }
                    }
                }
            }
        }

        protected void CreateSelectionEntity(Entity entity, EntityUniverseFacade facade, Vector3I location) {
            if (entity.Logic is PlayerEntityLogic logic) {
                _selectionEntity = BotEntityBuilder.Spawn(location, facade, logic.DisplayName(), logic.Uid(),
                    BorderBotEntityBuilder.KindCode);
            }
        }

        public override bool TryResolveAltInteractVerb(Entity entity, EntityUniverseFacade facade, Vector3I location,
            TileConfiguration lookedAtTile, out string verb) {
            verb = "nimbusfox.worldedit.verb.pos2";
            return true;
        }

        public override bool TryResolveMainInteractVerb(Entity entity, EntityUniverseFacade facade, Vector3I location,
            TileConfiguration lookedAtTile, out string verb) {
            verb = "nimbusfox.worldedit.verb.pos1";
            return true;
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
