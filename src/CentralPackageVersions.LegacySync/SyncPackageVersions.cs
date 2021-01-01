using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using System;

namespace CentralPackageVersions.SyncLegacy
{
    public class SyncPackageVersions : Microsoft.Build.Utilities.Task
    {
        [Required]
        public ITaskItem SignedAssemblyFolder { get; set; }

        public override bool Execute()
        {
            throw new NotImplementedException();
        }
    }
}
