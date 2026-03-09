using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace ServiceStack.OrmLite.Sqlite
{
    /// <summary>
    /// Description of SqliteExpressionVisitor.
    /// </summary>
    public class SqliteExpressionVisitor<T> : SqlExpressionVisitor<T>
    {
        protected override object VisitColumnAccessMethod(MethodCallExpression m)
        {
            List<Object> args = this.VisitExpressionList(m.Arguments);
            var quotedColName = Visit(m.Object);
            string statement;

            switch (m.Method.Name)
            {
                case "Substring":
                    var startIndex = Int32.Parse(args[0].ToString()) + 1;
                    if (args.Count == 2)
                    {
                        var length = Int32.Parse(args[1].ToString());
                        statement = string.Format("substr({0}, {1}, {2})", quotedColName, startIndex, length);
                    }
                    else
                        statement = string.Format("substr({0}, {1})", quotedColName, startIndex);
                    break;
                default:
                    return base.VisitColumnAccessMethod(m);
            }
            return new PartialSqlString(statement);
        }

        private string PobierzKomunikatSubquery(MethodCallExpression m,object quotedColName,bool zaprzeczenie=false)
        {
            var p2 = m.Arguments[1] as MethodCallExpression;
            var wynik = Expression.Lambda(p2).Compile().DynamicInvoke();

            string SelectExpression = wynik.GetType().GetProperty("SelectExpression").GetValue(wynik, null) as string;
            string WhereExpression = wynik.GetType().GetProperty("WhereExpression").GetValue(wynik, null) as string;

            string sqlWzor = zaprzeczenie? "{0} not in ({1})": "{0} in ({1})";
            return string.Format(sqlWzor, quotedColName, SelectExpression + " " + WhereExpression).Replace("\n", "");
        }
        protected override object VisitSqlMethodCall(MethodCallExpression m)
        {
            List<Object> args = this.VisitExpressionList(m.Arguments);
            object quotedColName = args[0];
            args.RemoveAt(0);

            var statement = "";

            switch (m.Method.Name)
            {
                case "InSubquery":
                    statement = PobierzKomunikatSubquery(m, quotedColName);
                    break;
                case "NotInSubquery":
                    statement = PobierzKomunikatSubquery(m, quotedColName, true);
                    break;
                case "In":
                    var member = Expression.Convert(m.Arguments[1], typeof(object));
                    var lambda = Expression.Lambda<Func<object>>(member);
                    var getter = lambda.Compile();

                    var inArgs = Sql.Flatten(getter() as IEnumerable);


                    if (inArgs.Count == 0 || (inArgs.Count == 1 && inArgs[0] == null))
                    {
                        //NULL pusta lista
                        statement = "( 1=1 )";
                        break;
                    }


                    var sIn = new StringBuilder();
                    foreach (var e in inArgs)
                    {
                        sIn.AppendFormat("{0}{1}",
                                     sIn.Length > 0 ? "," : "",
                                     OrmLiteConfig.DialectProvider.GetQuotedValue(e, e.GetType()));
                    }
                    statement = string.Format("{0} {1} ({2})", quotedColName, m.Method.Name, sIn);
                    break;
                case "Desc":
                    statement = string.Format("{0} DESC", quotedColName);
                    break;
                case "As":
                    statement = string.Format("{0} As {1}", quotedColName,
                        OrmLiteConfig.DialectProvider.GetQuotedColumnName(RemoveQuoteFromAlias(args[0].ToString())));
                    break;
                case "Sum":
                case "Count":
                case "Min":
                case "Max":
                case "Avg":
                    statement = string.Format("{0}({1}{2})",
                                         m.Method.Name,
                                         quotedColName,
                                         args.Count == 1 ? string.Format(",{0}", args[0]) : "");
                    break;
                default:
                    throw new NotSupportedException();
            }

            return new PartialSqlString(statement);
        }
    }
}
