namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Messaging;
	using Serialization;

	internal static class MessageBuilder
	{
		private const string LabelFormat = "NMB:{0}:{1}";

		public static PhysicalMessage Deserialize(this Message message, ISerializeMessages serializer)
		{
			return (PhysicalMessage)serializer.Deserialize(message.BodyStream);
		}

		public static Message BuildMsmqMessage(this PhysicalMessage message, Stream serialized)
		{
			var label = string.Format(
				CultureInfo.InvariantCulture,
				LabelFormat,
				message.LogicalMessages.Count,
				message.LogicalMessages.First().GetType().FullName);

			return new Message
			{
				Label = label,
				BodyStream = serialized,
				Recoverable = message.Durable,
				TimeToBeReceived = message.Expiration.GetTimeToBeReceived(),
			};
		}
		private static TimeSpan GetTimeToBeReceived(this DateTime expiration)
		{
			if (expiration == DateTime.MinValue || expiration == DateTime.MaxValue)
				return MessageQueue.InfiniteTimeout;

			return expiration.Subtract(DateTime.UtcNow);
		}
	}
}