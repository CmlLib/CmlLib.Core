using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using CmlLib.Core.Mojang;

namespace CmlLibWinFormSample
{
    public partial class MojangServerForm : Form
    {
        public MojangServerForm()
        {
            InitializeComponent();
        }

        private void MojangServerForm_Load(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                var status = MojangServerStatus.GetStatus();
                var properties = status.GetType().GetProperties();

                Invoke(new Action(() => 
                {
                    foreach (var item in properties)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            item.Name,
                            item.GetValue(status).ToString()
                        }));
                    }
                }));
            }).Start();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
