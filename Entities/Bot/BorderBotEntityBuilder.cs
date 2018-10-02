using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Staxel.Logic;

namespace NimbusFox.WorldEdit.Entities.Bot {
    public class BorderBotEntityBuilder : BotEntityBuilder {
        public override string Kind => KindCode;
        public new static string KindCode => "nimbusfox.worldedit.entity.bot.border";

        public override EntityPainter Instance() {
            return new BorderBotEntityPainter();
        }

        public override EntityLogic Instance(Entity entity, bool server) {
            return new BorderBotEntityLogic(entity);
        }
    }
}
