using System;

namespace redis.clients.johm
{


	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false]
	public class Array : System.Attribute
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> of();
		internal object of;

		internal int length;

		public Array(object of, int length)
		{
			this.of = of;
			this.length = length;
		}
	}

}