using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CentralPackageVersions.Legacy
{
    public class SyncPackageVersionsTask : Microsoft.Build.Utilities.Task
    {
        private static Regex VersionRegex = new Regex(@"^\[+(.+?)\]*$");

        [Required]
        public ITaskItem CentralPackagesFile { get; set; }

        [Required]
        public ITaskItem ProjectPackagesFile { get; set; }

        [Output]
        public ITaskItem[] PackagesToSync { get; set; }


        public override bool Execute()
        {
            var packagesToSync = new List<ITaskItem>();

            string projectPath = Path.GetDirectoryName(ProjectPackagesFile.ItemSpec);

            string centralPackageFilePath;
            if (Path.IsPathRooted(CentralPackagesFile.ItemSpec))
            {
                centralPackageFilePath = CentralPackagesFile.ItemSpec;
            }
            else
            {
                centralPackageFilePath = FileUtilities.GetPathOfFileAbove(CentralPackagesFile.ItemSpec, projectPath);
            }

            // Read ProjectPackagesFile
            XElement centralPackages = XElement.Load(Path.Combine(projectPath, centralPackageFilePath));
            XElement projectPackages = XElement.Load(ProjectPackagesFile.ItemSpec);

            var centralPackageItems = centralPackages.Element("ItemGroup").Elements("PackageReference");

            bool sucess = true;

            foreach (var item in projectPackages.Elements("package"))
            {
                string packageId = item.Attribute("id").Value;
                string projectPackageVersion = item.Attribute("version").Value;

                var centralPackage = centralPackageItems.FirstOrDefault(p => p.Attribute("Update").Value == packageId);

                if (centralPackage != null)
                {
                    string centralVersion = centralPackage.Attribute("Version").Value;

                    var versionMatch = VersionRegex.Match(centralVersion);

                    if (versionMatch.Success) 
                    {
                        var updateVersion = versionMatch.Groups[1].Value;
                     
                        if (updateVersion != projectPackageVersion) 
                        {
                            packagesToSync.Add(new TaskItem(packageId, new Dictionary<string, string> { { "Version", updateVersion } }));

                            if (this.BuildEngine != null)
                            {
                                Log.LogMessage($"{packageId} v{projectPackageVersion} package need syncronized to v{updateVersion}");
                            }

                        }
                        else
                        {
                            if (this.BuildEngine != null)
                            {
                                Log.LogMessage($"{packageId} v{updateVersion} package already syncronized ");
                            }
                        }


                    }
                    else
                    {
                        sucess = false;
                        if (this.BuildEngine != null)
                        {
                            Log.LogError($"Invalid package version {packageId} {centralVersion}");
                        }
                    }
                }
            }

            PackagesToSync = packagesToSync.ToArray();
            return sucess;
        }
    }
}
