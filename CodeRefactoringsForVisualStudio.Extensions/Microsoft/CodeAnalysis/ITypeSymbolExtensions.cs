﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.CodeAnalysis
{
    public static class ITypeSymbolExtensions
    {
        public static IEnumerable<string> GetUsings(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
            {
                yield break;
            }    

            if ((typeSymbol.ContainingNamespace != null) && (!typeSymbol.ContainingNamespace.IsGlobalNamespace))
            {
                yield return typeSymbol.ContainingNamespace.ToString();
            }

            if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                foreach (var typeArgument in namedTypeSymbol.TypeArguments)
                {
                    foreach (var item in typeArgument.GetUsings())
                    {
                        yield return item;
                    }
                }    
            }
            
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                foreach (var item in arrayTypeSymbol.ElementType.GetUsings())
                {
                    yield return item;
                }
            }
        }


        public static ITypeSymbol UnpackTypeFromTaskAndActionResult(this ITypeSymbol type)
        {
            var toRemove = new[] { "Task", "ActionResult" };

            for (int i = 0; i < 2; ++i)
            {
                if ((type is INamedTypeSymbol namedTypeSymbol) && (toRemove.Contains(namedTypeSymbol.Name)) && namedTypeSymbol.TypeArguments.Length == 1)
                {
                    type = namedTypeSymbol.TypeArguments.First();
                }
            }

            return type;
        }
    }
}