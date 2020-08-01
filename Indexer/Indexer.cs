using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Indexer
{
    public interface Indexer
    {

        public bool IsIndexExists(string indexName);

        public void CreateIndex(string indexName);

    }
}
