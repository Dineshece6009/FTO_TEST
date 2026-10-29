#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.NetLogic;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.System;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.MicroController;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
#endregion

public class ArrayPlot : BaseNetLogic
{
    private readonly string[] inX  = { "Model/Xarr", "Model/XarrG1", "Model/XarrG2" };
    private readonly string[] inY  = { "Model/Yarr", "Model/YarrG1", "Model/YarrG2" };
    private readonly string[] outX = { "Model/X", "Model/X1", "Model/X2" };
    private readonly string[] outY = { "Model/Y", "Model/Y1", "Model/Y2" };

    private const int ClearDelayMs = 500;   // how long the pens stay disabled (old plot clears)
    private const int PointDelayMs = 300;   // pause between points

    [ExportMethod]
    public void MoveAll()
    {
        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                var enableTag = Project.Current.GetVariable("Model/enabled");
                if (enableTag == null) { Log.Error("MoveAll", "Model/enabled not found"); return; }

                var ax = new int[3][];
                var ay = new int[3][];
                var vx = new IUAVariable[3];
                var vy = new IUAVariable[3];
                int n = int.MaxValue;

                for (int g = 0; g < 3; g++)
                {
                    ax[g] = ReadArray(inX[g]);
                    ay[g] = ReadArray(inY[g]);
                    vx[g] = Project.Current.GetVariable(outX[g]);
                    vy[g] = Project.Current.GetVariable(outY[g]);

                    if (vx[g] == null || vy[g] == null)
                    {
                        Log.Error("MoveAll", outX[g] + " or " + outY[g] + " not found");
                        return;
                    }
                    n = Math.Min(n, Math.Min(ax[g].Length, ay[g].Length));
                }
                if (n == 0 || n == int.MaxValue) { Log.Error("MoveAll", "An input array is empty"); return; }

                // 1. clear: disable the pens and wait so the chart really sees the change
                enableTag.Value = false;
                System.Threading.Thread.Sleep(ClearDelayMs);

                // 2. put every pen on its first point, then enable again
                for (int g = 0; g < 3; g++)
                {
                    vx[g].Value = ax[g][0];
                    vy[g].Value = ay[g][0];
                }
                enableTag.Value = true;

                // 3. send the points one by one
                for (int i = 0; i < n; i++)
                {
                    for (int g = 0; g < 3; g++)
                    {
                        vx[g].Value = ax[g][i];
                        vy[g].Value = ay[g][i];
                    }
                    System.Threading.Thread.Sleep(PointDelayMs);
                }
                Log.Info("MoveAll", "Finished, moved " + n + " points");
            }
            catch (Exception ex)
            {
                Log.Error("MoveAll", ex.ToString());
            }
        });
    }

    private int[] ReadArray(string path)
    {
        var v = Project.Current.GetVariable(path);
        if (v == null) { Log.Error("ReadArray", "Variable not found: " + path); return new int[0]; }

        var arr = v.Value.Value as Array;
        if (arr == null) { Log.Error("ReadArray", path + " is not an array"); return new int[0]; }

        var list = new System.Collections.Generic.List<int>();
        foreach (var o in arr) list.Add(Convert.ToInt32(o));
        return list.ToArray();
    }
}