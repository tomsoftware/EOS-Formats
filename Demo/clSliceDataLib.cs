using System.Runtime.InteropServices;
using System;


namespace Demo
{
    public class clSliceDataLib
    {

        private const string LIB_DLL_NAME = "eos-format.dll";


        /// <summary>
        /// Homogeneous coordinates Matrix - used for DLL-Libary call.
        /// </summary>
        public class clMatrix3x2
        {
            //-------------------------------------------------------//
            public struct ty_Matrix
            {
                //- do not change! used for dll-libary-call
                public float m11, m12, m13;
			    public float m21, m22, m23;
            }

            public ty_Matrix m;


            //-------------------------------------------------------//
            public clMatrix3x2()
            {
                //- load Identity matrix
                m.m11 = 1; m.m12 = 0; m.m13 = 0;
                m.m21 = 0; m.m22 = 1; m.m23 = 0;
            }


            //-------------------------------------------------------//
            public static clMatrix3x2 MatrixMult(clMatrix3x2 A, clMatrix3x2 B)
            {
                clMatrix3x2 C = new clMatrix3x2();

                C.m.m11 = A.m.m11 * B.m.m11 + A.m.m12 * B.m.m21 + A.m.m13 * 0;
                C.m.m12 = A.m.m11 * B.m.m12 + A.m.m12 * B.m.m22 + A.m.m13 * 0;
                C.m.m13 = A.m.m11 * B.m.m13 + A.m.m12 * B.m.m23 + A.m.m13 * 1;

                C.m.m21 = A.m.m21 * B.m.m11 + A.m.m22 * B.m.m21 + A.m.m23 * 0;
                C.m.m22 = A.m.m21 * B.m.m12 + A.m.m22 * B.m.m22 + A.m.m23 * 0;
                C.m.m23 = A.m.m21 * B.m.m13 + A.m.m22 * B.m.m23 + A.m.m23 * 1;

                return C;
            }
        }



        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 sf_initLib();


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 sf_getPartCount(int sliI);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern float sf_getLayerThickness(int sliI);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 sf_getLayerCount(int sliI, int partIndex);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern float sf_getMaxLayerPos(int sliI, int partIndex);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern float sf_getLayerPos(int sliI, int partIndex, int layerIndex);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int sf_getLayerIndexByPos(int sliI, int partIndex, float layerPos);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 sf_freeLib(int sliI);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 sf_addRasterObject(int sliI, int [,] outFilledPicture, int [,] outLinePicture, int partIndex, clMatrix3x2.ty_Matrix matrix, int color, int width, int height);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 sf_readFromFile(int sliI, byte [] fileName);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 sf_readSliceData(int sliI, int partIndex, int LayerIndex);
        
        
        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sf_getPartName(int sliI, int partIndex);


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sf_getPartProperty(int sliI, int partIndex);
        

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sf_getLastError();


        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sf_getLastDebug();


        private int m_SLI_lib = 0;



        //---------------------------------------------------//
        public clSliceDataLib()
        {
            m_SLI_lib = sf_initLib();
        }


        //---------------------------------------------------//
        public void readFromFile(string fileName)
        {
            int i = sf_readFromFile(m_SLI_lib, Str2CChr(fileName));
            checkForErros();
        }

                
        //---------------------------------------------------//
        private void checkForErros()
        {
            Form1.addError( getLastError(), false);
            Form1.addError( getLastDebug(), true);
        }


        //---------------------------------------------------//
        public void readSliceData(int layer, int partIndex)
        {
            int i = sf_readSliceData(m_SLI_lib, partIndex, layer);
            checkForErros();
        }
        

        //---------------------------------------------------//
        /// <summary>
        /// randers the polyline to [outAreaPicture] and [outLinePicture]. [outAreaPicture] will be filled with [RGBColor]. [outLinePicture] will only be outlined with [RGBColor].
        /// </summary>
        /// <param name="outAreaPicture">Picture-Buffer.<br /> <b>No</b> [RGBColor] values are allowed within the buffer-data!<br /> Can be NULL.</param>
        /// <param name="outLinePicture">Picture-Buffer.<br /> Can be NULL.</param>
        /// <param name="partIndex">Index of the Part to draw</param>
        /// <param name="matrix">Homogeneous coordinates to transform the point data</param>
        /// <param name="RGBColor">RGB color to draw with</param>
        public void addRasterObject(int[,] outAreaPicture, int[,] outLinePicture, int partIndex, clMatrix3x2 matrix, int RGBColor)
        {
            int w=0;
            int h=0;
            if (outLinePicture != null)
            {
                w=outLinePicture.GetLength(0);
                h=outLinePicture.GetLength(1);
            }
            else if (outAreaPicture !=  null)
            {
                w=outAreaPicture.GetLength(0);
                h=outAreaPicture.GetLength(1);
            }
            else
            {
                return;
            }


            int i = sf_addRasterObject(m_SLI_lib, outAreaPicture, outLinePicture, partIndex, matrix.m, RGBColor, w, h);
            checkForErros();

            //System.Diagnostics.Debug.WriteLine("addRasterObject() return = " + i);
        }


        //---------------------------------------------------//
        public int getLayerCount(int partIndex=-1)
        {
            if (partIndex>=0) return sf_getLayerCount(m_SLI_lib, partIndex);

            //- find maximum of index
            int max_index = 0;
            int partCount = getPartCount();

            for (int i = 0; i < partCount; i++ )
            {
                max_index = System.Math.Max(max_index, sf_getLayerCount(m_SLI_lib, i));
            }
            

            return max_index;
        }


        //---------------------------------------------------//
        public float getLayerThickness()
        {
            return sf_getLayerThickness(m_SLI_lib);
        }


        //---------------------------------------------------//
        public float getMaxLayerPos(int partIndex = -1)
        {
            if (partIndex >= 0) return sf_getMaxLayerPos(m_SLI_lib, partIndex);

            //- find maximum layer position
            float max_pos = 0;
            int partCount = getPartCount();

            for (int i = 0; i < partCount; i++)
            {
                max_pos = System.Math.Max(max_pos, sf_getMaxLayerPos(m_SLI_lib, i));
            }


            return max_pos;
        }
        

        //---------------------------------------------------//
        public float getLayerPos(int partIndex, int LayerIndex)
        {
            return sf_getLayerPos(m_SLI_lib, partIndex, LayerIndex);
        }
           

        //---------------------------------------------------//
        public int getLayerIndexByPos(int partIndex, float layerPos)
        {
            return sf_getLayerIndexByPos(m_SLI_lib, partIndex, layerPos);
        }
           
        

        //---------------------------------------------------//
        public string getPartName(int  partIndex)
        {
            IntPtr ptr = sf_getPartName(m_SLI_lib, partIndex);

            if (ptr == IntPtr.Zero) return "";

            return ptr2String(ptr);
        }
                
        
        //---------------------------------------------------//
        public string getPartProperty(int partIndex)
        {
            IntPtr ptr = sf_getPartProperty(m_SLI_lib, partIndex);
            
            if (ptr == IntPtr.Zero) return "";

            return ptr2String(ptr);
        }
        
        //---------------------------------------------------//
        public int getPartCount()
        {
            return sf_getPartCount(m_SLI_lib);
        }

        
        //---------------------------------------------------//
        private byte[] Str2CChr(string strData = "")
        {
            //- C# uses UNICODE for char-type so 2Byte!
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(strData);
            Array.Resize(ref bytes, bytes.Length + 1);
            bytes[bytes.Length - 1] = 0;
            return bytes;
        }


        //---------------------------------------------------//
        private string ptr2String(IntPtr ptr)
        {
            int len=0;

            while (System.Runtime.InteropServices.Marshal.ReadByte(ptr, len) != 0) len++;

            if (len == 0)  return "";

            byte[] array = new byte[len];

            System.Runtime.InteropServices.Marshal.Copy(ptr, array, 0, len);

            return System.Text.Encoding.Default.GetString(array);
        }


        //---------------------------------------------------//
        public string getLastError()
        {
            IntPtr ptr = sf_getLastError();

            if (ptr == IntPtr.Zero) return "";

            return ptr2String(ptr);
        }


        //---------------------------------------------------//
        public string getLastDebug()
        {
            IntPtr ptr = sf_getLastDebug();

            if (ptr == IntPtr.Zero) return "";

            return ptr2String(ptr);
        }
        
    }
}
