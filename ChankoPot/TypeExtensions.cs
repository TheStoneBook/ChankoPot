﻿using System;
using System.Collections.Generic;

public static class TypeExtensions
{
    public static IEnumerable<Type> GetBaseTypes(this Type self)
    {
        for (var baseType = self.BaseType; null != baseType; baseType = baseType.BaseType)
        {
            yield return baseType;
        }
    }
}
