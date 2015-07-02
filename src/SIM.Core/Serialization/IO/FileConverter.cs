namespace SIM.Serialization.IO
{
  using System;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using SIM.Abstract.IO;

  public class FileConverter : JsonConverter
  {
    public FileConverter([NotNull] IFileSystem fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      FileSystem = fileSystem;
    }

    [NotNull]
    public IFileSystem FileSystem { get; }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Assert.ArgumentNotNull(serializer, nameof(serializer));

      var file = (IFile)value;
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

      return FileSystem.ParseFile(path);
    }

    public override bool CanConvert(Type objectType)
    {
      return typeof(IFile).IsAssignableFrom(objectType);
    }
  }
}