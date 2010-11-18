namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;

	public class MessageHandlerTable<TContainer> : ITrackMessageHandlers
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageHandlerTable<TContainer>));
		private static readonly IDictionary<Type, ICollection<Func<TContainer, IHandleMessages>>> Routes =
			new Dictionary<Type, ICollection<Func<TContainer, IHandleMessages>>>();
		private readonly TContainer childContainer;

		public MessageHandlerTable(TContainer childContainer)
		{
			this.childContainer = childContainer;
		}

		public static void RegisterHandler<TMessage>(Func<TContainer, IHandleMessages<TMessage>> route)
			where TMessage : class
		{
			var key = typeof(TMessage);

			ICollection<Func<TContainer, IHandleMessages>> routes;
			if (!Routes.TryGetValue(key, out routes))
				Routes[key] = routes = new LinkedList<Func<TContainer, IHandleMessages>>();

			Log.Debug(Diagnostics.RegisteringHandler, typeof(TMessage));
			routes.Add(c => new MessageHandler<TMessage>(route(c)));
		}

		public virtual IEnumerable<IHandleMessages> GetHandlers(Type messageType)
		{
			ICollection<Func<TContainer, IHandleMessages>> routeCallbacks;
			if (!Routes.TryGetValue(messageType, out routeCallbacks))
			{
				Log.Warn(Diagnostics.NoRegisteredHandlersFound, messageType);
				return new IHandleMessages[] { };
			}

			Log.Verbose(Diagnostics.GettingHandlers, messageType);
			return routeCallbacks.Select(route => route(this.childContainer));
		}

		private class MessageHandler<TMessage> : IHandleMessages where TMessage : class
		{
			private readonly IHandleMessages<TMessage> handler;

			public MessageHandler(IHandleMessages<TMessage> handler)
			{
				this.handler = handler;
			}

			public void Handle(object message)
			{
				Log.Verbose(Diagnostics.RoutingLogicalMessageToHandler, message.GetType(), this.handler.GetType());
				this.handler.Handle(message as TMessage);
				Log.Debug(Diagnostics.HandlerCompleted, message.GetType(), this.handler.GetType());
			}
		}
	}
}