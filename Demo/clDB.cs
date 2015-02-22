using System;
using System.Data.Odbc;
using System.Collections;

namespace Demo
{
    public class clDB
    {
        public enum ENUM_DB_TYPE
        {
            DB_MYSQL,
            DB_DB2
        }

        private OdbcConnection m_con;
        private OdbcDataReader m_rec;
        private Hashtable m_fieldNames;
        public bool eof;
        private ENUM_DB_TYPE m_db_type;

        //---------------------------------------------------//
        public clDB(String connectionString, ENUM_DB_TYPE DBType)
        {
            m_con = null;
            m_rec = null;
            eof = true;
            m_db_type = DBType;

            try
            {
                m_con = new OdbcConnection(connectionString);
                m_con.Open();
            }
            catch(Exception e)
            {
                m_con = null;
                Form1.addError(e.Message);
            }
        }

        //---------------------------------------------------//
        private bool execute_query(out OdbcDataReader rec, out bool set_eof, out Hashtable fieldNames, string query, int RecordLimit = -1)
        {
            rec = null;
            set_eof = true;
            fieldNames = new Hashtable(0);

            if (m_con==null) return false;

            OdbcCommand cmd;

            if (RecordLimit >= 0)
            {
                query += SQL_LIMIT_FORMAT(RecordLimit);
            }

            try
            {
                cmd = new OdbcCommand(query, m_con);
            }
            catch (Exception e)
            {
                Form1.addError(e.Message);
                Form1.addError(query);
                return false;
            }

            //-- execute --/
            try
            {
                rec = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Form1.addError(e.Message);
                Form1.addError(query);
                return false;
            }


            try
            {
                set_eof = !rec.Read();
            }
            catch (Exception e)
            {
                Form1.addError(e.Message);
                Form1.addError(query);
                set_eof = true;
                return false;
            }

            //-- read Fieldnames --//
            int l = rec.FieldCount;
            fieldNames = new Hashtable(l);

            for (int i = 0; i < l; i++)
            {
                fieldNames.Add(rec.GetName(i).ToLower(), i);
            }

            return true;
        }


        //---------------------------------------------------//
        public bool execute(string query, int RecordLimit = -1)
        {
            return execute_query(out m_rec, out eof, out m_fieldNames, query, RecordLimit);
        }

        

        //---------------------------------------------------//
        public void setListViewHeader(System.Windows.Forms.ListView listView, string keyField, int[] ColumnWidth)
        {
            if (m_rec == null) return;
            //if (eof) return;

            int count = m_rec.FieldCount;
            int countOut = count;

            if (count < 1) return;

            listView.Columns.Clear();

            //- find field-Index for key
            string key = keyField.ToLower();
            int keyFieldIndex = -1;
            if (m_fieldNames.ContainsKey(key))
            {
                keyFieldIndex = (int)m_fieldNames[key];
                countOut = count - 1;
            }

            //- read row
            string[] items = new string[countOut];
            int j = 0;
            int currentColumnWidth = 200;

            for (int i = 0; i < count; i++)
            {
                if (i != keyFieldIndex)
                {
                    if (j < ColumnWidth.Length) currentColumnWidth = ColumnWidth[j];

                    listView.Columns.Add(m_rec.GetName(i), currentColumnWidth);
                    j++;
                }
            }

        }

        public static string Hex2String(string hex)
        {
            string result = "";
            int count = hex.Length / 2;
            int s;

            for (s = 0; s < count; s++)
            {
                string zeichen = hex.Substring(s * 2, 2);
                result += (char)Convert.ToUInt16(zeichen, 16);
            }

            return result;
        }
        //---------------------------------------------------//
        public System.Windows.Forms.ListViewItem getListViewRow(string keyField)
        {
            if (m_rec == null) return new System.Windows.Forms.ListViewItem("");
            if (eof) return new System.Windows.Forms.ListViewItem("");

            int count = m_rec.FieldCount;
            int countOut = count;

            if (count<1) return new System.Windows.Forms.ListViewItem("");

            //- find field-Index for key
            string key = keyField.ToLower();
            int keyFieldIndex = -1;
            if (m_fieldNames.ContainsKey(key))
            {
                keyFieldIndex = (int)m_fieldNames[key];
                countOut = count - 1;
            }

            //- read row
            string[] items = new string[countOut];
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                if (i != keyFieldIndex)
                {
                    if (m_rec.GetDataTypeName(i).ToUpper() != "BLOB")
                    {
                        try
                        {
                            items[j] = m_rec.GetValue(i).ToString();
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        try
                        {
                            items[j] = Hex2String(m_rec.GetString(i));
                        }
                        catch (Exception) { }   
                    }

                    j++;
                }
            }

            //- create new intem
            System.Windows.Forms.ListViewItem Item = new System.Windows.Forms.ListViewItem(items);

            //- add key-Value to item
            if (keyFieldIndex >= 0) Item.Name = m_rec.GetValue(keyFieldIndex).ToString();

            return Item;
        }


        //---------------------------------------------------//
        public bool MoveNext()
        {
            if (m_rec == null) return false;

            try
            {
                eof = !m_rec.Read();
            }
            catch (Exception e)
            {
                Form1.addError(e.Message);
                eof = true;
                return false;
            }


            return !eof;
        }

                

        //---------------------------------------------------//
        public string FieldName(int index)
        {
            if (m_rec == null) return "";
            if (eof) return "";
            if ((index < 0) && (index >= m_rec.FieldCount)) return "";

            return m_rec.GetName(index);
        }

        //---------------------------------------------------//
        public int FieldCount()
        {
            if (m_rec == null) return 0;
            return m_rec.FieldCount;
        }


        //---------------------------------------------------//
        public string FieldStr(string fieldName, string defaultReturn="")
        {
            if (m_rec == null) return defaultReturn;
            if (eof) return defaultReturn;

            string key = fieldName.ToLower();

            if (!m_fieldNames.ContainsKey(key)) return defaultReturn;

            try
            {
                return m_rec.GetValue((int)m_fieldNames[key]).ToString();
            }
            catch (Exception)
            {
                return defaultReturn;
            }
        }


        //---------------------------------------------------//
        public int FieldInt(string fieldName, int defaultReturn = 0)
        {
            if (m_rec == null) return defaultReturn;
            if (eof) return defaultReturn;

            try
            {
                return Convert.ToInt32(FieldStr(fieldName, defaultReturn.ToString()));
            }
            catch
            {
                return defaultReturn;
            }
        }

        //---------------------------------------------------//
        public double FieldDouble(string fieldName, double defaultReturn = 0)
        {
            if (m_rec == null) return defaultReturn;
            if (eof) return defaultReturn;

            try
            {
                return Convert.ToDouble(FieldStr(fieldName, defaultReturn.ToString()));
            }
            catch
            {
                return defaultReturn;
            }
        }
        //---------------------------------------------------//
        public Record[] execute_to_array(string query, string keyField, string valueField)
        {
            OdbcDataReader my_rec;
            Hashtable my_fieldNames;
            bool my_eof;
            Record [] outRec = new Record[0];

            if (!execute_query(out my_rec, out my_eof, out my_fieldNames, query)) return outRec;

            string key_v = keyField.ToLower();
            string value_v = valueField.ToLower();

            if (!my_fieldNames.ContainsKey(key_v)) return outRec;
            if (!my_fieldNames.ContainsKey(value_v)) return outRec;

            int key_i = (int)my_fieldNames[key_v];
            int value_i = (int)my_fieldNames[value_v];

            int RecCount = 0;
            outRec = new Record[10];

            while (!my_eof)
            {
                try
                {
                    outRec[RecCount] = new Record(my_rec.GetValue(key_i).ToString(), my_rec.GetValue(value_i).ToString());
                    RecCount++;

                    my_eof = !my_rec.Read();
                }
                catch (Exception e)
                {
                    Form1.addError(e.Message);
                    my_eof = true;
                    return outRec;
                }



                if (my_eof) break;

                if (RecCount >= outRec.Length)
                {
                    Array.Resize(ref outRec,  outRec.Length + 20);
                }
            }

            //- remove empty elements at the end!
            Array.Resize(ref outRec, RecCount);

            return outRec;
            
        }

        //---------------------------------------------------//
        public string SQL_LIMIT_FORMAT(int count)
        {
            switch(m_db_type)
            {
                case ENUM_DB_TYPE.DB_DB2:
                    return " FETCH FIRST " + count + " ROWS ONLY ";
                case ENUM_DB_TYPE.DB_MYSQL:
                    return " LIMIT 0," + count + " ";
            }
            return "";
        }
        

        //---------------------------------------------------//
        public string ToEscapeStr(string value2Escape)
        {
            try
            {
                return "'" + System.Text.RegularExpressions.Regex.Replace(value2Escape, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0") + "'";
	        }
	        catch
	        {
                return "''";
	        }
            
        }
        

        //---------------------------------------------------//
        //---------------------------------------------------//
        //---------------------------------------------------//
        public class Record
        {
            public string value;
            public string key;


            public Record(string new_key, string new_value)
            {
                value = new_value;
                key = new_key;
            }

            public override string  ToString()
            {
                return value;
            }
        }
    }
}
