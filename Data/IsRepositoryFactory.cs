using System;
using System.Collections.Generic;

namespace Starship.Azure.Data {
    public interface IsRepositoryFactory {
        IEnumerable<Type> GetTypes();
        IsRepository GetRepository();
    }
}