using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

[Extension(typeof(DataTable))]
public class DataTableExtensions : MarshalByValueComponent
{
    public DataRow this[int index] => Unsafe.As<DataTable>(this).Rows[index];
    public static implicit operator DataView(DataTableExtensions table) => Unsafe.As<DataTable>(table).DefaultView;
}

