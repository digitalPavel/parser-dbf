using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserDbf
{
    public class ConvertInBytes
    {
        //The size of the DBF file header.
        const int fieldDescriptorSize = 32;
        //Carriage return. After this symbol record of headers is over
        const string carriageReturn = "\r";


        // This method reads the contents of a DBF file into a byte array
        public byte[] ReturnFileInBytes(string dbfPath)
        {
            Byte[] allBytes = File.ReadAllBytes(dbfPath);

            return allBytes;
        }

        // This method extracts the header section of a DBF file and returns it as a byte array
        public byte[] ReturnHeaderInBytes(string dbfPath, byte[] allBytes)
        {
            //Size of the table header in bytes.
            int value = BitConverter.ToInt16(allBytes, 8);

            Byte[] headerBytes = new Byte[value];
            using (FileStream reader = new FileStream(dbfPath, FileMode.Open))
            {
                reader.Seek(fieldDescriptorSize, SeekOrigin.Begin);
                reader.Read(headerBytes, 0, value);
            }

            return headerBytes;
        }

        // Method to return the list of column descriptors from the header bytes of the DBF file
        public ArrayList ReturnDiscriptorsForEachHeader(Byte[] headerBytes)
        {
            // Create a default column descriptor for deleted records
            Columns defaul = new Columns("DELETED", "", 1);

            // Add the default column descriptor to the list of descriptors
            ArrayList arlist = new ArrayList();
            arlist.Add(defaul);

            string encFieldName1;
            string encFieldName;
            string fieldName;
            string fieldType;
            int fieldSize;

            // Iterate over the header bytes, parsing each column descriptor and adding it to the list
            for (int c = 0; c < headerBytes.Length - fieldDescriptorSize; c += fieldDescriptorSize)
            {
                // Get the current field record data
                ArraySegment<byte> fieldRecordData = new ArraySegment<byte>(headerBytes, c, fieldDescriptorSize);

                // Get the field name as a string, removing any null characters
                encFieldName = Encoding.UTF8.GetString(fieldRecordData).Replace("\0", "");

                // If the field name contains only spaces and is equal to the carriage return character, exit the loop
                if (encFieldName.Replace(" ", "") == carriageReturn)
                {
                    break;
                }
                else
                {
                    // Get the field name, field type, and field size from the field record data
                    ArraySegment<byte> fieldRecordData1 = new ArraySegment<byte>(headerBytes, c, fieldDescriptorSize);
                    encFieldName1 = Encoding.UTF8.GetString(fieldRecordData1).Trim();
                    fieldName = encFieldName1.Substring(0, 11).Replace("\0", "");
                    fieldType = encFieldName1.Substring(11, 1);
                    var subSize = encFieldName1.Substring(16, 1);
                    fieldSize = (int)subSize[0];

                    // Create a new column descriptor object and add it to the list of descriptors
                    Columns column = new Columns(fieldName, fieldType, fieldSize);
                    arlist.Add(column);
                }
            }
            return arlist;
        }
    }
}
