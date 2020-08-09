using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CmlLib.Core;

namespace CmlLibWinFormSample
{
    public partial class PathForm : Form
    {
        public MinecraftPath MinecraftPath;

        public PathForm()
        {
            InitializeComponent();
        }

        public PathForm(MinecraftPath path)
        {
            this.MinecraftPath = path;
            InitializeComponent();
        }

        private void InitializingForm_Load(object sender, EventArgs e)
        {
            if (MinecraftPath == null)
                btnSetDefault_Click(null, null);
            else
                apply(MinecraftPath);
        }

        private void btnSetDefault_Click(object sender, EventArgs e)
        {
            var defaultPath = MinecraftPath.GetOSDefaultPath();
            apply(new MinecraftPath(defaultPath));
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            apply(new MinecraftPath(txtPath.Text));
        }

        void apply(MinecraftPath path)
        {
            this.MinecraftPath = path;

            txtPath.Text = path.BasePath;
            txtAssets.Text = path.Assets;
            txtLibrary.Text = path.Library;
            txtRuntime.Text = path.Runtime;
            txtVersion.Text = path.Versions;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cbEditMore.Checked)
            {
                MinecraftPath.Library = txtLibrary.Text;
                MinecraftPath.Runtime = txtRuntime.Text;
                MinecraftPath.Versions = txtVersion.Text;

                // You have to call SetAssetsPath when you want to change assets directory to what you want.
                // SetAssetsPath change not only Assets property, but also AssetsLegacy, AssetsObject, Index property.
                if (txtAssets.Text != MinecraftPath.Assets)
                    MinecraftPath.SetAssetsPath(txtAssets.Text);
            }

            this.Close();
        }

        private void btnUseDefaultAssets_Click(object sender, EventArgs e)
        {
            txtAssets.Text = System.IO.Path.Combine(MinecraftPath.GetOSDefaultPath(), "assets");
        }

        private void cbEditMore_CheckedChanged(object sender, EventArgs e)
        {
            gPaths.Enabled = cbEditMore.Checked;

            if (!cbEditMore.Checked)
                apply(MinecraftPath);
        }
    }
}
