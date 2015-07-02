namespace SIM.Commands
{
  using System;
  using System.Data;
  using JetBrains.Annotations;
  using SIM.Common;
  using SIM.IO;
  using SIM.Profiles;

  public class ProfileCommand : AbstractCommand<IProfile>
  {
    [NotNull]
    private readonly IProfileProvider ProfileProvider;

    public ProfileCommand([NotNull] ProfileProvider profileProvider)
    {
      Assert.ArgumentNotNull(profileProvider, nameof(profileProvider));

      ProfileProvider = profileProvider;
    }

    public virtual string DefaultLocation { get; [UsedImplicitly] set; }

    public virtual string License { get; [UsedImplicitly] set; }

    protected override void DoExecute(CommandResult<IProfile> result)
    {
      Ensure.IsNotNull(result, nameof(result));

      var profile = ProfileProvider.TryRead();
      var isValid = profile != null;
      profile = profile ?? new Profile();
      
      var changes = 0;     
      var defaultLocation = DefaultLocation;
      if (!string.IsNullOrEmpty(defaultLocation))
      {
        profile.DefaultLocation = LocalFileSystem.Default.ParseFolder(defaultLocation);
        changes += 1;
      }

      var license = License;
      if (!string.IsNullOrEmpty(license))
      {
        profile.License = LocalFileSystem.Default.ParseFile(license);
        changes += 1;
      }                    

      if (changes > 0 || !isValid)
      {
        ProfileProvider.Save(profile);
      }

      try
      {
        result.Data = ProfileProvider.Read();
      }
      catch (Exception ex)
      {
        throw new DataException("Profile file is corrupted", ex);
      }
    }
  }
}