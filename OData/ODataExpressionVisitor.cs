using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CrowdCode.Library.OData.Query;

namespace Starship.Azure.OData {
    public class ODataExpressionVisitor : ExpressionVisitor {
        public ODataExpressionVisitor() {
            QueryCollection = new Dictionary<string, string>();
        }

        public override Expression Visit(Expression node) {
            return base.Visit(node);
        }

        protected override Expression VisitConstant(ConstantExpression node) {
            if (node.Value != null && node.Value is IsODataClientContext) {
                return base.VisitConstant(node);
            }

            if (node.Value == null) {
                Append("null");
            }
            else {
                Append(ODataConverter.Format(node.Value.GetType(), node.Value));
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) {
            var priorContext = Context;

            switch (node.Method.Name.ToLower()) {
                case "where":
                    Context = "filter";
                    break;
                case "orderby":
                    Context = "orderby";
                    break;
                case "take":
                    Context = "top";
                    break;
                case "skip":
                    Context = "skip";
                    break;
            }

            var result = base.VisitMethodCall(node);
            Context = priorContext;
            return result;
        }

        protected override Expression VisitMember(MemberExpression node) {
            if (node.Expression is MemberExpression || node.Expression == null) {
                var member = Expression.Convert(node, typeof(object));
                var lambda = Expression.Lambda<Func<object>>(member);
                var getter = lambda.Compile();
                var value = getter();

                Append(ODataConverter.Format(value.GetType(), value));
                return node;
            }

            var constant = node.Expression as ConstantExpression;

            if (constant != null) {
                var field = node.Member as FieldInfo;
                var value = field.GetValue(constant.Value);

                value = ODataConverter.Format(value.GetType(), value);

                Append(value.ToString());
                return node;
            }

            Append(node.Member.Name);

            return base.VisitMember(node);
        }

        protected override Expression VisitBinary(BinaryExpression node) {
            if (QueryCollection.ContainsKey(Context)) {
                Append(" and ");
            }

            Visit(node.Left);

            Append(" ");

            switch (node.NodeType) {
                case ExpressionType.Equal:
                    Append("eq");
                    break;
                case ExpressionType.NotEqual:
                    Append("ne");
                    break;
                case ExpressionType.GreaterThan:
                    Append("gt");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    Append("ge");
                    break;
                case ExpressionType.LessThan:
                    Append("lt");
                    break;
                case ExpressionType.LessThanOrEqual:
                    Append("le");
                    break;
                /*case ExpressionType.AndAlso:
                case ExpressionType.And:
                    Append("and");
                    break;*/
                case ExpressionType.OrElse:
                    Append("or");
                    break;
                case ExpressionType.Not:
                    Append("not");
                    break;
            }

            Append(" ");

            Visit(node.Right);

            return node;
        }

        public ODataQuery GetQuery() {
            var query = new ODataQuery();

            if (QueryCollection.ContainsKey("filter")) {
                query.Filter = QueryCollection["filter"];
            }

            if (QueryCollection.ContainsKey("take")) {
                query.Take = int.Parse(QueryCollection["take"]);
            }

            return query;
        }
        
        public string GetQueryString() {
            var query = string.Empty;

            if (QueryCollection.Any()) {
                query += "?";
            }

            var index = 0;

            foreach (var pair in QueryCollection) {
                if (index > 0) {
                    query += "&";
                }

                query += "$" + pair.Key + "=" + pair.Value;
                index += 1;
            }

            return query;
        }

        private void Append(string value) {
            if (string.IsNullOrEmpty(Context)) {
                throw new Exception("OData expression must have a context.");
            }

            if (!QueryCollection.ContainsKey(Context)) {
                QueryCollection.Add(Context, value);
            }
            else {
                QueryCollection[Context] += value;
            }
        }

        private string Context { get; set; }

        private Dictionary<string, string> QueryCollection { get; set; }
    }
}