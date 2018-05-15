using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Starship.Azure.Interfaces;
using Starship.Azure.OData;

namespace CrowdCode.Library.OData.Query {
    public interface IsODataClientContext {
    }

    public class ODataClientContext<T> : IOrderedQueryable<T>, IsODataClientContext {
        public ODataClientContext(IsQueryInvoker invoker) {
            Provider = new ODataQueryProvider(typeof(T), invoker);
            ElementType = typeof(T);
            Expression = Expression.Constant(this);
        }

        public ODataClientContext(IQueryProvider provider, Expression expression) {
            Provider = provider;
            Expression = expression;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        public IEnumerator GetEnumerator() {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        public Type ElementType { get; private set; }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }
    }
}