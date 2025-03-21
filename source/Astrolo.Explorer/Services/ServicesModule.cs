using Autofac;

namespace Astrolo.Explorer.Services;

public sealed class ServicesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder
            .RegisterType<YiJingBrowser>()
            .As<IYiJingBrowser>();
    }
}
