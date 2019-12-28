using System;
using ThermoBox.tools;

namespace ThermoBox.slicedata
{
    public class clSliceDataList : absSliceDataSource
    {
        private System.Collections.Generic.List <absSliceDataSource> m_List = new System.Collections.Generic.List<absSliceDataSource>();

        private String m_filename;
        private String m_filePath;

        private int m_max_Layers;
        private float m_layerthickness;
        private clError m_error;
        private bool m_is_enabeld = true;

        //------------------------------------------//
        public clSliceDataList(string filename="")
        {
            m_max_Layers = 0;
            m_layerthickness = 0;

            m_error = new clError("SliceDataList");

            setFileName(filename);
        }

        //------------------------------------------//
        public void addSlice(string filename)
        {
            if (filename != "")
            {
                if (System.IO.File.Exists(filename))
                {
                    addSlice(clHelper.OpenSliceFile(filename));
                }
                else
                {
                    addSlice(clHelper.OpenSliceFile(m_filePath + filename));
                }
            }
        }


        //------------------------------------------//
        public void addSlice(absSliceDataSource sliceData)
        {
            if (m_max_Layers < sliceData.getLayerCount()) m_max_Layers = sliceData.getLayerCount();
            if (m_layerthickness != sliceData.getLayerThickness()) m_layerthickness = sliceData.getLayerThickness();


            m_List.Add(sliceData);
        }


        //------------------------------------------//
        public void setFileName(System.String filename)
        {
            m_filename = filename;
            m_filePath = System.IO.Path.GetDirectoryName(filename);
            if (m_filePath[m_filePath.Length - 1] != System.IO.Path.DirectorySeparatorChar) m_filePath = m_filePath + System.IO.Path.DirectorySeparatorChar.ToString();

            String[] files;

            try
            {

                files = System.IO.File.ReadAllLines(filename);
            }
            catch 
            {
                files = new string[0]; 
            }


            for (int i = 0; i < files.Length; i++)
            {
                addSlice(files[i]);
            }

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
            return m_List[ObjectIndex].getObjectName(0);
        }


        //------------------------------------------//
        public System.String getObjectInfo(int ObjectIndex)
        {
            if ((ObjectIndex < 0) || (ObjectIndex >= m_List.Count)) return "";
            return m_List[ObjectIndex].getObjectInfo(0);
        }

        //------------------------------------------//
        public bool getObjectEnabled(int ObjectIndex)
        {
            if ((ObjectIndex < 0) || (ObjectIndex >= m_List.Count)) return false;
            return m_List[ObjectIndex].getObjectEnabled(0);
        }

        //------------------------------------------//
        public void setObjectEnabled(int ObjectIndex, bool enable)
        {
            if (ObjectIndex == 0) m_is_enabeld = enable;
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

        //------------------------------------------//
        //- gibt die Position der Oberseite des Layers zurück
        public float getLayerUpPosition(int LayerIndex)
        {
           
                return -1;

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

            return m_List[ObjectIndex].getSliceData(0, LayerIndex, jobLayerThickness);
        }


        //-------------------------------------//
        public ty_Matrix3x2 getSliceTransformMatrix(int ObjectIndex, int LayerIndex)
        {
            ty_Matrix3x2 tmp = new ty_Matrix3x2(); ;
            return tmp;
        }
    }
}
