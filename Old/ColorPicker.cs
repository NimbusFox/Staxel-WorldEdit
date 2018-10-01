using System;
using System.Drawing;
using System.Windows.Forms;
using Staxel.Core;

namespace NimbusFox.WorldEdit {
    public partial class ColorPicker : Form {
        internal Microsoft.Xna.Framework.Color Primary;
        internal Microsoft.Xna.Framework.Color Secondary;

        private Color GetFromXna(Microsoft.Xna.Framework.Color color) {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private Microsoft.Xna.Framework.Color ToXna(Color color) {
            return ColorMath.FromDrawingColorToXnaColor(color);
        }

        public ColorPicker() {
            InitializeComponent();

            Primary = ColorMath.ParseString("F5C900");
            Secondary = Microsoft.Xna.Framework.Color.Black;

            BtnRefresh();
        }

        private void BtnRefresh() {
            btnPrimary.BackColor = GetFromXna(Primary);
            btnPrimary.ForeColor = GetFromXna(Secondary);

            btnSecondary.BackColor = GetFromXna(Secondary);
            btnSecondary.ForeColor = GetFromXna(Primary);
        }

        private void btnPrimary_Click(object sender, EventArgs e) {
            var confirm = colorDialog1.ShowDialog();

            if (confirm == DialogResult.OK) {
                Primary = ToXna(colorDialog1.Color);
                BtnRefresh();
            }
        }

        private void btnSecondary_Click(object sender, EventArgs e) {
            var confirm = colorDialog1.ShowDialog();

            if (confirm == DialogResult.OK) {
                Secondary = ToXna(colorDialog1.Color);
                BtnRefresh();
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
