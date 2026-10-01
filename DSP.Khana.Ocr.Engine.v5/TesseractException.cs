
using System;
using System.Runtime.Serialization;

namespace DSP.Khana.Ocr
{
	/// <summary>
	/// Desctiption of KhanaOcrEngineException.
	/// </summary>
	[Serializable]
    internal class KhanaOcrEngineException : Exception, ISerializable
	{
		public KhanaOcrEngineException()
		{
		}

	 	public KhanaOcrEngineException(string message) : base(message)
		{
		}

		public KhanaOcrEngineException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected KhanaOcrEngineException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
