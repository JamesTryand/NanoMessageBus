namespace NanoMessageBus.Core
{
	using System.Linq;
	using Logging;

	public class TransportMessageHandler : IHandleMessages<TransportMessage>
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(TransportMessageHandler));
		private readonly ITrackMessageHandlers handlerTable;
		private readonly IMessageContext context;

		public TransportMessageHandler(ITrackMessageHandlers handlerTable, IMessageContext context)
		{
			this.handlerTable = handlerTable;
			this.context = context;
		}

		public virtual void Handle(TransportMessage message)
		{
			Log.Debug(Diagnostics.LogicalMessageCount, message.LogicalMessages.Count);
			foreach (var logicalMessage in message.LogicalMessages.Where(x => x != null))
				this.HandleLogicalMessage(logicalMessage);
		}
		private void HandleLogicalMessage(object message)
		{
			Log.Debug(Diagnostics.RoutingLogicalMessageToHandlers, message.GetType());

			var handlers = this.handlerTable.GetHandlers(message);
			foreach (var handler in handlers.TakeWhile(handler => this.context.ContinueProcessing))
				handler.Handle(message);

			Log.Debug(Diagnostics.LogicalMessageHandled, message.GetType());
		}
	}
}