namespace SIM.IO
{
  using SIM.Abstract.IO;

  public class SystemPathHelper : IPathHelper
  {
    public string GetFullPath(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      return System.IO.Path.GetFullPath(path);
    }

    public string Combine(string path1, string path2)
    {
      Assert.ArgumentNotNullOrEmpty(path1, nameof(path1));
      Assert.ArgumentNotNullOrEmpty(path2, nameof(path2));

      return System.IO.Path.Combine(path1, path2);
    }
  }
}