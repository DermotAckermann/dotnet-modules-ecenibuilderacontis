
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using AA.Modules.EcMasterAcontis;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace AA.Modules.EcEniBuilderAcontisModule;

public class EcEniBuilderAcontis
{
    //*** Class data

    #region Fields & Properties
    string? _pathBuilderConfigXml;
    string? _pathBuilderExe;

    #endregion

    //*** Constructors

    public EcEniBuilderAcontis(string pathBuilderExe)
    {
        _pathBuilderExe = pathBuilderExe;
    }

    //*** Methods public

    public string CreateEni(List<SlaveDeviceInfo> slaves, string pathBuilderConfigXml, string eniFileName)
    {
        if (string.IsNullOrWhiteSpace(pathBuilderConfigXml))
            throw new Exception("Xml Path cannot be null");

        string xml = CreateEniBuilderXml(slaves, eniFileName);
        string eni;

        
        File.WriteAllText(pathBuilderConfigXml, xml, Encoding.UTF8);

        //Create Eni
        var psi = new ProcessStartInfo
        {
            FileName = _pathBuilderExe,
            Arguments = $"\"{pathBuilderConfigXml}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        string output, error;

        using (var process = Process.Start(psi))
        {
            output = process!.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            process.WaitForExit();
        }

        //Console.WriteLine("Standard Output:");
        //Console.WriteLine(output);

           if(!string.IsNullOrWhiteSpace(error))
        {
            //Console.WriteLine("Standard Error:");
            //Console.WriteLine(error);
            throw new Exception(error);
        }

        //Load File
        var pathEni = Path.GetDirectoryName(pathBuilderConfigXml) + "\\" + eniFileName;
        eni = File.ReadAllText(pathEni);

        return eni;
    }

    public static Port MapPort(int portIndex) =>
    portIndex switch
    {
        0 => Port.A,
        1 => Port.B,
        2 => Port.C,
        3 => Port.D,
        _ => Port.None // includes 4 and beyond
    };

    public static List<SlaveDeviceInfo>  ConvertBusSlaveInfoToSlavesList(List<EcBusSlaveInfo> busSlaveList, List<string?> slaveNames)
    {
        var result = new List<SlaveDeviceInfo>();

        for (int i = 0; i < busSlaveList.Count; i++)
        {
            var busInfo = busSlaveList[i];
            var name = slaveNames[i];

            ushort prevPhysAddr = 0;             
            uint prevSlaveId = busInfo.PortSlaveIds![0]; // Get the connected to slave ID

            // Find the slave in the list with matching SlaveId
            var prevSlave = busSlaveList.FirstOrDefault(s => s.SlaveId == prevSlaveId);
            if (prevSlave != null)
            {
                prevPhysAddr = prevSlave.StationAddress;
            }

            var slaveDevice = new SlaveDeviceInfo
            {
                Name = name!,
                PhysAddr = busInfo.StationAddress,
                VendorId = busInfo.VendorId,
                ProductCode = busInfo.ProductCode,
                RevisionNo = busInfo.RevisionNumber,
                PrevPort = MapPort(busInfo.PrevPort),
                PrevPhysAddr = prevPhysAddr
            };

            result.Add(slaveDevice);
        }

        return result;
    }

    //*** Methods private

    private static string CreateEniBuilderXml(List<SlaveDeviceInfo> slaves, string eniFileName)
    {
        const string eniTemplate =
    @"<?xml version=""1.0"" encoding=""utf-8""?>
    <Config>
        <Info>
        <EniFileName>{1}</EniFileName>
        <FileFormatVersion>1.1</FileFormatVersion>
        </Info>
        <Master Name=""Class-A Master"">
        <CycleTime>1000</CycleTime>
        <Dc Mode=""BusShift"" SyncWindowMonitoring=""0"" SystemTime64Bit=""0"" />
        </Master>
        <Slaves>
    {0}
        </Slaves>
    </Config>";

        var sb = new StringBuilder();

        foreach (var slave in slaves)
        {
            sb.AppendLine($"    <Slave Name=\"{System.Security.SecurityElement.Escape(slave.Name)}\" PhysAddr=\"{slave.PhysAddr}\">");

            // Description block
            sb.AppendLine("      <Description>");
            sb.AppendLine($"        <VendorId>{slave.VendorId}</VendorId>");
            sb.AppendLine($"        <ProductCode>#x{slave.ProductCode:X}</ProductCode>");
            sb.AppendLine($"        <RevisionNo>#x{slave.RevisionNo:X}</RevisionNo>");
            sb.AppendLine("      </Description>");

            // Optional PreviousPort block
            if (slave.PrevPhysAddr.HasValue && slave.PrevPort != Port.None)
            {
                string portLetter = slave.PrevPort.ToString(); // Enum value directly matches A/B/C/D
                sb.AppendLine("      <PreviousPort>");
                sb.AppendLine($"        <PhysAddr>{slave.PrevPhysAddr.Value}</PhysAddr>");
                sb.AppendLine($"        <Port>{portLetter}</Port>");
                sb.AppendLine("      </PreviousPort>");
            }

            sb.AppendLine("    </Slave>");
        }

        string escapedFileName = System.Security.SecurityElement.Escape(eniFileName);

        return string.Format(CultureInfo.InvariantCulture, eniTemplate, sb.ToString(), escapedFileName);
    }

}


