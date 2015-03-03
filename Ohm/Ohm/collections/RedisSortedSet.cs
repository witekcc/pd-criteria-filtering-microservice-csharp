using System;
using System.Collections.Generic;

namespace redis.clients.johm.collections
{



	/// <summary>
	/// RedisSortedSet is a JOhm-internal SortedSet implementation to serve as a
	/// proxy for the Redis persisted sorted set.
	/// </summary>
	public class RedisSortedSet<T> : HashSet<T>
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final redis.clients.johm.Nest<? extends T> nest;
		private readonly Nest<?> nest;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Class<? extends T> clazz;
		private readonly Type<?> clazz;
		private readonly Field field;
		private readonly object owner;
		private readonly string byFieldName;

		public RedisSortedSet<T1, T2>(Type<T1> clazz, string byField, Nest<T2> nest, Field field, object owner) where T1 : T where T2 : T
		{
			this.clazz = clazz;
			this.nest = nest;
			this.field = field;
			this.owner = owner;
			this.byFieldName = byField;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private synchronized java.util.Set<T> scrollElements()
		private HashSet<T> scrollElements()
		{
			lock (this)
			{
				HashSet<string> ids = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).zrange(0, -1);
				HashSet<T> elements = new LinkedHashSet<T>();
				foreach (string id in ids)
				{
					elements.Add((T) JOhm.get(clazz, Convert.ToInt32(id)));
				}
				return elements;
			}
		}

		private void indexValue(T element)
		{
			if (field.isAnnotationPresent(typeof(Indexed)))
			{
				nest.cat(field.Name).cat(JOhmUtils.getId(element)).sadd(JOhmUtils.getId(owner).ToString());
			}
		}

		private void unindexValue(T element)
		{
			if (field.isAnnotationPresent(typeof(Indexed)))
			{
				nest.cat(field.Name).cat(JOhmUtils.getId(element)).srem(JOhmUtils.getId(owner).ToString());
			}
		}

		private bool internalAdd(T element)
		{
			bool success = false;
			if (element != null)
			{
				try
				{
					Field byField = element.GetType().getDeclaredField(byFieldName);
					byField.Accessible = true;
					object fieldValue = byField.get(element);
					if (fieldValue == null)
					{
						fieldValue = 0f;
					}
					success = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).zadd(typeof(float?).cast(fieldValue), JOhmUtils.getId(element).ToString()) > 0;
					indexValue(element);
				}
				catch (SecurityException e)
				{
					throw new JOhmException(e);
				}
				catch (System.ArgumentException e)
				{
					throw new JOhmException(e);
				}
				catch (IllegalAccessException e)
				{
					throw new JOhmException(e);
				}
				catch (NoSuchFieldException e)
				{
					throw new JOhmException(e);
				}
			}
			return success;
		}

		private bool internalRemove(T element)
		{
			bool success = false;
			if (element != null)
			{
				success = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).srem(JOhmUtils.getId(element).ToString()) > 0;
				unindexValue(element);
			}
			return success;
		}

		public virtual bool add(T e)
		{
			return internalAdd(e);
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

		public virtual void clear()
		{
			nest.cat(JOhmUtils.getId(owner)).cat(field.Name).del();
		}

		public virtual bool contains(object o)
		{
			return scrollElements().Contains(o);
		}

		public virtual bool containsAll<T1>(ICollection<T1> c)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			return scrollElements().containsAll(c);
		}

		public virtual bool Empty
		{
			get
			{
				return this.size() == 0;
			}
		}

		public virtual IEnumerator<T> iterator()
		{
			return scrollElements().GetEnumerator();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean remove(Object o)
		public virtual bool remove(object o)
		{
			return internalRemove((T) o);
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

		public virtual int size()
		{
			return (int)nest.cat(JOhmUtils.getId(owner)).cat(field.Name).zcard();
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
	}
}