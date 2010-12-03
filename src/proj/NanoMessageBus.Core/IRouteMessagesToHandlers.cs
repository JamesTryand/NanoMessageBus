namespace NanoMessageBus.Core
{
	using System;

	public interface IRouteMessagesToHandlers : IMessageContext, IDisposable
	{
		void Route(TransportMessage message);
	}
}