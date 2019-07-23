using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDb.Model
{
    /// <summary>
    /// Identifies a specific kind of declaration in .NET metadata
    /// </summary>
    public enum ApiDeclarationKind
    {
        Unknown,
        Assembly,
        Module,
        Class,
        Struct,
        Interface,
        Enum,
        Delegate,
        Method,
        Property,
        Field,
        Event,
    }
}
