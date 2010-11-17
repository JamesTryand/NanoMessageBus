namespace NanoMessageBus.Transports
{
	using System;

	public interface ITransportMessages : IDisposable
	{
		void Start();
		void Stop();

		void Send(PhysicalMessage message, params string[] recipients);
	}
}