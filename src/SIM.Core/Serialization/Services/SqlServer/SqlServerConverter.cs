namespace SIM.Serialization.Services.SqlServer
{
  using System;
  using Newtonsoft.Json;
  using SIM.Abstract.Services.SqlServer;
  using SIM.Services.SqlServer;

  public class SqlServerConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Assert.ArgumentNotNull(writer, nameof(writer));
      Assert.ArgumentNotNull(serializer, nameof(serializer));
               
      serializer.Serialize(writer, ((IConnectionString)value)?.Value);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      Assert.ArgumentNotNull(reader, nameof(reader));
      Assert.ArgumentNotNull(serializer, nameof(serializer));

      var text = serializer.Deserialize<string>(reader);
      if (text == null)
      {
        return null;
      }

      return new SqlServerConnectionString(text);
    }

    public override bool CanConvert(Type objectType)
    {
      return typeof(IConnectionString).IsAssignableFrom(objectType);
    }
  }
}
