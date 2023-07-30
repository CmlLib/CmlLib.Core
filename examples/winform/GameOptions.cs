using CmlLib.Utils;
using System;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class GameOptions : Form
    {
        public string Path { get; set; }

        public GameOptions(string path)
        {
            this.Path = path;
            InitializeComponent();
        }

        GameOptionsFile optionFile;

        private void GameOptions_Load(object sender, EventArgs e)
        {
            txtPath.Text = Path;
            btnLoad_Click(null, null);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.Path = txtPath.Text;

            optionFile = GameOptionsFile.ReadFile(this.Path);
            foreach (var item in optionFile)
            {
                listView1.Items.Add(new ListViewItem(new []
                {
                    item.Key,
                    item.Value
                }));
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenPanel("", "", true);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            var key = listView1.SelectedItems[0].Text;
            listView1.Items.RemoveByKey(key);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            var key = listView1.SelectedItems[0].Text;
            var value = listView1.SelectedItems[0].SubItems[1].Text;

            OpenPanel(key, value, false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                optionFile.SetRawValue(item.Text, item.SubItems[1].Text);
            }

            optionFile.Save();
        }

        private void OpenPanel(string key, string value, bool enableKey)
        {
            pKeyValue.Visible = true;
            txtKey.Text = key;
            txtValue.Text = value;
            txtKey.Enabled = enableKey;
        }

        string oKey = "";
        string oValue = "";

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtKey.Enabled)
                listView1.Items.Add(new ListViewItem(new [] { txtKey.Text, txtValue.Text }));
            else
                listView1.Items.Find(oKey, false)[0].SubItems[1].Text = txtValue.Text;
            pKeyValue.Visible = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pKeyValue.Visible = false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtKey.Text = oKey;
            txtValue.Text = oValue;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
