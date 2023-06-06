using ParserDbf;
using System.Data;

public class Programs
{
    public static void Main(string[] args)
    {
        string datatable = ConvertTo.Json("C:\\Users\\p.gayevsky\\Documents\\SomeProjects\\DbfSqlVerifiyer\\DbfSqlVerifiyer\\JUL13_21.dbf");
        
    }

    public static void ProcessDataTable(DataTable dataTable)
    {
        foreach (DataRow record in dataTable.Rows)
        {
            if (record["DELETED"].ToString() == "")
            {
                Console.WriteLine($"exec carton_clipperbandaid @cartonNum = '{record["REF"]}', @trackmom = '{record["TRACK_MOM"]}', @trackno = '{record["TRACK_NO"]}'," +
                    $" @actualWeight = {record["ACT_WGT"]}, @shippingMethod = '{record["CARRIER"]}', @freightCost = {record["TOT_CHG"]}");
            }
        }
    }


}
