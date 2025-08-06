using Xunit.Priority;
using AA.Modules.EcTopologyHandlerModule;

namespace AA.Modules.EcTopologyHandlerModule.TestsXUnit;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class EcTopologyHandlerTests
{


    static EcTopologyHandlerTests()
    {
        
    }


    [Fact, Priority(1)]
    public void FirstTest()
    {

    }


}


