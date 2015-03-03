using System;
namespace redis.clients.johm
{


	/// <summary>
	/// JOhm's Model is the fundamental annotation for a persistable entity in Redis.
	/// Any object desired to be persisted to Redis, should use this annotation and
	/// declare corresponding persistable Attribute's, Reference's, and Indexed's,
	/// and various Collections, if so needed.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class Model : System.Attribute
	{

	}

}