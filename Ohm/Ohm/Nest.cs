using System;
using System.Collections.Generic;
using System.Text;

namespace redis.clients.johm
{
    
	using Jedis = redis.clients.jedis.Jedis;
	using JedisPool = redis.clients.jedis.JedisPool;
	using TransactionBlock = redis.clients.jedis.TransactionBlock;

	public class Nest<T>
	{
		private const string COLON = ":";
		private StringBuilder sb;
		private string key_Renamed;
		private JedisPool jedisPool;

		public virtual JedisPool JedisPool
		{
			set
			{
				this.jedisPool = value;
				checkRedisLiveness();
			}
		}

		public virtual Nest<T> fork()
		{
			return new Nest<T>(key());
		}

		public Nest()
		{
			this.key_Renamed = "";
		}

		public Nest(string key)
		{
			this.key_Renamed = key;
		}

		public Nest(Type clazz)
		{
			this.key_Renamed = clazz.Name;
		}

		public Nest(T model)
		{
			this.key_Renamed = model.GetType().Name;
		}

		public virtual string key()
		{
			prefix();
			string generatedKey = sb.ToString();
			generatedKey = generatedKey.Substring(0, generatedKey.Length - 1);
			sb = null;
			return generatedKey;
		}

		private void prefix()
		{
			if (sb == null)
			{
				sb = new StringBuilder();
				sb.Append(key_Renamed);
				sb.Append(COLON);
			}
		}

		public virtual Nest<T> cat(int id)
		{
			prefix();
			sb.Append(id);
			sb.Append(COLON);
			return this;
		}

		public virtual Nest<T> cat(object field)
		{
			prefix();
			sb.Append(field);
			sb.Append(COLON);
			return this;
		}

		public virtual Nest<T> cat(string field)
		{
			prefix();
			sb.Append(field);
			sb.Append(COLON);
			return this;
		}

		// Redis Common Operations
		public virtual string set(string value)
		{
			Jedis jedis = Resource;
			string set = jedis.set(key(), value);
			returnResource(jedis);
			return set;
		}

		public virtual string get()
		{
			Jedis jedis = Resource;
			string @string = jedis.get(key());
			returnResource(jedis);
			return @string;
		}

		public virtual long? incr()
		{
			Jedis jedis = Resource;
			long? incr = jedis.incr(key());
			returnResource(jedis);
			return incr;
		}

		public virtual IList<object> multi(TransactionBlock transaction)
		{
			Jedis jedis = Resource;
			IList<object> multi = jedis.multi(transaction);
			returnResource(jedis);
			return multi;
		}

		public virtual long? del()
		{
			Jedis jedis = Resource;
			long? del = jedis.del(key());
			returnResource(jedis);
			return del;
		}

		public virtual bool? exists()
		{
			Jedis jedis = Resource;
			bool? exists = jedis.exists(key());
			returnResource(jedis);
			return exists;
		}

		// Redis Hash Operations
		public virtual string hmset(IDictionary<string, string> hash)
		{
			Jedis jedis = Resource;
			string hmset = jedis.hmset(key(), hash);
			returnResource(jedis);
			return hmset;
		}

		public virtual IDictionary<string, string> hgetAll()
		{
			Jedis jedis = Resource;
			IDictionary<string, string> hgetAll = jedis.hgetAll(key());
			returnResource(jedis);
			return hgetAll;
		}

		public virtual string hget(string field)
		{
			Jedis jedis = Resource;
			string value = jedis.hget(key(), field);
			returnResource(jedis);
			return value;
		}

		public virtual long? hdel(string field)
		{
			Jedis jedis = Resource;
			long? hdel = jedis.hdel(key(), field);
			returnResource(jedis);
			return hdel;
		}

		public virtual long? hlen()
		{
			Jedis jedis = Resource;
			long? hlen = jedis.hlen(key());
			returnResource(jedis);
			return hlen;
		}

		public virtual HashSet<string> hkeys()
		{
			Jedis jedis = Resource;
			HashSet<string> hkeys = jedis.hkeys(key());
			returnResource(jedis);
			return hkeys;
		}

		// Redis Set Operations
		public virtual long? sadd(string member)
		{
			Jedis jedis = Resource;
			long? reply = jedis.sadd(key(), member);
			returnResource(jedis);
			return reply;
		}

		public virtual long? srem(string member)
		{
			Jedis jedis = Resource;
			long? reply = jedis.srem(key(), member);
			returnResource(jedis);
			return reply;
		}

		public virtual HashSet<string> smembers()
		{
			Jedis jedis = Resource;
			HashSet<string> members = jedis.smembers(key());
			returnResource(jedis);
			return members;
		}

		// Redis List Operations
		public virtual long? rpush(string @string)
		{
			Jedis jedis = Resource;
			long? rpush = jedis.rpush(key(), @string);
			returnResource(jedis);
			return rpush;
		}

		public virtual string lset(int index, string value)
		{
			Jedis jedis = Resource;
			string lset = jedis.lset(key(), index, value);
			returnResource(jedis);
			return lset;
		}

		public virtual string lindex(int index)
		{
			Jedis jedis = Resource;
			string lindex = jedis.lindex(key(), index);
			returnResource(jedis);
			return lindex;
		}

		public virtual long? llen()
		{
			Jedis jedis = Resource;
			long? llen = jedis.llen(key());
			returnResource(jedis);
			return llen;
		}

		public virtual long? lrem(int count, string value)
		{
			Jedis jedis = Resource;
			long? lrem = jedis.lrem(key(), count, value);
			returnResource(jedis);
			return lrem;
		}

		public virtual IList<string> lrange(int start, int end)
		{
			Jedis jedis = Resource;
			IList<string> lrange = jedis.lrange(key(), start, end);
			returnResource(jedis);
			return lrange;
		}

		// Redis SortedSet Operations
		public virtual HashSet<string> zrange(int start, int end)
		{
			Jedis jedis = Resource;
			HashSet<string> zrange = jedis.zrange(key(), start, end);
			returnResource(jedis);
			return zrange;
		}

		public virtual long? zadd(float score, string member)
		{
			Jedis jedis = Resource;
			long? zadd = jedis.zadd(key(), score, member);
			returnResource(jedis);
			return zadd;
		}

		public virtual long? zcard()
		{
			Jedis jedis = Resource;
			long? zadd = jedis.zcard(key());
			returnResource(jedis);
			return zadd;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void returnResource(final redis.clients.jedis.Jedis jedis)
		private void returnResource(Jedis jedis)
		{
			jedisPool.returnResource(jedis);
		}

		private Jedis Resource
		{
			get
			{
				Jedis jedis;
				jedis = jedisPool.Resource;
				return jedis;
			}
		}

		private void checkRedisLiveness()
		{
			if (jedisPool == null)
			{
				throw new JOhmException("JOhm will fail to do most useful tasks without Redis");
			}
		}
	}

}