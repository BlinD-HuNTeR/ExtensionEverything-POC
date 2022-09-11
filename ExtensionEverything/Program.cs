using System;
using System.Data;

var table = new DataTable();
table.Columns.Add("A"); table.Columns.Add("B");
table.Rows.Add("ABC", "DEF");
table.Rows.Add("GHI", "JKL");

#if EXTENSION_SYSTEM_DATA
Console.WriteLine($"A0: {table[0]["A"]}, B0: {table[0]["B"]}");
Console.WriteLine($"A1: {table[1]["A"]}, B1: {table[1]["B"]}");

DataView view = table;
Console.WriteLine(view.GetType());
Console.Read();

#endif
