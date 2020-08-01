using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Indexer
{
    public class ElasticIndexer : Indexer
    {

        public static readonly string DefaultHostName = "localhost";
        public static readonly int DefaultPortNumber = 9200;

        private ElasticClient client = null;

        public ElasticIndexer(string hostName = null, int? portNumber = null)
        {
            hostName = hostName == null ? DefaultHostName : hostName;
            portNumber = portNumber == null ? DefaultPortNumber : portNumber;
            var node = new Uri($"http://{hostName}:{portNumber}");
            var config = new ConnectionSettings(node);
            this.client = new ElasticClient(config);
        }

        public void CreateIndex(string indexName)
        {
            this.client.Indices.Create(indexName);
        }

        public bool IsIndexExists(string indexName)
        {
            return this.client.Indices.Exists(indexName).Exists;
        }
    }
}
