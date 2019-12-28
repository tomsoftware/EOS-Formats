using System;
using System.IO;
using ThermoBox.tools;
using System.Runtime.InteropServices;

//--------------------------
//- http://www.forwiss.uni-passau.de/~welisch/papers/cli_format.html
//--------------------------

namespace ThermoBox.slicedata
{
    class clCliFileReader : absSliceDataSource
    {
        //- für die Umwandlung von Int->Float verhält sich wie ein Union
        [StructLayout(LayoutKind.Explicit)]
        public struct BinaryConvertIntToFloat
        {
            [FieldOffset(0)]
            public float toFloat;
            [FieldOffset(0)]
            public int toInt;
        }
        private BinaryConvertIntToFloat converterIntFloat;


        private enum FileReadState
        {
            none,
            inhead,
            afterhead,
            ingeo
        }

        private struct tyFileHeader
        {
            public bool isBinary; //- binary format or asccii format?
            public float unit; //- scale Factor
            public int version;
            public int fileDate; //- Datum der "Datei"
            public double d_x1;
            public double d_y1;
            public double d_z1;
            public double d_x2;
            public double d_y2;
            public double d_z2;
            public int layers; //- Layer count
            public bool align; //- ich glaub -> bitreihenfolge??
            //public string userdata;
            public int dataOffset; //- start von "$$GEOMETRYSTART" in Datei

        }


        private struct tyIndexTable
        {
            public int FileOffset; //- offset in File
            public float layerPos; //- Syntax : $$LAYER/z  - Start of a layer with upper surface at height z (z*units [mm]).
            public int polyCount; //- count of Polylines
        }


        private int m_IndexTable_count;
        private tyIndexTable[] m_IndexTable;



        //- maximale länge die der Puffer haben darf (mit Kommentaren)
        const int HEADER_BUFFER_LEN = 3000;

        //private tyFileHead m_FileHead;
        private String m_filename;
        private BinaryReader m_reader;
        private bool m_eof;
        private int m_blocklen;
        private clError m_error;
        private int m_offset;
        private tyFileHeader m_FileHead;
        private System.String m_objectName;
        private bool m_is_enabeld = true;


        //------------------------------------------//
        public clCliFileReader(System.String filename = null)
        {
            m_error = new clError("CliFileReader");
            m_IndexTable_count = 0;

            setFileName(filename);
        }


        //------------------------------------------//
        public void setFileName(System.String filename)
        {
            if (openStream(filename))
            {
                m_filename = filename;
                readFile(filename);

                m_objectName = Path.GetFileName(m_filename);
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
            if (ObjectIndex != 0) return "";
            return m_objectName;
        }

        //------------------------------------------//
        public System.String getObjectInfo(int ObjectIndex)
        {
            if (ObjectIndex != 0) return "";
            return "";
        }


        //------------------------------------------//
        public bool getObjectEnabled(int ObjectIndex)
        {
            if (ObjectIndex != 0) return false;
            return m_is_enabeld;
        }

        //------------------------------------------//
        public void setObjectEnabled(int ObjectIndex, bool enable)
        {
            if (ObjectIndex == 0) m_is_enabeld = enable;
        }

        //------------------------------------------//
        public int getLayerCount()
        {
            return m_IndexTable_count;
        }


        //------------------------------------------//
        public float getLayerThickness() //- in [mm]
        {
            if (m_IndexTable.Length > 2)
                return (m_IndexTable[1].layerPos - m_IndexTable[0].layerPos);
            else
                return 0;

        }

        //- gibt die Position der Oberseite des Layers zurück
        public float getLayerUpPosition(int LayerIndex) //- in [mm]
        {
            if ((LayerIndex >= 0) && (LayerIndex < m_IndexTable_count))
            {
                return m_IndexTable[LayerIndex].layerPos;
            }
            else
            {
                return -1;
            }
        }

        //------------------------------------------//
        public int getObjectCount()
        {
            return 1;
        }

        //-------------------------------------//
        public ty_Matrix3x2 getSliceTransformMatrix(int ObjectIndex, int LayerIndex)
        {
            ty_Matrix3x2 tmp = new ty_Matrix3x2(); ;
            return tmp;
        }

        //------------------------------------------//
        public clSliceData getSliceData(int ObjectIndex, int LayerIndex, float jobLayerThickness)
        {
            clSliceData retSlice = new clSliceData();

            if (ObjectIndex != 0) return retSlice;

            if ((LayerIndex < 0) || (LayerIndex >= m_IndexTable_count)) return retSlice;

            setOffset(m_IndexTable[LayerIndex].FileOffset);

            float scale = m_FileHead.unit; //- Faktor in [mm]


            for (int i=0; i<m_IndexTable[LayerIndex].polyCount; i++)
            {
                int cmd = readIntBE(2);
                int pointCount=0;
                float [,] pointBuffer;

                switch (cmd)
                {
                    case 129: //- Start PolyLine short
                        readIntBE(2); //- id 
                        readIntBE(2); //- dir

                        pointCount = readIntBE(2); //- number of points 

                        pointBuffer = new float[pointCount,2];

                        for (int p=0; p<pointCount; p++)
                        {
                            pointBuffer[p, 0] = scale * readIntBE(2);
                            pointBuffer[p, 1] = scale * readIntBE(2);
                        }

                        retSlice.addPolygon(pointBuffer, pointCount);

                        break;


                    case 130: //- Start PolyLine long 

                        readIntBE(4); //- id 
                        readIntBE(4); //- dir
                        pointCount = readIntBE(4); //- number of points 

                        pointBuffer = new float[pointCount,2];

                        for (int p=0; p<pointCount; p++)
                        {
                            pointBuffer[p, 0] = readRealBE(4) * scale;
                            pointBuffer[p, 1] = readRealBE(4) * scale;
                        }

                        retSlice.addPolygon(pointBuffer, pointCount);
                        break;


                    default:
                        m_error.addError("indexFileBinary() : data out of sync. Unknown command: " + cmd + " at pos: " + m_offset);
                        break;

                }
            }

            return retSlice;

        }

        //------------------------------------------//
        private void readFile(String filename)
        {
            //- alle werte zurücksetzen
            m_FileHead = new tyFileHeader();

            if (readFileHeader())
            {
                if (m_FileHead.isBinary)
                {
                    indexFileBinary();
                }
                else
                {
                    m_error.addError("CLI-Ascii Format not supported");
                    //readFileAscii();
                }

            }
        }


        //------------------------------------------//
        private bool indexFileBinary()
        {
            setOffset(m_FileHead.dataOffset);


            m_IndexTable_count = m_FileHead.layers;
            m_IndexTable = new tyIndexTable[m_IndexTable_count];

            int currentLayer=-1;

            while ((currentLayer < (m_IndexTable_count-1)) && (!m_eof))
            {
                int cmd = readIntBE(2);
                int pointCount=0;

                switch (cmd)
                {
                    case 127: //- Start Layer long 
                        currentLayer++;
                        m_IndexTable[currentLayer].layerPos = readRealBE() * m_FileHead.unit;
                        m_IndexTable[currentLayer].FileOffset = m_offset;
                        m_IndexTable[currentLayer].polyCount = 0;
                        break;

                    case 128: //- Start Layer short 
                        currentLayer++;
                        m_IndexTable[currentLayer].layerPos = readIntBE(2) * m_FileHead.unit;
                        m_IndexTable[currentLayer].FileOffset = m_offset;
                        m_IndexTable[currentLayer].polyCount = 0;
                        break;

                    case 129: //- Start PolyLine short
                        m_IndexTable[currentLayer].polyCount++;

                        readIntBE(2); //- id 
                        readIntBE(2); //- dir

                        pointCount = readIntBE(2); //- number of points 

                        setOffset(m_offset + pointCount * 2 * 2); //- Daten überspringen (2 - für int ; 2 x+y)
                        break;

                    case 130: //- Start PolyLine long 
                        m_IndexTable[currentLayer].polyCount++;

                        readIntBE(4); //- id 
                        readIntBE(4); //- dir
                        pointCount = readIntBE(4); //- number of points 

                        setOffset(m_offset + pointCount * 4 * 2); //- Daten überspringen
                        break;

                    default:
                        m_error.addError("indexFileBinary() : data out of sync. Unknown command: " + cmd + " at pos: " + m_offset);
                        m_IndexTable_count = currentLayer+1;
                        return false;

                }

            }
            return true;
        }

        //------------------------------------------//
        private bool readFileHeader()
        {
            /*
                $$HEADERSTART
                $$BINARY
                $$UNITS/00000000.010000
                $$VERSION/200
                $$LABEL/1,part1
                $$DATE/040513
                $$DIMENSION/00000000.000000,00000000.000000,00000000.000000,00000010.000000,00000012.000000,00000012.980000
                $$LAYERS/000650
                $$HEADEREND
             */
            FileReadState state = FileReadState.none;


            //- der Header sollte nicht länger als 1000 Byte sein (in der Regel 300 Byte)
            byte[] buffer = readByte(HEADER_BUFFER_LEN+5, 0);
            int bufLen = buffer.Length-1;


            for (int i = 0; i < bufLen; i++)
            {
                if ((buffer[i] == '/') && (buffer[i + 1] == '/')) //- ein Kommentar beginnt
                {
                    for (i += 3; i < bufLen; i++)
                    {
                        if ((buffer[i-1] == '/') && (buffer[i] == '/')) break; //- Kommentar Ende
                    }

                }
                else if ((buffer[i] == '$') && (buffer[i + 1] == '$')) //- ein Befehl
                {
                    i+=2; //- 2 bytes ($$) überspringen
                    bool okToken = false;

                    switch (state)
                    {
                        case FileReadState.none:
                            if (compareAndMoveBuffer(buffer, ref i, bufLen, "HEADERSTART"))
                            {
                                state = FileReadState.inhead;
                                okToken = true;
                            }
                            break;

                        case FileReadState.inhead:
                            if (compareAndMoveBuffer(buffer, ref i, bufLen, "BINARY"))
                            {
                                m_FileHead.isBinary = true;
                                okToken = true;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "ASCII"))
                            {
                                m_FileHead.isBinary = false;
                                okToken = true;
                                break;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "LAYERS/"))
                            {
                                m_FileHead.layers = readIntAndMove(buffer, ref i, bufLen);
                                okToken = true;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "UNITS/"))
                            {
                                m_FileHead.unit = (float)readRealAndMove(buffer, ref i, bufLen);
                                okToken = true;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "VERSION/"))
                            {
                                m_FileHead.version = readIntAndMove(buffer, ref i, bufLen);
                                okToken = true;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "DIMENSION/"))
                            {
                                m_FileHead.d_x1 = readRealAndMove(buffer, ref i, bufLen);
                                if ((buffer[i] != ',')) break; i++;//- trennzeichen dazwischen
                                m_FileHead.d_y1 = readRealAndMove(buffer, ref i, bufLen);
                                if ((buffer[i] != ',')) break; i++;//- trennzeichen dazwischen
                                m_FileHead.d_z1 = readRealAndMove(buffer, ref i, bufLen);
                                if ((buffer[i] != ',')) break; i++;//- trennzeichen dazwischen
                                m_FileHead.d_x2 = readRealAndMove(buffer, ref i, bufLen);
                                if ((buffer[i] != ',')) break; i++;//- trennzeichen dazwischen
                                m_FileHead.d_y2 = readRealAndMove(buffer, ref i, bufLen);
                                if ((buffer[i] != ',')) break; i++;//- trennzeichen dazwischen
                                m_FileHead.d_z2 = readRealAndMove(buffer, ref i, bufLen);

                                okToken = true;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "DATE/"))
                            {
                                m_FileHead.fileDate = readIntAndMove(buffer, ref i, bufLen);

                                okToken = true;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "LABEL/"))
                            {
                                //- id: Identifier to allow more than one model information in one file. For every id used in the commands start polyline (short/long/ASCII) and start hatches (short/long/ASCII) there shall be one command $$LABEL. Each id causes the building-process to build a different part. 
                                readIntAndMove(buffer, ref i, bufLen);

                                //- trennzeichen dazwischen
                                if ((buffer[i] != ',')) break; i++;
                                

                                //- text: An ASCII string that gives some comment on the part. 
                                readStrAndMove(buffer, ref i, bufLen);

                                okToken = true;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "ALIGN"))
                            {
                                m_FileHead.align = true;
                                okToken = true;
                            }
                            else if (compareAndMoveBuffer(buffer, ref i, bufLen, "HEADEREND"))
                            {
                                m_FileHead.dataOffset = i;
                                state = FileReadState.afterhead;
                                return true;
                            }

                            break;
                            
                    }

                    if (!okToken)
                    {
                        m_error.addError("Unknown token in file at pos: " + i);
                    }


                }
                else //- unbekannte Zeichenfolge
                {
                    if ((buffer[i] != ' ') && (buffer[i] != 9) && (buffer[i] != 10) && (buffer[i] != 13))
                    {
                        m_error.addError("Unknown char in file at pos: "+ i);
                    }
                }
            }

            return false;

        }
        

        //-------------------------------------//
        private double readRealAndMove(byte[] buffer, ref int startIndex, int bufLen)
        {
            int i=startIndex;

            int pre = readIntAndMove(buffer, ref i, bufLen);

            if (buffer[i] != '.')
            {
                startIndex = i;
                return 0;
            }

            i++;

            int post = readIntAndMove(buffer, ref i, bufLen);

            startIndex = i;

            return (double)pre + (1.0 / post);

        }

        //-------------------------------------//
        private float readRealBE(int offset = -1)
        {
            converterIntFloat.toInt = readIntBE(4, offset);
            return converterIntFloat.toFloat;
        }

        //-------------------------------------//
        private string readStrAndMove(byte[] buffer, ref int startIndex, int bufLen)
        {
            int i = startIndex;
            int startPos = startIndex;

            //- irgendwelche leerzeichen?
            for (; i < bufLen; i++)
            {
                if ((buffer[i] == 13) || (buffer[i] == 10) || (buffer[i] == '$') || (buffer[i] == '/')) break;
            }

            startIndex = i;

            return System.Text.Encoding.ASCII.GetString(buffer, startPos, i - startPos);
        }


        //-------------------------------------//
        private int readIntAndMove(byte[] buffer, ref int startIndex, int bufLen)
        {
            int i = startIndex;
            bool neg = false;
            long ret=0;
            int c=0;

            //- irgendwelche leerzeichen?
            for (; i < bufLen; i++)
            {
                if ((buffer[i]!=' ') && (buffer[i]!=9)) break;
            }

            //-- Vorzeichen
            if (buffer[i]=='-')
            {
                i++;
                neg=true;
            }
            else if (buffer[i]=='+')
            {
                i++;
            }


            for (; i < bufLen; i++)
            {
                if ((buffer[i]<'0') || (buffer[i]>'9')) break;

                c++;
                if (c < 11) 
                {
                    ret = ret * 10 + (buffer[i]-'0');
                }
                else
                {
                    ret = 0; //- zahl ist zu lang??!!
                }

                
            }

            startIndex = i;

            if (ret>2147483648) return 0; //- zah ist zu groß

            if (neg) ret = -ret;

            return (int)ret;


        }


        //-------------------------------------//
        private bool compareAndMoveBuffer(byte[] buffer, ref int startIndex, int bufLen, string compStr)
        {
            int l = compStr.Length;
            if ((startIndex + l) > bufLen) return false;

            byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(compStr);

            for (int i = 0; i < l; i++)
            {
                if (buffer[i + startIndex] != asciiBytes[i]) return false;
            }

            startIndex += l;

            return true;
        }

        //-------------------------------------//
        private void setOffset(int offset)
        {
            if (offset > -1)
            {
                m_reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
                m_offset = offset;
            }
        }

        //-------------------------------------//
        private int readIntBE(int lenght, int offset = -1)
        {
            int outVal = 0;
            if (lenght > 4) lenght = 4;

            byte[] tmpData = this.readByte(lenght, offset);

            if (tmpData != null)
            {
                lenght = tmpData.Length;

                if (lenght == 4)
                {
                    outVal = (int)tmpData[0] + (tmpData[1] << 8) + (tmpData[2] << 16) + (tmpData[3] << 24);
                }
                else if (lenght == 2)
                {
                    outVal = (int)tmpData[0] + (tmpData[1] << 8);
                }
                else if (lenght == 1)
                {
                    outVal = (int)tmpData[0];
                }
                else if (lenght == 3)
                {
                    outVal = (int)tmpData[0] + (tmpData[1] << 8) + (tmpData[2] << 16);
                }
            }

            return outVal;
        }
		

        //-------------------------------------//
        private byte[] readByte(int lenght, int offset = -1)
        {
            if (lenght < 0) lenght = 0;
            setOffset(offset);

            byte[] dataArray = null;


            if (m_offset < m_blocklen)
            {
                if ((m_offset + lenght) > m_blocklen)
                {
                    m_eof = true;
                    lenght = m_blocklen - m_offset;
                }

                dataArray = new byte[lenght];


                try
                {
                    lenght = m_reader.Read(dataArray, 0, lenght);
                }
                catch
                {
                    lenght = 0;
                    m_eof = true;
                }

                if (lenght != dataArray.Length) System.Array.Resize(ref dataArray, lenght);
            }
            else
            {
                if (lenght > 0)
                {
                    m_error.addWarning("ReadStr:EOF!");
                    lenght = 0;
                }

                m_eof = true;
            }


            if (lenght > 0) m_offset = m_offset + lenght;

            return dataArray;
        }

        //-----------------------------------------------//
        private bool openStream(String filename)
        {

            this.m_eof = true;
            this.m_blocklen = 0;
            this.m_offset = 0;

            try
            {
                m_reader = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read));

                this.m_eof = false;
                m_blocklen = (int)m_reader.BaseStream.Length; //- Beschränkung auf max 2GB Dateigröße
            }
            catch (System.Exception ex)
            {
                this.m_error.addError("IO.BinaryReader fail", ex.Message, filename);
                return false;
            }

            return true;
        }
    }
}
