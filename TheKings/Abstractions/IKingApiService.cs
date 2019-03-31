using System.Collections.Generic;

namespace TheKings
{
    public interface IKingApiService
    {
        IEnumerable<King> GetKings();
    }
}