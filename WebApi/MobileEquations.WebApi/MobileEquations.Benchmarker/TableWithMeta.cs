using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MobileEquations.Benchmarker
{
    public class TableWithMeta
    {
        public int InputSize { get; set; }

        public string Header { get; set; }

        public DataTable Table { get; set; }

        public int ColumnCount => Table.Columns.Count;
    }
}
