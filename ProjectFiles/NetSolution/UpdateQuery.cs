   #region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.MicroController;
using FTOptix.NativeUI;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.DataLogger;
using FTOptix.Store;
using FTOptix.RecipeX;
using FTOptix.ODBCStore;
using FTOptix.EventLogger;
#endregion

public class UpdateQuery : BaseNetLogic
{
    [ExportMethod]
    public void UpdateRecord()
    {
        try
        {
            // 1. Read the part number directly from the PLC tag.
            var partNumberTag = Project.Current.GetVariable(
                "CommDrivers/RAMicroControllerDriver1/RAMicroControllerStation1/Tags/trend Data");

            if (partNumberTag == null)
            {
                Log.Error("UpdateDatabase",
                    "Part number PLC tag was not found.");
                return;
            }

            string partNumber =
                partNumberTag.Value.Value.ToString();

            // 2. Get the existing MySQL DataStore.
            var store = Project.Current.Get<Store>(
                "DataStores/Mysql");

            if (store == null)
            {
                Log.Error("UpdateDatabase",
                    "DataStores/Mysql was not found.");
                return;
            }

            // 3. Escape single quotes in the part number.
            string safePartNumber =
                partNumber.Replace("'", "''");

            // 4. Update only the row with id = 100.
            string query =
                "UPDATE dismandalsing " +
                "SET part_number = '" + safePartNumber + "' " +" ,DISMANDALING_DATAS='UPDATED'"+
                "WHERE S_NO = 100";

            object[,] result;
            string[] header;

            store.Query(query, out header, out result);
            var Data_saved = Project.Current.GetVariable("CommDrivers/RAMicroControllerDriver1/RAMicroControllerStation1/Tags/DO_1");
		if(Data_saved != null)
{
		Data_saved.Value=false;
		}    
            Log.Info("UpdateDatabase",
                "UPDATE query executed for S_NO=100.");
        }
        catch (Exception ex)
        {
            Log.Error("UpdateDatabase",
                "Database update failed: " + ex.Message);
        }
    }
}
