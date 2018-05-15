using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CrowdCode.Library.OData.Query;
using Starship.Azure.Interfaces;
using Starship.Core.Extensions;

namespace Starship.Azure.OData {
    public class ODataQueryProvider : IQueryProvider {
        public ODataQueryProvider(Type type, IsQueryInvoker invoker) {
            EntityType = type;
            Invoker = invoker;
        }

        public IQueryable CreateQuery(Expression expression) {
            return new ODataClientContext<object>(this, expression); // Todo: use reflection
        }

        public IQueryable<T> CreateQuery<T>(Expression expression) {
            return new ODataClientContext<T>(this, expression);
        }

        public T Execute<T>(Expression expression) {
            var actualType = typeof(T);
            var typename = actualType.Name;

            var listType = typeof(T).GetGenericType() ?? typeof(T);
            var visitor = new ODataExpressionVisitor();
            visitor.Visit(expression);

            var query = visitor.GetQuery();
            var results = Invoker.InvokeGenericMethod("Get", listType, query);

            if (results == null) {
                var type = typeof(List<>).MakeGenericType(listType);

                results = Activator.CreateInstance(type).As<List<T>>();
            }

            if (typeof(T).IsCollection()) {
                return (T)results;
            }

            return results.As<IEnumerable<T>>().FirstOrDefault();
        }

        public object Execute(Expression expression) {
            var visitor = new ODataExpressionVisitor();
            visitor.Visit(expression);

            throw new NotImplementedException();
            //return null;// Invoker.Get(visitor.GetQuery());
        }

        public Type EntityType { get; set; }

        private IsQueryInvoker Invoker { get; set; }
    }
}