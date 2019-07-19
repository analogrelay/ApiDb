using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace ApiDb
{
    internal static class CecilUtils
    {
        /// <summary>
        /// Gets all attributes that match the full name of the specified local attribute type. Assembly
        /// name is not compared.
        /// </summary>
        /// <param name="attributes">The set of attributes to search.</param>
        /// <param name="localAttributeType">A local type to match the name against.</param>
        /// <returns>The matching attributes.</returns>
        internal static IEnumerable<CustomAttribute> GetByLocalType(this IEnumerable<CustomAttribute> attributes, Type localAttributeType)
            => GetByName(attributes, localAttributeType.FullName);

        /// <summary>
        /// Gets all attributes that match the specified full name. Assembly name is not compared.
        /// </summary>
        /// <param name="attributes">The set of attributes to search.</param>
        /// <param name="fullName">A name to search for.</param>
        /// <returns>The matching attributes.</returns>
        internal static IEnumerable<CustomAttribute> GetByName(this IEnumerable<CustomAttribute> attributes, string fullName)
        {
            foreach (var attribute in attributes)
            {
                var type = attribute.AttributeType;
                if (type.FullName.Equals(fullName))
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>
        /// Gets the custom attribute's constructor values.
        /// </summary>
        /// <typeparam name="T">The type of the first constructor argument.</typeparam>
        /// <param name="attribute">The attribute to get values from.</param>
        /// <returns>The value.</returns>
        internal static T GetConstructorValues<T>(this CustomAttribute attribute)
        {
            return (T)attribute.ConstructorArguments[0].Value;
        }

        /// <summary>
        /// Gets the custom attribute's constructor values.
        /// </summary>
        /// <typeparam name="T1">The type of the first constructor argument.</typeparam>
        /// <typeparam name="T1">The type of the second constructor argument.</typeparam>
        /// <param name="attribute">The attribute to get values from.</param>
        /// <returns>The values.</returns>
        internal static (T1, T2) GetConstructorValues<T1, T2>(this CustomAttribute attribute)
        {
            return ((T1)attribute.ConstructorArguments[0].Value,
                (T2)attribute.ConstructorArguments[1].Value);
        }
    }
}