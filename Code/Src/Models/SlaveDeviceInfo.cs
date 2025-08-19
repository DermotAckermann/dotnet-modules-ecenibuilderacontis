using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AA.Modules.EcEniBuilderAcontisModule;

public class SlaveDeviceInfo
{
    public string Name { get; set; } = string.Empty; // e.g. "SubDevice_1001 [EK1100]"
    public ushort PhysAddr { get; set; }                    // e.g. 1001
    public uint VendorId { get; set; }                      // e.g. 2
    public uint ProductCode { get; set; }                   // decimal but will be formatted #xHEX
    public uint RevisionNo { get; set; }                    // decimal but will be formatted #xHEX

    // Optional previous device info
    public ushort? PrevPhysAddr { get; set; }
    public Port PrevPort { get; set; } = Port.None;

    public ushort[] ExcludePdoRemove { get; set; }

    public ushort[] ExcludePdoAdd { get; set; }
}


