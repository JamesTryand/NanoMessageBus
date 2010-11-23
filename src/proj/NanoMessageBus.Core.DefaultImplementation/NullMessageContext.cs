namespace NanoMessageBus.Core
{
	using System;

	public class NullMessageContext : IMessageContext
	{
		private static readonly PhysicalMessage NullMessage =
			new PhysicalMessage(Guid.Empty, 0, 0, string.Empty, DateTime.MinValue, false, null, null);

		public void DeferMessage()
		{
		}
		public void DropMessage()
		{
		}
		public PhysicalMessage CurrentMessage
		{
			get { return NullMessage; }
		}
		public bool ContinueProcessing
		{
			get { return false; }
		}
	}
}