using Autofac;
using SoftDelete.Core.Interfaces;
using SoftDelete.Core.Services;

namespace SoftDelete.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<ToDoItemSearchService>()
        .As<IToDoItemSearchService>().InstancePerLifetimeScope();
  }
}
