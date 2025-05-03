using System.Diagnostics;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.CreateMSI);

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory;

    Project LookupTableEditor => Solution.GetProject("LookupTableEditor");

    Target Clean =>
        _ =>
            _.Before(Restore)
                .Executes(() =>
                {
                    SourceDirectory
                        .GlobDirectories("**/bin", "**/obj")
                        .Where(x => !x.ToString().Contains("build"))
                        .ForEach(i => i.DeleteDirectory());
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

    Target CopyAddInFiles =>
        _ =>
            _.DependsOn(Compile)
                .OnlyWhenDynamic(() => LookupTableEditor != null)
                .Executes(() =>
                {
                    var addinFile = LookupTableEditor.Directory / "LookupTableEditor.addin";
                    if (!addinFile.FileExists())
                        return;

                    LookupTableEditor
                        .Directory.GlobDirectories("**/bin")
                        .SelectMany(d => d.GlobDirectories("*"))
                        .Where(d => !d.Name.Contains("Debug"))
                        .ForEach(d =>
                        {
                            addinFile.CopyToDirectory(
                                d,
                                ExistsPolicy.FileOverwrite,
                                createDirectories: true
                            );
                        });
                });
    Target CreateMSI =>
        _ =>
            _.DependsOn(CopyAddInFiles)
                .Executes(() =>
                {
                    var projPath = Solution
                        .AllProjects.FirstOrDefault(p => p.Name == "InstallerBuilder")
                        .Directory;
                    var exe = Path.Combine(projPath, "bin", "Release", "InstallerBuilder.exe");
                    Process.Start(exe, Solution.Directory);
                });
}
