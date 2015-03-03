using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace redis.clients.johm
{


	using redis.clients.johm.collections;
using RedisList = redis.clients.johm.collections.RedisList;
using RedisMap = redis.clients.johm.collections.RedisMap;
using RedisSet = redis.clients.johm.collections.RedisSet;
using RedisSortedSet = redis.clients.johm.collections.RedisSortedSet;

	public sealed class JOhmUtils
	{
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static String getReferenceKeyName(final MemberInfo field)
		internal static string getReferenceKeyName(MemberInfo field)
		{
            
			return field.Name + "_id";
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static Long getId(final Object model)
		public static long? getId(object model)
		{
			return getId(model, true);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static Long getId(final Object model, boolean checkValidity)
		public static long? getId(object model, bool checkValidity)
		{
			long? id = null;
			if (model != null)
			{
				if (checkValidity)
				{
					Validator.checkValidModel(model);
				}
				id = Validator.checkValidId(model);
			}
			return id;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static boolean isNew(final Object model)
		internal static bool isNew(object model)
		{
			return getId(model) == null;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static void initCollections(final Object model, final Nest<?> nest)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		internal static void initCollections<T1>(object model, Nest<T1> nest)
		{
			if (model == null || nest == null)
			{
				return;
			}
			foreach (PropertyInfo member in model.GetType().GetMembers())
			{
				
				try
				{
                    if(member.GetCustomAttribute<CollectionList>() != null )
                    {
                        Validator.checkValidCollection(member);
						IList<object> list = (IList<object>) member.GetValue(model);
						
                        if (list == null)
						{
							CollectionList annotation = member.GetCustomAttribute<CollectionList>();
							RedisList<object> redisList = new RedisList<object>(annotation.of(), nest, member, model);
							member.set(model, redisList);
						}
                    }

					
					if (member.isAnnotationPresent(typeof(CollectionSet)))
					{
						Validator.checkValidCollection(member);
						HashSet<object> set = (HashSet<object>) member.get(model);
						if (set == null)
						{
							CollectionSet annotation = member.getAnnotation(typeof(CollectionSet));
							RedisSet<object> redisSet = new RedisSet<object>(annotation.of(), nest, member, model);
							member.set(model, redisSet);
						}
					}
					if (member.isAnnotationPresent(typeof(CollectionSortedSet)))
					{
						Validator.checkValidCollection(member);
						HashSet<object> sortedSet = (HashSet<object>) member.get(model);
						if (sortedSet == null)
						{
							CollectionSortedSet annotation = member.getAnnotation(typeof(CollectionSortedSet));
							RedisSortedSet<object> redisSortedSet = new RedisSortedSet<object>(annotation.of(), annotation.by(), nest, member, model);
							member.set(model, redisSortedSet);
						}
					}
					if (member.isAnnotationPresent(typeof(CollectionMap)))
					{
						Validator.checkValidCollection(member);
						IDictionary<object, object> map = (IDictionary<object, object>) member.get(model);
						if (map == null)
						{
							CollectionMap annotation = member.getAnnotation(typeof(CollectionMap));
							RedisMap<object, object> redisMap = new RedisMap<object, object>(annotation.key(), annotation.value(), nest, member, model);
							member.set(model, redisMap);
						}
					}
				}
				catch (System.ArgumentException)
				{
					throw new InvalidFieldException();
				}
				catch (IllegalAccessException)
				{
					throw new InvalidFieldException();
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void loadId(final Object model, final Long id)
		internal static void loadId(object model, long? id)
		{
			if (model != null)
			{
				bool idFieldPresent = false;
				foreach (MemberInfo field in model.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
				{
					field.Accessible = true;
					if (field.isAnnotationPresent(typeof(Id)))
					{
						idFieldPresent = true;
						Validator.checkValidIdType(field);
						try
						{
							field.set(model, id);
						}
						catch (System.ArgumentException e)
						{
							throw new JOhmException(e);
						}
						catch (IllegalAccessException e)
						{
							throw new JOhmException(e);
						}
						break;
					}
				}
				if (!idFieldPresent)
				{
					throw new JOhmException("JOhm does not support a Model without an Id");
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static boolean detectJOhmCollection(final MemberInfo field)
		internal static bool detectJOhmCollection(MemberInfo field)
		{
			bool isJOhmCollection = false;
			if (field.isAnnotationPresent(typeof(CollectionList)) || field.isAnnotationPresent(typeof(CollectionSet)) || field.isAnnotationPresent(typeof(CollectionSortedSet)) || field.isAnnotationPresent(typeof(CollectionMap)))
			{
				isJOhmCollection = true;
			}
			return isJOhmCollection;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static JOhmCollectionDataType detectJOhmCollectionDataType(final Class<?> dataClazz)
		public static JOhmCollectionDataType detectJOhmCollectionDataType<T1>(Type<T1> dataClazz)
		{
			JOhmCollectionDataType type = null;
			if (Validator.checkSupportedPrimitiveClazz(dataClazz))
			{
				type = JOhmCollectionDataType.PRIMITIVE;
			}
			else
			{
				try
				{
					Validator.checkValidModelClazz(dataClazz);
					type = JOhmCollectionDataType.MODEL;
				}
				catch (JOhmException)
				{
					// drop it
				}
			}

			if (type == null)
			{
				throw new JOhmException(dataClazz.Name + " is not a supported JOhm Collection Data Type");
			}

			return type;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static boolean isNullOrEmpty(final Object obj)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public static bool isNullOrEmpty(object obj)
		{
			if (obj == null)
			{
				return true;
			}
			if (obj.GetType().Equals(typeof(ICollection)))
			{
				return ((ICollection) obj).Count == 0;
			}
			else
			{
				if (obj.ToString().Trim().Length == 0)
				{
					return true;
				}
			}

			return false;
		}

		internal static IList<MemberInfo> gatherAllFields(Type<T1> clazz)
		{
			IList<MemberInfo> allFields = new List<MemberInfo>();
			foreach (MemberInfo field in clazz.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
			{
				allFields.Add(field);
			}
			while ((clazz = clazz.BaseType) != null)
			{
				allFields.AddRange(gatherAllFields(clazz));
			}

			return Collections.unmodifiableList(allFields);
		}

		public sealed class JOhmCollectionDataType
		{
			public static readonly JOhmCollectionDataType PRIMITIVE, MODEL = new JOhmCollectionDataType("PRIMITIVE, MODEL", InnerEnum.PRIMITIVE, MODEL);

			private static readonly IList<JOhmCollectionDataType> valueList = new List<JOhmCollectionDataType>();

			static JOhmCollectionDataType()
			{
				valueList.Add(PRIMITIVE, MODEL);
			}

			public enum InnerEnum
			{
				PRIMITIVE, MODEL
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			private JOhmCollectionDataType(string name, InnerEnum innerEnum)
			{
				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			public static IList<JOhmCollectionDataType> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static JOhmCollectionDataType valueOf(string name)
			{
				foreach (JOhmCollectionDataType enumInstance in JOhmCollectionDataType.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		public sealed class Convertor
		{
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static Object convert(final MemberInfo field, final String value)
			internal static object convert(MemberInfo field, string value)
			{
				return convert(field.Type, value);
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static Object convert(final Class<?> type, final String value)
			public static object convert<T1>(Type<T1> type, string value)
			{
				if (type.Equals(typeof(sbyte?)) || type.Equals(typeof(sbyte)))
				{
					return Convert.ToSByte(value);
				}
				if (type.Equals(typeof(char?)) || type.Equals(typeof(char)))
				{
					if (!isNullOrEmpty(value))
					{
						if (value.Length > 1)
						{
							throw new System.ArgumentException("Non-character value masquerading as characters in a string");
						}
						return value[0];
					}
					else
					{
						// This is the default value
						return '\u0000';
					}
				}
				if (type.Equals(typeof(short?)) || type.Equals(typeof(short)))
				{
					return Convert.ToInt16(value);
				}
				if (type.Equals(typeof(int?)) || type.Equals(typeof(int)))
				{
					if (value == null)
					{
						return 0;
					}
					return Convert.ToInt32(value);
				}
				if (type.Equals(typeof(float?)) || type.Equals(typeof(float)))
				{
					if (value == null)
					{
						return 0f;
					}
					return Convert.ToSingle(value);
				}
				if (type.Equals(typeof(double?)) || type.Equals(typeof(double)))
				{
					return Convert.ToDouble(value);
				}
				if (type.Equals(typeof(long?)) || type.Equals(typeof(long)))
				{
					return Convert.ToInt64(value);
				}
				if (type.Equals(typeof(bool?)) || type.Equals(typeof(bool)))
				{
					return new bool?(value);
				}

				// Higher precision folks
				if (type.Equals(typeof(decimal)))
				{
					return new decimal(value);
				}
				if (type.Equals(typeof(System.Numerics.BigInteger)))
				{
					return Convert.ToInt64(value);
				}

				if (type.IsEnum || type.Equals(typeof(Enum)))
				{
					// return Enum.valueOf(type, value);
					return null; // TODO: handle these
				}

				// Raw Collections are unsupported
				if (type.Equals(typeof(ICollection)))
				{
					return null;
				}

				// Raw arrays are unsupported
				if (type.IsArray)
				{
					return null;
				}

				return value;
			}
		}

		internal sealed class Validator
		{
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidAttribute(final MemberInfo field)
			internal static void checkValidAttribute(MemberInfo field)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> type = field.getType();
				Type<?> type = field.Type;
				if ((type.Equals(typeof(sbyte?)) || type.Equals(typeof(sbyte))) || type.Equals(typeof(char?)) || type.Equals(typeof(char)) || type.Equals(typeof(short?)) || type.Equals(typeof(short)) || type.Equals(typeof(int?)) || type.Equals(typeof(int)) || type.Equals(typeof(float?)) || type.Equals(typeof(float)) || type.Equals(typeof(double?)) || type.Equals(typeof(double)) || type.Equals(typeof(long?)) || type.Equals(typeof(long)) || type.Equals(typeof(bool?)) || type.Equals(typeof(bool)) || type.Equals(typeof(decimal)) || type.Equals(typeof(System.Numerics.BigInteger)) || type.Equals(typeof(string)))
				{
				}
				else
				{
					throw new JOhmException(field.Type.SimpleName + " is not a JOhm-supported Attribute");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidReference(final MemberInfo field)
			internal static void checkValidReference(MemberInfo field)
			{
				if (!field.Type.GetType().IsInstanceOfType(typeof(Model)))
				{
					throw new JOhmException(field.Type.SimpleName + " is not a subclass of Model");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static Long checkValidId(final Object model)
			internal static long? checkValidId(object model)
			{
				long? id = null;
				bool idFieldPresent = false;
				foreach (MemberInfo field in model.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
				{
					field.Accessible = true;
					if (field.isAnnotationPresent(typeof(Id)))
					{
						Validator.checkValidIdType(field);
						try
						{
							id = (long?) field.get(model);
							idFieldPresent = true;
						}
						catch (System.ArgumentException e)
						{
							throw new JOhmException(e);
						}
						catch (IllegalAccessException e)
						{
							throw new JOhmException(e);
						}
						break;
					}
				}
				if (!idFieldPresent)
				{
					throw new JOhmException("JOhm does not support a Model without an Id");
				}
				return id;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidIdType(final MemberInfo field)
			internal static void checkValidIdType(MemberInfo field)
			{
				Annotation[] annotations = field.GetCustomAttributes(true);
				if (annotations.Length > 1)
				{
					foreach (Annotation annotation in annotations)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> annotationType = annotation.annotationType();
						Type<?> annotationType = annotation.annotationType();
						if (annotationType.Equals(typeof(Id)))
						{
							continue;
						}
						if (JOHM_SUPPORTED_ANNOTATIONS.Contains(annotationType))
						{
							throw new JOhmException("Element annotated @Id cannot have any other JOhm annotations");
						}
					}
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Class<?> type = field.getType().getClass();
				Type<?> type = field.Type.GetType();
				if (!type.IsInstanceOfType(typeof(long?)) || !type.IsInstanceOfType(typeof(long)))
				{
					throw new JOhmException(field.Type.SimpleName + " is annotated an Id but is not a long");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static boolean isIndexable(final String attributeName)
			internal static bool isIndexable(string attributeName)
			{
				// Prevent null/empty keys and null/empty values
				if (!isNullOrEmpty(attributeName))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidModel(final Object model)
			internal static void checkValidModel(object model)
			{
				checkValidModelClazz(model.GetType());
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidModelClazz(final Class<?> modelClazz)
			internal static void checkValidModelClazz<T1>(Type<T1> modelClazz)
			{
				if (!modelClazz.isAnnotationPresent(typeof(Model)))
				{
					throw new JOhmException("Class pretending to be a Model but is not really annotated");
				}
				if (modelClazz.IsInterface)
				{
					throw new JOhmException("An interface cannot be annotated as a Model");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidCollection(final MemberInfo field)
			internal static void checkValidCollection(MemberInfo member)
			{
				bool isList = false, isSet = false, isMap = false, isSortedSet = false;
				if (member.isAnnotationPresent(typeof(CollectionList)))
				{
					checkValidCollectionList(member);
					isList = true;
				}
				if (member.isAnnotationPresent(typeof(CollectionSet)))
				{
					checkValidCollectionSet(member);
					isSet = true;
				}
				if (member.isAnnotationPresent(typeof(CollectionSortedSet)))
				{
					checkValidCollectionSortedSet(member);
					isSortedSet = true;
				}
				if (member.isAnnotationPresent(typeof(CollectionMap)))
				{
					checkValidCollectionMap(member);
					isMap = true;
				}
				if (isList && isSet && isMap && isSortedSet)
				{
					throw new JOhmException(member.Name + " can be declared a List or a Set or a SortedSet or a Map but not more than one type");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidCollectionList(final MemberInfo field)
			internal static void checkValidCollectionList(MemberInfo field)
			{
				if (!field.Type.GetType().IsInstanceOfType(typeof(IList)))
				{
					throw new JOhmException(field.Type.SimpleName + " is not a subclass of List");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidCollectionSet(final MemberInfo field)
			internal static void checkValidCollectionSet(MemberInfo field)
			{
				if (!field.Type.GetType().IsInstanceOfType(typeof(HashSet)))
				{
					throw new JOhmException(field.Type.SimpleName + " is not a subclass of Set");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidCollectionSortedSet(final MemberInfo field)
			internal static void checkValidCollectionSortedSet(MemberInfo field)
			{
				if (!field.Type.GetType().IsInstanceOfType(typeof(HashSet)))
				{
					throw new JOhmException(field.Type.SimpleName + " is not a subclass of Set");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidCollectionMap(final MemberInfo field)
			internal static void checkValidCollectionMap(MemberInfo field)
			{
				if (!field.Type.GetType().IsInstanceOfType(typeof(IDictionary)))
				{
					throw new JOhmException(field.Type.SimpleName + " is not a subclass of Map");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkValidArrayBounds(final MemberInfo field, int actualLength)
			internal static void checkValidArrayBounds(MemberInfo field, int actualLength)
			{
				if (field.getAnnotation(typeof(Array)).length() < actualLength)
				{
					throw new JOhmException(field.Type.SimpleName + " has an actual length greater than the expected annotated array bounds");
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void checkAttributeReferenceIndexRules(final MemberInfo field)
			internal static void checkAttributeReferenceIndexRules(MemberInfo field)
			{
				bool isAttribute = field.isAnnotationPresent(typeof(Attribute));
				bool isReference = field.isAnnotationPresent(typeof(Reference));
				bool isIndexed = field.isAnnotationPresent(typeof(Indexed));
				if (isAttribute)
				{
					if (isReference)
					{
						throw new JOhmException(field.Name + " is both an Attribute and a Reference which is invalid");
					}
					if (isIndexed)
					{
						if (!isIndexable(field.Name))
						{
							throw new InvalidFieldException();
						}
					}
					if (field.Type.Equals(typeof(Model)))
					{
						throw new JOhmException(field.Type.SimpleName + " is an Attribute and a Model which is invalid");
					}
					checkValidAttribute(field);
				}
				if (isReference)
				{
					checkValidReference(field);
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static boolean checkSupportedPrimitiveClazz(final Class<?> primitiveClazz)
			public static bool checkSupportedPrimitiveClazz<T1>(Type<T1> primitiveClazz)
			{
				return JOHM_SUPPORTED_PRIMITIVES.Contains(primitiveClazz);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static final java.util.Set<Class<?>> JOHM_SUPPORTED_PRIMITIVES = new java.util.HashSet<Class<?>>();
		private static readonly HashSet<Type<?>> JOHM_SUPPORTED_PRIMITIVES = new HashSet<Type<?>>();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static final java.util.Set<Class<?>> JOHM_SUPPORTED_ANNOTATIONS = new java.util.HashSet<Class<?>>();
		private static readonly HashSet<Type<?>> JOHM_SUPPORTED_ANNOTATIONS = new HashSet<Type<?>>();
		static JOhmUtils()
		{
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(string));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(sbyte?));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(sbyte));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(char?));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(char));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(short?));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(short));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(int?));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(int));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(float?));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(float));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(double?));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(double));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(long?));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(long));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(bool?));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(bool));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(decimal));
			JOHM_SUPPORTED_PRIMITIVES.Add(typeof(System.Numerics.BigInteger));

			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(Array));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(Attribute));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(CollectionList));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(CollectionMap));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(CollectionSet));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(CollectionSortedSet));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(Id));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(Indexed));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(Model));
			JOHM_SUPPORTED_ANNOTATIONS.Add(typeof(Reference));
		}
	}

}