namespace SIM
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;
  using JetBrains.Annotations;
  using SIM.Extensions;
                  
  public static class ApplicationManager
  {                    
    #region Constants

    public const string AppDataRoot = @"%AppData%\Sitecore\SIM Next";         

    #endregion

    #region Fields

    [NotNull]
    public static readonly string AppLabel;

    [NotNull]
    public static readonly string AppRevision;       

    [NotNull]
    public static readonly string AppVersion;                    

    [NotNull]
    public static readonly string DataFolder;      

    public static readonly bool IsQA;

    public static readonly string ProcessName;

    [NotNull]
    public static readonly string LogsFolder;
    
    [NotNull]
    public static readonly string ProfilesFolder;

    [NotNull]
    public static readonly string TempFolder;     

    #endregion

    #region Constructors

    static ApplicationManager()
    {
      var processName = Process.GetCurrentProcess().ProcessName + ".exe";
      ProcessName = processName;
      IsQA = processName.ContainsIgnoreCase(".QA.");

      DataFolder = InitializeFolder(Environment.ExpandEnvironmentVariables(AppDataRoot + (IsQA ? "-QA" : "")));
      ProfilesFolder = InitializeDataFolder("Profiles");
      LogsFolder = InitializeDataFolder("Logs");
      AppRevision = GetRevision();
      AppVersion = GetVersion();
      AppLabel = GetLabel();

      TempFolder = InitializeDataFolder("Temp");
    }

    #endregion

    #region Public methods

    #endregion

    #region Private methods

    private static string GetLabel()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof(AssemblyDescriptionAttribute);
      var descriptionAttribute = assembly.GetCustomAttributes(type, true);
      if (descriptionAttribute.Length == 0)
      {
        return string.Empty;
      }

      var label = descriptionAttribute[0] as AssemblyDescriptionAttribute;
      return label != null ? label.Description : string.Empty;
    }

    private static string GetRevision()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof(AssemblyInformationalVersionAttribute);
      var revisionAttribute = assembly.GetCustomAttributes(type, true);
      if (revisionAttribute.Length == 0)
      {
        return DateTime.Now.ToString("yyMMdd");
      }

      var revision = revisionAttribute[0] as AssemblyInformationalVersionAttribute;
      var rev = "rev. ";
      return revision != null ? revision.InformationalVersion.Remove(0, revision.InformationalVersion.IndexOf(rev, StringComparison.Ordinal) + rev.Length) : string.Empty;
    }

    private static string GetShortVersion()
    {
      var version = GetVersion();
      if (string.IsNullOrEmpty(version))
      {
        return string.Empty;
      }

      return version.Substring(0, 3);
    }

    private static string GetVersion()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof(AssemblyFileVersionAttribute);
      var versionAttribute = assembly.GetCustomAttributes(type, true);
      if (versionAttribute.Length == 0)
      {
        return string.Empty;
      }

      var version = versionAttribute[0] as AssemblyFileVersionAttribute;
      return version != null ? version.Version : string.Empty;
    }

    #endregion

    #region Methods

    [NotNull]
    private static string InitializeDataFolder([NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));

      return InitializeFolder(Path.Combine(DataFolder, folder));
    }

    [NotNull]
    private static string InitializeFolder([NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));

      var path = Path.Combine(Environment.CurrentDirectory, folder);
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }

      return path;
    }

    #endregion
  }
}