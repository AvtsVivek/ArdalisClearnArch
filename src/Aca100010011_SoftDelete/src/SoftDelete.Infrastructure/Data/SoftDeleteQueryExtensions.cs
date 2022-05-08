using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using SoftDelete.SharedKernel;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SoftDelete.Infrastructure.Data;
public static class SoftDeleteQueryExtensions
{
  public static void AddSoftDeleteQueryFilter(
      this IMutableEntityType entityType)
  {
    var allBaseTypes = entityType.GetAllBaseTypes();
    var allBaseTypesList = entityType.GetAllBaseTypes().ToList();
    var allBaseTypesCount = entityType.GetAllBaseTypes().Count();

    var allBaseTypesAscending = entityType.GetAllBaseTypesAscending();
    var allBaseTypesAscendingList = entityType.GetAllBaseTypesAscending().ToList();
    var allBaseTypesAscendingCount = entityType.GetAllBaseTypesAscending().Count();

    var allBaseTypesInclusive = entityType.GetAllBaseTypesInclusive();
    var allBaseTypesInclusiveList = entityType.GetAllBaseTypesInclusive().ToList();
    var allBaseTypesInclusiveCount = entityType.GetAllBaseTypesInclusive().Count();

    var allBaseTypesInclusiveAscending = entityType.GetAllBaseTypesInclusiveAscending();
    var allBaseTypesInclusiveAscendingList = entityType.GetAllBaseTypesInclusiveAscending().ToList();
    var allBaseTypesInclusiveAscendingCount = entityType.GetAllBaseTypesInclusiveAscending().Count();

    if (allBaseTypesCount == 0)
    {
      var methodToCall = typeof(SoftDeleteQueryExtensions)
        .GetMethod(nameof(GetSoftDeleteFilter),
        BindingFlags.NonPublic | BindingFlags.Static)!
        .MakeGenericMethod(entityType.ClrType);

      var filter = methodToCall.Invoke(null, new object[] { });
      entityType.SetQueryFilter((LambdaExpression)filter!);
    }
    else if (allBaseTypesCount == 1)
    {
      var baseEntityType = entityType.BaseType!;
      var methodToCall = typeof(SoftDeleteQueryExtensions)
        .GetMethod(nameof(GetSoftDeleteFilter),
        BindingFlags.NonPublic | BindingFlags.Static)!
        .MakeGenericMethod(baseEntityType.ClrType);

      var filter = methodToCall.Invoke(null, new object[] { });
      baseEntityType.SetQueryFilter((LambdaExpression)filter!);
    }
    else if (allBaseTypesCount == 2)
    {
      var baseBaseEntityType = entityType.BaseType!.BaseType!;
      var methodToCall = typeof(SoftDeleteQueryExtensions)
        .GetMethod(nameof(GetSoftDeleteFilter),
        BindingFlags.NonPublic | BindingFlags.Static)!
        .MakeGenericMethod(baseBaseEntityType.ClrType);

      var filter = methodToCall.Invoke(null, new object[] { });
      Debugger.Break();
      baseBaseEntityType.SetQueryFilter((LambdaExpression)filter!);
    }
    else if (allBaseTypesCount == 3)
    {
      var baseBaseBaseEntityType = entityType.BaseType!.BaseType!.BaseType!;
      var methodToCall = typeof(SoftDeleteQueryExtensions)
        .GetMethod(nameof(GetSoftDeleteFilter),
        BindingFlags.NonPublic | BindingFlags.Static)!
        .MakeGenericMethod(baseBaseBaseEntityType.ClrType);

      var filter = methodToCall.Invoke(null, new object[] { });

      Debugger.Break();
      baseBaseBaseEntityType.SetQueryFilter((LambdaExpression)filter!);
    }

    var indexName = entityType.ClrType.FullName + "SoftDeleteIndex";

    entityType.AddIndex(entityType.FindProperty(nameof(ISoftDelete.IsSoftDeleted))!, indexName);
  }

  private static LambdaExpression GetSoftDeleteFilter<TEntity>()
      where TEntity : class, ISoftDelete
  {
    Expression<Func<TEntity, bool>> filter = x => !x.IsSoftDeleted;
    return filter;
  }
}

// Reference
// https://www.thereformedprogrammer.net/ef-core-in-depth-soft-deleting-data-with-global-query-filters/
