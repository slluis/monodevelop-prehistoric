
using System;

namespace MonoQuery.Collections
{
	public class ConnectionProviderDescriptor
	{
		private Type providerType;
		private string name;
		private string connectionStringExample;
		
		public Type ProviderType
		{
			get { return providerType; }
		}
		public string Name
		{
			get { return name; }
		}
		public string ConnectionStringExample
		{
			get { return connectionStringExample; }
		}
		
		public ConnectionProviderDescriptor( string name, Type type ) : this ( name, type, null ) {}
		public ConnectionProviderDescriptor( string name, Type type, string example )
		{
			this.name = name;
			this.providerType = type;
			this.connectionStringExample = example;
		}
	}
}