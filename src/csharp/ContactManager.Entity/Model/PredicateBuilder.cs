using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ContactManager.Entity.Model
{
    public class Filter
    {
        public string PropertyName { get; set; }
        public string Operation { get; set; }
        public object Value { get; set; }
    }

    public static class PredicateBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });


        public static Expression<Func<T, bool>> GetExpression<T>(IList<Filter> filters)
        {
            if (filters.Count == 0)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpression<T>(param, filters[0]);
            else if (filters.Count == 2)
                exp = GetExpression<T>(param, filters[0], filters[1]);
            else
            {
                while (filters.Count > 0)
                {
                    var f1 = filters[0];
                    var f2 = filters[1];

                    if (exp == null)
                        exp = GetExpression<T>(param, filters[0], filters[1]);
                    else
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        private static Expression GetExpression<T>(ParameterExpression param, Filter filter)
        {
            MemberExpression member = Expression.Property(param, filter.PropertyName);
            ConstantExpression constant = Expression.Constant(filter.Value);
            string serarchvalue = constant.Value.ToString();
            ConstantExpression expVal = Expression.Constant(serarchvalue);

            switch (filter.Operation.ToLower())
            {
                case "equals":
                    return Expression.Equal(member, expVal);

                case "greaterthan":
                    return Expression.GreaterThan(member, expVal);

                case "greaterthanorequal":
                    return Expression.GreaterThanOrEqual(member, expVal);

                case "lessthan":
                    return Expression.LessThan(member, expVal);

                case "lessthanorequal":
                    return Expression.LessThanOrEqual(member, expVal);

                case "contains":
                    //var exp = Expression.Call(member, member.Type.GetMethod("Contains",/*Here you need the type parameters*/), constant);
                    return Expression.Call(member, containsMethod, expVal);

                case "startswith":
                    return Expression.Call(member, startsWithMethod, expVal);

                case "endswith":
                    return Expression.Call(member, endsWithMethod, expVal);
            }

            return null;
        }

        private static BinaryExpression GetExpression<T>(ParameterExpression param, Filter filter1, Filter filter2)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);

            return Expression.AndAlso(bin1, bin2);
        }
    }    
}
