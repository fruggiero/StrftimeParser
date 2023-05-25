// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
// ReSharper disable UnusedMember.Local
// ReSharper disable IdentifierTypo

class Build : NukeBuild
{
    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath TestsDirectory => RootDirectory / "test";
    static AbsolutePath OutputDirectory => RootDirectory / "output";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Package description")]
    readonly string PackageDescription;

    [Parameter("Authors")]
    readonly string Authors;

    [Parameter("Solution")]
    readonly string Solution;
    
    [Parameter("ProjectUrl")] readonly string ProjectUrl;

    [NerdbankGitVersioning]
    readonly NerdbankGitVersioning NerdbankVersioning;

    public static int Main () => Execute<Build>(x => x.Compile);

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(o => o.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(o => o.DeleteDirectory());
        });

    Target CleanOutput => _ => _
        .Before(Pack)
        .Executes(() =>
        {
            OutputDirectory.CreateOrCleanDirectory();
        });
    
    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(_ =>
            {
                _ = _.SetProjectFile(Solution)
                    .SetConfiguration(Configuration)
                    .SetAuthors(Authors)
                    .SetAssemblyVersion(NerdbankVersioning.AssemblyVersion)
                    .SetFileVersion(NerdbankVersioning.AssemblyFileVersion)
                    .SetInformationalVersion(NerdbankVersioning.AssemblyInformationalVersion);

                return _;
            });
        });

    Target Test => _ => _
        .After(Compile)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(_ => _
                .SetConfiguration(Configuration)
                .SetFilter("FullyQualifiedName!~IntegrationTests")
                .EnableNoBuild());
        });

    Target Pack => _ => _
        .After(Compile)
        .DependsOn(CleanOutput)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(_ => _
                .SetOutputDirectory(OutputDirectory)
                .SetConfiguration(Configuration)
                .SetProperties(new Dictionary<string, object>
                {
                    {"Version", NerdbankVersioning.NuGetPackageVersion},
                    {"Description", PackageDescription},
                    {"Authors", Authors},
                    {"PackageProjectUrl", ProjectUrl}
                })
                .EnableNoBuild());
        });

    Target Deploy => _ => _
        .After(Pack)
        .Executes(() =>
        {
            FileSystemTasks.CopyDirectoryRecursively(OutputDirectory, @"D:\local_packages", DirectoryExistsPolicy.Merge,
                FileExistsPolicy.Overwrite);
        });
}
