namespace SIM.IO
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;

  public class MockFileSystem : Dictionary<string, IFileSystemEntry>, IFileSystem
  {
    public IPathHelper Path { get; } = new SystemPathHelper();

    public IFolder ParseFolder(string path)
    {
      return new MockFolder(this, path);
    }

    public IFile ParseFile(string path)
    {
      return ParseFile(path, null, null);
    }

    [NotNull]
    public IFile ParseFile([NotNull] string path, [NotNull] string contents)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNullOrEmpty(contents, nameof(contents));

      return new MockFile(this, path, contents);
    }

    [NotNull]
    public IFile ParseFile([NotNull] string path, [NotNull] Stream stream)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(stream, nameof(stream));

      return new MockFile(this, path, stream);
    }

    [NotNull]
    public IFile ParseFile([NotNull] string path, Func<Stream> openRead = null, Func<Stream> openWrite = null)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));                                                            

      return new MockFile(this, path, openRead, openWrite);
    }
  }
}