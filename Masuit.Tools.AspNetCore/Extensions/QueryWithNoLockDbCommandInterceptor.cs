﻿using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Masuit.Tools.Core;

public class QueryWithNoLockDbCommandInterceptor : DbCommandInterceptor
{
    private static readonly Regex TableAliasRegex = new Regex(@"(?<tableAlias>AS \[[a-zA-Z]\w*\](?! WITH \(NOLOCK\)))", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
        );
        return base.ScalarExecuting(command, eventData, result);
    }

#if NETCOREAPP3_1

    public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
CancellationToken cancellationToken = new CancellationToken())
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
        );
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

#else
    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
CancellationToken cancellationToken = new CancellationToken())
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
        );
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }
#endif

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
        );
        return result;
    }

#if NETCOREAPP3_1

    public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
CancellationToken cancellationToken = new CancellationToken())
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
        );
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

#else
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
CancellationToken cancellationToken = new CancellationToken())
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
        );
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }
#endif
}
