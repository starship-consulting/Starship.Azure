using System;

namespace Starship.Azure.Providers.Cosmos {

    public class BulkDeletionState {

        public int Deleted { get; set; }

        public bool Continuation { get; set; }
    }
}