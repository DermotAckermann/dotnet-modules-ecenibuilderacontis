using AA.Modules.EcMasterAcontis;
using System.Text.Json;

namespace AA.Modules.EcEniBuilderAcontisModule.Tests;


public static class EcEniBuilderAcontisTests
{

    private static EcEniBuilderAcontis builder = new("D:\\Work Jean\\DotNet Modules\\EC ENI Builder Acontis Git\\Code\\ENI Builder\\EniBuilder.exe");

    public static void RunAll()
    {
        TestEni();

        Console.WriteLine("✅ All EcTopologyDataHandler tests passed!");
    }


    public static void TestEni()
    {
        var busSlaveList = CreateList();
        busSlaveList[2].PrevPort = 999;
        var slaveNames = CreateNames();

        busSlaveList[2].PortSlaveIds![0] = 99;
        var slaveDeviceInfoList = EcEniBuilderAcontis.ConvertBusSlaveInfoToSlavesList(busSlaveList, slaveNames!);

        string xmlPath = "D:\\Work Jean\\DotNet Modules\\EC ENI Builder Acontis Git\\Code\\ENI Builder\\config.xml";

        var eni = builder.CreateEni(slaveDeviceInfoList, xmlPath);

    }

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


