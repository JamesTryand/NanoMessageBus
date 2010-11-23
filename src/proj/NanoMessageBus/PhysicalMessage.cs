namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class PhysicalMessage
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		private readonly Guid messageId;

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		private readonly int index;

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		private readonly int count;

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		private readonly string returnAddress;

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		private readonly DateTime expiration;

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		private readonly bool durable;

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		private readonly IDictionary<string, string> headers;

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		private readonly ICollection<object> logicalMessages;

		public PhysicalMessage(
			Guid messageId,
			int index,
			int count,
			string returnAddress,
			DateTime expiration,
			bool durable,
			IDictionary<string, string> headers,
			ICollection<object> logicalMessages)
		{
			this.messageId = messageId;
			this.index = index;
			this.count = count;
			this.returnAddress = returnAddress;
			this.expiration = expiration;
			this.durable = durable;
			this.headers = headers;
			this.logicalMessages = logicalMessages;
		}

		public Guid MessageId
		{
			get { return this.messageId; }
		}
		public int Index
		{
			get { return this.index; }
		}
		public int Count
		{
			get { return this.count; }
		}
		public string ReturnAddress
		{
			get { return this.returnAddress; }
		}
		public DateTime Expiration
		{
			get { return this.expiration; }
		}
		public bool Durable
		{
			get { return this.durable; }
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