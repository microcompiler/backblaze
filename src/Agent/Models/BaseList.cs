using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Models
{
    public abstract class BaseList<T> : IEnumerable<T> where T : class
    {
        protected IApiClient Client { get; }

        public BaseList(IApiClient client)
        {
            Client = client;
        }

        protected abstract List<T> GetNextPage(out bool isDone);

        public IEnumerator<T> GetEnumerator()
        {
            bool isDone;
            do
            {
                List<T> page = GetNextPage(out isDone);

                foreach (T item in page)
                    yield return item;

                if (!page.Any())
                    yield break;
            } while (!isDone);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
