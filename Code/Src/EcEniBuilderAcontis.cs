
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AA.Modules.EcMasterAcontis;


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

    public string CreateEni(List<SlaveDeviceInfo> slaves, string pathBuilderConfigXml)
    {
        string xml = CreateEniBuilderXml(slaves);

        string eni;


        var psi = new ProcessStartInfo
        {
            FileName = _pathBuilderExe,
            Arguments = pathBuilderConfigXml,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var p = Process.Start(psi))
            p!.WaitForExit();

        var pathEni = _pathBuilderExe?.Replace("EniBuilder.exe", "").Trim() + "EniBuilderConfig_eni.xml";
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

    public static List<SlaveDeviceInfo>  ConvertBusSlafeInfoToSlavesList(List<EcBusSlaveInfo> busSlaveList, List<string> slaveNames)
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
                
                Name = name,
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

    private static string CreateEniBuilderXml(List<SlaveDeviceInfo> slaves)
    {
        const string eniTemplate =
    @"<?xml version=""1.0"" encoding=""utf-8""?>
    <Config>
        <Info>
        <EniFileName>EniBuilderConfig_eni.xml</EniFileName>
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

        return string.Format(CultureInfo.InvariantCulture, eniTemplate, sb.ToString());
    }

}
