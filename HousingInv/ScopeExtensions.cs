using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Activation;
using Ninject.Syntax;

namespace Ninject.Extensions.NamedScope;
public static class ScopeExtensions
{
    /// <summary>
    /// Defines that a binding is in a named scope.
    /// </summary>
    /// <typeparam name="T">The type of the binding.</typeparam>
    /// <param name="syntax">The In syntax.</param>
    /// <param name="scopeParameterName">Name of the scope parameter.</param>
    /// <returns>The Named syntax.</returns>
    // public static IBindingNamedWithOrOnSyntax<T> InPluginScope<T>(this IBindingInSyntax<T> syntax)
    // {
    //     return syntax.InScope(context => this);
    // }
}
