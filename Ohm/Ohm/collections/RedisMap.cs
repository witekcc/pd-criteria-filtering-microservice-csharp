using System;
using System.Collections.Generic;

namespace redis.clients.johm.collections
{


	using Convertor = redis.clients.johm.JOhmUtils.Convertor;
	using JOhmCollectionDataType = redis.clients.johm.JOhmUtils.JOhmCollectionDataType;

	/// <summary>
	/// RedisMap is a JOhm-internal Map implementation to serve as a proxy for the
	/// Redis persisted hash and provide lazy-loading semantics to minimize datastore
	/// network traffic. It does a best-effort job of minimizing hash staleness but
	/// does so without any locking and is not thread-safe. Only add and remove
	/// operations trigger a remote-sync of local internal storage.
	/// 
	/// RedisMap does not support null keys or values.
	/// </summary>
	public class RedisMap<K, V> : IDictionary<K, V>
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final redis.clients.johm.Nest<? extends V> nest;
		private readonly Nest<?> nest;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Class<? extends K> keyClazz;
		private readonly Type<?> keyClazz;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Class<? extends V> valueClazz;
		private readonly Type<?> valueClazz;
		private readonly JOhmUtils.JOhmCollectionDataType johmKeyType;
		private readonly JOhmUtils.JOhmCollectionDataType johmValueType;
		private readonly Field field;
		private readonly object owner;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public RedisMap(final Class<? extends K> keyClazz, final Class<? extends V> valueClazz, final redis.clients.johm.Nest<? extends V> nest, Field field, Object owner)
		public RedisMap<T1, T2, T3>(Type<T1> keyClazz, Type<T2> valueClazz, Nest<T3> nest, Field field, object owner) where T1 : K where T2 : V where T3 : V
		{
			this.keyClazz = keyClazz;
			this.valueClazz = valueClazz;
			johmKeyType = JOhmUtils.detectJOhmCollectionDataType(keyClazz);
			johmValueType = JOhmUtils.detectJOhmCollectionDataType(valueClazz);
			this.nest = nest;
			this.field = field;
			this.owner = owner;
		}

		private void indexValue(K element)
		{
			if (field.isAnnotationPresent(typeof(Indexed)))
			{
				if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					nest.cat(field.Name).cat(element).sadd(JOhmUtils.getId(owner).ToString());
				}
				else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					nest.cat(field.Name).cat(JOhmUtils.getId(element)).sadd(JOhmUtils.getId(owner).ToString());
				}
			}
		}

		private void unindexValue(K element)
		{
			if (field.isAnnotationPresent(typeof(Indexed)))
			{
				if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					nest.cat(field.Name).cat(element).srem(JOhmUtils.getId(owner).ToString());
				}
				else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					nest.cat(field.Name).cat(JOhmUtils.getId(element)).srem(JOhmUtils.getId(owner).ToString());
				}
			}
		}

		public virtual void Clear()
		{
			IDictionary<string, string> savedHash = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hgetAll();
			foreach (KeyValuePair<string, string> entry in savedHash.SetOfKeyValuePairs())
			{
				nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hdel(entry.Key);
			}
			nest.cat(JOhmUtils.getId(owner)).cat(field.Name).del();
		}

		public virtual bool ContainsKey(object key)
		{
			return scrollElements().ContainsKey(key);
		}

		public virtual bool containsValue(object value)
		{
			return scrollElements().ContainsValue(value);
		}

		public virtual HashSet<KeyValuePair<K, V>> entrySet()
		{
			return scrollElements().SetOfKeyValuePairs();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V get(Object key)
		public virtual V get(object key)
		{
			V value = null;
			string valueKey = null;
			if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
			{
				valueKey = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hget(key.ToString());
			}
			else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL)
			{
				valueKey = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hget(JOhmUtils.getId(key).ToString());
			}

			if (!JOhmUtils.isNullOrEmpty(valueKey))
			{
				if (johmValueType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					value = (V) JOhmUtils.Convertor.convert(valueClazz, valueKey);
				}
				else if (johmValueType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					value = JOhm.get<V> (valueClazz, int.Parse(valueKey));
				}
			}
			return value;
		}

		public virtual bool Empty
		{
			get
			{
				return this.Count == 0;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Set<K> keySet()
		public virtual HashSet<K> keySet()
		{
			HashSet<K> keys = new LinkedHashSet<K>();
			foreach (string key in nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hkeys())
			{
				if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
				{
					keys.Add((K) JOhmUtils.Convertor.convert(keyClazz, key));
				}
				else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL)
				{
					keys.Add(JOhm.get<K> (keyClazz, int.Parse(key)));
				}
			}
			return keys;
		}

		public virtual V put(K key, V value)
		{
			V previousValue = get(key);
			internalPut(key, value);
			return previousValue;
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(java.util.Map<? extends K, ? extends V> mapToCopyIn)
		public virtual void putAll<T1>(IDictionary<T1> mapToCopyIn) where T1 : K where ? : V
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Map.Entry<? extends K, ? extends V> entry : mapToCopyIn.entrySet())
			foreach (KeyValuePair<?, ?> entry in mapToCopyIn.SetOfKeyValuePairs())
			{
				internalPut(entry.Key, entry.Value);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V remove(Object key)
		public virtual V remove(object key)
		{
			V value = get(key);
			if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
			{
				nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hdel(key.ToString());
			}
			else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL)
			{
				nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hdel(JOhmUtils.getId(key).ToString());
			}
			unindexValue((K) key);
			return value;
		}

		public virtual int Count
		{
			get
			{
				return (int)nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hlen();
			}
		}

		public virtual ICollection<V> Values
		{
			get
			{
				return scrollElements().Values;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private V internalPut(final K key, final V value)
		private V internalPut(K key, V value)
		{
			IDictionary<string, string> hash = new LinkedHashMap<string, string>();
			string keyString = null;
			string valueString = null;
			if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE && johmValueType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
			{
				keyString = key.ToString();
				valueString = value.ToString();
			}
			else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE && johmValueType == JOhmUtils.JOhmCollectionDataType.MODEL)
			{
				keyString = key.ToString();
				valueString = JOhmUtils.getId(value).ToString();
			}
			else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL && johmValueType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
			{
				keyString = JOhmUtils.getId(key).ToString();
				valueString = value.ToString();
			}
			else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL && johmValueType == JOhmUtils.JOhmCollectionDataType.MODEL)
			{
				keyString = JOhmUtils.getId(key).ToString();
				valueString = JOhmUtils.getId(value).ToString();
			}

			hash[keyString] = valueString;
			nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hmset(hash);
			indexValue(key);
			return value;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private synchronized java.util.Map<K, V> scrollElements()
		private IDictionary<K, V> scrollElements()
		{
			lock (this)
			{
				IDictionary<string, string> savedHash = nest.cat(JOhmUtils.getId(owner)).cat(field.Name).hgetAll();
				IDictionary<K, V> backingMap = new Dictionary<K, V>();
				K savedKey = null;
				V savedValue = null;
				foreach (KeyValuePair<string, string> entry in savedHash.SetOfKeyValuePairs())
				{
					if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE && johmValueType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
					{
						savedKey = (K) JOhmUtils.Convertor.convert(keyClazz, entry.Key);
						savedValue = (V) JOhmUtils.Convertor.convert(valueClazz, entry.Value);
					}
					else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE && johmValueType == JOhmUtils.JOhmCollectionDataType.MODEL)
					{
						savedKey = (K) JOhmUtils.Convertor.convert(keyClazz, entry.Key);
						savedValue = JOhm.get<V> (valueClazz, int.Parse(entry.Value));
					}
					else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL && johmValueType == JOhmUtils.JOhmCollectionDataType.PRIMITIVE)
					{
						savedKey = JOhm.get<K> (keyClazz, int.Parse(entry.Key));
						savedValue = (V) JOhmUtils.Convertor.convert(valueClazz, entry.Value);
					}
					else if (johmKeyType == JOhmUtils.JOhmCollectionDataType.MODEL && johmValueType == JOhmUtils.JOhmCollectionDataType.MODEL)
					{
						savedKey = JOhm.get<K> (keyClazz, int.Parse(entry.Key));
						savedValue = JOhm.get<V> (valueClazz, int.Parse(entry.Value));
					}
        
					backingMap[savedKey] = savedValue;
				}
        
				return backingMap;
			}
		}
	}

}