
#region Using directives
using System;
using UAManagedCore;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.Core;
using FTOptix.Store;
using FTOptix.EventLogger;
#endregion

public class insert_Query : BaseNetLogic
{
    [ExportMethod]
    public void InsertRecord()
    {
        try
        {
            // 1. Read the user-entered values.
            var nameTag = Project.Current.GetVariable("Model/NameTag");
            var ageTag = Project.Current.GetVariable("Model/Age");

            if (nameTag == null || ageTag == null)
            {
                Log.Error("insert_Query",
                    "NameTag or Age variable was not found.");
                return;
            }

            string name = nameTag.Value.Value.ToString();
            string age = ageTag.Value.Value.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Error("insert_Query",
                    "Please enter a name.");
                return;
            }

            // 2. Get the existing MySQL DataStore.
            var store = Project.Current.Get<Store>(
                "DataStores/Mysql");

            if (store == null)
            {
                Log.Error("insert_Query",
                    "Mysql DataStore was not found.");
                return;
            }

            // 3. Get the existing database table.
            var table = store.Tables.Get<Table>("emp_details");

            if (table == null)
            {
                Log.Error("insert_Query",
                    "Table emp_details was not found in the DataStore.");
                return;
            }

            // 4. Prepare one row with two string values.
            string[] columns = { "name", "age" };

            object[,] values = new object[1, 2];
            values[0, 0] = name;
            values[0, 1] = age;

            // 5. Insert the row into MySQL.
            table.Insert(columns, values);

            Log.Info("insert_Query",
                "Employee record inserted successfully.");
        }
        catch (Exception ex)
        {
            Log.Error("insert_Query",
                "Insert failed: " + ex.Message);
        }
    }
}
