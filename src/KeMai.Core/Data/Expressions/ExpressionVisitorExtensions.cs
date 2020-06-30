using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeMai.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExpressionVisitorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TExpression"></typeparam>
        /// <param name="expression"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public static Expression Visit<TExpression>(this Expression expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
        {
            return ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TExpression"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="expression"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public static TReturn Visit<TExpression, TReturn>(this TReturn expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
            where TReturn : Expression
        {
            return (TReturn)ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TExpression"></typeparam>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="expression"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public static Expression<TDelegate> Visit<TExpression, TDelegate>(this Expression<TDelegate> expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
        {
            return ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TExpression"></typeparam>
    internal class ExpressionVisitor<TExpression> : ExpressionVisitor where TExpression : Expression
    {
        private readonly Func<TExpression, Expression> _visitor;

        public ExpressionVisitor(Func<TExpression, Expression> visitor)
        {
            _visitor = visitor;
        }

        public override Expression Visit(Expression expression)
        {
            if (expression is TExpression && _visitor != null)
                expression = _visitor(expression as TExpression);

            return base.Visit(expression);
        }

        public static Expression Visit(Expression expression, Func<TExpression, Expression> visitor)
        {
            return new ExpressionVisitor<TExpression>(visitor).Visit(expression);
        }

        public static Expression<TDelegate> Visit<TDelegate>(Expression<TDelegate> expression, Func<TExpression, Expression> visitor)
        {
            return (Expression<TDelegate>)new ExpressionVisitor<TExpression>(visitor).Visit(expression);
        }
    }
}
