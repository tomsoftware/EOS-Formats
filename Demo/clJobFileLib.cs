using System.Runtime.InteropServices;
using System;

namespace Demo
{

    class clJobFileLib
    {
        private const string LIB_DLL_NAME = "eos-format.dll";

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 jf_initLib();

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern void jf_freeLib(int jobI);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 jf_readFromFile(int jobI, byte []  fileName);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 jf_printXML(int jobI);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jf_getKeyName(int jobI, int keyIndex);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 jf_getFirstKeyChild(int jobI, int keyIndex);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 jf_getNextKeyChild(int jobI, int keyIndex);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 jf_getFirstProperty(int jobI, int propertyIndex);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 jf_getNextProperty(int jobI, int propertyIndex);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jf_getPropertyName(int jobI, int propertyIndex);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jf_getPropertyValue(int jobI, int propertyIndex);

        [DllImport(LIB_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jf_getPropertyComment(int jobI, int propertyIndex);


        private int m_lib = 0;

        //---------------------------------------------------//
        public clJobFileLib()
        {
            try
            {
                m_lib = jf_initLib();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("jf_initLib Error: " + e.Message);
            }
        }

        //---------------------------------------------------//
        ~clJobFileLib()
        {
            try
            {
                jf_freeLib(m_lib);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("jf_initLib Error: " + e.Message);
            }
        }

        //---------------------------------------------------//
        public bool readFromFile(string filename)
        {
            try
            {
                int ret = jf_readFromFile(m_lib, Str2CChr(filename));
                if (ret > 0) return true;

                System.Windows.Forms.MessageBox.Show("jf_readFromFile Error: " + ret);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("jf_initLib Error: " + e.Message);
            }
            return false;
        }

        //---------------------------------------------------//
        public int getFirstKeyChild(int KeyID = 0)
        {
            try
            {
                return jf_getFirstKeyChild(m_lib, KeyID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return 0;
            }
        }


        //---------------------------------------------------//
        public int getNextKeyChild(int KeyID = 0)
        {
            try
            {
                return jf_getNextKeyChild(m_lib, KeyID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return 0;
            }
        }

        //---------------------------------------------------//
        public int getFirstProperty(int KeyID = 0)
        {
            try
            {
                return jf_getFirstProperty(m_lib, KeyID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return 0;
            }
        }


        //---------------------------------------------------//
        public int getNextProperty(int PropID = 0)
        {
            try
            {
                return jf_getNextProperty(m_lib, PropID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return 0;
            }
        }
        

        //---------------------------------------------------//
        public string getKeyName(int KeyID = 0)
        {
            IntPtr ptr = jf_getKeyName(m_lib, KeyID);

            if (ptr == IntPtr.Zero) return "";

            return ptr2String(ptr);
        }

        //---------------------------------------------------//
        public string getPropertyName(int PropID = 0)
        {
            IntPtr ptr = jf_getPropertyName(m_lib, PropID);

            if (ptr == IntPtr.Zero) return "";

            return ptr2String(ptr);
        }

        //---------------------------------------------------//
        public string getPropertyValue(int PropID = 0)
        {
            IntPtr ptr = jf_getPropertyValue(m_lib, PropID);

            if (ptr == IntPtr.Zero) return "";

            return ptr2String(ptr);
        }

        //---------------------------------------------------//
        public string getPropertyComment(int PropID = 0)
        {
            IntPtr ptr = jf_getPropertyComment(m_lib, PropID);

            if (ptr == IntPtr.Zero) return "";

            return ptr2String(ptr);
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
        private byte[] Str2CChr(string strData="")
        {
            //- C# uses UNICODE for char-type so 2Byte!
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(strData);
            Array.Resize(ref bytes, bytes.Length + 1);
            bytes[bytes.Length - 1] = 0;
            return bytes;
        }

    }
}
