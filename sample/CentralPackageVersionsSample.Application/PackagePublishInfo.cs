using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralPackageVersionsSample.Application
{
    public class PackagePublishInfo
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public DateTime Published { get; set; }
    }
}
