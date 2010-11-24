namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;
	using SubscriptionStorage;
	using Transports;

	public class MessageBus : ISendMessages, IPublishMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageBus));
		private readonly ITransportMessages transport;
		private readonly IStoreSubscriptions subscriptions;
		private readonly IDictionary<Type, IEnumerable<string>> recipients;
		private readonly IMessageContext context;
		private readonly MessageBuilder builder;

		public MessageBus(
			ITransportMessages transport,
			IStoreSubscriptions subscriptions,
			IDictionary<Type, IEnumerable<string>> recipients,
			IMessageContext context,
			MessageBuilder builder)
		{
			this.transport = transport;
			this.subscriptions = subscriptions;
			this.recipients = recipients;
			this.context = context;
			this.builder = builder;
		}

		public virtual void Send(params object[] messages)
		{
			Log.Debug(Diagnostics.SendingMessage);
			this.Dispatch(messages, type => this.recipients.GetRecipients(type));
		}
		public virtual void Reply(params object[] messages)
		{
			Log.Debug(Diagnostics.Replying, this.context.CurrentMessage.ReturnAddress);
			this.Dispatch(messages, type => new[] { this.context.CurrentMessage.ReturnAddress });
		}
		public virtual void Publish(params object[] messages)
		{
			Log.Debug(Diagnostics.Publishing);
			this.Dispatch(messages, type => this.subscriptions.GetSubscribers(new[] { type }));
		}

		private void Dispatch(object[] messages, Func<Type, ICollection<string>> getMessageRecipients)
		{
			messages = messages.PopulatedMessagesOnly();
			if (!messages.Any())
				return;

			var primaryLogicalMessageType = messages[0].GetType();
			var addresses = getMessageRecipients(primaryLogicalMessageType);

			if (addresses.Count == 0)
			{
				Log.Warn(Diagnostics.DroppingMessage, primaryLogicalMessageType);
				return;
			}

			this.transport.Send(this.builder.BuildMessage(messages), addresses.ToArray());
		}
	}
}