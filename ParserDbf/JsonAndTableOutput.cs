using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserDbf
{
    public class JsonAndTableOtput
    {
        // Build the JSON string base on list of descriptors
        public string ReturnJson(ArrayList descipList, byte[] allBytes)
        {
            //Size of the table header in bytes
            int value = BitConverter.ToInt16(allBytes, 8);

            StringBuilder jsonString = new StringBuilder();

            // Initialize a variable to define the record number for each JSON rows. It strats from 0
            int recordNumber = 0;

            // Build the JSON string
            while (value < allBytes.Length - 1)
            {
                // Append a left curly brace to the jsonString
                jsonString.Append("{");

                // skip records with the value "*" in a certain column
                bool skip = false;

                //Iterate through the column descriptions in the ArrayList
                for (int i = 0; i < descipList.Count; i++)
                {
                    // ColumnClass object is created from the current element in the ArrayList
                    Columns ar = (Columns)descipList[i];

                    // A byte array is created and filled with the data for the current column
                    byte[] elements = new byte[ar.ColumnSize];

                    // Iterates through the columns in a data record
                    for (int j = 0; j < ar.ColumnSize; j++)
                    {
                        //The value variable is incremented by the size of the current column
                        elements[j] = allBytes[value + j];
                    }

                    // Keep track of the current position in the byte array 'allBytes' and move to the next column to be processed in the loop.
                    value = value + ar.ColumnSize;

                    // Converts the byte array elements to a string representation
                    var value1 = Encoding.ASCII.GetString(elements).Trim();

                    // Checks if the current column has no specified type and if its value is equal to "*"
                    //If both conditions are true, the "skip" flag is set to true, indicating that this record should be skipped
                    if (ar.ColumnType == "")
                    {
                        if (value1 == "*")
                        {
                            skip = true;
                        }
                    }

                    // Is a special column that is used to indicate a deleted record, it is skipped
                    if (ar.ColumnName != "DELETED")
                    {
                        jsonString.Append(ar.ColumnName + ":'" + value1 + "', ");
                    }
                }

                // If the flag is false then the code adds the column name and its corresponding value to the JSON string 
                if (skip == false)
                {
                    jsonString.Append("_recordNumber:'" + recordNumber + "'}");
                    recordNumber++;
                }
            }

            return jsonString.ToString();
        }

        // Build the Table base on list of descriptors
        public DataTable ReturnTable(ArrayList descipList, byte[] allBytes)
        {
            //Size of the table header in bytes
            int value = BitConverter.ToInt16(allBytes, 8);

            // Create a new DataTable object with the same columns as the input file
            DataTable table = new DataTable();
            foreach (Columns descriptor in descipList)
            {
                table.Columns.Add(descriptor.ColumnName, GetColumnType(descriptor.ColumnType));
            }

            // Iterate over the data records in the input file
            while (value < allBytes.Length - 1)
            {
                // Create a new DataRow object to hold the current data record
                DataRow row = table.NewRow();

                // skip records with the value "*" in a certain column
                bool skip = false;

                // Iterate over the columns in the current data record
                foreach (Columns descriptor in descipList)
                {
                    // A byte array is created and filled with the data for the current column
                    byte[] elements = new byte[descriptor.ColumnSize];
                    for (int j = 0; j < descriptor.ColumnSize; j++)
                    {
                        elements[j] = allBytes[value + j];
                    }

                    // Keep track of the current position in the byte array 'allBytes' and move to the next column to be processed in the loop.
                    value += descriptor.ColumnSize;

                    // Converts the byte array elements to a string representation
                    var value1 = Encoding.ASCII.GetString(elements).Trim();

                    // Checks if the current column has no specified type and if its value is equal to "*"
                    //If both conditions are true, the "skip" flag is set to true, indicating that this record should be skipped
                    if (descriptor.ColumnType == "")
                    {
                        if (value1 == "*")
                        {
                            skip = true;
                        }
                    }

                    // If a special column that is used to indicate a deleted record, it is skipped
                    if (descriptor.ColumnName != "DELETED")
                    {
                        row[descriptor.ColumnName] = ConvertToColumnType(value1, descriptor.ColumnType);
                    }
                }

                // If the flag is false then the code adds the row to the DataTable
                if (skip == false)
                {
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        // Helper method to convert a string type to its corresponding .NET data type
        private static Type GetColumnType(string type)
        {
            switch (type)
            {
                case "C":
                    return typeof(string);
                case "D":
                    return typeof(DateTime);
                case "L":
                    return typeof(bool);
                case "N":
                    return typeof(decimal);
                default:
                    return typeof(object);
            }
        }

        // Helper method to convert a string value to its corresponding .NET data type
        private static object ConvertToColumnType(string value, string type)
        {
            switch (type)
            {
                case "C":
                    return value;
                case "D":
                    return DateTime.ParseExact(value, "yyyyMMdd", null);
                case "L":
                    return (value == "T");
                case "N":
                    return decimal.Parse(value);
                default:
                    return value;
            }
        }
    }
}
