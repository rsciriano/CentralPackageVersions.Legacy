using FluentAssertions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace CentralPackageVersions.Legacy.Tests
{
    public class SyncPackageVersionsShould
    {
        public bool CallMsBuild(string projectFilePath, out string consoleOutput)
        {
            var projectCollection = new ProjectCollection { /*DefaultToolsVersion = "16.0" */};
            var project = projectCollection.LoadProject(projectFilePath);

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            WriteHandler handler = (x) => writer.WriteLine(x);
            var logger = new ConsoleLogger(LoggerVerbosity.Normal, handler, null, null);
            projectCollection.RegisterLogger(logger);

            bool result = project.Build();

            projectCollection.UnregisterAllLoggers();
            consoleOutput = builder.ToString();
            return result;
        }

        void CallNugetRestore(out string consoleOutput)
        {
            Process nugetProcess = new Process();
            nugetProcess.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory, "nuget.exe");
            nugetProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            nugetProcess.StartInfo.Arguments = "restore TestFiles\\TestFiles.sln";
            nugetProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            nugetProcess.StartInfo.CreateNoWindow = true;
            nugetProcess.StartInfo.UseShellExecute = false;
            nugetProcess.StartInfo.RedirectStandardOutput = true;
            nugetProcess.StartInfo.RedirectStandardError = true;

            bool started = nugetProcess.Start();
            nugetProcess.WaitForExit();

            consoleOutput = nugetProcess.StandardOutput.ReadToEnd();

            if (nugetProcess.ExitCode != 0)
            {
                throw new ApplicationException(nugetProcess.StandardError.ReadToEnd());
            }
            nugetProcess.Close();
        }

        [Fact]
        public void Return_packages_to_sync_using_centralfile_without_path()
        {
            var task = new SyncPackageVersionsTask();

            task.CentralPackagesFile = new TaskItem("Packages.props");
            task.ProjectPackagesFile = new TaskItem(Path.Combine(Environment.CurrentDirectory, "TestFiles", "TestProject", "packages.config"));

            task.Execute();

            task.PackagesToSync.Should().BeEquivalentTo( new[]
            {
                new TaskItem ("MediatR", new Dictionary<string, string> { {"Version", "5.0.1"} })
            });
        }

        [Fact]
        public void Return_packages_to_sync_using_centralfile_with_fullpath()
        {
            var task = new SyncPackageVersionsTask();

            task.CentralPackagesFile = new TaskItem(Path.Combine(Environment.CurrentDirectory, "TestFiles","Packages.props"));
            task.ProjectPackagesFile = new TaskItem(Path.Combine(Environment.CurrentDirectory, "TestFiles", "TestProject", "packages.config"));

            task.Execute();

            task.PackagesToSync.Should().BeEquivalentTo(new[]
            {
                new TaskItem ("MediatR", new Dictionary<string, string> { {"Version", "5.0.1"} })
            });
        }


        [Fact]
        public void Update_packages_file_when_run_into_msbuild_project()
        {
            string projectFilePath = Path.Combine(Environment.CurrentDirectory, "TestFiles", "TestProject", "TestProject.csproj");
            string packagesFilePath = Path.Combine(Path.GetDirectoryName(projectFilePath), "packages.config");
            string consoleOutput;

            // Restore packages
            CallNugetRestore(out consoleOutput);


            bool success = CallMsBuild(projectFilePath, out consoleOutput);

            Console.Write(consoleOutput);
            
            success.Should().BeTrue(consoleOutput);

            var packages = XElement.Load(packagesFilePath);
            var mediatrReference = packages.Elements("package").FirstOrDefault(p => p.Attribute("id").Value == "MediatR");

            mediatrReference.Should().NotBeNull();
            mediatrReference.Attribute("version").Value.Should().Be("5.0.1");

        }

    }
}
