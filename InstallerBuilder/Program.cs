using System.Collections.Generic;
using System.IO;
using System.Linq;
using WixSharp;
using File = WixSharp.File;

internal class Program
{
    private const string baseBundlePath =
        @"C:\ProgramData\Autodesk\ApplicationPlugins\LookupTableEditor.bundle";

    private static void Main(string[] args)
    {
        string solutionPath = args[0];

        string path = Path.Combine(solutionPath, @"LookupTableEditor\bin");

        var dir = new DirectoryInfo(path);
        var dirs = new List<Dir>();
        foreach (var item in dir.GetDirectories().Where(d => d.Name.Contains("Release")))
        {
            var dir2 = GetDir(item);

            Dir[] sub = item.GetDirectories().Select(d => GetDir(d)).ToArray();

            dir2.Dirs = dir2.Dirs.Concat(sub).ToArray();
            dirs.Add(dir2);
        }

        string packageContents = Path.Combine(
            solutionPath,
            @"InstallerBuilder\PackageContents.xml"
        );
        var contentXml = new File(packageContents);

        var project = new Project(
            "LookupTableEditor",
            new Dir(baseBundlePath, contentXml, new Dir("Contents", dirs.ToArray()))
        );
        project.UI = WUI.WixUI_ProgressOnly;
        project.BuildMsi(
            Path.Combine(solutionPath, @"LookupTableEditor\bin\LookupTableEditor installer")
        );
    }

    private static Dir GetDir(DirectoryInfo? item)
    {
        return new Dir(item!.Name, item.GetFiles().Select(f => new File(f.FullName)).ToArray());
    }
}
