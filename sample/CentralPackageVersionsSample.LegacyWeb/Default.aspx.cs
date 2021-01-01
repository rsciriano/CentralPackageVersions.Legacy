using CentralPackageVersionsSample.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CentralPackageVersionsSample.LegacyWeb
{
    public partial class Default : System.Web.UI.Page
    {
        // This property will be set for you by the PropertyInjectionModule.
        public IMediator Mediator { get; set; }


        public PackagePublishInfo PackagePublishInfo { get; private set; }

        public string ProcessMessage { get; set; }

        protected async void Page_Load(object sender, EventArgs e)
        {
        }

        protected async void btnGetInfo_Click(object sender, EventArgs e)
        {
            /*
            var nugetService = new NugetService();
            PackagePublishInfo = await nugetService.GetPackagePublishInfo(tbPackageId.Text);
            */
            try
            {
                PackagePublishInfo = await Mediator.Send(new GetPackagePublishInfoQuery { PackageId = tbPackageId.Text });
                ProcessMessage = "Successfully delivered package information";
            }
            catch(Exception ex)
            {
                PackagePublishInfo = null;
                ProcessMessage = $"An error occurred while querying the package information. {ex.Message}";
            }
        }
    }
}