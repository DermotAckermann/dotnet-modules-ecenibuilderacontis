using System.Text.Json;
using Xunit.Priority;
using AA.Modules.EcEniBuilderAcontisModule;
using AA.Modules.EcMasterAcontis;

namespace AA.Modules.EcEniBuilderAcontisModule.TestsXUnit;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class EcEniBuilderAcontisUnitTests
{
    private static EcEniBuilderAcontis builder = new("D:\\Work Jean\\DotNet Modules\\EC ENI Builder Acontis Git\\Code\\ENI Builder\\EniBuilder.exe");


    static EcEniBuilderAcontisUnitTests()
    {
        
    }


    [Fact, Priority(1)]
    public void TestCreateEni_WrongVendorId()
    {
        var busSlaveInfoList = CreateList();
        busSlaveInfoList[0].VendorId = 100;
        var slaveNames = CreateNames();

        string xmlPath = "D:\\Work Jean\\DotNet Modules\\EC ENI Builder Acontis Git\\Code\\ENI Builder\\config.xml";
        var slaveDeviceInfoList = EcEniBuilderAcontis.ConvertBusSlaveInfoToSlavesList(busSlaveInfoList, slaveNames!);

        Assert.Throws<Exception>(() => builder.CreateEni(slaveDeviceInfoList, xmlPath));
    }

    [Fact, Priority(1)]
    public void TestCreateEni_WrongProductCode()
    {
        var busSlaveInfoList = CreateList();
        busSlaveInfoList[0].ProductCode = 999999;
        var slaveNames = CreateNames();

        string xmlPath = "D:\\Work Jean\\DotNet Modules\\EC ENI Builder Acontis Git\\Code\\ENI Builder\\config.xml";
        var slaveDeviceInfoList = EcEniBuilderAcontis.ConvertBusSlaveInfoToSlavesList(busSlaveInfoList, slaveNames!);

        Assert.Throws<Exception>(() => builder.CreateEni(slaveDeviceInfoList, xmlPath));
    }

    [Fact, Priority(1)]
    public void TestCreateEni_WrongRevisionNo()
    {
        var busSlaveInfoList = CreateList();
        busSlaveInfoList[0].RevisionNumber = 12345678; // Intentionally incorrect
        var slaveNames = CreateNames();

        string xmlPath = "D:\\Work Jean\\DotNet Modules\\EC ENI Builder Acontis Git\\Code\\ENI Builder\\config.xml";
        var slaveDeviceInfoList = EcEniBuilderAcontis.ConvertBusSlaveInfoToSlavesList(busSlaveInfoList, slaveNames!);

        Assert.Throws<Exception>(() => builder.CreateEni(slaveDeviceInfoList, xmlPath));
    }

    [Fact, Priority(1)]
    public void TestCreateEni_EmptyXmlPath()
    {
        var busSlaveInfoList = CreateList();
        busSlaveInfoList[0].RevisionNumber = 12345678; // Intentionally incorrect
        var slaveNames = CreateNames();

        string xmlPath = "";
        var slaveDeviceInfoList = EcEniBuilderAcontis.ConvertBusSlaveInfoToSlavesList(busSlaveInfoList, slaveNames!);

        Assert.Throws<Exception>(() => builder.CreateEni(slaveDeviceInfoList, xmlPath));
    }

    [Fact, Priority(2)]
    public void TestCreateEni6Slaves()
    {
        var busSlaveInfoList = CreateList();
        var slaveNames = CreateNames();

        string xmlPath = "D:\\Work Jean\\DotNet Modules\\EC ENI Builder Acontis Git\\Code\\ENI Builder\\config.xml";
        var slaveDeviceInfoList = EcEniBuilderAcontis.ConvertBusSlaveInfoToSlavesList(busSlaveInfoList, slaveNames!);

        var eni = builder.CreateEni(slaveDeviceInfoList, xmlPath);

        Assert.NotNull(eni);
        Assert.Equal(eni, eniFile6Slaves);
    }


    private static string eniFile6Slaves = File.ReadAllText("D:\\Work Jean\\EC-Engineer_SDK_EniEngine_Layer5_DotNet4_OEM_V3.9.9\\StandardSetup_EniBuilder_eni.xml");
    private static List<EcBusSlaveInfo> CreateList()
    {
        var json = File.ReadAllText("D:\\Work Jean\\Chat Gpt\\busSlaveInfoList.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        var slaveList = JsonSerializer.Deserialize<List<EcBusSlaveInfo>>(json, options);

        return slaveList!;
    }
    private static List<string> CreateNames()
    {
        return new List<string>
        {
            "EK1100",
            "EL2502",
            "EL5151",
            "EL2004",
            "EL1004",
            "EL3312"
        };
    }

}


