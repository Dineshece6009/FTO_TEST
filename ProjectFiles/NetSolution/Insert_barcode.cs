#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.NetLogic;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.System;
using FTOptix.RecipeX;
using FTOptix.DataLogger;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.MicroController;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.EventLogger;
#endregion

public class Insert_barcode : BaseNetLogic
{
    private const string BasePath = "CommDrivers/RAMicroControllerDriver1/RAMicroControllerStation1/Tags/";

    [ExportMethod]
    public void LogStation1()
    {
        Log.Info("LogStation1", "Method started");
        try
        {
            var store = Project.Current.Get<Store>("DataStores/Mysql");
            if (store == null) { Log.Error("LogStation1", "Store not found: DataStores/Mysql"); return; }

            var table = store.Tables.Get<Table>("record");
            if (table == null) { Log.Error("LogStation1", "Table 'record' not found under the store"); return; }

            var barcodeVar = Project.Current.GetVariable(BasePath + "Final_barcode");
            if (barcodeVar == null) { Log.Error("LogStation1", "Final_barcode not found"); return; }
            string barcode = barcodeVar.Value.Value.ToString();

            int leakValue   = GetInt(BasePath + "Leak_value");
            int torqueValue = GetInt(BasePath + "Torque_value");
            int angle       = GetInt(BasePath + "Angle");
            int cycleTime   = GetInt(BasePath + "Cycle_time");
            string status   = GetString(BasePath + "Station_1_Result");

            Log.Info("LogStation1", $"leak={leakValue} torque={torqueValue} angle={angle} cycle={cycleTime} status='{status}' barcode='{barcode}'");

            string[] columns = { "Timestamp", "barcode", "S1_leak_value", "S1_torque_value", "S1_Angle", "S1_cycle_time", "S1_Status" };
            var values = new object[1, 7];
            values[0, 0] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            values[0, 1] = barcode;
            values[0, 2] = leakValue;
            values[0, 3] = torqueValue;
            values[0, 4] = angle;
            values[0, 5] = cycleTime;
            values[0, 6] = status;

            table.Insert(columns, values);
            Log.Info("LogStation1", "Row inserted");
        }
        catch (Exception ex)
        {
            Log.Error("LogStation1", ex.ToString());
        }
    }

    [ExportMethod]
    public void LogStation2()
    {
        Log.Info("LogStation2", "Method started");
        try
        {
            var store = Project.Current.Get<Store>("DataStores/Mysql");
            if (store == null) { Log.Error("LogStation2", "Store not found: DataStores/Mysql"); return; }

            var barcodeVar = Project.Current.GetVariable(BasePath + "Final_barcode");
            if (barcodeVar == null) { Log.Error("LogStation2", "Final_barcode not found"); return; }
            string barcode = barcodeVar.Value.Value.ToString();

            int torque    = GetInt(BasePath + "St2_Torque");
            int angle     = GetInt(BasePath + "St2_Angle");
            int cycleTime = GetInt(BasePath + "Cycle_time_2");
            string status = GetString(BasePath + "Station_2_Result");

            Log.Info("LogStation2", $"torque={torque} angle={angle} cycle={cycleTime} status='{status}' barcode='{barcode}'");

            string sql = "UPDATE record SET " +
                         "S2_torque_value = " + torque + ", " +
                         "S2_Angle = " + angle + ", " +
                         "S2_cycle_time = " + cycleTime + ", " +
                         "S2_Status = '" + Esc(status) + "' " +
                         "WHERE barcode = '" + Esc(barcode) + "'";

            store.Query(sql, out string[] header, out object[,] resultSet);
            Log.Info("LogStation2", "Row updated for barcode " + barcode);
        }
        catch (Exception ex)
        {
            Log.Error("LogStation2", ex.ToString());
        }
    }

    private int GetInt(string path)
    {
        var v = Project.Current.GetVariable(path);
        if (v == null) { Log.Error("LogStation", "Variable not found: " + path); return 0; }
        return Convert.ToInt32(v.Value.Value);
    }

    private string GetString(string path)
    {
        var v = Project.Current.GetVariable(path);
        if (v == null) { Log.Error("LogStation", "Variable not found: " + path); return ""; }
        return v.Value.Value?.ToString() ?? "";
    }

    private string Esc(string s)
    {
        return (s ?? "").Replace("'", "''");
    }
}