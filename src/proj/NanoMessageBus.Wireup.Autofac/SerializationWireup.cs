namespace NanoMessageBus.Wireup
{
	using System;
	using Autofac;
	using Serialization;

	public class SerializationWireup : WireupModule
	{
		private ISerializeMessages messageSerializer;
		private ITransformMessages messageTransformer;
		private bool compress;
		private byte[] encryptionKey;

		public SerializationWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual SerializationWireup BinarySerializer()
		{
			this.messageSerializer = new BinaryMessageSerializer();
			return this;
		}
		public virtual SerializationWireup XmlSerializer()
		{
			this.messageSerializer = new XmlMessageSerializer();
			return this;
		}
		public virtual SerializationWireup JsonSerializer()
		{
			this.messageSerializer = new JsonMessageSerializer();
			return this;
		}
		public virtual SerializationWireup BsonSerializer()
		{
			this.messageSerializer = new BsonMessageSerializer();
			return this;
		}
		public virtual SerializationWireup ProtocolBufferSerializer(params Type[] messageTypes)
		{
			this.messageSerializer = new ProtocolBufferSerializer(messageTypes);
			return this;
		}
		public virtual SerializationWireup CompressMessages()
		{
			this.compress = true;
			return this;
		}
		public virtual SerializationWireup EncryptMessages(byte[] key)
		{
			this.encryptionKey = key;
			return this;
		}
		public virtual SerializationWireup TransformMessages(ITransformMessages transformer)
		{
			this.messageTransformer = transformer;
			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder
				.RegisterInstance(this.BuildSerializer())
				.As<ISerializeMessages>()
				.SingleInstance()
				.ExternallyOwned();
		}
		protected virtual ISerializeMessages BuildSerializer()
		{
			var serializer = this.messageSerializer ?? this.BuildDefaultSerializer();

			if (this.messageTransformer != null)
				serializer = new TransformationMessageSerializer(serializer, this.messageTransformer);

			if (this.compress)
				serializer = new GzipMessageSerializer(serializer);

			if (this.encryptionKey != null)
				serializer = new EncryptMessageSerializer(serializer, this.encryptionKey);

			return serializer;
		}
		protected virtual ISerializeMessages BuildDefaultSerializer()
		{
			return new BinaryMessageSerializer();
		}
	}
}