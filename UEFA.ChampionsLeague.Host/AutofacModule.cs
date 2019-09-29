using Autofac;

namespace UEFA.ChampionsLeague.Host
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule<Business.AutofacModule>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                .PublicOnly()
                .AsSelf()
                .InstancePerLifetimeScope()
                .Except<Program>()
                .Except<Startup>();
        }
    }
}
