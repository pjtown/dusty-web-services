using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace Dust.Serialization
{
	public class QueryStringDeserializer<T>
	{
		private readonly Func<NameValueCollection, T> deserialize;

		public QueryStringDeserializer()
		{
			var queryStringType = typeof(NameValueCollection);

			var queryStringParameter = Expression.Parameter(queryStringType, "queryString");

			var getValueMethodInfo = queryStringType.GetMethod("Get", new[] { typeof(string) });

			var memberBindings = new List<MemberBinding>();

			foreach (var propertyInfo in typeof(T).GetProperties())
			{
				var keyConstant = Expression.Constant(propertyInfo.Name.ToLower());

				var getQueryStringValueCall = Expression.Call(queryStringParameter, getValueMethodInfo, keyConstant);

				MemberAssignment memberAssignment;

				if (propertyInfo.PropertyType == getQueryStringValueCall.Type)
				{
					memberAssignment = Expression.Bind(propertyInfo, getQueryStringValueCall);
				}
				else
				{
					var getPropertyValueMethodCall = Expression.Call(typeof(QueryStringDeserializer<T>), "GetPropertyValue", new[] { propertyInfo.PropertyType }, queryStringParameter, keyConstant);

					memberAssignment = Expression.Bind(propertyInfo, getPropertyValueMethodCall);
				}
				
				memberBindings.Add(memberAssignment);
			}

			var newExpression = Expression.New(typeof(T));

			var memberInitExpression = Expression.MemberInit(newExpression, memberBindings);

			this.deserialize = Expression.Lambda<Func<NameValueCollection, T>>(memberInitExpression, queryStringParameter).Compile();
		}

		public static TValue GetPropertyValue<TValue>(NameValueCollection queryString, string key)
		{
			var value = queryString.Get(key);
			
			if (value == null)
			{
				return default(TValue);
			}

			return (TValue)Convert.ChangeType(value, typeof(TValue));
		}

		public T Deserialize(NameValueCollection queryString)
		{
			return this.deserialize(queryString);
		}

		
	}
}