using System;

namespace redis.clients.johm
{


	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false]
	public class CollectionSet : System.Attribute
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> of();
		internal object of;

		public CollectionSet(object of)
		{
			this.of = of;
		}
	}

}