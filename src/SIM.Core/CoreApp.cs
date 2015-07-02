namespace SIM
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;
  using SIM.IO;

  public static class CoreApp
  {
    public static IFileSystem FileSystem = new LocalFileSystem();

    public static void LogMainInfo()
    {
      try
      {
        var nativeArgs = Environment.GetCommandLineArgs();
        var commandLineArgs = nativeArgs.Skip(1).ToArray();
        var argsToLog = commandLineArgs.Length > 0 ? string.Join("|", commandLineArgs) : "<NO ARGUMENTS>";

        // TODO: Replace with new logging engine; Log.Info("**********************************************************************");
        // TODO: Replace with new logging engine; Log.Info("**********************************************************************");
        // TODO: Replace with new logging engine; Log.Info("Sitecore Instance Manager started");
        // TODO: Replace with new logging engine; Log.Info("Version: {0}", ApplicationManager.AppVersion);
        // TODO: Replace with new logging engine; Log.Info("Revision: {0}", ApplicationManager.AppRevision);
        // TODO: Replace with new logging engine; Log.Info("Label: {0}", ApplicationManager.AppLabel);
        // TODO: Replace with new logging engine; Log.Info("IsQA: {0}", ApplicationManager.IsQA);
        // TODO: Replace with new logging engine; Log.Info("Executable: {0}", nativeArgs.FirstOrDefault() ?? ApplicationManager.ProcessName);
        // TODO: Replace with new logging engine; Log.Info("Arguments: {0}", argsToLog);
        // TODO: Replace with new logging engine; Log.Info("Directory: {0}", Environment.CurrentDirectory);
        // TODO: Replace with new logging engine; Log.Info("**********************************************************************");
        // TODO: Replace with new logging engine; Log.Info("**********************************************************************");
      }
      catch
      {
        Debug.WriteLine("Error during log main info");
      }
    }

    public static void InitializeLogging()
    {
      // TODO: Replace with new logging engine; Log.Initialize(new Log4NetLogProvider());
      /*
      var logConfig = new FileInfo("Log.config");
      if (logConfig.Exists)
      {
        XmlConfigurator.Configure(logConfig);
      }
      else
      {
        var infoLogger = new LogFileAppender
        {
          AppendToFile = true,
          File = "hard-coded",
          Layout = new PatternLayout("%4t %d{ABSOLUTE} %-5p %m%n"),
          SecurityContext = new WindowsSecurityContext(),
          Threshold = Level.Info
        };

        var debugLogger = new LogFileAppender
        {
          AppendToFile = true,
          File = "$(debugPath)",
          Layout = new PatternLayout("%4t %d{ABSOLUTE} %-5p %m%n"),
          SecurityContext = new WindowsSecurityContext(),
          Threshold = Level.Debug
        };

        BasicConfigurator.Configure(infoLogger, debugLogger);
      }
      */
    }

    public static bool DoNotTrack()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "donottrack.txt");

      return File.Exists(path);
    }                       

    [NotNull]
    public static string GetCookie()
    {
      var tempFolder = ApplicationManager.TempFolder;
      var path = Path.Combine(tempFolder, "cookie.txt");
      if (Directory.Exists(tempFolder))
      {
        if (File.Exists(path))
        {
          var cookie = File.ReadAllText(path);
          if (!string.IsNullOrEmpty(cookie))
          {
            return cookie;
          }

          try
          {
            File.Delete(path);
          }
          catch (Exception ex)
          {
            // TODO: Replace with new logging engine; Log.Error(ex, "Cannot delete cookie file");
          }
        }
      }
      else
      {
        Directory.CreateDirectory(tempFolder);
      }

      var newCookie = Guid.NewGuid().ToString("N");
      try
      {
        File.WriteAllText(path, newCookie);
      }
      catch (Exception ex)
      {
        // TODO: Replace with new logging engine; Log.Error(ex, "Cannot write cookie");
      }

      return newCookie;
    }

    public static void OpenFile(string path)
    {
      RunApp("explorer.exe", path.Replace('/', '\\'));
    }

    public static void OpenFolder(string path)
    {
      OpenFile(path);
    }

    public static void OpenInBrowser(string url, string[] parameters = null)
    {
      var app = "explorer.exe";
      if (!string.IsNullOrEmpty(app))
      {
        var arguments = parameters?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();
        arguments.Add(url);
        RunApp(app, arguments.ToArray());

        return;
      }

      OpenFile(url);
    }

    public static Process RunApp(string app, params string[] @params)
    {
      var resultParams = string.Join(" ", @params.Select(x => x.Trim('\"')).Select(x => x.Contains(" ") || x.Contains("=") ? "\"" + x + "\"" : x));
      // TODO: Replace with new logging engine; Log.Debug("resultParams: {0}", resultParams);

      var process = Process.Start(app, resultParams);

      return process;
    }

    public static Process RunApp(ProcessStartInfo startInfo)
    {
      return Process.Start(startInfo);
    }
  }
}