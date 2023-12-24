using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vb6callgraph
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Filter = "(*.frm,*.bas)|*.frm;*.bas";
            var res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                var files = dlg.FileNames;
                var grp = new GraphMaker();
                grp.MakerMain(files);
            }
        }
    }
}
