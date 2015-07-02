namespace SIM.Verbs
{
  using CommandLine;
  using CommandLine.Text;
  using JetBrains.Annotations;

  public class VerbsPalette
  {
    [HelpVerbOption]
    [UsedImplicitly]
    public string GetUsage(string verb)
    {
      return HelpText.AutoBuild(this, verb);
    }

    #region Nested Commands                                            

    [UsedImplicitly]
    [VerbOption("setup", HelpText = "Set up the profile.")]
    public SetupVerb SetupVerb { get; set; }

    [UsedImplicitly]
    [VerbOption("profile", HelpText = "Show profile.")]
    public ProfileVerb ProfileVerb { get; set; }

    #endregion
  }
}