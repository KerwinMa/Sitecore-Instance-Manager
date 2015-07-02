namespace SIM.Serialization.IO
{
  using System;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using SIM.Abstract.IO;

  public class FolderConverter : JsonConverter
  {
    public FolderConverter([NotNull] IFileSystem fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      FileSystem = fileSystem;
    }

    [NotNull]
    public IFileSystem FileSystem { get; }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Assert.ArgumentNotNull(serializer, nameof(serializer));

      var file = (IFolder)value;
      serializer.Serialize(writer, file?.FullPath);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      Assert.ArgumentNotNull(serializer, nameof(serializer));

      var path = serializer.Deserialize<string>(reader);
      if (path == null)
      {
        return null;
      }

      return FileSystem.ParseFolder(path);
    }

    public override bool CanConvert(Type objectType)
    {
      return typeof(IFolder).IsAssignableFrom(objectType);
    }
  }
}