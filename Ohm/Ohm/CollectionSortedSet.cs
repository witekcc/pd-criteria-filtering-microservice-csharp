using System;

namespace redis.clients.johm
{


	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class CollectionSortedSet : System.Attribute
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> of();
		internal object of;

		internal string by;

		public CollectionSortedSet(object of, String by)
		{
			this.of = of;
			this.by = by;
		}
	}

}