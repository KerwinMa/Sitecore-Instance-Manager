﻿namespace SIM.Tool.Windows.UserControls.Install
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Xml;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  [UsedImplicitly]
  public partial class InstanceDetails : IWizardStep, IFlowControl
  {
    #region Fields

    [NotNull]
    private readonly ICollection<string> allFrameworkVersions = Environment.Is64BitOperatingSystem ? new[]
    {
      "v2.0", "v2.0 32bit", "v4.0", "v4.0 32bit"
    } : new[]
    {
      "v2.0", "v4.0"
    };

    private InstallWizardArgs installParameters = null;
    private IEnumerable<Product> standaloneProducts;

    #endregion

    #region Constructors

    public InstanceDetails()
    {
      this.InitializeComponent();

      this.NetFramework.ItemsSource = this.allFrameworkVersions;
    }

    #endregion

    #region Public properties

    public static bool InstallEverywhere
    {
      get
      {
        return WindowsSettings.AppInstallEverywhere.Value;
      }
    }

    #endregion

    #region Public Methods

    public bool OnMovingBack(WizardArgs wizardArg)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var productRevision = this.ProductRevision;
      Assert.IsNotNull(productRevision, nameof(productRevision));

      Product product = productRevision.SelectedValue as Product;
      Assert.IsNotNull(product, nameof(product));

      var rootName = GetValidRootName();

      var rootPath = GetValidRootPath(rootName);
      if (rootPath == null)
        return false;

      var name = GetValidWebsiteName();
      if (name == null)
        return false;

      var hostNames = GetValidHostNames();

      var sqlPrefix = GetValidSqlPrefix();

      var attachSql = this.attachSql.IsChecked ?? true;

      var connectionString = ProfileManager.GetConnectionString();
      SqlServerManager.Instance.ValidateConnectionString(connectionString);

      var licensePath = ProfileManager.Profile.License;
      Assert.IsNotNull(licensePath, @"The license file isn't set in the Settings window");
      FileSystem.FileSystem.Local.File.AssertExists(licensePath, "The {0} file is missing".FormatWith(licensePath));

      var appPoolInfo = GetAppPoolInfo();

      var args = (InstallWizardArgs)wizardArgs;
      args.InstanceName = name;
      args.InstanceHostNames = hostNames;
      args.InstanceSqlPrefix = sqlPrefix;
      args.InstanceAttachSql = attachSql;
      args.InstanceWebRootPath = GetWebRootPath(rootPath);
      args.InstanceRootName = rootName;
      args.InstanceRootPath = rootPath;
      args.InstanceProduct = product;
      args.InstanceConnectionString = connectionString;
      args.LicenseFileInfo = new FileInfo(licensePath);
      args.InstanceAppPoolInfo = appPoolInfo;
      args.Product = product;

      return true;
    }

    [NotNull]
    private AppPoolInfo GetAppPoolInfo()
    {
      var netFramework = this.NetFramework;
      Assert.IsNotNull(netFramework, nameof(netFramework));

      var framework = netFramework.SelectedValue.ToString();
      var frameworkArr = framework.Split(' ');
      Assert.IsTrue(frameworkArr.Length > 0, "impossible");

      var force32Bit = frameworkArr.Length == 2;
      var mode = this.Mode;
      Assert.IsNotNull(mode, nameof(mode));

      var modeItem = (ListBoxItem) mode.SelectedValue;
      Assert.IsNotNull(modeItem, nameof(modeItem));

      var isClassic = ((string) modeItem.Content).EqualsIgnoreCase("Classic");
      var appPoolInfo = new AppPoolInfo
                        {
                          FrameworkVersion = Extensions.EmptyToNull(frameworkArr[0]) ?? "v2.0",
                          Enable32BitAppOnWin64 = force32Bit,
                          ManagedPipelineMode = !isClassic
                        };
      return appPoolInfo;
    }

    private static string GetWebRootPath(string rootPath)
    {
      var webRootPath = Path.Combine(rootPath, "Website");
      return webRootPath;
    }

    [NotNull]
    private string GetValidRootName()
    {
      var rootName = this.RootName;
      Assert.IsNotNull(rootName, nameof(rootName));

      var root = rootName.Text.EmptyToNull();
      Assert.IsNotNull(rootName, "Root folder name must not be emoty");
      return root;
    }

    [CanBeNull]
    private string GetValidRootPath(string root)
    {
      var location = this.locationFolder.Text.EmptyToNull();
      Assert.IsNotNull(location, @"The location folder isn't set");

      var rootPath = Path.Combine(location, root);
      var locationIsPhysical = FileSystem.FileSystem.Local.Directory.HasDriveLetter(rootPath);
      Assert.IsTrue(locationIsPhysical, "The location folder path must be physical i.e. contain a drive letter. Please choose another location folder");

      var webRootPath = GetWebRootPath(rootPath);

      var rootFolderExists = FileSystem.FileSystem.Local.Directory.Exists(rootPath);
      if (!rootFolderExists || InstanceManager.Instances == null)
        return rootPath;
      if (InstanceManager.Instances.Any(i => i != null && i.WebRootPath.EqualsIgnoreCase(webRootPath)))
      {
        Alert("There is another instance with the same root path, please choose another folder");
        return null;
      }

      if (WindowHelper.ShowMessage("The folder with the same name already exists. Would you like to delete it?", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) != MessageBoxResult.OK)
      {
        return null;
      }

      FileSystem.FileSystem.Local.Directory.DeleteIfExists(rootPath);

      return rootPath;
    }

    [CanBeNull]
    private string GetValidWebsiteName()
    {
      var instanceName = this.InstanceName;
      Assert.IsNotNull(instanceName, nameof(instanceName));

      var name = instanceName.Text.EmptyToNull();
      Assert.IsNotNull(name, @"Instance name isn't set");

      var websiteExists = WebServerManager.WebsiteExists(name);
      if (websiteExists)
      {
        using (var context = WebServerManager.CreateContext("InstanceDetails.OnMovingNext('{0}')".FormatWith(name)))
        {
          var site = context.Sites.Single(s => s != null && Extensions.EqualsIgnoreCase(s.Name, name));
          var path = WebServerManager.GetWebRootPath(site);
          if (FileSystem.FileSystem.Local.Directory.Exists(path))
          {
            this.Alert("The website with the same name already exists, please choose another instance name.");
            return null;
          }

          if (WindowHelper.ShowMessage("There website with the same name already exists, but points to non-existing location. Would you like to delete it?", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) != MessageBoxResult.OK)
          {
            return null;
          }

          site.Delete();
          context.CommitChanges();
        }
      }

      websiteExists = WebServerManager.WebsiteExists(name);
      Assert.IsTrue(!websiteExists, "The website with the same name already exists, please choose another instance name.");
      return name;
    }

    [NotNull]
    private string GetValidSqlPrefix()
    {
      var sqlPrefix = this.sqlPrefix;
      Assert.IsNotNull(sqlPrefix, nameof(sqlPrefix));

      var prefix = sqlPrefix.Text.EmptyToNull();
      Assert.IsNotNull(prefix, @"Sql prefix isn't set");

      return prefix;
    }

    [NotNull]
    private string[] GetValidHostNames()
    {
      var hostName = this.HostNames;
      Assert.IsNotNull(hostName, "HostNames is null");

      var hostNamesString = hostName.Text.EmptyToNull();
      Assert.IsNotNull(hostNamesString, "Host names can not be empty");

      var hostNames = hostNamesString.Split(new[] { '\r', '\n', ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
      Assert.IsTrue(hostNames.Any(), "Host names can not be empty");

      foreach (var host in hostNames)
      {
        var hostExists = WebServerManager.HostBindingExists(host);
        Assert.IsTrue(!hostExists, $"Website with the host name '{host}' already exists");
      }
      return hostNames;
    }

    #endregion

    #region Methods

    #region Protected methods

    protected void Alert([NotNull] string message, [NotNull] params object[] args)
    {
      Assert.ArgumentNotNull(message, nameof(message));
      Assert.ArgumentNotNull(args, nameof(args));

      WindowHelper.ShowMessage(message.FormatWith(args), "Conflict is found", MessageBoxButton.OK, MessageBoxImage.Stop);
    }

    #endregion

    #region Private methods

    private void Init()
    {
      using (new ProfileSection("Initializing InstanceDetails", this))
      {
        this.DataContext = new Model();
        this.standaloneProducts = ProductManager.StandaloneProducts;
      }
    }

    private void InstanceNameTextChanged([CanBeNull] object sender, [CanBeNull] TextChangedEventArgs e)
    {
      using (new ProfileSection("Instance name text changed", this))
      {
        var name = this.InstanceName.Text;

        this.RootName.Text = name;
        this.sqlPrefix.Text = name;
        this.HostNames.Text = GenerateHostName(name);
      }
    }

    [NotNull]
    private string GenerateHostName([NotNull]string name)
    {
      var hostName = name;
      if (ProductHelper.Settings.CoreProductReverseHostName.Value)
      {
        // convert example.cm1 into cm1.example
        hostName = string.Join(".", hostName.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Reverse());
      }
      if (ProductHelper.Settings.CoreProductHostNameEndsWithLocal.Value && !hostName.EndsWith(".local", StringComparison.InvariantCultureIgnoreCase))
      {
        // convert to cm1.example.local
        hostName = hostName + (ProductHelper.Settings.CoreProductHostNameEndsWithLocal.Value ? ".local" : "");
      }
      return hostName;
    }

    private void PickLocationFolder([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose location folder", this.locationFolder, null);
    }

    private void ProductNameChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      var productName = this.ProductName;
      Assert.IsNotNull(productName, nameof(productName));

      var grouping = productName.SelectedValue as IGrouping<string, Product>;
      if (grouping == null)
      {
        return;
      }

      var productVersion = this.ProductVersion;
      Assert.IsNotNull(productVersion, nameof(productVersion));

      productVersion.DataContext = grouping.Where(x => x != null).GroupBy(p => p.ShortVersion).Where(x => x != null).OrderBy(p => p.Key);
      this.SelectFirst(productVersion);
    }

    private void ProductRevisionChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      using (new ProfileSection("Product revision changed", this))
      {
        var product = this.ProductRevision.SelectedValue as Product;
        if (product == null)
        {
          return;
        }

        var name = product.DefaultInstanceName;
        this.InstanceName.Text = name;

        var frameworkVersions = new ObservableCollection<string>(this.allFrameworkVersions);

        var m = product.Manifest;
        if (m != null)
        {
          var node = (XmlElement)m.SelectSingleNode("/manifest/*/limitations");
          if (node != null)
          {
            foreach (XmlElement limitation in node.ChildNodes)
            {
              var lname = limitation.Name;
              switch (lname.ToLower())
              {
                case "framework":
                {
                  var supportedVersions = limitation.SelectElements("supportedVersion");
                  if (supportedVersions != null)
                  {
                    ICollection<string> supportedVersionNames = supportedVersions.Select(supportedVersion => supportedVersion.InnerText).ToArray();
                    for (int i = frameworkVersions.Count - 1; i >= 0; i--)
                    {
                      if (!supportedVersionNames.Contains(frameworkVersions[i]))
                      {
                        frameworkVersions.RemoveAt(i);
                      }
                    }
                  }

                  break;
                }
              }
            }
          }
        }

        var netFramework = this.NetFramework;
        Assert.IsNotNull(netFramework, nameof(netFramework));

        netFramework.ItemsSource = frameworkVersions;
        netFramework.SelectedIndex = 0;
      }
    }

    private void ProductVersionChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      var productVersion = this.ProductVersion;
      Assert.IsNotNull(productVersion, nameof(productVersion));

      var grouping = productVersion.SelectedValue as IGrouping<string, Product>;
      if (grouping == null)
      {
        return;
      }

      this.ProductRevision.DataContext = grouping.OrderBy(p => p.Revision);
      this.SelectLast(this.ProductRevision);
    }

    private void Select([NotNull] Selector element, [NotNull] string value)
    {
      Assert.ArgumentNotNull(element, nameof(element));
      Assert.ArgumentNotNull(value, nameof(value));

      if (element.Items.Count <= 0)
      {
        return;
      }

      for (int i = 0; i < element.Items.Count; ++i)
      {
        object item0 = element.Items[i];
        IGrouping<string, Product> item1 = item0 as IGrouping<string, Product>;
        if (item1 != null)
        {
          var key = item1.Key;
          if (key.EqualsIgnoreCase(value))
          {
            element.SelectedIndex = i;
            break;
          }
        }
        else
        {
          Product item2 = item0 as Product;
          if (item2 != null)
          {
            var key = item2.Revision;
            if (key.EqualsIgnoreCase(value))
            {
              element.SelectedIndex = i;
              break;
            }
          }
        }
      }
    }

    private void SelectByValue([NotNull] Selector element, string value)
    {
      Assert.ArgumentNotNull(element, nameof(element));

      if (string.IsNullOrEmpty(value))
      {
        this.SelectLast(element);
        return;
      }

      if (element.Items.Count > 0)
      {
        if (element.Items[0].GetType() == typeof(Product))
        {
          foreach (Product item in element.Items)
          {
            if (item.Name.EqualsIgnoreCase(value, true))
            {
              element.SelectedItem = item;
              break;
            }

            if (item.Revision.EqualsIgnoreCase(value, true))
            {
              element.SelectedItem = item;
              break;
            }
          }
        }
        else
        {
          foreach (var item in element.Items)
          {
            if (item is ContentControl)
            {
              if ((item as ContentControl).Content.ToString().EqualsIgnoreCase(value, true))
              {
                element.SelectedItem = item;
                break;
              }
            }

            if (item is string)
            {
              if ((item as string).EqualsIgnoreCase(value, true))
              {
                element.SelectedItem = item;
                break;
              }
            }
          }
        }
      }
    }

    private void SelectFirst([NotNull] Selector element)
    {
      Assert.ArgumentNotNull(element, nameof(element));

      if (element.Items.Count > 0)
      {
        element.SelectedIndex = 0;
      }
    }

    private void SelectLast([NotNull] Selector element)
    {
      Assert.ArgumentNotNull(element, nameof(element));

      if (element.Items.Count > 0)
      {
        element.SelectedIndex = element.Items.Count - 1;
      }
    }

    private void SelectProductByValue([CanBeNull] Selector element, [NotNull] string value)
    {
      Assert.ArgumentNotNull(value, nameof(value));

      if (element == null)
      {
        return;
      }

      if (string.IsNullOrEmpty(value))
      {
        this.SelectLast(element);
        return;
      }

      var items = element.Items;
      Assert.IsNotNull(items, nameof(items));
      if (items.Count > 0)
      {
        foreach (IGrouping<string, Product> item in items)
        {
          if (item.First().Name.EqualsIgnoreCase(value, true))
          {
            element.SelectedItem = item;
            break;
          }

          if (item.First().Version.EqualsIgnoreCase(value, true))
          {
            element.SelectedItem = item;
            break;
          }
        }
      }
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      using (new ProfileSection("Window loaded", this))
      {
        var args = this.installParameters;
        Assert.IsNotNull(args, nameof(args));

        var product = args.Product;
        if (product != null)
        {
          return;
        }

        this.SelectProductByValue(this.ProductName, WindowsSettings.AppInstallationDefaultProduct.Value);
        this.SelectProductByValue(this.ProductVersion, WindowsSettings.AppInstallationDefaultProductVersion.Value);
        this.SelectByValue(this.ProductRevision, WindowsSettings.AppInstallationDefaultProductRevision.Value);

        var netFramework = this.NetFramework;
        Assert.IsNotNull(netFramework, nameof(netFramework));

        if (string.IsNullOrEmpty(WindowsSettings.AppInstallationDefaultFramework.Value))
        {
          netFramework.SelectedIndex = 0;
        }
        else
        {
          this.SelectByValue(netFramework, WindowsSettings.AppInstallationDefaultFramework.Value);
        }

        var mode = this.Mode;
        Assert.IsNotNull(mode, nameof(mode));

        if (string.IsNullOrEmpty(WindowsSettings.AppInstallationDefaultPoolMode.Value))
        {
          mode.SelectedIndex = 0;
        }
        else
        {
          this.SelectByValue(mode, WindowsSettings.AppInstallationDefaultPoolMode.Value);
        }
      }
    }

    #endregion

    #endregion

    #region Nested type: Model

    public class Model
    {
      #region Fields

      [CanBeNull]
      [UsedImplicitly]
      public readonly Product[] Products = ProductManager.StandaloneProducts.ToArray();

      [NotNull]
      private string name;

      #endregion

      #region Properties

      [NotNull]
      [UsedImplicitly]
      public string Name
      {
        get
        {
          return this.name;
        }

        set
        {
          Assert.IsNotNull(value.EmptyToNull(), "Name must not be empty");
          this.name = value;
        }
      }

      [UsedImplicitly]
      public IGrouping<string, Product> SelectedProductGroup1 { get; set; }

      #endregion
    }

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      this.Init();

      this.locationFolder.Text = ProfileManager.Profile.InstancesFolder;
      this.ProductName.DataContext = this.standaloneProducts.GroupBy(p => p.Name);

      var args = (InstallWizardArgs)wizardArgs;
      this.installParameters = args;

      Product product = args.Product;
      if (product != null)
      {
        this.Select(this.ProductName, product.Name);
        this.Select(this.ProductVersion, product.ShortVersion);
        this.Select(this.ProductRevision, product.Revision);
      }
      else
      {
        this.SelectFirst(this.ProductName);
      }

      AppPoolInfo info = args.InstanceAppPoolInfo;
      if (info != null)
      {
        var frameworkValue = info.FrameworkVersion + " " + (info.Enable32BitAppOnWin64 ? "32bit" : string.Empty);
        this.SelectByValue(this.NetFramework, frameworkValue);
        this.SelectByValue(this.Mode, info.ManagedPipelineMode ? "Integrated" : "Classic");
      }

      var name = args.InstanceName;
      if (!string.IsNullOrEmpty(name))
      {
        this.InstanceName.Text = name;
      }

      var rootName = args.InstanceRootName;
      if (!string.IsNullOrEmpty(rootName))
      {
        this.RootName.Text = rootName;
      }

      var hostNames = args.InstanceHostNames;
      if (hostNames != null && hostNames.Any())
      {
        this.HostNames.Text = hostNames.Join("\r\n");
      }

      if (rootName == null)
        return;
      var location = args.InstanceRootPath.TrimEnd(rootName).Trim('/', '\\');
      if (!string.IsNullOrEmpty(location))
      {
        this.locationFolder.Text = location;
      }
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion
  }
}