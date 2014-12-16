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
        private bool m_doingRender = false;

        private clJobFileLib jLib;
        private clSliceDataLib sliFile;
        private int picOutput_Start_X=0;
        private int picOutput_Start_Y=0;
        private int[] colorTable;
        private static System.Windows.Forms.ListBox s_lstError;
        private static bool s_showDebug = false;

        //---------------------------------------------------//
        public Form1()
        {
            InitializeComponent();

            colorTable = new int[16];
            colorTable[0] = RGB2int(255, 0, 255);
            colorTable[1] = RGB2int(255, 0, 0);
            colorTable[2] = RGB2int(255, 255, 0);
            colorTable[3] = RGB2int(0, 255, 0);
            colorTable[4] = RGB2int(0, 255, 255);
            colorTable[5] = RGB2int(0, 0, 255);
            colorTable[6] = RGB2int(255, 255, 255);
            colorTable[7] = RGB2int(128, 128, 128);
            colorTable[8] = RGB2int(192, 192, 192);
            colorTable[9] = RGB2int(128, 0, 0);
            colorTable[10] = RGB2int(128, 128, 0);
            colorTable[11] = RGB2int(0, 128, 0);
            colorTable[12] = RGB2int(0, 128, 128);
            colorTable[13] = RGB2int(0, 0, 128);
            colorTable[14] = RGB2int(128, 0, 128);


            s_lstError = lstError;
            s_showDebug = chkShowDebug.Checked;
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

        //---------------------------------------------------//
        private void button1_Click(object sender, EventArgs e)
        {
            sliFile = new clSliceDataLib();

            sliFile.readFromFile(txtSliFileName.Text);
            
            sliceLayer.Minimum = 0;
            sliceLayer.Value = 0;
            sliceLayer.Maximum = sliFile.getLayerCount() - 1;


            System.Diagnostics.Debug.Print( sliFile.getLastError());

            refreshPartNameList();

            showLayer();
        }

                
        //----------------------------------------------//
        static public void addError(string ErrorString, bool isDebug)
        {
            bool doShow = true;
            if ((!s_showDebug) && (isDebug)) doShow = false;

            string [] split = ErrorString.Split(new Char [] {'\n', '\r'});

            foreach (string s in split) 
            {
                if (s.Trim() != "") 
                {
                    if (doShow) s_lstError.Items.Add(s);
                    Console.WriteLine(s);
                }
            }
        }
        

        //----------------------------------------------//
        void refreshPartNameList()
        {
            lstPartNames.Items.Clear();
            int layer = sliceLayer.Value;

            int partsCount = sliFile.getPartCount();

            for (int i = 0; i < partsCount; i++)
            {

                string ExpParName = sliFile.getPartProperty(i);

                bool doCheck = true;

                if (ExpParName.ToLower() == "no_exposure") doCheck = false;

                lstPartNames.Items.Add(getPartListName(i, layer), doCheck);
            }
        }


        //----------------------------------------------//
        string getPartListName(int partIndex, int layer)
        {
            return d2s(sliFile.getLayerPos(partIndex, layer)) + "\t" + sliFile.getPartName(partIndex) + "\t" + sliFile.getPartProperty(partIndex);
        }


        //----------------------------------------------//
        bool isPartEnabled(int partIndex)
        {
            if ((partIndex < 0) || (partIndex >= lstPartNames.Items.Count) ) return false;

            return lstPartNames.GetItemChecked(partIndex);
        }

        //----------------------------------------------//
        void showLayer(int layerIndex=-1)
        {
            if (m_doingRender) return;
            m_doingRender = true;


            if (layerIndex == -1) layerIndex = sliceLayer.Value;

            //- get Position of the layer
            labLayerIndex.Text = i2s(layerIndex);
            labLayerPos.Text = d2s(sliFile.getLayerPos(0, layerIndex)) + " mm";



            //- get Part Count
            int partCount = sliFile.getPartCount();

            for (int part = 0; part < partCount; part++)
            {
                //- read layer data (only do this if layer has changed)
                if (isPartEnabled(part)) sliFile.readSliceData(layerIndex, part);
            }



            //- create transformations Matrix (only 2d but Homogeneous coordinates)
            clSliceDataLib.clMatrix3x2 m = new clSliceDataLib.clMatrix3x2();

            m.m.m11 = (float)s2d(txt_m11.Text);
            m.m.m12 = (float)s2d(txt_m12.Text);
            m.m.m13 = (float)s2d(txt_m13.Text);

            m.m.m21 = (float)s2d(txt_m21.Text);
            m.m.m22 = (float)s2d(txt_m22.Text);
            m.m.m23 = (float)s2d(txt_m23.Text);

            //- Homogeneous coordinates
            //- m31 = 0;
            //- m32 = 0;
            //- m33 = 1;

            //- output image
            int w = picOutput.Width;
            int h = picOutput.Height;

            int[,] img_polyline = null;
            int[,] img_filled = null;

            //- create img Buffer
            if (chkFill.Checked)
            {
                img_filled = new int[w, h];
            }
            else
            {
                img_polyline = new int[w, h];
            }



            //- rander slice
            for (int part = 0; part < partCount; part++)
            {
                int color = colorTable[part % colorTable.Length];

                //- because it is faster to do both (fild and outline)
                if (isPartEnabled(part)) sliFile.addRasterObject(img_filled, img_polyline, part, m, color);
            }

            //- show randered slice
            if (chkFill.Checked)
            {
                picOutput.Image = getBitmap(img_filled);
            }
            else
            {
                picOutput.Image = getBitmap(img_polyline);
            }



            for (int part = 0; part < partCount; part++)
            {
                lstPartNames.Items[part] = getPartListName(part, layerIndex);
            }

            m_doingRender = false;
        }

        //----------------------------------------------//
        int RGB2int(byte r, byte g, byte b)
        {
            return r << 16 | g << 8 | b;
        }

        //----------------------------------------------//
        public System.Drawing.Bitmap getBitmap(int[,] rgbValues)
        {
            int outWidth = rgbValues.GetLength(0);
            int outHeight = rgbValues.GetLength(1);

            //- create empty image
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(outWidth, outHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            // Lock the bitmap's bits.  
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);


            // Get the address of the first line.
            System.IntPtr ptr = bmpData.Scan0;
            
            // Declare an array to hold the bytes of the bitmap. 
            // This code is specific to a bitmap with 24 bits per pixels. 
            int pixelcount = System.Math.Abs(bmpData.Stride) * bmp.Height / 4;
            int[] tmp = new int[pixelcount];


            Buffer.BlockCopy(rgbValues, 0, tmp, 0, pixelcount * sizeof(int));
  

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(tmp, 0, ptr, pixelcount);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);


            //System.Diagnostics.Debug.WriteLine("getBitmap "+ (clHelper.getElapsedTicks() - startTimer).ToString());

            return bmp;
        }


        //----------------------------------------------//
        private void sliceLayer_Scroll(object sender, EventArgs e)
        {
            showLayer();
        }

        private void btnSelectFile_Click_1(object sender, EventArgs e)
        {

        }

        private void treeKeys_AfterSelect_1(object sender, TreeViewEventArgs e)
        {

        }

        //----------------------------------------------//
        private void btnOpen_Click(object sender, EventArgs e)
        {
            jLib = new clJobFileLib();

            jLib.readFromFile(txtFileName.Text);

            treeKeys.Nodes.Clear();

            fillTreeView(treeKeys.Nodes, jLib, 0);
        }



        //----------------------------------------------//
        /// <summary>string to double converting</summary>
        /// <param name="stringNumber">Number String with ',' or '.' as decimal separator</param>
        /// <returns>double interpretation for the stringNumber value</returns>
        public static double s2d(String stringNumber)
        {
            try
            {
                return Convert.ToDouble(stringNumber.Replace('.', ','));
            }
            catch
            {
                return 0;
            }
        }

        //----------------------------------------------//
        public static String d2s(double d, int decimals = 2)
        {
            return Math.Round(d, decimals).ToString();
        }


        //----------------------------------------------//
        public static String i2s(int i)
        {
            return i.ToString();
        }


        //----------------------------------------------//
        private void picOutput_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button== System.Windows.Forms.MouseButtons.Left)
            {
                if ((Control.ModifierKeys & Keys.Control) != 0)
                {
                    float m11 = (float)s2d(txt_m11.Text);
                    float m22 = (float)s2d(txt_m11.Text);

                    picOutput_Start_X = e.X - (int)Math.Round((m11 * 10));
                    picOutput_Start_Y = e.Y - (int)Math.Round((m22 * 10));

                }
                else
                {
                    picOutput_Start_X = e.X;
                    picOutput_Start_Y = e.Y;
                }
            }
        }

        //----------------------------------------------//
        private void picOutput_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if ((Control.ModifierKeys & Keys.Control) != 0)
                {
                    int dX = e.X - picOutput_Start_X;
                    int dY = e.Y - picOutput_Start_Y;


                    float r = ((dX) + (dY)) * 0.05f;


                    txt_m11.Text = d2s(r);
                    txt_m22.Text = d2s(r);

                }
                else
                {
                    txt_m13.Text = d2s(s2d(txt_m13.Text) + e.X - picOutput_Start_X);
                    txt_m23.Text = d2s(s2d(txt_m23.Text) + e.Y - picOutput_Start_Y);

                    picOutput_Start_X = e.X;
                    picOutput_Start_Y = e.Y;
                }


                showLayer();
            }
        }

        private void picOutput_Click(object sender, EventArgs e)
        {

        }

        private void lstPartNames_Validated(object sender, EventArgs e)
        {
            
        }

        private void lstPartNames_SelectedValueChanged(object sender, EventArgs e)
        {
            showLayer();
        }

        private void chkFill_CheckedChanged(object sender, EventArgs e)
        {
            showLayer();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
        
        }

        private void btnErrorCLS_Click(object sender, EventArgs e)
        {
            lstError.Items.Clear();
        }

        private void chkShowDebug_CheckedChanged(object sender, EventArgs e)
        {
            s_showDebug = chkShowDebug.Checked;
        }


    }
}
