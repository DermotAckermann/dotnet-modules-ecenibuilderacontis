
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AA.Modules.EcEniBuilderAcontisModule;

public static class EcEniBuilderAcontis
{

    public static string CreateEniBuilderXml(List<SlaveDeviceInfo> slaves)
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

    public static Port MapPort(int portIndex) =>
    portIndex switch
    {
        0 => Port.A,
        1 => Port.B,
        2 => Port.C,
        3 => Port.D,
        _ => Port.None // includes 4 and beyond
    };
}
