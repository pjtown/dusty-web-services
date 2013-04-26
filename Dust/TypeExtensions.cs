using System;
using System.Collections.Generic;
using System.Linq;

namespace Dust
{
	public static class TypeExtensions
	{
		public static Type GetGenericInterfaceType(this Type localType, Type interfaceDefinition)
		{
			if (!interfaceDefinition.IsGenericTypeDefinition)
			{
				throw new NotSupportedException("Only finds generic typed for interface definition types");
			}

			foreach (var localInterfaceType in localType.GetInterfaces())
			{
				if (!localInterfaceType.IsGenericType)
				{
					continue;
				}

				if (localInterfaceType.GetGenericTypeDefinition() != interfaceDefinition)
				{
					continue;
				}

				return localInterfaceType;
			}

			return null;
		}

		public static TAttribute GetAttribute<TAttribute>(this Type localType)
			where TAttribute : Attribute
		{
			var attributes = localType.GetAttributes<TAttribute>().ToList();

			return attributes.Count > 0 ? attributes[0] : null;
		}

		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Type localType)
			where TAttribute : Attribute
		{
			foreach (var attributeObject in localType.GetCustomAttributes(true))
			{
				var attribute = attributeObject as TAttribute;

				if (attribute == null)
				{
					continue;
				}
				
				yield return attribute;
			}
		}
	}
}