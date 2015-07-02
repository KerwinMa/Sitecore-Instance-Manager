namespace SIM.Profiles
{
  using SIM.Abstract.IO;
  using SIM.IO;

  public class Profile : IProfile
  {                                                                                                                    
    public IFolder DefaultLocation { get; set; }

    public IFile License { get; set; }     
  }
}