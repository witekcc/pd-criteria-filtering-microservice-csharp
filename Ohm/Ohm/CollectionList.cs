using System;

namespace redis.clients.johm
{


	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false]
	public class CollectionList : System.Attribute
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> of();
		internal Type of;

		public CollectionList(Type of)
		{
			this.of = of;
		}
	}

}