namespace SIM.Services.SqlServer
{
  using Xunit;

  public class SqlServerConnectionStringTests
  {
    [Fact]
    public void SqlServerConnectionString_New_Empty()
    {
      // arrange & act
      var connectionString = new SqlServerConnectionString("");

      // assert
      Assert.Equal("", connectionString.Value);
    }

    [Fact]
    public void SqlServerConnectionString_New_DataSource()
    {
      // arrange & act
      var value = "Data Source=.\\SQLEXPRESS";
      var connectionString = new SqlServerConnectionString(value);

      // assert
      Assert.Equal(value, connectionString.Value);
    }

    [Fact]
    public void SqlServerConnectionString_New_Full()
    {
      // arrange & act
      var value = "Data Source=.\\SQLEXPRESS; User Id=sa; Password=12345; Initial Catalog=Sitecore_Core";
      var connectionString = new SqlServerConnectionString(value);

      // assert
      Assert.Equal(value, connectionString.Value);
    }
  }
}