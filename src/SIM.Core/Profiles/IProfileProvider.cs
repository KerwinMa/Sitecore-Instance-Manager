namespace SIM.Profiles
{
  using JetBrains.Annotations;

  public interface IProfileProvider
  {
    [NotNull]
    IProfile Read();
               
    IProfile TryRead();

    void Save([NotNull] IProfile profile);
  }
}