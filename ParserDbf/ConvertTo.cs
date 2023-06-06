using System.Collections;
using System.Data;

namespace ParserDbf
{
    public class ConvertTo
    {
        public static DataTable Table(string dbfPath)
        {
            // Read the DBF file into a byte array
            byte[] allBytes = ReadDbfFile(dbfPath);

            // Parse the DBF header to create a list of column descriptors
            ArrayList descipList = ParseDbfHeader(dbfPath, allBytes);

            // Create an instance of the Output class
            JsonAndTableOtput output = new JsonAndTableOtput();

            // Generate a DataTable from the column descriptors and byte array data
            DataTable table = output.ReturnTable(descipList, allBytes);

            // Return the generated DataTable
            return table;
        }

        public static string Json(string dbfPath)
        {
            // Read the DBF file into a byte array
            byte[] allBytes = ReadDbfFile(dbfPath);

            // Parse the DBF header to create a list of column descriptors
            ArrayList descipList = ParseDbfHeader(dbfPath, allBytes);

            // Create an instance of the Output class
            JsonAndTableOtput output = new JsonAndTableOtput();

            // Generate a JSON string from the column descriptors and byte array data
            string json = output.ReturnJson(descipList, allBytes);

            Console.WriteLine(json);

            // Return the generated JSON string
            return json;
        }

        private static byte[] ReadDbfFile(string dbfPath)
        {
            // Create an instance of the ConvertInByte class
            ConvertInBytes convertDbf = new ConvertInBytes();

            // Read the contents of the DBF file into a byte array
            return convertDbf.ReturnFileInBytes(dbfPath);
        }

        private static ArrayList ParseDbfHeader(string dbfPath, byte[] allBytes)
        {
            // Create an instance of the ConvertInByte class
            ConvertInBytes convertDbf = new ConvertInBytes();

            // Extract the header section of the DBF file and parse it to create a list of column descriptors
            byte[] headerBytes = convertDbf.ReturnHeaderInBytes(dbfPath, allBytes);
            return convertDbf.ReturnDiscriptorsForEachHeader(headerBytes);
        }

    }
}