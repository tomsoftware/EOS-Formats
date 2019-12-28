using System;
using System.IO;
using ThermoBox.tools;
using System.Runtime.InteropServices;

namespace ThermoBox.slicedata
{
    public class clSliFileReader : absSliceDataSource
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



        private struct tyFileHead
		{
            public System.String magic;
            public int int01; //- Header länge ?
            public int int02; //- Header anzahl ?
            public int FileDataOffset; //- Header offset

            public int   int05;

            public int int07;
            public int FileSliceDataOffset;
            public int FileIndexPos ;
            public System.String String2 ;
            public int LayerCount1 ;
            public int LayerCount2;
  
            public int   int14;

            public float scaleFactor;
		};
        

        private struct tyIndexTable
        {
            public int FileOffset;
            public float layerPos; //- Syntax : $$LAYER/z  - Start of a layer with upper surface at height z (z*units [mm]).
        }

        private int m_IndexTable_count;
        private tyIndexTable[] m_IndexTable;

        private tyFileHead m_FileHead;
        private String m_filename;
        private BinaryReader m_reader;
        private bool m_eof;
        private int m_blocklen;
        private clError m_error;
        private int m_offset;
        private System.String m_objectName;
        private bool m_is_enabeld = true;

        //------------------------------------------//
        public clSliFileReader(System.String filename=null)
        {
            m_error = new clError("SliFileReader");

            if (filename != null) this.setFileName(filename);

        }

        //------------------------------------------//
        public void setFileName(System.String filename)
        {
            this.m_filename = filename;

            m_objectName = Path.GetFileName(m_filename);

            if (openStream(filename))
            {
                readHead();
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
        public System.String getObjectInfo(int ObjectIndex)
        {
            if (ObjectIndex != 0) return "";
            return "";
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


        //------------------------------------------//
        public int getObjectCount()
        {
            return 1;
        }


        //------------------------------------------//
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


        //-------------------------------------//
        public ty_Matrix3x2 getSliceTransformMatrix(int ObjectIndex, int LayerIndex)
        {
            ty_Matrix3x2 tmp = new ty_Matrix3x2(); ; // ist Einheitsmatrix
            return tmp;
        }


        //------------------------------------------//
        public clSliceData getSliceData(int ObjectIndex, int LayerIndex, float jobLayerThickness)
        {
            clSliceData sd = new clSliceData();
            int n = 0;

            if (ObjectIndex != 0) return sd;


            float scaleFactor = m_FileHead.scaleFactor;

            //m_error.addInfo(System.IO.Path.GetFileName( m_filename)  +" : factor: " + scaleFactor + "; ");


            int convertedLayerindex = LayerIndex - (int)(m_IndexTable[0].layerPos / jobLayerThickness);     // Min z




            if ((convertedLayerindex >= 0) && (convertedLayerindex < m_IndexTable_count))
            {




                setOffset(m_IndexTable[convertedLayerindex].FileOffset);

                int OType =0;
                while (OType!=2)
                {
                    //- Befehl/Operator Byte
                    OType = readIntBE(1);
    
                    switch (OType)
                    {
                        case 1:
                            //- ersten 2 Byte sind ca. die Höhe??!!!
                            readByte(11); //- Header überspringen
                            //- Header... lesen???!!!
                            break;

                        case 2:
                            //- Ende
                            break;

                        case 3:
                            //Command : start polyline
                            //Syntax : $$POLYLINE/id,dir,n,p1x,p1y,...pnx,pny
                            //Parameters:
                            //
                            //	id		: INTEGER
                            //	dir,n		: INTEGER
                            //	p1x..pny	: REAL
                            //
                            //
                            //id : identifier to allow more than one model information in one file.
                            //id refers to the parameter id of command $$LABEL (HEADER-section).
                            //dir : Orientation of the line when viewing in the negative z-direction
                            //0 : clockwise (internal)
                            //1 : counter-clockwise (external)
                            //2 : open line (no solid)
                            //n : number of points
                            //p1x..pny : coordinates of the points 1..n 

                            if (readIntBE(1) != 0) //- vielleicht "dir"?
                            {
                                m_error.addWarning("unbekanntes Byte [dir?] @ " + m_offset);
                            }
        
                            n = readIntBE(2);
        
        
                            if (n > 0)
                            {
                                //int n2 = n*2; //- x+y
                                float[,] points = new float[n,2];

                                //- Punkte lesen
                                for (int i=0;i<n;i++)
                                {
                                    //- Punkt lesen und mit internem Faktor skalieren
                                    points[i, 0] = scaleFactor * readIntBE(2);
                                    points[i, 1] = scaleFactor * readIntBE(2);
                                }

                                sd.addPolygon(points, n);
                            }
        
                            break;
                        case 4:
                            //- support
                            //m_error.addWarning("Support is not supported!");

                            //Command : start hatches
                            //Syntax : $$HATCHES/id,n,p1sx,p1sy,p1ex,p1ey,...pnex,pney
                            //Parameters:
                            //
                            //	id		: INTEGER
                            //	n		: INTEGER
                            //	p1sx..pney	: REAL
                            //
                            //id : identifier to allow more than one model information in one file.
                            //id refers to the parameter id of command $$LABEL (HEADER-section).
                            //n : number of hatches (n*4 =number of coordinates)
                            //p1sx..pney : coordinates of the hatches 1..n
                            //4 parameters for every hatch (startx,starty,endx,endy) 


                            //- Keine Ahnung
                            if (readIntBE(1) != 0)
                            {
                                m_error.addWarning("unbekanntes Byte @ " + m_offset);
                            }

                            n = readIntBE(2);

                            if (n > 0)
                            {
                                //- hier gibt es 2 Punkt pro Linie
                                int m = n * 2;

                                float[,] points = new float[m, 2];

                                //- Punkte lesen: es werden 2*n Punkte eingelesen da jede Linie einen Start und ein Ende-Punkt hat
                                for (int i = 0; i < m; i++)
                                {
                                    points[i, 0] = scaleFactor * readIntBE(2);//- start/ende-x
                                    points[i, 1] = scaleFactor * readIntBE(2);//- start/ende-y
                                }

                                sd.addHatch(points, m);
                            }

                            break;

                        default:
                            m_error.addWarning("Data-Stream-Error??? Unknow Data Type: " + OType + " @ " + m_offset);
                            break;
        
                    }
                }
                
            }

            return sd;
        }



        //------------------------------------------//
        private bool readHead()
        {
            //- los gehts bei position 0
            setOffset(0);
  
            //- Datei Magic lesen und prüfen
            m_FileHead.magic = readStr(40);

            if (m_FileHead.magic.CompareTo("EOS 1993 SLI FILE                       ") != 0) //- Achtung Leerzeichen!
            {
                m_error.addError("file header magic is wrong? soll: >EOS 1993 SLI FILE                       < ist >"+ m_FileHead.magic +"<");
                return false;
            }
  

          m_FileHead.int01 = readIntBE(2);;

          m_FileHead.int02 = readIntBE(2);

          m_FileHead.FileDataOffset = readIntBE(4);

          m_FileHead.int05 = readIntBE(4);

          m_FileHead.int07 = readIntBE(4);

          m_FileHead.FileSliceDataOffset = readIntBE(4);

          m_FileHead.FileIndexPos = readIntBE(4);

          m_FileHead.String2 = readStr(40);

          m_FileHead.LayerCount1 = readIntBE(4);

          m_FileHead.LayerCount2 = readIntBE(4);

          m_FileHead.int14 = readIntBE(4);


          //- 8 DWords überspringen
          setOffset(m_offset + 4 * 8);
          
          m_FileHead.scaleFactor = readFloatBE(4);


          /*
              Debug.Print "------------------------------"
              Debug.Print "Header:"
              Debug.Print "magic: " & FileHead.magic
              Debug.Print "int 1: " & FileHead.int01
              Debug.Print "int 2: " & FileHead.int02
              Debug.Print "DataOffset: " & FileHead.FileDataOffset
              Debug.Print "int 5: " & FileHead.int05
              Debug.Print "int 7: " & FileHead.int07
              Debug.Print "FileSliceDataOffset: " & FileHead.FileSliceDataOffset
              Debug.Print "FileIndexPos: " & FileHead.FileIndexPos
              Debug.Print "string2: " & FileHead.String2
              Debug.Print "LayerCount1: " & FileHead.LayerCount1
              Debug.Print "LayerCount2: " & FileHead.LayerCount2
              Debug.Print "int14: " & FileHead.int14
              Debug.Print "------------------------------"
          */


          return readIndex(m_FileHead.FileIndexPos, m_FileHead.FileDataOffset, m_FileHead.LayerCount1);
        }



        //-------------------------------------//
        private bool readIndex(int FilePos, int FileOffset , int LayerCount)
        {
            setOffset( FilePos + FileOffset );
  
            m_IndexTable_count = 0;

            if (m_IndexTable == null) m_IndexTable = new tyIndexTable[LayerCount];
            if (m_IndexTable.Length < LayerCount) System.Array.Resize(ref m_IndexTable, LayerCount);
  

            while ((!m_eof) && (m_IndexTable_count < LayerCount))
            {
                //- Start of a layer with upper surface at height z (z*units [mm]). All layers must be sorted in ascending order with respect to z. The thickness of the layer is given by the difference between the z values of the current and previous layers. A thickness for the first (lowest) layer can be specified by including a "zero-layer" with a given z value but with no polyline. 
                m_IndexTable[m_IndexTable_count].layerPos = readIntBE(2) * m_FileHead.scaleFactor;

                m_IndexTable[m_IndexTable_count].FileOffset = readIntBE(4) + FileOffset;


                m_IndexTable_count++;
            }

            //Debug.Print "Layer Count " & IndexTable_count
            return true;
        }



        //-------------------------------------//
		private String readStr(int lenght, int offset=-1)
		{
			String outVal="";
		
			byte[] tmpData = this.readByte(lenght, offset);
			
			if (tmpData!=null)
			{
				outVal = System.Text.Encoding.Default.GetString(tmpData);
			}

            return outVal;
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
        private float readFloatBE(int lenght = 4, int offset = -1)
        {
            converterIntFloat.toInt = readIntBE(lenght, offset);
            return converterIntFloat.toFloat;
        }

        //-------------------------------------//
        private byte[] readByte(int lenght, int offset=-1)
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


		
    }
}
