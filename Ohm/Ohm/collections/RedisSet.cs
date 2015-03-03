using System;
using System.Collections.Generic;

namespace redis.clients.johm.collections
{


	using Convertor = redis.clients.johm.JOhmUtils.Convertor;
	using JOhmCollectionDataType = redis.clients.johm.JOhmUtils.JOhmCollectionDataType;

	/// <summary>
	/// RedisSet is a JOhm-internal Set implementation to serve as a proxy for the
	/// Redis persisted set and provide lazy-loading semantics to minimize datastore
	/// network traffic. It does a best-effort job of minimizing set entity staleness
	/// but does so without any locking and is not thread-safe. It also maintains
	/// whatever order in which Redis returns its set elements. Only add and remove
	/// trigger a remote-sync of local internal storage.
	/// 
	/// RedisSet does not support null data elements.
	/// </summary>
	public class RedisSet<T> : HashSet<T>
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final redis.clients.johm.Nest<? extends T> nest;
		private readonly Nest<object> nest;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Class<? extends T> elementClazz;
		private readonly Type elementClazz;
		private readonly JOhmUtils.JOhmCollectionDataType johmElementType;
		private readonly object owner;
		private readonly Field field;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public RedisSet(final Class<? extends T> clazz, final redis.clients.johm.Nest<? extends T> nest, Field field, Object owner)
		public RedisSet<T1, T2>(Type clazz, Nest<T2> nest, Field field, object owner) where T1 : T where T2 : T
		{
			this.elementClazz = clazz;
			johmElementType = JOhmUtils.detectJOhmCollectionDataType(clazz);
			this.nest = nest;
			this.field = field;
			this.owner = owner;
		}

		private void indexValue(T element)
		{
			if (field.isAnnotationPresent(typeof(Indexed)))
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
		}

		private void unindexValue(T element)
		{
			if (field.isAnnotationPresent(typeof(Indexed)))
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
		}

		public virtual int size()
		{
			return nest.cat(JOhmUtils.getId(owner)).cat(field.Name).smembers().size();
		}

		public virtual bool Empty
		{
			get
			{
				return this.size() == 0;
			}
		}

		public virtual bool contains(object o)
		{
			return scrollElements().Contains(o);
		}

		public virtual IEnumerator<T> iterator()
		{
			return scrollElements().GetEnumerator();
		}

		public virtual object[] toArray()
		{
			return scrollElements().ToArray();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("hiding") public <T> T[] toArray(T[] a)
		public virtual T[] toArray<T>(T[] a)
		{
			return scrollElements().toArray(a);
		}

		public virtual bool add(T element)
		{
			return internalAdd(element);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean remove(Object o)
		public virtual bool remove(object o)
		{
			return internalRemove((T) o);
		}

		public virtual bool containsAll<T1>(ICollection<T1> c)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			return scrollElements().containsAll(c);
		}

		public virtual bool addAll<T1>(ICollection<T1> collection) where T1 : T
		{
			bool success = true;
			foreach (T element in collection)
			{
				success &= internalAdd(element);
			}
			return success;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean retainAll(java.util.Collection<?> c)
		public virtual bool retainAll<T1>(ICollection<T1> c)
		{
			this.clear();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<?> iterator = (java.util.Iterator<?>) c.iterator();
			IEnumerator<?> iterator = (IEnumerator<?>) c.GetEnumerator();
			bool success = true;
			while (iterator.MoveNext())
			{
				T element = (T) iterator.Current;
				success &= internalAdd(element);
			}
			return success;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean removeAll(java.util.Collection<?> c)
		public virtual bool removeAll<T1>(ICollection<T1> c)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<?> iterator = (java.util.Iterator<?>) c.iterator();
			IEnumerator<?> iterator = (IEnumerator<?>) c.GetEnumerator();
			bool success = true;
			while (iterator.MoveNext())
			{
				T element = (T) iterator.Current;
				success &= internalRemove(element);
			}
			return success;
		}

		public virtual void clear()
		{
			nest.cat(JOhmUtils.getId(owner)).cat(field.Name).del();
		}

		private bool internalAdd(T element)
		{
			bool success = false;
			if (element != null)
			{
				if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					success = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).sadd(element.ToString()) > 0;
				}
				else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					success = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).sadd(JOhmUtils.getId(element).ToString()) > 0;
				}
				indexValue(element);
			}
			return success;
		}

		private bool internalRemove(T element)
		{
			bool success = false;
			if (element != null)
			{
				if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					success = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).srem(element.ToString()) > 0;
				}
				else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					success = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).srem(JOhmUtils.getId(element).ToString()) > 0;
				}
				unindexValue(element);
			}
			return success;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private synchronized java.util.Set<T> scrollElements()
		private HashSet<T> scrollElements()
		{
			lock (this)
			{
				HashSet<string> keys = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).smembers();
				HashSet<T> elements = new HashSet<T>();
				foreach (string key in keys)
				{
					if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
					{
						elements.Add((T) JOhmUtils.Convertor.convert(elementClazz, key));
					}
					else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
					{
						elements.Add((T) JOhm.get(elementClazz, Convert.ToInt32(key)));
					}
				}
				return elements;
			}
		}
	}
}