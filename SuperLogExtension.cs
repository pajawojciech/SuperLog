public static class SuperLogExtension /*v18.2*/
{
    private static System.Action<string> ts = (string st) =>
    {
        System.Diagnostics.Trace.WriteLine(st);
    };

    private static void tr(object t, int i = 0, string s = "", object pp = null)
    {
        var sb = new System.Text.StringBuilder();
        var sb2 = new System.Text.StringBuilder();
        int pPos = 0;
        if (i != 0)
        {
            for (int j = 0; j < i; j++)
            {
                System.Diagnostics.Trace.Write("\t");
            }
        }
        if (!string.IsNullOrEmpty(s))
        {
            System.Diagnostics.Trace.TraceWarning(s + ":");
        }
        if (t != null)
        {
            if (t.GetType() == typeof(string))
            {
                ts(">" + (string)t + "<");
            }
            else if (t.GetType().IsArray)
            {
                ts(t.GetType().ToString() + " " + ((System.Array)t).Length + "--- array");
                foreach (object t2 in t as System.Array)
                {
                    tr(t2, i + 1);
                }
                ts("--- end ---");
            }
            else if (t is System.Collections.ICollection)
            {
                ts(t.GetType().ToString() + " " + ((System.Collections.ICollection)t).Count + "--- collection");
                foreach (object t2 in t as System.Collections.ICollection)
                {
                    tr(t2, i + 1);
                }
                ts("--- end ---");
            }
            else if (t is System.Exception)
            {
                ts(t.GetType().ToString() + " > " + ((System.Exception)t).Message);
            }
            else if (t is System.Data.SqlClient.SqlDataAdapter)
            {
                tr(((System.Data.SqlClient.SqlDataAdapter)t).SelectCommand);
            }
            else if (t is System.Data.SqlClient.SqlCommand)
            {
                System.Data.SqlClient.SqlCommand sqlc = (System.Data.SqlClient.SqlCommand)t;
                string sql = "--SQL\n";
                foreach (System.Data.SqlClient.SqlParameter p in sqlc.Parameters)
                {
                    sql += "Declare " + p.ParameterName;
                    if (p.SqlDbType.ToString() == "VarChar" || p.SqlDbType.ToString() == "NVarChar")
                    {
                        sql += " " + p.SqlDbType.ToString() + "(" + p.Size + ") = '" + (p.Value as string).Replace("'", "''") + "' \n";
                    }
                    else if (p.SqlDbType.ToString() == "Decimal")
                    {
                        sql += " " + p.SqlDbType.ToString() + "(" + p.Precision + "," + p.Scale + ") = '" + p.Value.ToString().Replace(",", ".") + "' \n";
                    }
                    else if (new string[] { "Date", "DateTime", "DateTime2" }.Contains(p.SqlDbType.ToString()))
                    {
                        sql += " " + p.SqlDbType.ToString() + " = '";
                        if (p.Value is DateTime)
                        {
                            if (p.SqlDbType.ToString() == "Date")
                            {
                                sql += ((DateTime)p.Value).ToString("yyyy-MM-dd");
                            }
                            else if (p.SqlDbType.ToString() == "DateTime")
                            {
                                sql += ((DateTime)p.Value).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }
                            else if (p.SqlDbType.ToString() == "DateTime2")
                            {
                                sql += ((DateTime)p.Value).ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                            }
                        }
                        else
                        {
                            sql += p.Value.ToString();
                        }
                        sql += "' \n";
                    }
                    else if (p.SqlDbType.ToString() == "Structured")
                    {
                        sql += " " + p.TypeName + "\n";
                        if (p.Value is System.Data.DataTable)
                        {
                            System.Data.DataTable dt = (System.Data.DataTable)p.Value;
                            sql += "INSERT INTO " + p.ParameterName + " (";
                            bool first = true;
                            foreach (System.Data.DataColumn x in dt.Columns)
                            {
                                if (!first) sql += ",";
                                sql += x.ColumnName;
                                first = false;
                            }
                            sql += ") VALUES \n";
                            first = true;
                            foreach (System.Data.DataRow dr in dt.Rows)
                            {
                                sql += first ? "" : ",";
                                first = false;
                                sql += "(";
                                for (int l = 0; l < dt.Columns.Count; l++)
                                {
                                    sql += (l != 0) ? ", " : "";
                                    if (dt.Columns[l].DataType.Name == "Decimal")
                                    {
                                        sql += dr[l].ToString().Replace(",", ".");
                                    }
                                    else if (dt.Columns[l].DataType.Name == "DateTime")
                                    {
                                        sql += "'" + ((DateTime)dr[l]).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                                    }
                                    else
                                    {
                                        sql += "'" + dr[l].ToString() + "'";
                                    }
                                }
                                sql += ")\n";
                            }
                        }
                    }
                    else
                    {
                        sql += " " + p.SqlDbType.ToString() + " = '" + p.Value.ToString() + "' \n";
                    }
                }
                sql += sqlc.CommandText;
                ts(sql + "\n--END");
            }
            else if (t is System.Collections.DictionaryEntry)
            {
                ts(t.GetType().ToString() + " > ");
                tr(((System.Collections.DictionaryEntry)t).Key, i + 1);
                tr(((System.Collections.DictionaryEntry)t).Value, i + 1);
            }
            else if (t is System.Data.DataTable)
            {
                System.Data.DataTable dt = (System.Data.DataTable)t;
                ts("TABLE: " + dt.TableName);
                string cols = "";
                foreach (System.Data.DataColumn x in dt.Columns)
                {
                    cols += x.ColumnName + " " + x.DataType.ToString() + "\t";
                }
                ts(cols);
                string rows = "";
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    for (int l = 0; l < dt.Columns.Count; l++)
                    {
                        rows += dr[l].ToString() + "\t";
                    }
                    tr(rows);
                    rows = string.Empty;
                }
            }
#if (true)
            else if (t is System.Web.HttpRequest)
            {
                ts("-----REQUEST");
                foreach (string key in (t as System.Web.HttpRequest).Form.Keys)
                {
                    ts(key + "\t" + (t as System.Web.HttpRequest).Form[key]);
                }
                ts("-----REQUEST END");
            }
            else if (t is System.Web.SessionState.HttpSessionState)
            {
                ts("\nSESSION");
                foreach (string t2 in t as System.Web.SessionState.HttpSessionState)
                {
                    ts(t2);
                    tr(((System.Web.SessionState.HttpSessionState)t)[t2], i + 1);
                }
            }
            else if (t is System.Linq.IQueryable)
            {
                if (!t.GetType().FullName.StartsWith("System.Data.Linq.DataQuery`1"))
                {
                    ts("IQueryable is not DataQuery");
                }
                var field = t.GetType().GetField("context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field == null) ts("IQueryable with null DataContext");
                tr((field.GetValue(t) as System.Data.Linq.DataContext).GetCommand((System.Linq.IQueryable)t));
            }
#endif
#if (true)
            else if (t.GetType().ToString() == "Dapper.DynamicParameters")
            {
                sb.Append("--SQL\n");
                foreach (var name in (t as Dapper.DynamicParameters).ParameterNames)
                {
                    sb2.AppendFormat("@{0} = @{0},", name);
                    var pValue = (t as Dapper.DynamicParameters).Get<dynamic>(name);
                    if (pValue == null)
                    {
                        sb.AppendFormat("DECLARE @{0} VARCHAR(MAX) \n", name);
                        continue;
                    }
                    var type = pValue.GetType();
                    if (type == typeof(System.DateTime)) sb.AppendFormat("DECLARE @{0} DATETIME ='{1}'\n", name, pValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    else if (type == typeof(bool)) sb.AppendFormat("DECLARE @{0} BIT = {1}\n", name, (bool)pValue ? 1 : 0);
                    else if (type == typeof(int)) sb.AppendFormat("DECLARE @{0} INT = {1}\n", name, pValue);
                    else if (type == typeof(System.Collections.Generic.List<int>)) sb.AppendFormat("-- REPLACE @{0} IN SQL: ({1})\n", name, string.Join(",", (System.Collections.Generic.List<int>)pValue));
                    else if (type == typeof(decimal) || type == typeof(double)) sb.AppendFormat("DECLARE @{0} DECIMAL({2},{3}) = {1}\n", name, pValue.ToString().Replace(",", "."), pValue.ToString().Length - 1, pValue.ToString().Split(",")[1].Length);
                    else if (type.ToString() == "Dapper.TableValuedParameter")
                    {
                        if (pp is Array)
                        {
                            if ((pp as Array).Length > pPos)
                            {
                                var obj = (pp as Array).GetValue(pPos);
                                if ((pp as Array).Length > pPos + 1 && (pp as Array).GetValue(pPos + 1) is string)
                                {
                                    sb.AppendFormat("\nDECLARE @{0} {1} \n", name, (pp as Array).GetValue(pPos + 1).ToString());
                                    pPos++;
                                }
                                else
                                {
                                    sb.AppendFormat("\nDECLARE @{0} dbo.TYPE \n", name);

                                }
                                sb.Append(tp(obj, name));
                                pPos++;
                            }
                        }
                        else
                        {
                            sb.AppendFormat("\nDECLARE @{0} dbo.TYPE \n", name);
                            sb.Append(tp(pp, name));
                        }
                    }
                    else sb.AppendFormat("DECLARE @{0} NVARCHAR(MAX) = '{1}'\n", name, pValue.ToString());
                }
                ts(sb.ToString());
                ts(string.Format("EXEC {0} --PROCEDURE", s));
                if (sb2.Length > 0)
                {
                    sb2.Remove(sb2.Length - 1, 1);
                    ts(sb2.ToString());
                }
                ts("--END SQL");
            }
#endif
            else
            {
                ts(t.GetType().ToString() + " > " + t.ToString());
            }
        }
        else
        {
            ts("NULL");
        }
    }

    private static string tp(object t, string name)
    {
        if (t == null) return "";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        bool firstRow = true;
        int i = 1;
        dynamic td = t;
        foreach (object y in td)
        {
            if (firstRow)
            {
                sb.AppendFormat("insert into @{0} values", name);
            }
            else
            {
                sb.Append(",");
            }
            sb.Append('(');

            bool firstCol = true;
            foreach (System.Reflection.PropertyInfo prop in y.GetType().GetProperties())
            {
                if (!firstCol)
                {
                    sb.Append(',');
                }
                object x = prop.GetValue(y);
                if (x == null)
                {
                    sb.Append("NULL");
                }
                else if (x is decimal || x is double || x is int)
                {
                    sb.Append($@"{x.ToString().Replace(',', '.')}");
                }
                else if (x is DateTime)
                {
                    sb.Append("'" + ((DateTime)x).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");
                }
                else
                {
                    sb.Append($@"'{x}'");
                }
                firstCol = false;
            }
            sb.Append(')');
            firstRow = false;
            if (++i == 1000)
            {
                firstRow = true;
                i = 1;
                sb.AppendLine();
            }
        }
        sb.AppendLine();
        return sb.ToString();
    }
    public static object log(this object t, string s = "", object p = null)
    {
        tr(t, 0, s, p);
        return t;
    }
}