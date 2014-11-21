using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Demo
{
    public partial class Form1 : Form
    {

        private clJobFileLib jLib;

        public Form1()
        {
            InitializeComponent();
        }

        //---------------------------------------------------//
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog opendlg = new System.Windows.Forms.OpenFileDialog();


            opendlg.FileName = System.IO.Path.GetFileNameWithoutExtension(txtFileName.Text);
            opendlg.AddExtension = true;
            opendlg.Filter = "EOS Job File (*.job)|*.job|" +
                             "Config Job File (*.cft)|*.cft|" + 
                             "All Filetypes (*.*)|*.*";

            if (opendlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFileName.Text = opendlg.FileName;
            }
        }


        //---------------------------------------------------//
        private void button1_Click(object sender, EventArgs e)
        {
            jLib = new clJobFileLib();

            jLib.readFromFile(txtFileName.Text);

            treeKeys.Nodes.Clear();

            fillTreeView(treeKeys.Nodes, jLib, 0);

        }

        //---------------------------------------------------//
        private void fillTreeView(TreeNodeCollection currentTreeNode, clJobFileLib jLib, int keyID)
        {
            int id = jLib.getFirstKeyChild(keyID);
            while (id > 0)
            {
                TreeNode treeNode = new TreeNode(jLib.getKeyName(id), id, id);
                currentTreeNode.Add(treeNode);

                fillTreeView(treeNode.Nodes, jLib, id);

                id = jLib.getNextKeyChild(id);
            }

        }


        //---------------------------------------------------//
        private void treeKeys_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lstProperty.Items.Clear();

            int keyId = treeKeys.SelectedNode.ImageIndex;

            if (keyId > 0)
            {
                int propId = jLib.getFirstProperty(keyId);

                while(propId > 0)
                {
                    string[] item = { jLib.getPropertyName(propId), jLib.getPropertyValue(propId), jLib.getPropertyComment(propId) };

                    lstProperty.Items.Add(new ListViewItem(item));

                    propId = jLib.getNextProperty(propId);
                }
                
            }

        }
    }
}
