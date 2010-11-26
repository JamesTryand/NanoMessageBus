namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Messaging;
	using System.Runtime.Serialization;
	using Logging;
	using Serialization;

	public class MsmqReceiverEndpoint : IReceiveFromEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqReceiverEndpoint));
		private static readonly TimeSpan Timeout = 500.Milliseconds();
		private readonly MsmqConnector connector;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqReceiverEndpoint(MsmqConnector connector, ISerializeMessages serializer)
		{
			this.serializer = serializer;
			this.connector = connector;
		}
		~MsmqReceiverEndpoint()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed || !disposing)
				return;

			this.disposed = true;
			this.connector.Dispose();
		}

		public string EndpointAddress
		{
			get { return this.connector.Address; }
		}

		public virtual PhysicalMessage Receive()
		{
			try
			{
				var message = this.connector.Receive(Timeout);

				Log.Info(Diagnostics.MessageReceived, message.BodyStream.Length, this.connector.Address);

				using (message)
				using (message.BodyStream)
					return this.Deserialize(message);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					return this.NoMessageAvailable();

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(Diagnostics.AccessDenied, this.connector.Address);

				throw new EndpointException(e.Message, e);
			}
		}
		private PhysicalMessage Deserialize(Message message)
		{
			try
			{
				return (PhysicalMessage)this.serializer.Deserialize(message.BodyStream);
			}
			catch (Exception e)
			{
				Log.Error(Diagnostics.UnableToDeserialize, message.Id);
				throw new SerializationException(e.Message, e);
			}
		}

		private PhysicalMessage NoMessageAvailable()
		{
			Log.Verbose(Diagnostics.NoMessageAvailable, this.connector.Address);
			return null;
		}
	}
}