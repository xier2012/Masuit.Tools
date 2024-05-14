﻿using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Masuit.Tools.Dynamics.Behaviors;
using Masuit.Tools.Dynamics.Implementation;

namespace Masuit.Tools.Dynamics;

public class Clay(IEnumerable<IClayBehavior> behaviors) : IDynamicMetaObjectProvider, IClayBehaviorProvider
{
    private readonly ClayBehaviorCollection _behavior = new(behaviors);

    public Clay() : this(Enumerable.Empty<IClayBehavior>())
    {
    }

    public Clay(params IClayBehavior[] behaviors) : this(behaviors.AsEnumerable())
    {
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
    {
        return new ClayMetaObject(this, parameter);
    }

    IClayBehavior IClayBehaviorProvider.Behavior => _behavior;

    /// <inheritdoc />
    public override string ToString()
    {
        var fallback = base.ToString();
        return _behavior.InvokeMember(() => fallback, this, "ToString", Arguments.Empty()) as string ?? string.Empty;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return _behavior.OfType<PropBehavior>().FirstOrDefault()?.GetProps();
    }

    /// <summary>
    /// 移除属性
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public static Clay operator -(Clay left, string right)
    {
        left.ToDictionary().Remove(right);
        return left;
    }

    /// <summary>
    /// 移除属性
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public static Clay operator +(Clay left, string right)
    {
        left[right] = DynamicFactory.NewObject();
        return left;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    public object this[string key]
    {
        get => ToDictionary()[key];
        set => ToDictionary()[key] = value;
    }
}
