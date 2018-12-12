using Jtext103.CFET2.CFET2App.ExampleThings;
using Jtext103.CFET2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.CFET2App
{
    partial  class Cfet2Program : CFET2Host
    {
        private void AddThings()
        {
            //This is an example of NIDAQ Things
            //var niMaster = new AIThing();
            
            //niMaster.basicAI = new NIAI();
            //niMaster.dataFileFactory = new HDF5DataFileFactory();

            //MyHub.TryAddThing(niMaster,
            //                    @"/",
            //                    "nimaster",
            //                    new { ConfigFilePath = @"C:\Users\jtext103\Desktop\HDF5NIDAQ\niMaster.txt", DataFileParentDirectory = @"D:\Data\ni\niMaster" });

            //var aiManagement = new AIManagementThing();
            //MyHub.TryAddThing(aiManagement,
            //                    @"/",
            //                    "aimanagement",
            //                    new
            //                    {
            //                        AllAIThingPaths = new string[] { "/nimaster" },
            //                        AutoArmAIThingPaths = new string[] { }
            //                    });
        }
    }
}
