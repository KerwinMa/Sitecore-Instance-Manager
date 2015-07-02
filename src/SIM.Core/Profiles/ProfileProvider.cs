namespace SIM.Profiles
{
  using System.IO;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;
  using SIM.IO;
  using SIM.Serialization;

  public class ProfileProvider : IProfileProvider
  {
    [NotNull]
    private readonly IFile ProfileFile;

    public ProfileProvider(IFileSystem fileSystem) : this(FindProfileFile(fileSystem))
    {
    }

    public ProfileProvider([NotNull] IFile profileFile)
    {
      Assert.ArgumentNotNull(profileFile, nameof(profileFile));

      ProfileFile = profileFile;
    }

    public IProfile Read()
    {
      var deserializer = new Serializer(ProfileFile.FileSystem);

      using (var textReader = new StreamReader(ProfileFile.OpenRead()))
      {
        return deserializer.Deserialize<Profile>(textReader);
      }
    }

    public IProfile TryRead()
    {
      try
      {
        return Read();
      }
      catch
      {
        return null;
      }
    }

    public void Save(IProfile profile)
    {
      Assert.ArgumentNotNull(profile, nameof(profile));

      var serializer = new Serializer(ProfileFile.FileSystem);
      ProfileFile.Folder.TryCreate();
      using (var textWriter = new StreamWriter(ProfileFile.OpenWrite()))
      {
        serializer.Serialize(textWriter, profile);
      }
    }

    [NotNull]
    private static IFile FindProfileFile([NotNull] IFileSystem fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      return fileSystem.ParseFolder(ApplicationManager.ProfilesFolder).GetChildFile("profile.json");
    }
  }
}