using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild
        ? Configuration.Debug
        : Configuration.Release;

    [Solution]
    readonly Solution Solution;
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean =>
        _ =>
            _.Before(Restore)
                .Executes(() =>
                {
                    Solution
                        .Projects.Where(p => p.Name != "_build")
                        .SelectMany(p => p.Directory.GlobDirectories("**/bin", "**/obj"))
                        .ForEach(c =>
                        {
                            Log.Information($"Delete directory: {c.Parent}\\{c.Name}");
                            c.DeleteDirectory();
                        });
                });

    Target Restore =>
        _ =>
            _.DependsOn(Clean)
                .Executes(() =>
                {
                    DotNetRestore(s => s.SetProjectFile(Solution));
                });

    Target Compile =>
        _ =>
            _.DependsOn(Restore)
                .Executes(() =>
                {
                    foreach (var item in Solution.Configurations)
                    {
                        var t = item.Key.Split('|').First();
                        if (t.Contains("Debug"))
                            continue;
                        DotNetBuild(s => s.SetProjectFile(Solution).SetConfiguration(t));
                    }
                });
}
