﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Annotations
{
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }
    }
}
