using Autofac;

namespace UEFA.ChampionsLeague.Business
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule<Data.AutofacModule>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                .PublicOnly()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
