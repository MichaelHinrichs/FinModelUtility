using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace fin.util.reflection {
  public static class CallbackUtil {
    public static IEnumerable<string> GetParameterNames<TIn, TOut>(
        Expression<Func<TIn, TOut>> callback)
      => CallbackUtil.GetParameterNamesImpl_(callback);

    public static IEnumerable<string> GetParameterNames<TIn1, TIn2, TOut>(
        Expression<Func<TIn1, TIn2, TOut>> callback)
      => CallbackUtil.GetParameterNamesImpl_(callback);


    public static IEnumerable<string> GetParameterNames<TIn>(
        Expression<Action<TIn>> callback)
      => CallbackUtil.GetParameterNamesImpl_(callback);

    public static IEnumerable<string> GetParameterNames<TIn1, TIn2>(
        Expression<Action<TIn1, TIn2>> callback)
      => CallbackUtil.GetParameterNamesImpl_(callback);


    private static IEnumerable<string> GetParameterNamesImpl_(
        LambdaExpression expression)
      => expression.Parameters.Select(param => param.Name);
  }
}