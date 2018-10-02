using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.WorldEdit.Entities.Bot {
    public class BotEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2 {
        public string Kind => KindCode;
        public static string KindCode => "nimbusfox.worldedit.entity.bot";

        public EntityPainter Instance() {
            return new BotEntityPainter();
        }

        public EntityLogic Instance(Entity entity, bool server) {
            return new BotEntityLogic(entity);
        }

        public void Load() { }


        public bool IsTileStateEntityKind() {
            return false;
        }

        public static Entity Spawn(Vector3I position, EntityUniverseFacade universe) {
            var entity = new Entity(universe.AllocateNewEntityId(), false, KindCode, true);

            var blob = BlobAllocator.Blob(true);
            blob.SetString("kind", KindCode);
            blob.FetchBlob("position").SetVector3D(position.ToTileCenterVector3D());
            blob.FetchBlob("location").SetVector3I(position);

            entity.Construct(blob, universe);

            universe.AddEntity(entity);

            return entity;
        }
    }
}
