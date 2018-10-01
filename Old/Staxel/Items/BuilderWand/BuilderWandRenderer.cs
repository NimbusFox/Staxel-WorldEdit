using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Draw;
using Staxel.Items;
using Staxel.Particles;
using Staxel.Rendering;

namespace NimbusFox.WorldEdit.Staxel.Items.BuilderWand {
    public class BuilderWandRenderer : ItemRenderer {

        private ParticleSource _particleSource;
        private Matrix4F _inHandMatrix;

        public BuilderWandRenderer(ParticleSource particles) {
            this._particleSource = particles;
        }

        public override ItemRenderer ReadyRenderer(Item item) {
            _inHandMatrix = Matrix4F.CreateTranslation(item.Configuration.UsageOffset).Multiply(item.Configuration.RotationMatrix);

            return this;
        }
    }
}
