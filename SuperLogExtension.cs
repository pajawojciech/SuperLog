public static class SuperLogExtension
{
#if (true) // core?
    public static void tr(object t, int i = 0, string s = "") /*v4.3 */
    {
        System.Action<string> ts = (string st) =>
        {
            System.Diagnostics.Debug.WriteLine(st);
        };
        var sb = new System.Text.StringBuilder();
        var sb2 = new System.Text.StringBuilder();
        if (t.GetType().ToString() == "Dapper.DynamicParameters")
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
                else if (type == typeof(decimal) || type == typeof(double)) sb.AppendFormat("DECLARE @{0} DECIMAL(15,2) = {1}\n", name, pValue);
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
                if (p.SqlDbType.ToString() == "VarChar")
                {
                    sql += " varchar(50) = '" + p.Value as string + "'\n";
                }
                else if (p.SqlDbType.ToString() == "Int")
                {
                    sql += " int = " + p.Value as string + "\n";
                }
                else if (p.SqlDbType.ToString() == "DateTime")
                {
                    sql += " datetime = '" + ((DateTime)p.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'\n";
                }
                else
                {
                    sql += " varchar(50) = '" + p.Value as string + "'\n";
                }
            }
            sql += sqlc.CommandText;
            ts(sql + "\n--END");
        }
        else
        {
            ts(t.GetType().ToString() + " > " + t.ToString());
        }
    }
#else
    public static void tr(object t, int i = 0, string s = "") /*v15.1*/
    {
        System.Action<string> ts = (string st) =>
        {
            System.Diagnostics.Trace.WriteLine(st);
        };
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
            else if (t is System.Web.SessionState.HttpSessionState)
            {
                ts("\nSESSION");
                foreach (string t2 in t as System.Web.SessionState.HttpSessionState)
                {
                    ts(t2);
                    tr(((System.Web.SessionState.HttpSessionState)t)[t2], i + 1);
                }
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
                    if (p.SqlDbType.ToString() == "VarChar")
                    {
                        sql += " varchar(50) = '" + p.Value as string + "'\n";
                    }
                    else if (p.SqlDbType.ToString() == "Int")
                    {
                        sql += " int = " + p.Value as string + "\n";
                    }
                    else
                    {
                        sql += " varchar(50) = '" + p.Value as string + "'\n";
                    }
                }
                sql += sqlc.CommandText;
                ts(sql + "\n--END");
            }
            else if (t is System.Web.HttpRequest)
            {
                ts("-----REQUEST");
                foreach (string key in (t as System.Web.HttpRequest).Form.Keys)
                {
                    ts(key + "\t" + (t as System.Web.HttpRequest).Form[key]);
                }
                ts("-----REQUEST END");
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
#endif
    public static object log(this object t, string s = "")
    {
        tr(t, 0, s);
        return t;
    }
}