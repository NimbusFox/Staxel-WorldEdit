using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.WorldEdit.Entities.Bot {
    public class BotEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2 {
        public virtual string Kind => KindCode;
        public static string KindCode => "nimbusfox.worldedit.entity.bot";

        public virtual EntityPainter Instance() {
            return new BotEntityPainter();
        }

        public virtual EntityLogic Instance(Entity entity, bool server) {
            return new BotEntityLogic(entity);
        }

        public void Load() { }


        public bool IsTileStateEntityKind() {
            return false;
        }

        public static Entity Spawn(Vector3I position, EntityUniverseFacade universe, string owner, string uid, string bot = "nimbusfox.worldedit.entity.bot") {
            var entity = new Entity(universe.AllocateNewEntityId(), false, bot, true);

            var blob = BlobAllocator.Blob(true);
            blob.SetString("kind", bot);
            blob.FetchBlob("position").SetVector3D(position.ToTileCenterVector3D());
            blob.FetchBlob("location").SetVector3I(position);
            blob.SetString("owner", owner);
            blob.SetString("uid", uid);

            entity.Construct(blob, universe);

            universe.AddEntity(entity);

            return entity;
        }
    }
}
