using CmlLib.Core;
using CmlLib.Core.VersionMetadata;

namespace CmlLibWinFormSample
{
    public partial class VersionSortOptionForm : Form
    {
        public VersionSortOptionForm(MinecraftLauncher launcher, MVersionSortOption sortOption)
        {
            this.sortOption = sortOption;
            this.launcher = launcher;
            InitializeComponent();
        }

        private readonly MinecraftLauncher launcher;
        private VersionMetadataCollection? versions;
        public MVersionSortOption sortOption { get; private set; }

        private async void VersionSortOptionForm_Load(object sender, EventArgs e)
        {
            foreach (var item in sortOption.TypeOrder)
            {
                txtTypeOrder.AppendText(item.ToString() + "\n");
            }

            cbAscending.Checked = sortOption.AscendingPropertyOrder;
            cbTypeClassification.Checked = sortOption.TypeClassification;
            cbCustomAsRelease.Checked = sortOption.CustomAsRelease;

            cbPropertyOrder.Text = sortOption.PropertyOrderBy.ToString();

            versions = await launcher.GetAllVersionsAsync();
        }

        private MVersionSortOption? buildSortOption()
        {
            var typeList = new List<MVersionType>();
            foreach (var item in txtTypeOrder.Text.Split('\n'))
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                    
                switch (item.ToLowerInvariant())
                {
                    case "custom":
                        typeList.Add(MVersionType.Custom);
                        break;
                    case "release":
                        typeList.Add(MVersionType.Release);
                        break;
                    case "snapshot":
                        typeList.Add(MVersionType.Snapshot);
                        break;
                    case "oldbeta":
                        typeList.Add(MVersionType.OldBeta);
                        break;
                    case "oldalpha":
                        typeList.Add(MVersionType.OldAlpha);
                        break;
                    default:
                        MessageBox.Show("Unknown version type: " + item);
                        return null;
                }
            }

            MVersionSortPropertyOption propertyOrder;
            if (cbPropertyOrder.Text == "Name")
                propertyOrder = MVersionSortPropertyOption.Name;
            else if (cbPropertyOrder.Text == "ReleaseDate")
                propertyOrder = MVersionSortPropertyOption.ReleaseDate;
            else if (cbPropertyOrder.Text == "Version")
                propertyOrder = MVersionSortPropertyOption.Version;
            else
            {
                MessageBox.Show("Unknown property: " + cbPropertyOrder.Text);
                return null;
            }

            return new MVersionSortOption
            {
                TypeOrder = typeList.ToArray(),
                PropertyOrderBy = propertyOrder,
                AscendingPropertyOrder = cbAscending.Checked,
                TypeClassification = cbTypeClassification.Checked,
                CustomAsRelease = cbCustomAsRelease.Checked
            };
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            var option = buildSortOption();
            if (option == null || versions == null)
                return;

            listPreview.Items.Clear();
            var vers = versions.ToArray(option);
            foreach (var item in vers)
            {
                listPreview.Items.Add($"{item.Type} {item.Name}");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            var option = buildSortOption();
            if (option == null)
                return;

            sortOption = option;
            this.Close();
        }

        private void btnPreset1_Click(object sender, EventArgs e)
        {
            
        }

        private void btnPreset2_Click(object sender, EventArgs e)
        {
            
        }

        private void btnPreset3_Click(object sender, EventArgs e)
        {
            
        }

        private void btnPreset4_Click(object sender, EventArgs e)
        {
            
        }
    }
}