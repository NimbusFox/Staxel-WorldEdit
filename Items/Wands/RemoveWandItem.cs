using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.WorldEdit.Entities.Bot;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;

namespace NimbusFox.WorldEdit.Items.Wands {
    public class RemoveWandItem : BaseBotSpawnItem {
        private RemoveWandItemBuilder Builder { get; }

        public RemoveWandItem(RemoveWandItemBuilder builder, ItemConfiguration configuration) : base(builder, configuration) {
            Builder = builder;
        }

        public override void SpawnBot(Entity entity, EntityUniverseFacade facade, Vector3I location) {
            if (entity.Logic is PlayerEntityLogic logic) {
                BotEntityBuilder.Spawn(location, facade, logic.DisplayName(), logic.Uid());
            }
        }
    }
}
