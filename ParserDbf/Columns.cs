using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserDbf
{
    public class Columns
    {
        public Columns(string columnName, string columnType, int columnSize)
        {
            ColumnName = columnName;
            ColumnType = columnType;
            ColumnSize = columnSize;
        }

        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int ColumnSize { get; set; }
    }
}
