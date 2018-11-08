using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Service
{
    public interface IReadWriteEventStoreConnection : IDisposable
    {
        Task<WriteResult> AppendToStream(string stream, long expectedVersion, params EventData[] events);
        Task<StreamEventsSlice> ReadStreamEventsForward(string stream, long start, int count, bool resolveLinkTos);
    }
}
