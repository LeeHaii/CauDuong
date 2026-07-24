#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public sealed class XbimIfcBuildProcessor :
    IPreprocessBuildWithReport,
    IPostprocessBuildWithReport
{
    private const string ConverterProject = "Tools/XbimIfcConverter/XbimIfcConverter.csproj";
    private const string PublishDirectory =
        "Tools/XbimIfcConverter/bin/Release/net8.0/win-x64/publish";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != UnityEditor.BuildTarget.StandaloneWindows64)
        {
            Debug.LogWarning("xBIM runtime IFC import is only packaged for Windows x64.");
            return;
        }

        var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        var projectPath = Path.Combine(projectRoot, ConverterProject);
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments =
                $"publish \"{projectPath}\" --configuration Release " +
                "--runtime win-x64 --self-contained true",
            WorkingDirectory = projectRoot,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new BuildFailedException("Could not start dotnet to publish xBIM.");
        }

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        Task.WaitAll(outputTask, errorTask);

        if (process.ExitCode != 0)
        {
            throw new BuildFailedException(
                $"Failed to publish the xBIM converter.\n" +
                $"{outputTask.Result}\n{errorTask.Result}");
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != UnityEditor.BuildTarget.StandaloneWindows64)
        {
            return;
        }

        var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        var source = Path.Combine(projectRoot, PublishDirectory);
        var playerDirectory = Path.GetDirectoryName(report.summary.outputPath);
        var playerName = Path.GetFileNameWithoutExtension(report.summary.outputPath);
        var destination = Path.Combine(
            playerDirectory!,
            playerName + "_Data",
            "XbimIfcConverter");

        CopyDirectory(source, destination);
    }

    private static void CopyDirectory(string source, string destination)
    {
        if (!Directory.Exists(source))
        {
            throw new BuildFailedException($"xBIM publish output is missing: {source}");
        }

        Directory.CreateDirectory(destination);

        foreach (var file in Directory.GetFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
        }

        foreach (var directory in Directory.GetDirectories(source))
        {
            CopyDirectory(
                directory,
                Path.Combine(destination, Path.GetFileName(directory)));
        }
    }
}
#endif
