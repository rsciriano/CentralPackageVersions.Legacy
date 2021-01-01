using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CentralPackageVersionsSample.Application
{
    public class GetPackagePublishInfoQueryHandler : IRequestHandler<GetPackagePublishInfoQuery, PackagePublishInfo>
    {
        public async Task<PackagePublishInfo> Handle(GetPackagePublishInfoQuery request, CancellationToken cancellationToken)
        {
            var client = new HttpClient();

            var response = await client.GetAsync($"https://api.nuget.org/v3/registration3/{request.PackageId}/index.json");

            response.EnsureSuccessStatusCode();

            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync());

            var item = data["items"][0]["items"][0]["catalogEntry"];

            return new PackagePublishInfo
            {
                Id = (string)item["id"],
                Version = (string)item["version"],
                Published = (DateTime)item["published"]
            };
        }
    }
}
