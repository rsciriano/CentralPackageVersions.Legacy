using MediatR;
using System;

namespace CentralPackageVersionsSample.Application
{
    public class GetPackagePublishInfoQuery: IRequest<PackagePublishInfo>
    {
        public string PackageId { get; set; }
    }
}
