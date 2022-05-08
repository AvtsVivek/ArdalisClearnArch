using Autofac;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Core.Services;

namespace ProjectManagement.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<ToDoItemSearchService>()
        .As<IToDoItemSearchService>().InstancePerLifetimeScope();
  }
}
