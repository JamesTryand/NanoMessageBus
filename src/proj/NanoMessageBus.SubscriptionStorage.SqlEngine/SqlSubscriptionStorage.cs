namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Transactions;
	using IsolationLevel = System.Transactions.IsolationLevel;

	public class SqlSubscriptionStorage : IStoreSubscriptions
	{
		private readonly Func<IDbConnection> connectionFactory;

		public SqlSubscriptionStorage(Func<IDbConnection> connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public virtual void Subscribe(string address, IEnumerable<string> messageTypes, DateTime expiration)
		{
			this.ExecuteCommand(address, messageTypes, command =>
			{
				command.CommandText = SqlStatements.InsertSubscription;
				command.AddParameter(SqlStatements.ExpirationParameter, expiration.ToNull());
			});
		}
		public virtual void Unsubscribe(string address, IEnumerable<string> messageTypes)
		{
			this.ExecuteCommand(address, messageTypes, command =>
			{
				command.CommandText = SqlStatements.DeleteSubscription;
			});
		}
		private void ExecuteCommand(string address, IEnumerable<string> messageTypes, Action<IDbCommand> callback)
		{
			if (string.IsNullOrEmpty(address) || messageTypes == null)
				return;

			using (var transaction = NewTransaction())
			using (var connection = this.connectionFactory())
			using (var command = connection.CreateCommand())
			{
				command.AddParameter(SqlStatements.MessageTypeParameter, null);
				command.AddParameter(SqlStatements.SubscriberParameter, address);
				callback(command);
				command.Prepare();

				foreach (var messageType in messageTypes)
				{
					((IDataParameter)command.Parameters[0]).Value = messageType;
					command.ExecuteNonQuery();
				}

				transaction.Complete();
			}
		}
		private static TransactionScope NewTransaction()
		{
			var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
			return new TransactionScope(TransactionScopeOption.RequiresNew, options);
		}

		public virtual ICollection<string> GetSubscribers(IEnumerable<string> messageTypes)
		{
			ICollection<string> subscribers = new LinkedList<string>();

			using (SuppressTransaction())
			using (var connection = this.connectionFactory())
			using (var query = connection.BuildGetSubscribersQuery(messageTypes))
			using (var reader = query.ExecuteReader())
			{
				while (reader.Read())
					subscribers.Add((string)reader[0]);
			}

			return subscribers;
		}
		private static IDisposable SuppressTransaction()
		{
			return new TransactionScope(TransactionScopeOption.Suppress);
		}
	}
}