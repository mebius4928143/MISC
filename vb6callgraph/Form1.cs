using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vb6callgraph.Properties;

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
            var dlg = new OpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "(*.frm,*.bas,*.cls)|*.frm;*.bas;*.cls"
            };
            var res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                var files = dlg.FileNames;
                var grp = new GraphMaker();
                var matrix = grp.MakerMain(files);
                var html = Resources.modulenet.Replace("\r\n", "\n").Split('\n');
                var nodes = new List<string>();
                var edges = new List<string>();
                foreach ( Position s in matrix.Positions)
                {
                    //{id: 1, label: 'A'},
                    //nodes.Add($"{{id: {s.index + 1}, label: '{s.VBMethodObject.GeyKey()}'}},");
                    // const v12 = graph.insertVertex(parent, null, '12', null, null, 30, 30, null)
                    nodes.Add($"const v{s.index + 1} = graph.insertVertex(parent, null, '{s.VBMethodObject.GeyKey()}', null, null, {s.VBMethodObject.GeyKey().Length * 10}, 30, null);");
                }
                foreach (Position s in matrix.Positions.Where(p => p.VBMethodObject.Children.Count > 0))
                {
                    //{from: 1, to: 2, arrows: 'to'},
                    // graph.insertEdge(parent, null, null, v3, v4, null)
                    s.VBMethodObject.Children.ForEach(c =>
                    {
                        //edges.Add($"{{from: {s.index + 1}, to: {matrix.Positions.First(p => p.VBMethodObject.GeyKey() == c.GeyKey()).index + 1}, arrows: 'to'}},");
                        edges.Add($"graph.insertEdge(parent, null, null, v{s.index + 1}, v{matrix.Positions.First(p => p.VBMethodObject.GeyKey() == c.GeyKey()).index + 1}, null);");
                    });
                }
                var nodeidx = html.ToList().FindIndex(t => t.Trim() == "/*@nodes@*/");
                html[nodeidx] = string.Join("\r\n", nodes);
                var edgeidx = html.ToList().FindIndex(t => t.Trim() == "/*@edges@*/");
                html[edgeidx] = string.Join("\r\n", edges);
                File.WriteAllLines("modlenet.html", html);
            }
        }
    }
}
