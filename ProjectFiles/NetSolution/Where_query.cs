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

public class Where_query : BaseNetLogic
{
    [ExportMethod]
public void RunSelect()
{
try
{
Log.Info("search Query", "Buttom Pressed");
var Grid = Owner.Get<FTOptix.UI.DataGrid>("dataGrid");

Log.Info("Search Query LOgged B");
var nameTag = Project.Current.GetVariable("Model/Find_Name");
Grid.Query = "SELECT name,age FROM emp_details WHERE name='" + nameTag.Value + "'";

Log.Info("Search Query LOgged " + Grid.Query);
}
catch(Exception ex)
{
Log.Error("searcg Query" , ex.ToString());
}
}
}
