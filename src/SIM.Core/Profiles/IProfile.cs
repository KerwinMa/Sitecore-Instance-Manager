namespace SIM.Profiles
{
  using SIM.Abstract.IO;
  using SIM.IO;

  public interface IProfile
  {
    IFolder DefaultLocation { get; set; }

    IFile License { get; set; }       
  }
}