using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.WorldEdit.Entities.Bot;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.WorldEdit.Entities.Border {
    class BorderEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2 {
        public virtual string Kind => KindCode;
        public static string KindCode => "nimbusfox.worldedit.entity.border";

        public virtual EntityPainter Instance() {
            return new BorderEntityPainter();
        }

        public virtual EntityLogic Instance(Entity entity, bool server) {
            return new BorderEntityLogic(entity);
        }

        public void Load() { }


        public bool IsTileStateEntityKind() {
            return false;
        }

        public static Entity Spawn(Vector3I position, EntityUniverseFacade universe, Entity owner) {
            var entity = new Entity(universe.AllocateNewEntityId(), false, KindCode, true);

            var blob = BlobAllocator.Blob(true);
            blob.SetString("kind", KindCode);
            blob.FetchBlob("position").SetVector3D(position.ToTileCenterVector3D());
            blob.FetchBlob("location").SetVector3I(position);
            blob.SetLong("owner", owner.Id.Id);

            entity.Construct(blob, universe);

            universe.AddEntity(entity);

            return entity;
        }
    }
}
