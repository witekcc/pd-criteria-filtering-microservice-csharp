using System;

namespace redis.clients.johm.collections
{


	using Convertor = redis.clients.johm.JOhmUtils.Convertor;
	using JOhmCollectionDataType = redis.clients.johm.JOhmUtils.JOhmCollectionDataType;

	/// <summary>
	/// RedisArray is a JOhm-internal 1-Dimensional Array implementation to serve as
	/// a proxy for the Redis persisted array. The backing Redis persistence model is
	/// that of a list.
	/// 
	/// This is a special case of JOhm collections and is treated different from
	/// others primarily due to the non-growable nature of the static-array data
	/// structure. As a consequence, we will not prevent JOhm users from doing their
	/// own array allocation using the 'new' keyword. This mode of usage makes the
	/// RedisArray somewhat of a bridge case between say an Attribute and a dynamic
	/// collection like RedisList.
	/// 
	/// RedisArray does not support null data elements.
	/// </summary>
	public class RedisArray<T>
	{
		private readonly int length;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Class<? extends T> elementClazz;
		private readonly Type elementClazz;
		private readonly JOhmUtils.JOhmCollectionDataType johmElementType;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final redis.clients.johm.Nest<? extends T> nest;
		private readonly Nest<object> nest;
		private readonly Field field;
		private readonly object owner;
		private bool isIndexed;

		public RedisArray<T>(int length, Type clazz, Nest<T> nest, Field field, object owner) 
		{
			this.length = length;
			this.elementClazz = clazz;
			johmElementType = JOhmUtils.detectJOhmCollectionDataType(clazz);
			this.nest = nest;
			this.field = field;
			isIndexed = field.isAnnotationPresent(typeof(Indexed));
			this.owner = owner;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T[] read()
        //public virtual T[] read()
        //{
        //    T[] streamed = (T[]) Array.CreateInstance(elementClazz, length);
        //    for (int iter = 0; iter < length; iter++)
        //    {
        //        streamed[iter] = elementClazz.cast(get(iter));
        //    }
        //    return streamed;
        //}

		public virtual void write(T[] backingArray)
		{
			if (backingArray == null || backingArray.Length == 0)
			{
				clear();
			}
			else
			{
				for (int iter = 0; iter < backingArray.Length; iter++)
				{
					delete(iter);
					save(backingArray[iter]);
				}
			}
		}

		public virtual long? clear()
		{
			return nest.cat(JOhmUtils.getId(owner)).cat(field.Name).del();
		}

		private bool save(T element)
		{
			bool success = false;
			if (element != null)
			{
				if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					success = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).rpush(element.ToString()) > 0;
				}
				else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					success = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).rpush(JOhmUtils.getId(element).ToString()) > 0;
				}
				if (isIndexed)
				{
					indexValue(element);
				}
			}
			return success;
		}

		private void delete(int index)
		{
			T element = this.get(index);
			internalDelete(element);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private T get(int index)
		private T get(int index)
		{
			T element = null;
			string key = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).lindex(index);
			if (!JOhmUtils.isNullOrEmpty(key))
			{
				if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					element = (T) JOhmUtils.Convertor.convert(elementClazz, key);
				}
				else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					element = JOhm.get<T> (elementClazz, Convert.ToInt32(key));
				}
			}
			return element;
		}

		private void indexValue(T element)
		{
			if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
			{
				nest.cat(field.Name).cat(element.ToString()).sadd(JOhmUtils.getId(owner).ToString());
			}
			else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
			{
				nest.cat(field.Name).cat(JOhmUtils.getId(element)).sadd(JOhmUtils.getId(owner).ToString());
			}
		}

		private void unindexValue(T element)
		{
			if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
			{
				nest.cat(field.Name).cat(element.ToString()).srem(JOhmUtils.getId(owner).ToString());
			}
			else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
			{
				nest.cat(field.Name).cat(JOhmUtils.getId(element)).srem(JOhmUtils.getId(owner).ToString());
			}
		}

		private bool internalDelete(T element)
		{
			if (element == null)
			{
				return false;
			}
			long? lrem = 0L;
			if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
			{
				lrem = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).lrem(1, element.ToString());
			}
			else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
			{
				lrem = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).lrem(1, JOhmUtils.getId(element).ToString());
			}
			if (isIndexed)
			{
				unindexValue(element);
			}
			return lrem > 0L;
		}
	}
}