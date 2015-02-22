using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
        private static System.Windows.Forms.TabControl s_tabControl1;
        private static bool s_showDebug = false;
        private clDB db;
        private string m_machine;

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
            s_tabControl1 = tabControl1;
            s_showDebug = chkShowDebug.Checked;
        }

        //---------------------------------------------------//
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog opendlg = new System.Windows.Forms.OpenFileDialog();


            opendlg.FileName = System.IO.Path.GetFileNameWithoutExtension(txtFileName.Text);
            opendlg.AddExtension = true;
            opendlg.Filter = "All EOS File (*.job;*.cft;*.hpr)|*.job;*.cft;*.hpr|" +
                             "EOS Job File (*.job)|*.job|" +
                             "Config Job File (*.cft)|*.cft|" +
                             "Exposure Config (*.hpr)|*.hpr|" + 
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
            sliceLayer.Maximum = (int)Math.Floor(sliFile.getMaxLayerPos() / sliFile.getLayerThickness());

            lblLayerThickness.Text = d2s(sliFile.getLayerThickness()) +" mm";

            System.Diagnostics.Debug.Print( sliFile.getLastError());

            refreshPartNameList();

            showLayer();
        }

                
        //----------------------------------------------//
        static public void addError(string ErrorString, bool isDebug=false)
        {
            bool doShow = true;
            bool foundError = false;

            if ((!s_showDebug) && (isDebug)) doShow = false;

            string [] split = ErrorString.Split(new Char [] {'\n', '\r'});

            foreach (string s in split) 
            {
                if (s.Trim() != "") 
                {
                    if (doShow) s_lstError.Items.Add(s);
                    Console.WriteLine(s);
                    foundError = true;
                }
            }

            if (foundError && !isDebug) s_tabControl1.SelectedIndex = s_tabControl1.TabCount - 1;
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
            //- don't run this more then once the same time
            if (m_doingRender) return;
            m_doingRender = true;

            if (layerIndex == -1) layerIndex = sliceLayer.Value;


            //- get Position of the layer
            labLayerIndex.Text = i2s(layerIndex);
            labLayerPos.Text = d2s(layerIndex * sliFile.getLayerThickness()) + " mm";


            float layerPos = layerIndex * sliFile.getLayerThickness();

            //- get Part Count
            int partCount = sliFile.getPartCount();

            for (int part = 0; part < partCount; part++)
            {
                if (isPartEnabled(part))
                {
                    //- find layerIndex for this position
                    int index = sliFile.getLayerIndexByPos(part, layerPos);

                    //- read layer data (ToDo: only do this if layer-index for this part has changed)
                    sliFile.readSliceData(index, part);
                }
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
                int index = sliFile.getLayerIndexByPos(part, layerPos);

                lstPartNames.Items[part] = getPartListName(part, index);
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


        //----------------------------------------------//
        private void picOutput_Click(object sender, EventArgs e)
        {

        }


        //----------------------------------------------//
        private void lstPartNames_Validated(object sender, EventArgs e)
        {
            
        }


        //----------------------------------------------//
        private void lstPartNames_SelectedValueChanged(object sender, EventArgs e)
        {
            showLayer();
        }

        //----------------------------------------------//
        private void chkFill_CheckedChanged(object sender, EventArgs e)
        {
            showLayer();
        }

        //----------------------------------------------//
        private void btnErrorCLS_Click(object sender, EventArgs e)
        {
            lstError.Items.Clear();
        }

        //----------------------------------------------//
        private void chkShowDebug_CheckedChanged(object sender, EventArgs e)
        {
            s_showDebug = chkShowDebug.Checked;
        }

        //----------------------------------------------//
        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog opendlg = new System.Windows.Forms.OpenFileDialog();


            opendlg.FileName = System.IO.Path.GetFileNameWithoutExtension(txtFileName.Text);
            opendlg.AddExtension = true;
            opendlg.Filter = "EOS Job File (*.job;*.sli)|*.job;*.sli|" +
                             "EOS Job File (*.job)|*.job|" +
                             "EOS Slice File (*.sli)|*.sli|" +
                             "All Filetypes (*.*)|*.*";

            if (opendlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSliFileName.Text = opendlg.FileName;
            }
        }

        //----------------------------------------------//
        private void button3_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text;
            string user = txtUser.Text;
            string psw = txtPsw.Text;

            db = new clDB("HOSTNAME=" + host + ";PORT=49999;DRIVER={IBM DB2 ODBC DRIVER};UID=" + user + ";PWD=" + psw + ";DATABASE=EOSLOG;PROTOCOL=TCPIP", clDB.ENUM_DB_TYPE.DB_DB2);

            //- read List of machines
            clDB.Record[] machines = db.execute_to_array("SELECT MA_ID AS F01, MA_ID || ' [' ||  MT_TYPE || ']' AS F02 FROM EOSLOG.machines_tbl ORDER BY MA_ID", "F01", "F02");


            listMachines.Items.AddRange(machines);

            //- select first machine
            if (listMachines.Items.Count > 0) listMachines.SelectedIndex = 0;


            timer_refreshState.Enabled = true;

            refreshMachinState();

        }


        //----------------------------------------------//
        private void refereshJobList(string machineID)
        {
            if (db == null) return;

            lstJobs.Clear();


            //-   JO_INC, JO_USER,JO_DEFJOBNAME,JO_ZPREC
            string Query = @"SELECT
                                EOSLOG.JOBS_TBL.JO_ID AS JOB_ID,
                                VARCHAR_FORMAT(EOSLOG.JOBS_TBL.JO_TIMESTAMP, 'YYYY-MM-DD HH:MI:SS') AS JO_TIMESTAMP,
                                count(EOSLOG.LAYERS_TBL.JO_ID) AS Layer_Count,
                                EOSLOG.JOBS_TBL.JO_FILENAME,
                                EOSLOG.JOBS_TBL.MAT_NAME,
                                EOSLOG.JOBS_TBL.MAT_THICKNESS
                            FROM
	                            EOSLOG.JOBS_TBL
                            LEFT JOIN EOSLOG.MAINEVENTS_TBL ON EOSLOG.MAINEVENTS_TBL.JO_ID = EOSLOG.JOBS_TBL.JO_ID AND EOSLOG.MAINEVENTS_TBL.EV_ID = 30051 AND EOSLOG.MAINEVENTS_TBL.EV_MODULE = 10
                            LEFT JOIN EOSLOG.LAYERS_TBL ON EOSLOG.LAYERS_TBL.JO_ID = EOSLOG.JOBS_TBL.JO_ID
                            WHERE UPPER(EOSLOG.MAINEVENTS_TBL.MA_ID)=" + db.ToEscapeStr(machineID) + @"
                            GROUP BY  EOSLOG.JOBS_TBL.JO_ID, EOSLOG.JOBS_TBL.JO_TIMESTAMP,  EOSLOG.JOBS_TBL.JO_FILENAME, EOSLOG.JOBS_TBL.MAT_NAME, EOSLOG.JOBS_TBL.MAT_THICKNESS
                            ORDER BY EOSLOG.JOBS_TBL.JO_TIMESTAMP DESC";

            const int ROW_COUNT = 300;

            db.execute(Query, ROW_COUNT);


            db.setListViewHeader(lstJobs, "JOB_ID", new int[5] { 118, 100, 460, 80, 120 });

            int i = ROW_COUNT;

            while (!db.eof)
            {
                lstJobs.Items.Add(db.getListViewRow("JOB_ID"));

                i--;
                if (i <= 0) break;

                db.MoveNext();


            }

        }


        //----------------------------------------------//
        private void refereshLayerList(string JobID)
        {
            if (db == null) return;

            lstLayers.Clear();


            string Query = @"SELECT
                                VARCHAR_FORMAT(LA_TIMESTAMP, 'YYYY-MM-DD HH:MI:SS') AS LA_TIMESTAMP,
                                CAST((LA_ZHEIGHT / (LA_ZDELTA * 1000)+1) as decimal(10)) AS Layer,
                                LA_ZHEIGHT,
                                LA_ZDELTA
                            FROM
	                            EOSLOG.LAYERS_TBL
                            WHERE
                                JO_ID = " + db.ToEscapeStr(JobID) + @"
                            ORDER BY EOSLOG.LAYERS_TBL.LA_TIMESTAMP ASC";


            db.execute(Query);


            db.setListViewHeader(lstLayers, "", new int[4] { 146, 90, 138, 89 });

            while(!db.eof)
            {
                lstLayers.Items.Add(db.getListViewRow(""));
                db.MoveNext();
            }

        }


        //----------------------------------------------//
        private void refereshEventList(string machineID, string JobID)
        {
            if (db == null) return;

            lstEvents.Clear();


            string Query = @"SELECT
                                JO_INC AS F01,
                                VARCHAR_FORMAT(JO_TIMESTAMP, 'YYYY-MM-DD HH:MI:SS') AS F02
                            FROM  EOSLOG.JOBS_TBL
                            WHERE
                                JO_ID = " + db.ToEscapeStr(JobID);

            db.execute(Query, 1);
            string jobStartTime = db.FieldStr("F02");
            string jobIndex = db.FieldStr("F01");

            Query = @"SELECT
                        VARCHAR_FORMAT(JO_TIMESTAMP, 'YYYY-MM-DD HH:MI:SS') AS F01
                    FROM  EOSLOG.JOBS_TBL
                    WHERE
                        JO_INC > " + jobIndex + @"
                    ORDER BY JO_INC";

            db.execute(Query, 1);
            string jobEndTime = db.FieldStr("F01");



            Query = @"SELECT
                        VARCHAR_FORMAT(ME_TIMESTAMP, 'YYYY-MM-DD HH:MI:SS') AS ME_TIMESTAMP,
                        AT_NAME,
                        EA_BINDATA
                    FROM EOSLOG.EVENTARGS_TBL
                    LEFT JOIN  EOSLOG.ARGTYPES_TBL ON EOSLOG.ARGTYPES_TBL.AT_ID=EOSLOG.EVENTARGS_TBL.AT_ID
                    WHERE
                        APP_ID = 0 AND
                        UPPER(MA_ID) = " + db.ToEscapeStr(machineID) + @" AND
                        ME_TIMESTAMP >= " + db.ToEscapeStr(jobStartTime);

            if (jobEndTime.Length>4) Query += " AND ME_TIMESTAMP <= " + db.ToEscapeStr(jobEndTime) +" ";
            
            Query +="ORDER BY EA_POS";


            db.execute(Query);


            db.setListViewHeader(lstEvents, "", new int[3] { 120, 60, 550 });

            while (!db.eof)
            {
                lstEvents.Items.Add(db.getListViewRow(""));
                db.MoveNext();
            }

        }
        //----------------------------------------------//
        private void refereshPartsList(string JobID)
        {
            if (db == null) return;

            lstParts.Clear();


            string Query = @"SELECT
                                P_PARTID,
                                P_FILEPATH,
                                EP_NAME,
                                '('||CHAR(DECIMAL(P_X,5,2))||','||CHAR(DECIMAL(P_Y,5,2))||','||CHAR(DECIMAL(P_Z,5,2))||')' AS POS,
                                '('||CHAR(DECIMAL(P_XSCALE,5,2))||','||CHAR(DECIMAL(P_YSCALE,5,2))||')' AS SCALE,
                                DECIMAL(P_ROT,5,2) AS P_ROT,
                                '('||CHAR(DECIMAL(P_MINX,5,2))||','||CHAR(DECIMAL(P_MINY,5,2))||','||CHAR(DECIMAL(P_MINZ,5,2))||') ('||CHAR(DECIMAL(P_MAXX,5,2))||','||CHAR(DECIMAL(P_MAXY,5,2))||','||CHAR(DECIMAL(P_MAXZ,5,2))||')' AS SIZE
                            FROM EOSLOG.PARTS_TBL
                            WHERE
                                JO_ID = " + db.ToEscapeStr(JobID) + @"
                             ORDER BY P_PARTID";


            db.execute(Query);


            db.setListViewHeader(lstParts, "", new int[4] { 50, 200, 138, 89 });

            while(!db.eof)
            {
                lstParts.Items.Add(db.getListViewRow(""));
                db.MoveNext();
            }

        }
        

        //----------------------------------------------//
        private void refereshJobInfo(string JobID)
        {
            if (db == null) return;

            lstJobInfo.Clear();


            string Query = @"SELECT
                                VARCHAR_FORMAT(EOSLOG.JOBPARAMS_TBL.JP_TIMESTAMP, 'YYYY-MM-DD HH:MI:SS') AS JP_TIMESTAMP,
                                EOSLOG.JOBPARAMS_TBL.JP_TEXTDATA,
                                EOSLOG.PARAMTYPES_TBL.PT_NAME
                            FROM
	                            EOSLOG.JOBPARAMS_TBL
                            LEFT JOIN EOSLOG.PARAMTYPES_TBL ON EOSLOG.PARAMTYPES_TBL.PT_ID=EOSLOG.JOBPARAMS_TBL.PT_ID
                            WHERE
                                JO_ID = " + db.ToEscapeStr(JobID);


            db.execute(Query);


            db.setListViewHeader(lstJobInfo, "", new int[3] { 120, 600, 89 });

            while(!db.eof)
            {
                lstJobInfo.Items.Add(db.getListViewRow(""));
                db.MoveNext();
            }

        }

                
        //----------------------------------------------//
        private void refreshMachinState()
        {
            if (db == null) return;

            String Query = @"SELECT
                                   EOSLOG.STATUS_TBL.SE_TEXTDATA AS F_STATE,
                                   EOSLOG.STATUS_TBL.SE_TIMESTAMP AS F_TIME
                             FROM
                             	EOSLOG.STATUS_TBL
                             WHERE MA_ID=" + db.ToEscapeStr(m_machine);
            db.execute(Query, 1);



            labServerTime.Text = "State Timestamp: " + db.FieldStr("F_TIME");
            labStatusText.Text = db.FieldStr("F_STATE");



            Query = @"SELECT
                        JO_ID,
                        LA_ZHEIGHT, 
                        VARCHAR_FORMAT(LA_TIMESTAMP, 'YYYY-MM-DD HH:MI:SS') AS F02 ,
                        LA_ZDELTA, 
                    	TIMESTAMPDIFF(4, CHAR(CURRENT TIMESTAMP - EOSLOG.LAYERS_TBL.LA_TIMESTAMP)) AS f_timeold,
                    	CURRENT TIMESTAMP AS f_systemtimes
                    FROM EOSLOG.LAYERS_TBL
                    ORDER BY LA_TIMESTAMP DESC";
                 
            db.execute(Query, 1);



            labLayer.Text = "Layer [mm]: " + 0.001 * db.FieldInt("LA_ZHEIGHT", -1);
            labLayerNr.Text = "Layer Index: " + (0.001 * db.FieldInt("LA_ZHEIGHT", 0) / db.FieldDouble("LA_ZDELTA", 1));

            labLayerzDelta.Text = "Layer thickness [mm]: " + db.FieldStr("LA_ZDELTA");

            
        }

        //----------------------------------------------//
        private void listMachines_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_machine = ((clDB.Record)listMachines.SelectedItem).key;

            refereshJobList(m_machine);
        }


        //----------------------------------------------//
        private void timer1_Tick(object sender, EventArgs e)
        {
            refreshMachinState();
        }


        //----------------------------------------------//
        private void lstJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstJobs.SelectedIndices.Count <= 0) return; 

            int intselectedindex = lstJobs.SelectedIndices[0]; 
            if (intselectedindex >= 0) 
            {
                string jobID = lstJobs.Items[intselectedindex].Name;

                refereshLayerList( jobID );
                refereshJobInfo( jobID );
                refereshPartsList(jobID);
                refereshEventList(m_machine, jobID);
            } 
        }


    }
}
