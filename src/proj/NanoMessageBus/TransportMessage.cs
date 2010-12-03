namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class TransportMessage
	{
		[DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
		private readonly Guid messageId;

		[DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
		private readonly string returnAddress;

		[IgnoreDataMember]
		private readonly TimeSpan timeToLive;

		[IgnoreDataMember]
		private readonly bool persistent;

		[DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
		private readonly IDictionary<string, string> headers;

		[DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
		private readonly ICollection<object> logicalMessages;

		protected TransportMessage()
		{
		}
		public TransportMessage(
			Guid messageId,
			string returnAddress,
			TimeSpan timeToLive,
			bool persistent,
			IDictionary<string, string> headers,
			ICollection<object> logicalMessages)
		{
			this.messageId = messageId;
			this.returnAddress = returnAddress;
			this.timeToLive = timeToLive;
			this.persistent = persistent;
			this.headers = headers ?? new Dictionary<string, string>();
			this.logicalMessages = logicalMessages;
		}

		public Guid MessageId
		{
			get { return this.messageId; }
		}
		public string ReturnAddress
		{
			get { return this.returnAddress; }
		}
		public TimeSpan TimeToLive
		{
			get { return this.timeToLive; }
		}
		public bool Persistent
		{
			get { return this.persistent; }
		}
		public IDictionary<string, string> Headers
		{
			get { return this.headers; }
		}
		public ICollection<object> LogicalMessages
		{
			get { return this.logicalMessages; }
		}
	}
}