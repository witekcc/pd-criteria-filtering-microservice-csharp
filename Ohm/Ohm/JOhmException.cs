using System;

namespace redis.clients.johm
{

	public class JOhmException : Exception
	{

		public JOhmException(Exception e) : base(e)
		{
		}

		public JOhmException(string message) : base(message)
		{
		}

		/// 
		private const long serialVersionUID = 5824673432789607128L;

	}

}