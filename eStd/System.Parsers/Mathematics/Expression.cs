using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Parsers.Mathematics
{
    //ToDo: implement brackets
    //ToDo: implement functions
    public class Expression
    {
        private readonly List<Stack> _stacks = new List<Stack>();

        internal Expression()
        {
        }

        public object Result { get; set; }
        public string Term { get; internal set; }

        public static Expression Parse(string expr, VariableStack[] vars)
        {
            var returns = new Expression {Term = expr};

            expr = expr.Replace("²", "²0");
            expr = expr.Replace("³", "³0");
            expr = expr.Replace("sqrt[", "√");
            expr = expr.Replace("pi", "π");
            expr = expr.Replace("]", "");
            expr = expr.Replace("√", "1√");
            expr = expr.Replace("π", "1π1");
            expr = expr.Replace("True", "1");
            expr = expr.Replace("False", "0");

            if (vars != null)
                expr = vars.Aggregate(expr, (current, var) => current.Replace(@var.Name, @var.Value.ToString()));

            char[] chars = expr.ToCharArray();
            string result = "";
            var group_stack = new GroupStack(new List<Stack>());

            bool bracket_open = false;
            foreach (char c in chars)
            {
                if (isNum(c))
                {
                    result += c;
                }
                else
                {
                    if (isOP(c))
                    {
                        if (!bracket_open)
                        {
                            returns._stacks.Add(new NumStack(Convert.ToDouble(result)));
                            returns._stacks.Add(new OpStack(c.ToString()));
                        }
                        else
                        {
                            group_stack.Value.Add(new NumStack(Convert.ToDouble(result)));
                            group_stack.Value.Add(new OpStack(c.ToString()));
                        }
                        result = "";
                    }
                    else if (c == '(')
                        bracket_open = true;
                    else if (c == ')')
                    {
                        bracket_open = false;
                        // group_stack.Value.Add(returns.getLastNum(expr, group_stack.Value));
                        returns._stacks.Add(group_stack);
                        group_stack = new GroupStack(new List<Stack>());
                    }
                }
            }

            group_stack.Value = returns.dotbeforestatement(group_stack.Value);
            returns.Calculate();

            return returns;
        }

        internal void Calculate()
        {
            for (int i = 0; i < _stacks.Count - 1; i++)
            {
                Stack stack = _stacks[i];
                var opStack = stack as OpStack;
                if (opStack != null)
                {
                    var first = (NumStack) _stacks[i - 1];
                    var second = (NumStack) _stacks[i + 1];
                    OpStack op = opStack;
                    var _result = new NumStack(0);
                    bool resultisbool = false;
                    bool boolresult = false;

                    switch (op.Value)
                    {
                        case "+":
                            _result = new NumStack(first.Value + second.Value);
                            break;
                        case "-":
                            _result = new NumStack(first.Value - second.Value);
                            break;
                        case "*":
                            _result = new NumStack(first.Value*second.Value);
                            break;
                        case "/":
                            _result = new NumStack(first.Value/second.Value);
                            break;
                        case "^":
                            _result = new NumStack(Math.Pow(first.Value, second.Value));
                            break;
                        case "%":
                            _result = new NumStack(first.Value%second.Value);
                            break;
                        case "&":
                            _result = new NumStack(Convert.ToDouble(first.Value.ToString() + second.Value.ToString()));
                            break;
                        case "²":
                            _result = new NumStack(first.Value*first.Value);
                            break;
                        case "³":
                            _result = new NumStack(first.Value*first.Value*first.Value);
                            break;
                        case "√":
                            _result = new NumStack(Math.Sqrt(second.Value));
                            break;
                        case "π":
                            _result = new NumStack(Math.PI);
                            break;
                        case "<":
                            resultisbool = true;
                            boolresult = first.Value < second.Value;
                            break;
                        case ">":
                            resultisbool = true;
                            boolresult = first.Value > second.Value;
                            break;
                        case "!":
                            resultisbool = true;
                            boolresult = first.Value != second.Value;
                            break;
                    }

                    _stacks.RemoveAt(0);
                    _stacks.RemoveAt(0);
                    _stacks.RemoveAt(0);
                    _stacks.Insert(0, _result);

                    Result = (_stacks[0] as NumStack).Value;

                    if (_stacks.Count != 1)
                    {
                        if (_stacks[0] != null && _stacks[1] != null && _stacks[2] != null)
                        {
                            Calculate();
                        }
                    }
                    if (resultisbool)
                    {
                        Result = boolresult;
                    }
                }
            }
        }

        #region "ObjectOperators"

        public static Expression operator +(Expression e1, Expression e2)
        {
            return Parse(e1.Result + "+" + e2.Result, null);
        }

        public static bool operator ==(Expression e1, Expression e2)
        {
            return e1 == e2;
        }

        public static bool operator !=(Expression e1, Expression e2)
        {
            return e1 != e2;
        }

        public static Expression operator +(Expression e1, string e2)
        {
            e2 = e2.Replace("=", "");
            return Parse(e2, null);
        }

        public static Expression operator -(Expression e1, Expression e2)
        {
            return Parse(e1.Result + "-" + e2.Result, null);
        }

        public static Expression operator *(Expression e1, Expression e2)
        {
            return Parse(e1.Result + "*" + e2.Result, null);
        }

        public static Expression operator /(Expression e1, Expression e2)
        {
            return Parse(e1.Result + "/" + e2.Result, null);
        }

        public static Expression operator <(Expression e1, Expression e2)
        {
            return Parse(e1.Result + "<" + e2.Result, null);
        }

        public static Expression operator >(Expression e1, Expression e2)
        {
            return Parse(e1.Result + ">" + e2.Result, null);
        }

        public static implicit operator Expression(string v)
        {
            double r;
            return double.TryParse(v, out r) ? Parse(v + "*1", null) : Parse(v, null);
        }

        public static implicit operator double(Expression instance)
        {
            return (double) instance.Result;
        }

        public static implicit operator bool(Expression instance)
        {
            return (bool) instance.Result;
        }

        public static implicit operator string(Expression instance)
        {
            return instance.Result.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return base.Equals(obj);
        }

        #endregion

        #region "Help Functions"

        private static bool isNum(char s)
        {
            double result;
            return double.TryParse(s.ToString(), out result);
        }

        private static bool isOP(char s)
        {
            return s == '+' | s == '-' | s == '*' | s == '/' | s == '^' | s == '%' | s == '&' | s == '²' | s == '3' |
                   s == '<' | s == '>' | s == '!' | s == '√' | s == 'π';
        }

        private bool isPunctiaton(char s)
        {
            return s == '*' | s == '/';
        }

        private NumStack getLastNum(string s, List<Stack> st)
        {
            string[] c = s.Split(Convert.ToChar((st[st.Count - 1] as OpStack).Value));
            return new NumStack(Convert.ToInt32(c[c.Length - 1]));
        }

        private List<Stack> dotbeforestatement(List<Stack> st)
        {
            List<Stack> returns = st;

            for (int i = 0; i < st.Count; i++)
            {
                Stack s = st[i];

                if (s is OpStack)
                {
                    var op = s as OpStack;
                    if (isPunctiaton(Convert.ToChar(op.Value)))
                    {
                        if (st.Count < 3)
                        {
                            Stack firstop = st[i - 2];
                            Stack first = st[i - 1];
                            Stack ops = s;
                            Stack second = st[i + 1];

                            returns.Remove(first);
                            returns.Remove(ops);
                            returns.Remove(second);
                            returns.Remove(firstop);

                            returns.InsertRange(0, new[] {first, ops, second, firstop});
                        }
                    }
                }
            }

            return returns;
        }

        #endregion

        #region "Implementations"

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Expression Clone()
        {
            return this;
        }

        public override string ToString()
        {
            return Result.ToString();
        }

        #endregion
    }
}