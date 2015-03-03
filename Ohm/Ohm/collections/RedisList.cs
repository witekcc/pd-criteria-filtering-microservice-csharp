using System;
using System.Collections.Generic;

namespace redis.clients.johm.collections
{


	using System.Reflection;
using Convertor = redis.clients.johm.JOhmUtils.Convertor;
using JOhmCollectionDataType = redis.clients.johm.JOhmUtils.JOhmCollectionDataType;

	/// <summary>
	/// RedisList is a JOhm-internal List implementation to serve as a proxy for the
	/// Redis persisted list and provide lazy-loading semantics to minimize datastore
	/// network traffic. It does a best-effort job of minimizing list entity
	/// staleness but does so without any locking and is not thread-safe. Only add
	/// and remove operations trigger a remote-sync of local internal storage.
	/// 
	/// RedisList does not support null elements.
	/// </summary>
	public class RedisList<T> : IList<T>
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final redis.clients.johm.Nest<? extends T> nest;
		private readonly Nest<object> nest;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Class<? extends T> elementClazz;
		private readonly Type elementClazz;
		private readonly JOhmUtils.JOhmCollectionDataType johmElementType;
		private readonly PropertyInfo field;
		private readonly object owner;

		public RedisList<T>(Type clazz, Nest<T> nest, PropertyInfo field, object owner) 
		{
			this.elementClazz = clazz;
			johmElementType = JOhmUtils.detectJOhmCollectionDataType(clazz);
			this.nest = nest;
			this.field = field;
			this.owner = owner;
		}

		public virtual bool add(T e)
		{
			return internalAdd(e);
		}

		public virtual void Insert(int index, T element)
		{
			internalIndexedAdd(index, element);
		}

		public virtual bool addAll<T1>(ICollection<T1> c) where T1 : T
		{
			bool success = true;
			foreach (T element in c)
			{
				success &= internalAdd(element);
			}
			return success;
		}

		public virtual bool addAll<T1>(int index, ICollection<T1> c) where T1 : T
		{
			foreach (T element in c)
			{
				internalIndexedAdd(index++, element);
			}
			return true;
		}

		public virtual void Clear()
		{
			nest.cat(JOhmUtils.getId(owner)).cat(field.Name).del();
		}

		public virtual bool Contains(object o)
		{
			return scrollElements().Contains(o);
		}

		public virtual bool containsAll<T1>(ICollection<T1> c)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			return scrollElements().containsAll(c);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T get(int index)
		public virtual T this[int index]
		{
			get
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
		}

		public virtual int IndexOf(object o)
		{
			return scrollElements().IndexOf(o);
		}

		public virtual bool Empty
		{
			get
			{
				return this.Count == 0;
			}
		}

		public virtual IEnumerator<T> GetEnumerator()
		{
			return scrollElements().GetEnumerator();
		}

		public virtual int LastIndexOf(object o)
		{
			return scrollElements().LastIndexOf(o);
		}

		public virtual IEnumerator<T> listIterator()
		{
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
			return scrollElements().GetEnumerator();
		}

		public virtual IEnumerator<T> listIterator(int index)
		{
			return scrollElements().listIterator(index);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean remove(Object o)
		public virtual bool Remove(object o)
		{
			return internalRemove((T) o);
		}

		public virtual T remove(int index)
		{
			return internalIndexedRemove(index);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean removeAll(java.util.Collection<?> c)
		public virtual bool removeAll<T1>(ICollection<T1> c)
		{
			bool success = true;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<?> iterator = (java.util.Iterator<?>) c.iterator();
			IEnumerator<?> iterator = (IEnumerator<?>) c.GetEnumerator();
			while (iterator.MoveNext())
			{
				T element = (T) iterator.Current;
				success &= internalRemove(element);
			}
			return success;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean retainAll(java.util.Collection<?> c)
		public virtual bool retainAll<T1>(ICollection<T1> c)
		{
			this.Clear();
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

		public virtual T setJavaToDotNetIndexer(int index, T element)
		{
			T previousElement = this[index];
			internalIndexedAdd(index, element);
			return previousElement;
		}

		public virtual int Count
		{
			get
			{
				return (int)nest.cat(JOhmUtils.getId(owner)).cat(field.Name).llen();
			}
		}

		public virtual IList<T> subList(int fromIndex, int toIndex)
		{
			return scrollElements().subList(fromIndex, toIndex);
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

		private bool internalAdd(T element)
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
				indexValue(element);
			}
			return success;
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

		private void internalIndexedAdd(int index, T element)
		{
			if (element != null)
			{
				if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					nest.cat(JOhmUtils.getId(owner)).cat(field.Name).lset(index, element.ToString());
				}
				else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					nest.cat(JOhmUtils.getId(owner)).cat(field.Name).lset(index, JOhmUtils.getId(element).ToString());
				}
				indexValue(element);
			}
		}

		private bool internalRemove(T element)
		{
			bool success = false;
			if (element != null)
			{
				long? lrem = 0L;
				if (johmElementType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					lrem = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).lrem(1, element.ToString());
				}
				else if (johmElementType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					lrem = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).lrem(1, JOhmUtils.getId(element).ToString());
				}
				unindexValue(element);
				success = lrem > 0L;
			}
			return success;
		}

		private T internalIndexedRemove(int index)
		{
			T element = this[index];
			internalRemove(element);
			return element;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private synchronized java.util.List<T> scrollElements()
		private IList<T> scrollElements()
		{
			lock (this)
			{
				IList<T> elements = new List<T>();
        
				IList<string> keys = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).lrange(0, -1);
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