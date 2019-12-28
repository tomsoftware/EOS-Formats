using System;
using ThermoBox.tools;
using ThermoBox.config;


namespace ThermoBox.slicedata
{
    public class clSliceJobFile : absSliceDataSource
    {

        private class ty_part
        {
            public double zs;
            public double z;
            public double ys;
            public double y;
            public double xs;
            public double x;
            public double rotation;
            public string filename;
            public string ExpParName;
            public bool isEnabled;
            public float layerthikness;

            public absSliceDataSource sliceClass;
        }

        private System.Collections.Generic.List <ty_part> m_List = new System.Collections.Generic.List<ty_part>();

        private String m_filename;
        private String m_filePath;

        private int m_max_Layers;
        private float m_layerthickness;
        private ty_part m_partZMax;
        private clError m_error;
        private clEosJobFileReader m_eosJob;



        //------------------------------------------//
        public clSliceJobFile(string filename = "")
        {
            m_max_Layers = 0;
            m_layerthickness = 0;
            m_error = new clError("clSliceJobFile");

            setFileName(filename);
        }



        //------------------------------------------//
        private void addSlice(ty_part partInfo)
        {
            if (partInfo.filename != "")
            {
                string tmp = "";

                try
                {
                    tmp = System.IO.Path.GetFileName(partInfo.filename);
                }
                catch (System.Exception e)
                {
                    m_error.addError("addSlice"+ e.Message);
                    return;
                }

                string filename = "";


                if (System.IO.File.Exists(m_filePath + partInfo.filename))
                {
                    filename = m_filePath + partInfo.filename;
                }
                else if (System.IO.File.Exists(m_filePath + tmp))
                {
                    filename = m_filePath + tmp;
                }
                else if (System.IO.File.Exists(partInfo.filename))
                {
                   filename = partInfo.filename;
                }



                if (filename != "")
                {
                    //- open slice file
                    absSliceDataSource sliceClass = clHelper.OpenSliceFile(filename);
                    
                    partInfo.sliceClass = sliceClass;

                    //int test = ((clSliFileReader)sliceClass).getLayerUpPosition(1);


                    if (m_max_Layers < sliceClass.getLayerCount())
                    {
                        m_max_Layers = sliceClass.getLayerCount();
                        m_partZMax = partInfo;  // Speichern des höchsten Bauteils für 
                    }

                    partInfo.layerthikness = sliceClass.getLayerThickness();

                    //- diese klasse hat als Layer-Stärke die minimale Slise-Stärke aller Teile
                    m_layerthickness = Math.Min(m_layerthickness, partInfo.layerthikness);
                    
                

                    m_List.Add(partInfo);
                }
                else
                {
                    m_error.addError("Can't find file: [" + partInfo.filename + "]");
                }
            }
        }



        //------------------------------------------//
        public void setFileName(System.String filename)
        {
            m_filename = filename;
            m_filePath = System.IO.Path.GetDirectoryName(filename);
            if (m_filePath.Substring(m_filePath.Length-1,1)!="\\") m_filePath = m_filePath + "\\";


            m_eosJob = new clEosJobFileReader(filename);

            int partsID = m_eosJob.getOpenTreeElement("Parts");
            int partsCount = m_eosJob.getElementChildCount(partsID);

            for (int i = 0; i < partsCount; i++)
            {
                int part = m_eosJob.getElementChild(partsID, i);
                int property = m_eosJob.getSubElement(part, "FileName");

                if (property > 0)
                {

                    ty_part newPart = new ty_part();
                    newPart.x = m_eosJob.getSubElementValueDouble(part,"x");
                    newPart.xs = m_eosJob.getSubElementValueDouble(part,"xs");
                    
                    newPart.y = m_eosJob.getSubElementValueDouble(part,"y");
                    newPart.ys = m_eosJob.getSubElementValueDouble(part,"ys");

                    newPart.z = m_eosJob.getSubElementValueDouble(part,"z");
                    newPart.zs = m_eosJob.getSubElementValueDouble(part,"zs");

                    newPart.rotation = m_eosJob.getSubElementValueDouble(part, "Rotation");

                    newPart.filename = m_eosJob.getSubElementValueString(part,"FileName");
                    newPart.ExpParName = m_eosJob.getSubElementValueString(part, "ExpParName").Trim();

                    newPart.isEnabled = true;
                    newPart.layerthikness = 0;

                    if (String.Equals(newPart.ExpParName, "No_Exposure", StringComparison.OrdinalIgnoreCase)) newPart.isEnabled = false;
                    if (String.Equals(newPart.ExpParName, "_ExternalSupport", StringComparison.OrdinalIgnoreCase)) newPart.isEnabled = false;

                    addSlice(newPart);

                }
            }

            //- debug out File!
            //System.Diagnostics.Debug.WriteLine(m_eosJob.getXML());
        }

        //------------------------------------------//
        public System.String getFileName()
        {
            return m_filename;
        }
        
        //------------------------------------------//
        public System.String getObjectName(int ObjectIndex)
        {
            if ((ObjectIndex < 0) || (ObjectIndex >= m_List.Count)) return "";
            return m_List[ObjectIndex].sliceClass.getObjectName(0);
        }

        //------------------------------------------//
        public System.String getObjectInfo(int ObjectIndex)
        {
            if ((ObjectIndex < 0) || (ObjectIndex >= m_List.Count)) return "";
            return m_List[ObjectIndex].ExpParName;
        }

        //------------------------------------------//
        public bool getObjectEnabled(int ObjectIndex)
        {
            if ((ObjectIndex < 0) || (ObjectIndex >= m_List.Count)) return false; 
            return m_List[ObjectIndex].isEnabled;
        }

        //------------------------------------------//
        public void setObjectEnabled(int ObjectIndex, bool enable)
        {
            if ((ObjectIndex < 0) || (ObjectIndex >= m_List.Count)) return; 
            m_List[ObjectIndex].isEnabled = enable;
        }


        //------------------------------------------//
        public int getLayerCount()
        {
            return m_max_Layers;
        }


        //------------------------------------------//
        public float getLayerThickness()
        {
            return m_layerthickness;

        }

        //- gibt die Position der Oberseite des Layers zurück
        public float getLayerUpPosition(int LayerIndex)
        {

            return -1;
            //return m_partZMax.sliceClass.getLayerUpPosition(LayerIndex);

        }


        //------------------------------------------//
        public int getObjectCount()
        {
            return m_List.Count;
        }


        //------------------------------------------//
        public clSliceData getSliceData(int ObjectIndex, int LayerIndex, float jobLayerThickness)
        {
            if ((ObjectIndex < 0) || (ObjectIndex >= m_List.Count)) return new clSliceData();

            double zshift = m_List[ObjectIndex].z;

            return m_List[ObjectIndex].sliceClass.getSliceData(0, (int)(LayerIndex * jobLayerThickness / m_List[ObjectIndex].layerthikness), jobLayerThickness);
        }



        //-------------------------------------//
        public ty_Matrix3x2 getSliceTransformMatrix(int ObjectIndex, int LayerIndex)
        {
            ty_Matrix3x2 tmpA = new ty_Matrix3x2();
            ty_Matrix3x2 tmpB = new ty_Matrix3x2();

            if ((ObjectIndex < 0) || (ObjectIndex >= m_List.Count)) return tmpA;

            ty_part tmpPart = m_List[ObjectIndex];

            float a = (float)(tmpPart.rotation * 2 * Math.PI/360.0);

            //- toDo : rotation und skalierung!
            tmpA.m00 = Math.Cos(a); tmpA.m10 = -Math.Sin(a);
            tmpA.m01 = Math.Sin(a); tmpA.m11 = Math.Cos(a);

            tmpA.m20 = 0;
            tmpA.m21 = 0;



            tmpB.m00 = 1; tmpB.m10 = 0;
            tmpB.m01 = 0; tmpB.m11 = 1;

            tmpB.m20 = (float)tmpPart.x;
            tmpB.m21 = (float)tmpPart.y;


            ty_Matrix3x2 resultMat = ty_Matrix3x2.MatrixMult(tmpB, tmpA);


            return resultMat;
        }
    }
}
