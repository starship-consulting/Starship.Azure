using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Starship.Azure.Data {
    public class DataContextProviderFactory {

        static DataContextProviderFactory() {
            Contexts = new Dictionary<string, IsRepository>();
        }

        public IsRepository GetInstance() {
            return Get();
        }

        internal static void Dispose(IsRepository context) {
            lock (Contexts) {
                CallContext.FreeNamedDataSlot("DatabaseContext");

                var key = context.Id.ToString();

                if (Contexts.ContainsKey(key)) {
                    Contexts.Remove(key);
                }
            }
        }

        public static IsRepository ForceNew() {
            lock (Contexts) {
                var existingKey = CallContext.LogicalGetData("DatabaseContext") as string;

                if (!string.IsNullOrEmpty(existingKey)) {
                    Dispose(Contexts[existingKey]);
                }

                var context = new EntityFrameworkRepository(new DatabaseContext());
                var key = context.Id.ToString();
                CallContext.LogicalSetData("DatabaseContext", key);
                Contexts.Add(key, context);
                return context;
            }
        }

        public static IsRepository Get() {
            lock (Contexts) {
                var id = CallContext.LogicalGetData("DatabaseContext") as string;
                return string.IsNullOrEmpty(id) ? ForceNew() : Contexts[id];
            }
        }

        private static Dictionary<string, IsRepository> Contexts { get; set; }
    }
}