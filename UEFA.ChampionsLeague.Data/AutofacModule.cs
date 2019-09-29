using Autofac;

namespace UEFA.ChampionsLeague.Data
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                .Where(
                    t =>
                        t.Namespace != null && (!t.Namespace.EndsWith(nameof(Models)))// &&
                                                //!t.Namespace.EndsWith(nameof(Migrations))
                )
                .PublicOnly()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
