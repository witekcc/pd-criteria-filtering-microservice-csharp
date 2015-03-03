using System;
using System.Collections.Generic;

namespace redis.clients.johm
{


	using JedisException = redis.clients.jedis.JedisException;
	using JedisPool = redis.clients.jedis.JedisPool;
	using TransactionBlock = redis.clients.jedis.TransactionBlock;
	using RedisArray = redis.clients.johm.collections.RedisArray;

	/// <summary>
	/// JOhm serves as the delegate responsible for heavy-lifting all mapping
	/// operations between the Object Models at one end and Redis Persistence Models
	/// on the other.
	/// </summary>
	public sealed class JOhm
	{
		private static JedisPool jedisPool;

		/// <summary>
		/// Read the id from the given model. This operation will typically be useful
		/// only after an initial interaction with Redis in the form of a call to
		/// save().
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static Long getId(final Object model)
		public static long? getId(object model)
		{
			return JOhmUtils.getId(model);
		}

		/// <summary>
		/// Check if given model is in the new state with an uninitialized id.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static boolean isNew(final Object model)
		public static bool isNew(object model)
		{
			return JOhmUtils.isNew(model);
		}

		/// <summary>
		/// Load the model persisted in Redis looking it up by its id and Class type.
		/// </summary>
		/// @param <T> </param>
		/// <param name="clazz"> </param>
		/// <param name="id">
		/// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T get(Class<?> clazz, long id)
		public static T get<T, T1>(Type<T1> clazz, long id)
		{
			JOhmUtils.Validator.checkValidModelClazz(clazz);

			Nest nest = new Nest(clazz);
			nest.JedisPool = jedisPool;
			if (!nest.cat(id).exists())
			{
				return null;
			}

			object newInstance;
			try
			{
				newInstance = clazz.newInstance();
				JOhmUtils.loadId(newInstance, id);
				JOhmUtils.initCollections(newInstance, nest);

				IDictionary<string, string> hashedObject = nest.cat(id).hgetAll();
				foreach (Field field in JOhmUtils.gatherAllFields(clazz))
				{
					fillField(hashedObject, newInstance, field);
					fillArrayField(nest, newInstance, field);
				}

				return (T) newInstance;
			}
			catch (InstantiationException e)
			{
				throw new JOhmException(e);
			}
			catch (IllegalAccessException e)
			{
				throw new JOhmException(e);
			}
		}

		/// <summary>
		/// Search a Model in redis index using its attribute's given name/value
		/// pair. This can potentially return more than 1 matches if some indexed
		/// Model's have identical attributeValue for given attributeName.
		/// </summary>
		/// <param name="clazz">
		///            Class of Model annotated-type to search </param>
		/// <param name="attributeName">
		///            Name of Model's attribute to search </param>
		/// <param name="attributeValue">
		///            Attribute's value to search in index
		/// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> java.util.List<T> find(Class<?> clazz, String attributeName, Object attributeValue)
		public static IList<T> find<T, T1>(Type<T1> clazz, string attributeName, object attributeValue)
		{
			JOhmUtils.Validator.checkValidModelClazz(clazz);
			IList<object> results = null;
			if (!JOhmUtils.Validator.isIndexable(attributeName))
			{
				throw new InvalidFieldException();
			}

			try
			{
				Field field = clazz.getDeclaredField(attributeName);
				field.Accessible = true;
				if (!field.isAnnotationPresent(typeof(Indexed)))
				{
					throw new InvalidFieldException();
				}
				if (field.isAnnotationPresent(typeof(Reference)))
				{
					attributeName = JOhmUtils.getReferenceKeyName(field);
				}
			}
			catch (SecurityException)
			{
				throw new InvalidFieldException();
			}
			catch (NoSuchFieldException)
			{
				throw new InvalidFieldException();
			}
			if (JOhmUtils.isNullOrEmpty(attributeValue))
			{
				throw new InvalidFieldException();
			}
			Nest nest = new Nest(clazz);
			nest.JedisPool = jedisPool;
			HashSet<string> modelIdStrings = nest.cat(attributeName).cat(attributeValue).smembers();
			if (modelIdStrings != null)
			{
				// TODO: Do this lazy
				results = new List<object>();
				object indexed = null;
				foreach (string modelIdString in modelIdStrings)
				{
					indexed = get(clazz, long.Parse(modelIdString));
					if (indexed != null)
					{
						results.Add(indexed);
					}
				}
			}
			return (IList<T>) results;
		}

		/// <summary>
		/// Save given model to Redis. By default, this does not save all its child
		/// annotated-models. If hierarchical persistence is desirable, use the
		/// overloaded save interface.
		/// </summary>
		/// @param <T> </param>
		/// <param name="model">
		/// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static <T> T save(final Object model)
		public static T save<T>(object model)
		{
			return JOhm.save<T> (model, false);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T save(final Object model, boolean saveChildren)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public static T save<T>(object model, bool saveChildren)
		{
			if (!isNew(model))
			{
				delete(model.GetType(), JOhmUtils.getId(model));
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Nest nest = initIfNeeded(model);
			Nest nest = initIfNeeded(model);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, String> hashedObject = new java.util.HashMap<String, String>();
			IDictionary<string, string> hashedObject = new Dictionary<string, string>();
			IDictionary<RedisArray<object>, Object[]> pendingArraysToPersist = null;
			try
			{
				string fieldName = null;
				foreach (Field field in JOhmUtils.gatherAllFields(model.GetType()))
				{
					field.Accessible = true;
					if (JOhmUtils.detectJOhmCollection(field) || field.isAnnotationPresent(typeof(Id)))
					{
						if (field.isAnnotationPresent(typeof(Id)))
						{
							JOhmUtils.Validator.checkValidIdType(field);
						}
						continue;
					}
					if (field.isAnnotationPresent(typeof(Array)))
					{
						object[] backingArray = (object[]) field.get(model);
						int actualLength = backingArray == null ? 0 : backingArray.Length;
						JOhmUtils.Validator.checkValidArrayBounds(field, actualLength);
						Array annotation = field.getAnnotation(typeof(Array));
						RedisArray<object> redisArray = new RedisArray<object>(annotation.length(), annotation.of(), nest, field, model);
						if (pendingArraysToPersist == null)
						{
							pendingArraysToPersist = new LinkedHashMap<RedisArray<object>, Object[]>();
						}
						pendingArraysToPersist[redisArray] = backingArray;
					}
					JOhmUtils.Validator.checkAttributeReferenceIndexRules(field);
					if (field.isAnnotationPresent(typeof(Attribute)))
					{
						fieldName = field.Name;
						object fieldValueObject = field.get(model);
						if (fieldValueObject != null)
						{
							hashedObject[fieldName] = fieldValueObject.ToString();
						}

					}
					if (field.isAnnotationPresent(typeof(Reference)))
					{
						fieldName = JOhmUtils.getReferenceKeyName(field);
						object child = field.get(model);
						if (child != null)
						{
							if (JOhmUtils.getId(child) == null)
							{
								throw new MissingIdException();
							}
							if (saveChildren)
							{
								save(child, saveChildren); // some more work to do
							}
							hashedObject[fieldName] = Convert.ToString(JOhmUtils.getId(child));
						}
					}
					if (field.isAnnotationPresent(typeof(Indexed)))
					{
						object fieldValue = field.get(model);
						if (fieldValue != null && field.isAnnotationPresent(typeof(Reference)))
						{
							fieldValue = JOhmUtils.getId(fieldValue);
						}
						if (!JOhmUtils.isNullOrEmpty(fieldValue))
						{
							nest.cat(fieldName).cat(fieldValue).sadd(Convert.ToString(JOhmUtils.getId(model)));
						}
					}
					// always add to the all set, to support getAll
					nest.cat("all").sadd(Convert.ToString(JOhmUtils.getId(model)));
				}
			}
			catch (System.ArgumentException e)
			{
				throw new JOhmException(e);
			}
			catch (IllegalAccessException e)
			{
				throw new JOhmException(e);
			}

			nest.multi(new TransactionBlockAnonymousInnerClassHelper(model, nest, hashedObject));

			if (pendingArraysToPersist != null && pendingArraysToPersist.Count > 0)
			{
				foreach (KeyValuePair<RedisArray<object>, Object[]> arrayEntry in pendingArraysToPersist.SetOfKeyValuePairs())
				{
					arrayEntry.Key.write(arrayEntry.Value);
				}
			}

			return (T) model;
		}

		private class TransactionBlockAnonymousInnerClassHelper : TransactionBlock
		{
			private object model;
			private Nest nest;
			private IDictionary<string, string> hashedObject;

			public TransactionBlockAnonymousInnerClassHelper(object model, Nest nest, IDictionary<string, string> hashedObject)
			{
				this.model = model;
				this.nest = nest;
				this.hashedObject = hashedObject;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute() throws redis.clients.jedis.JedisException
			public virtual void execute()
			{
				del(nest.cat(JOhmUtils.getId(model)).key());
				hmset(nest.cat(JOhmUtils.getId(model)).key(), hashedObject);
			}
		}

		/// <summary>
		/// Delete Redis-persisted model as represented by the given model Class type
		/// and id.
		/// </summary>
		/// <param name="clazz"> </param>
		/// <param name="id">
		/// @return </param>
		public static bool delete<T1>(Type<T1> clazz, long id)
		{
			return delete(clazz, id, true, false);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static boolean delete(Class<?> clazz, long id, boolean deleteIndexes, boolean deleteChildren)
		public static bool delete<T1>(Type<T1> clazz, long id, bool deleteIndexes, bool deleteChildren)
		{
			JOhmUtils.Validator.checkValidModelClazz(clazz);
			bool deleted = false;
			object persistedModel = get(clazz, id);
			if (persistedModel != null)
			{
				Nest nest = new Nest(persistedModel);
				nest.JedisPool = jedisPool;
				if (deleteIndexes)
				{
					// think about promoting deleteChildren as default behavior so
					// that this field lookup gets folded into that
					// if-deleteChildren block
					foreach (Field field in JOhmUtils.gatherAllFields(clazz))
					{
						if (field.isAnnotationPresent(typeof(Indexed)))
						{
							field.Accessible = true;
							object fieldValue = null;
							try
							{
								fieldValue = field.get(persistedModel);
							}
							catch (System.ArgumentException e)
							{
								throw new JOhmException(e);
							}
							catch (IllegalAccessException e)
							{
								throw new JOhmException(e);
							}
							if (fieldValue != null && field.isAnnotationPresent(typeof(Reference)))
							{
								fieldValue = JOhmUtils.getId(fieldValue);
							}
							if (!JOhmUtils.isNullOrEmpty(fieldValue))
							{
								nest.cat(field.Name).cat(fieldValue).srem(Convert.ToString(id));
							}
						}
					}
				}
				if (deleteChildren)
				{
					foreach (Field field in JOhmUtils.gatherAllFields(clazz))
					{
						if (field.isAnnotationPresent(typeof(Reference)))
						{
							field.Accessible = true;
							try
							{
								object child = field.get(persistedModel);
								if (child != null)
								{
									delete(child.GetType(), JOhmUtils.getId(child), deleteIndexes, deleteChildren); // children
								}
							}
							catch (System.ArgumentException e)
							{
								throw new JOhmException(e);
							}
							catch (IllegalAccessException e)
							{
								throw new JOhmException(e);
							}
						}
						if (field.isAnnotationPresent(typeof(Array)))
						{
							field.Accessible = true;
							Array annotation = field.getAnnotation(typeof(Array));
							RedisArray redisArray = new RedisArray(annotation.length(), annotation.of(), nest, field, persistedModel);
							redisArray.clear();
						}
					}
				}

				// now delete parent
				deleted = nest.cat(id).del() == 1;
			}
			return deleted;
		}

		/// <summary>
		/// Inject JedisPool into JOhm. This is a mandatory JOhm setup operation.
		/// </summary>
		/// <param name="jedisPool"> </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static void setPool(final redis.clients.jedis.JedisPool jedisPool)
		public static JedisPool Pool
		{
			set
			{
				JOhm.jedisPool = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void fillField(final java.util.Map<String, String> hashedObject, final Object newInstance, final Field field) throws IllegalAccessException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		private static void fillField(IDictionary<string, string> hashedObject, object newInstance, Field field)
		{
			JOhmUtils.Validator.checkAttributeReferenceIndexRules(field);
			if (field.isAnnotationPresent(typeof(Attribute)))
			{
				field.Accessible = true;
				field.set(newInstance, JOhmUtils.Convertor.convert(field, hashedObject[field.Name]));
			}
			if (field.isAnnotationPresent(typeof(Reference)))
			{
				field.Accessible = true;
				string serializedReferenceId = hashedObject[JOhmUtils.getReferenceKeyName(field)];
				if (serializedReferenceId != null)
				{
					long? referenceId = Convert.ToInt64(serializedReferenceId);
					field.set(newInstance, get(field.Type, referenceId.Value));
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static void fillArrayField(final Nest nest, final Object model, final Field field) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		private static void fillArrayField(Nest nest, object model, Field field)
		{
			if (field.isAnnotationPresent(typeof(Array)))
			{
				field.Accessible = true;
				Array annotation = field.getAnnotation(typeof(Array));
				RedisArray redisArray = new RedisArray(annotation.length(), annotation.of(), nest, field, model);
				field.set(model, redisArray.read());
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static Nest initIfNeeded(final Object model)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		private static Nest initIfNeeded(object model)
		{
			long? id = JOhmUtils.getId(model);
			Nest nest = new Nest(model);
			nest.JedisPool = jedisPool;
			if (id == null)
			{
				// lazily initialize id, nest, collections
				id = nest.cat("id").incr();
				JOhmUtils.loadId(model, id);
				JOhmUtils.initCollections(model, nest);
			}
			return nest;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> java.util.Set<T> getAll(Class<?> clazz)
		public static HashSet<T> getAll<T, T1>(Type<T1> clazz)
		{
			JOhmUtils.Validator.checkValidModelClazz(clazz);
			HashSet<object> results = null;
			Nest nest = new Nest(clazz);
			nest.JedisPool = jedisPool;
			HashSet<string> modelIdStrings = nest.cat("all").smembers();
			if (modelIdStrings != null)
			{
				results = new HashSet<object>();
				object indexed = null;
				foreach (string modelIdString in modelIdStrings)
				{
					indexed = get(clazz, long.Parse(modelIdString));
					if (indexed != null)
					{
						results.Add(indexed);
					}
				}
			}
			return (HashSet<T>) results;
		}
	}

}