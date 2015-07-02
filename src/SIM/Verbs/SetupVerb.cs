namespace SIM.Verbs
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Commands;
  using SIM.Profiles;

  public class SetupVerb : ProfileCommand
  {
    [UsedImplicitly]
    public SetupVerb() : base(new ProfileProvider(CoreApp.FileSystem))
    {
    }

    [Option('d', "defaultLocation", Required = true)]
    public override string DefaultLocation { get; set; }

    [Option('l', "license", Required = true)]
    public override string License { get; set; }
  }
}