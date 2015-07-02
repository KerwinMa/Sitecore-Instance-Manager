namespace SIM.Profiles
{
  using System.IO;
  using Newtonsoft.Json;
  using SIM.Extensions;
  using SIM.IO;
  using SIM.Serialization;
  using Xunit;

  public class ProfileProviderTests
  {
    [Fact]
    public void ProfileProvider_Read_MissingFile()
    {       
      // arrange                                                              
      var fs = new MockFileSystem();                                          
      var file = fs.ParseFile("C:\\1", () => { throw new FileNotFoundException("Could not find file \'C:\\1\'."); });

      // act & assert
      Assert.Throws<FileNotFoundException>(() => new ProfileProvider(file).Read());          
    }

    [Fact]
    public void ProfileProvider_Read_DefaultLocation()
    {
      // arrange  
      var profile = new
      {
        DefaultLocation = "D:\\Some\\Folder"
      };

      var fs = new MockFileSystem();
      var json = new Serializer(fs).SerializeObject(profile).IsNotNull();
      var file = fs.ParseFile("C:\\1", json);
      var sut = new ProfileProvider(file);

      // act
      var actual = sut.Read();

      // assert
      Assert.StrictEqual("D:\\Some\\Folder", actual.DefaultLocation?.FullPath);
      Assert.StrictEqual(null, actual.License);
    }

    [Fact]
    public void ProfileProvider_Read_LicenseFile()
    {
      // arrange                
      var profile = new
      {
        License = "D:\\Some\\File.lic"
      };

      var json = JsonConvert.SerializeObject(profile).IsNotNull();
      var file = new MockFileSystem().ParseFile("C:\\1", json);
      var sut = new ProfileProvider(file);

      // act
      var actual = sut.Read();

      // assert
      Assert.StrictEqual("D:\\Some\\File.lic", actual.License?.FullPath);
      Assert.StrictEqual(null, actual.DefaultLocation);
    }

    [Fact]
    public void ProfileProvider_Read()
    {
      // arrange
      var profile = new
      {
        DefaultLocation = "D:\\Some\\Folder",
        License = "D:\\Some\\File.lic"
      };

      var fs = new MockFileSystem();
      var json = new Serializer(fs).Serialize(profile);
      var file = fs.ParseFile("C:\\1", json);
      var sut = new ProfileProvider(file);

      // act
      var actual = sut.Read();

      // assert
      Assert.StrictEqual("D:\\Some\\Folder", actual.DefaultLocation?.FullPath);
      Assert.StrictEqual("D:\\Some\\File.lic", actual.License?.FullPath);
    }

    [Fact]
    public void ProfileProvider_Save()
    {
      // arrange
      var fs = new MockFileSystem();
      var file = fs.ParseFile("C:\\1");
      var sut = new ProfileProvider(file);
      var profile = new Profile
      {
        DefaultLocation = fs.ParseFolder("D:\\Some\\Folder"),
        License = fs.ParseFile("D:\\Some\\File.lic")
      };

      // act
      sut.Save(profile);

      // assert              
      var text = new StreamReader(file.OpenRead()).ActAndDispose(x => x.ReadToEnd());
      SIM.Assert.IsNotNull(text, nameof(text));

      var actual = new Serializer(fs).DeserializeObject<Profile>(text);

      Assert.Equal("D:\\Some\\Folder", actual.DefaultLocation?.FullPath);
      Assert.Equal("D:\\Some\\File.lic", actual.License?.FullPath);
    }
  }    
}