using System.Collections.Generic;
using Starship.Azure.OData;

namespace Starship.Azure.Interfaces {
    public interface IsQueryInvoker {

        List<T> Get<T>(ODataQuery query);
    }
}