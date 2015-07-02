namespace SIM.Verbs
{
  using JetBrains.Annotations;
  using SIM.Commands;
  using SIM.Profiles;

  public class ProfileVerb : ProfileCommand
  {
    [UsedImplicitly]
    public ProfileVerb() : base(new ProfileProvider(CoreApp.FileSystem))
    {
    }                                           
  }
}