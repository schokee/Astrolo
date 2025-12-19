using Autofac;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class ProfilingModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder
            .RegisterType<ProfileDirectory>()
            .As<IProfileDirectory>()
            .SingleInstance();

        builder
            .RegisterType<ComparisonSubject>();
    }
}
