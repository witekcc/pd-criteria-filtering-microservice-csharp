using System;

namespace redis.clients.johm
{


	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class CollectionMap : System.Attribute
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> key();
		internal object key;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> value();
		internal object value;

		public CollectionMap(object key, object value)
		{
			this.key = key;
			this.value = value;
		}
	}

}