using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using Autofac;
using Caliburn.Micro;
using Autofac.Core;
using Serilog;
using System.Threading.Tasks;
using Astrolo.Explorer.UI;
using Astrolo.Explorer.Windows;
using Astrolo.GeneKeys;
using Astrolo.HumanDesign;
using Astrolo.YiJing;
using Astrolo.YiJing.Generators;

namespace Astrolo.Explorer;

internal sealed class Bootstrapper : BootstrapperBase
{
    [DllImport("kernel32.dll")]
    private static extern int GetUserDefaultLCID();

    private ILifetimeScope Container { get; set; }

    private CultureInfo Culture { get; }

    public Bootstrapper() : this(true)
    {
    }

    public Bootstrapper(bool useApplication) : base(useApplication)
    {
        Culture = new(GetUserDefaultLCID());

        var logPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $@"Astrolo\Explorer\Logs\{DateTime.Now:yyyyMMdd_HHmm}.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] ({ThreadId}) {Message:lj} {NewLine}{Exception}")
            .WriteTo.File(logPath)
            .CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += (_, args) => Log.Error((Exception)args.ExceptionObject, "Unhandled thread exception");

        TryUpgradeSettings(Properties.Settings.Default);
        Initialize();

    }

    protected override async void OnStartup(object sender, StartupEventArgs e)
    {
        base.OnStartup(sender, e);

        var language = XmlLanguage.GetLanguage(Culture.IetfLanguageTag);
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(language));

        Log.Information("--- Application starting: v" + ProductInfo.Version);

        var exitCode = await RunAsync();

        Log.Information("--- Application exited with code {exitCode}", exitCode);

        Application.Shutdown(exitCode);
    }

    private async Task<int> RunAsync()
    {
        try
        {
            Container = ConfigureContainer();

            try
            {
                await Show.WindowAsync<MainViewModel>();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                Container.Dispose();
                Container = null;
            }
        }
        catch (Exception exception)
        {
            Log.ForContext<Bootstrapper>().Error(exception, "Uncaught exception");
            ShowExceptionWindow(exception);
            return -1;
        }

        return 0;
    }

    private ILifetimeScope ConfigureContainer()
    {
        var containerBuilder = new ContainerBuilder();

        ViewLocator.ConfigureTypeMappings(new()
        {
            IncludeViewSuffixInViewModelNames = false,
            ViewSuffixList = ["View", "Window"]
        });

        var assemblies = SelectAssemblies().ToArray();

        containerBuilder
            .RegisterAssemblyModules(assemblies);

        containerBuilder
            .RegisterAssemblyTypes(assemblies)
            .Where(x => !x.IsAbstract && (x.Name.EndsWith("ViewModel") || x.Name.EndsWith("Page")));

        containerBuilder
            .RegisterType<WindowManager>()
            .As<IWindowManager>()
            .SingleInstance();

        containerBuilder
            .RegisterType<UserPrompt>()
            .As<IUserPrompt>();

        containerBuilder
            .RegisterType<KingWenSequence>()
            .As<ISequence>()
            .SingleInstance();

        containerBuilder
            .Register<IGateDictionary>(_ => GateDictionary.Create())
            .SingleInstance();

        containerBuilder
            .RegisterType<GeneKeyTable>()
            .SingleInstance();

        containerBuilder
            .Register<Func<int, IGeneKey>>(c => n => c.Resolve<GeneKeyTable>()[n]);

        containerBuilder
            .RegisterType<CoinTossGenerator>()
            .As<IChangeGenerator>();

        containerBuilder
            .RegisterType<YarrowStalkGenerator>()
            .As<IChangeGenerator>();

        return containerBuilder.Build();
    }

    private static void TryUpgradeSettings(Properties.Settings settings)
    {
        if (settings.UpgradeRequired)
        {
            settings.Upgrade();
            settings.UpgradeRequired = false;
            settings.Save();

            Log.Information("Upgraded settings");
        }
    }

    private static void ShowExceptionWindow(Exception exception, string info = null)
    {
        if (exception is OperationCanceledException)
        {
            return;
        }

        ExceptionWindow.Show(exception, info);
    }

    #region Dependency Injection

    protected override void BuildUp(object instance)
    {
        Container.InjectProperties(instance);
    }

    protected override object GetInstance(Type type, string name)
    {
        var instance = string.IsNullOrEmpty(name) ? Container.Resolve(type) : Container.ResolveNamed(name, type);
        return instance;
    }

    protected override IEnumerable<object> GetAllInstances(Type type)
    {
        return (object[])Container.ResolveService(new TypedService(typeof(IEnumerable<>).MakeGenericType(type)));
    }

    #endregion
}
