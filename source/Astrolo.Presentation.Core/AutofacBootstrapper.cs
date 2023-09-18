using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using Astrolo.Presentation.Core.Windows;
using Autofac;
using Autofac.Core;
using Caliburn.Micro;

namespace Astrolo.Presentation.Core
{
    public abstract class AutofacBootstrapper : BootstrapperBase
    {
        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultLCID();

        protected ILifetimeScope Container { get; private set; }

        protected CultureInfo Culture { get; }

        protected AutofacBootstrapper(bool useApplication = true) : base(useApplication)
        {
            Container = new ContainerBuilder().Build();
            Culture = new CultureInfo(GetUserDefaultLCID());
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            var language = XmlLanguage.GetLanguage(Culture.IetfLanguageTag);
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(language));

        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);

            Container.Dispose();
        }

        protected ILifetimeScope BeginLifetimeScope(string name, Action<ContainerBuilder> configure)
        {
            // REVISIT: this is *not* thread safe.

            var scopeToRestore = Container;
            var newScope = Container.BeginLifetimeScope(name, configure);

            newScope.CurrentScopeEnding += (_, _) => { Container = scopeToRestore; };
            Container = newScope;

            return newScope;
        }

        //protected async Task ExecuteStartupTasksAsync(ILifetimeScope container)
        //{
        //    void RegisterStartupTasks(ContainerBuilder containerBuilder)
        //    {
        //        containerBuilder
        //            .RegisterAssemblyTypes(SelectAssemblies().ToArray())
        //            .Where(x => x.IsAssignableTo<IStartupTask>() && !x.IsAbstract)
        //            .As<IStartupTask>();
        //    }

        //    await using var innerScope = container.BeginLifetimeScope(RegisterStartupTasks);

        //    var startupTasks = innerScope
        //        .Resolve<IEnumerable<Lazy<IStartupTask, ISequenced>>>()
        //        .OrderBy(x => x.Metadata.Order);

        //    foreach (var task in startupTasks)
        //    {
        //        await task.Value.ExecuteAsync();
        //    }
        //}

        [Conditional("DEBUG")]
        protected static void ShowExceptionWindow(Exception exception, string? info = null)
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
            return (object[]?)Container.ResolveService(new TypedService(typeof(IEnumerable<>).MakeGenericType(type))) ?? Enumerable.Empty<object>();
        }

        #endregion
    }
}
