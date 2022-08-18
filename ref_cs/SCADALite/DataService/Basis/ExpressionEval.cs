using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataService
{
    public class ExpressionEval : IDisposable
    {
        Expression _param1;
        List<ITag> _tagList = new List<ITag>();
        public List<ITag> TagList
        {
            get
            {
                return _tagList;
            }
        }

        IDataServer _server;

        public ExpressionEval(IDataServer server)
        {
            _server = server;
            _param1 = Expression.Constant(this);
        }

        public Delegate Eval(string expression)
        {
            if (string.IsNullOrEmpty(expression)) return null;
            var lambda = ComplieRpnExp(RpnExpression(expression));
            if (lambda != null) return lambda.Compile();
            return null;
        }

        public Delegate WriteEval(string expression)
        {
            if (_server == null || string.IsNullOrEmpty(expression)) return null;
            if (_server[expression.ToUpper()] != null)
            {
                return new Func<object, int>((object value) => { return WriteTag(expression, value); });
            }
            return null;
        }

        public Func<int> WriteEval(string expression1, string expression2)
        {
            if (_server == null || string.IsNullOrEmpty(expression2)) return null;
            if (_server[expression1.ToUpper()] != null)
            {
                var dele = Eval(expression2);
                var funcbool = dele as Func<bool>;
                if (funcbool != null)
                    return () => { return WriteTag(expression1, funcbool()); };
                var funcint = dele as Func<int>;
                if (funcint != null)
                    return () => { return WriteTag(expression1, funcint()); };
                var funcfloat = dele as Func<float>;
                if (funcfloat != null)
                    return () => { return WriteTag(expression1, funcfloat()); };
                var funcstring = dele as Func<string>;
                if (funcstring != null)
                    return () => { return WriteTag(expression1, funcstring()); };
            }
            return null;
        }

        public static bool ValidationExpression(string expression)
        {
            return true; // 정규식 검증 추가 가능
        }

        /// <summary>
        /// 연산자 작업 수준
        /// </summary>
        /// <param name="strOperator">연산자</param>
        /// <returns>연산 연산자, 공백은 0을 반환하고 오류는 -1을 반환</returns>
        private static byte GetOperatorLevel(char strOperator)
        {
            switch (strOperator)
            {
                case '~':
                    return 10;
                case '*':
                case '/':
                case '%':
                    return 9;
                case '+':
                case '-':
                    return 8;
                case '>':
                case '<':
                    return 7;
                case '&':
                    return 6;
                case '^':
                    return 5;
                case '|':
                    return 4;
                case '=':
                case '!':
                case '?':
                    return 3;
                case '(':
                    return 2;
                case ')':
                    return 1;
                //case ':':
                default:
                    return 0;

            }
        }

        /// <summary>
        /// 중위 표기법을 역폴란드 표기법으로 변환
        /// 예, “3 + 4”의 수식은 “3 4 +”와 같은 방식으로 변경
        /// </summary>
        /// <param name="expression">중위 표기법</param>
        /// <returns>역폴란드 표기법</returns>
        public static List<string> RpnExpression(string expression)
        {
            // 닫는 태그 추가
            // 팝 및 푸시 스택 정의
            string[] strNum = expression.Split('~', '%', '>', '<', '=', '!', '&', '|', '?', '#', '^', '+', '-', '*', '/', '(', ')');
            if (strNum.Length < 2) return new List<string>() { expression };
            // 연산자 스택
            Stack<Operator> oper = new Stack<Operator>();
            // 출력 스택 정의
            List<string> output = new List<string>();

            // 접두사 식 문자 읽기 포인터 정의
            int i = 0;
            // 현재 읽은 숫자 배열에 대한 포인터 정의
            int n = -1;
            // 연산자 수준 기능 정의
            Operator op = new Operator();
            // 출력스택크기
            int intStackCount = 0;

            // 접두사 식을 왼쪽에서 오른쪽으로 읽음
            while (i < expression.Length)
            {
                // 문자 읽기
                char strChar = expression[i];
                // 캐릭터 획득 조작 레벨
                if (strChar == '#') { i++; continue; }
                byte intLevel = GetOperatorLevel(strChar);
                if (intLevel == 0)
                // 숫자는 출력 스택으로 직접 푸시
                {
                    while (n++ < strNum.Length)
                    {
                        if (strNum[n] != "")
                        {
                            output.Add(strNum[n]);
                            i += strNum[n].Length;
                            // 배열 포인터 이동
                            break;
                        }
                    }
                }
                else // 연산자 캐릭터는 레벨에 따라 연산자 스택에 푸시
                {
                    if (oper.Count == 0)
                    {
                        // 연산자 스택이 비어 스택에 직접 푸시
                        oper.Push(new Operator(strChar, intLevel));
                        // 문자 읽기 포인터 이동
                        i++;
                    }
                    else
                    {
                        op = oper.Peek();
                        if (intLevel > op.Level || intLevel == 2)
                        {
                            // 연산 문자가 연산자 스택의 마지막 수준보다 높거나 연산자가
                            // '(' 연산자 스택에 직접 푸시
                            oper.Push(new Operator(strChar, intLevel));
                            // 문자 읽기 포인터 이동
                            i++;
                        }
                        else
                        {
                            // 연산 문자가 연산자 스택의 마지막 수준보다 높지 않은 경우
                            // 연산자 스택은 더 높을 때까지 스택에서 팝
                            intStackCount = oper.Count;
                            for (int m = 0; m < intStackCount; m++)
                            {
                                op = oper.Peek();
                                if (op.Level >= intLevel)
                                {
                                    // 스택에서 연산자를 꺼내서 입력 스택에 푸시
                                    char o = op.OperatorStack;
                                    if (!(o == ')' || o == '('))
                                    {
                                        output.Add(o.ToString());
                                    }
                                    oper.Pop();
                                    if (op.Level == 2)
                                    {
                                        // 연산자 스택의 마지막 연산자가 '('이면 스택에서 팝을 중지
                                        i++;
                                        break;
                                    }
                                }
                                else
                                {
                                    // 연산자가 스택의 마지막 수준보다 높을 때까지 스택에 푸시
                                    oper.Push(new Operator(strChar, intLevel));
                                    i++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            intStackCount = oper.Count;
            for (int m = 0; m < intStackCount; m++)
            {
                op = oper.Peek();
                output.Add(op.OperatorStack.ToString());
                oper.Pop();
            }

            return output;
        }

        /// <summary>
        /// 역폴란드 표기법 풀이
        /// </summary>
        /// <param name="expression">역폴란드 표기법</param>
        /// <returns>역폴란드 표기법 해법</returns>
        public LambdaExpression ComplieRpnExp(List<string> strNum)
        {
            _tagList.Clear();
            // 역폴란드 표기법 분해
            int intLenth = strNum.Count;
            if (intLenth == 0) return null;
            // 숫자 스택 정의
            try
            {
                Stack<Expression> number = new Stack<Expression>();
                for (int i = 0; i < intLenth; i++)
                {
                    string expr = strNum[i];
                    switch (expr)
                    {
                        case "~":
                            if (number.Count > 0)
                            {
                                Expression left = number.Pop();
                                number.Push(Expression.Not(left));
                            }
                            break;
                        case "*":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(float))
                                        left = Expression.Convert(left, typeof(float));
                                    if (right.Type != typeof(float))
                                        right = Expression.Convert(right, typeof(float));
                                }
                                number.Push(Expression.Multiply(left, right));
                            }
                            break;
                        case "/":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(float))
                                        left = Expression.Convert(left, typeof(float));
                                    if (right.Type != typeof(float))
                                        right = Expression.Convert(right, typeof(float));
                                }
                                number.Push(Expression.Divide(left, right));
                            }
                            break;
                        case "%":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(int))
                                        left = Expression.Convert(left, typeof(int));
                                    if (right.Type != typeof(int))
                                        right = Expression.Convert(right, typeof(int));
                                }
                                number.Push(Expression.Modulo(left, right));
                            }
                            break;
                        case "+":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type == typeof(string) || right.Type == typeof(string))
                                {
                                    if (left.Type != typeof(string))
                                        left = Expression.Convert(left, typeof(object));
                                    if (right.Type != typeof(string))
                                        right = Expression.Convert(right, typeof(object));
                                    number.Push(Expression.Call(typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object) }), left, right));
                                }
                                else
                                {
                                    if (left.Type != right.Type)
                                    {
                                        if (left.Type != typeof(float))
                                            left = Expression.Convert(left, typeof(float));
                                        if (right.Type != typeof(float))
                                            right = Expression.Convert(right, typeof(float));
                                    }
                                    number.Push(Expression.Add(left, right));
                                }
                            }
                            break;
                        case "-":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(float))
                                        left = Expression.Convert(left, typeof(float));
                                    if (right.Type != typeof(float))
                                        right = Expression.Convert(right, typeof(float));
                                }
                                number.Push(Expression.Subtract(left, right));
                            }
                            break;
                        case ">":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(float))
                                        left = Expression.Convert(left, typeof(float));
                                    if (right.Type != typeof(float))
                                        right = Expression.Convert(right, typeof(float));
                                }
                                number.Push(Expression.GreaterThan(left, right));
                            }
                            break;
                        case "<":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(float))
                                        left = Expression.Convert(left, typeof(float));
                                    if (right.Type != typeof(float))
                                        right = Expression.Convert(right, typeof(float));
                                }
                                number.Push(Expression.LessThan(left, right));
                            }
                            break;
                        case "&":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                number.Push(Expression.And(left, right));
                            }
                            break;
                        case "^":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                number.Push(Expression.ExclusiveOr(left, right));
                            }
                            break;
                        case "|":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                number.Push(Expression.Or(left, right));
                            }
                            break;
                        case "=":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(float))
                                        left = Expression.Convert(left, typeof(float));
                                    if (right.Type != typeof(float))
                                        right = Expression.Convert(right, typeof(float));
                                }
                                number.Push(Expression.Equal(left, right));
                            }
                            break;
                        case "!":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(float))
                                        left = Expression.Convert(left, typeof(float));
                                    if (right.Type != typeof(float))
                                        right = Expression.Convert(right, typeof(float));
                                }
                                number.Push(Expression.NotEqual(left, right));
                            }
                            break;
                        case "?":
                            if (number.Count > 1)
                            {
                                Expression right = number.Pop();
                                Expression left = number.Pop();
                                if (left.Type != right.Type)
                                {
                                    if (left.Type != typeof(float))
                                        left = Expression.Convert(left, typeof(float));
                                    if (right.Type != typeof(float))
                                        right = Expression.Convert(right, typeof(float));
                                }
                                Expression test = number.Pop();
                                number.Push(Expression.Condition(test, left, right));
                            }
                            break;
                        default:
                            if (expr[0] == '@')
                            {
                                switch (expr.Substring(1).ToUpper())
                                {
                                    case "TIME":
                                        {
                                            Expression<Func<string>> f = () => DateTime.Now.ToShortTimeString();
                                            number.Push(f.Body);
                                        }
                                        goto lab1;
                                    case "DATE":
                                        {
                                            Expression<Func<string>> f = () => DateTime.Now.ToShortDateString();
                                            number.Push(f.Body);
                                        }
                                        goto lab1;
                                    case "DATETIME":
                                        {
                                            Expression<Func<string>> f = () => DateTime.Now.ToString();
                                            number.Push(f.Body);
                                        }
                                        goto lab1;
                                    case "APP":
                                        {
                                            Expression<Func<string>> f = () => AppDomain.CurrentDomain.FriendlyName;
                                            number.Push(f.Body);
                                        }
                                        goto lab1;
                                    case "NAME":
                                        {
                                            Expression<Func<string>> f = () => Environment.MachineName;
                                            number.Push(f.Body);
                                        }
                                        goto lab1;
                                    case "PATH":
                                        {
                                            Expression<Func<string>> f = () => Environment.CurrentDirectory;
                                            number.Push(f.Body);
                                        }
                                        goto lab1;
                                    case "USER":
                                        {
                                            Expression<Func<string>> f = () => Environment.UserName;
                                            number.Push(f.Body);
                                        }
                                        goto lab1;
                                    case "REGION":
                                        {
                                            Expression<Func<string>> f = () => System.Globalization.CultureInfo.CurrentCulture.Name;
                                            number.Push(f.Body);
                                        }
                                        goto lab1;
                                }
                            }
                            object result;
                            if (IsConstant(expr, out result))
                            {
                                number.Push(Expression.Constant(result));
                            }
                            else
                            {
                                number.Push(GetTagExpression(expr));
                            }
                        lab1:
                            break;
                    }
                }

                Expression d = number.Pop();
                return Expression.Lambda(d);
            }
            catch (Exception e) { return null; }
        }

        MethodInfo _boolinfo = typeof(ExpressionEval).GetMethod("GetBool");
        MethodInfo _floatinfo = typeof(ExpressionEval).GetMethod("GetFloat");
        MethodInfo _intinfo = typeof(ExpressionEval).GetMethod("GetInt");
        MethodInfo _stringinfo = typeof(ExpressionEval).GetMethod("GetString");

        public Expression GetTagExpression(string tagName)
        {
            if (_server == null) return Expression.Empty();
            ITag tag = _server[tagName];
            switch (tag.Address.VarType)
            {
                case DataType.BOOL:
                    return Expression.Call(_param1, _boolinfo, Expression.Constant(tagName));
                case DataType.BYTE:
                case DataType.WORD:
                case DataType.SHORT:
                case DataType.DWORD:
                case DataType.INT:
                    return Expression.Call(_param1, _intinfo, Expression.Constant(tagName));
                case DataType.FLOAT:
                    return Expression.Call(_param1, _floatinfo, Expression.Constant(tagName));
                case DataType.STR:
                    return Expression.Call(_param1, _stringinfo, Expression.Constant(tagName));
                default:
                    return Expression.Empty();
            }
        }

        public bool GetBool(string tagName)
        {
            if (_server == null) return false;
            ITag tag = _server[tagName];
            switch (tag.Address.VarType)
            {
                case DataType.BOOL:
                    return tag.Value.Boolean;
                case DataType.BYTE:
                    return Convert.ToBoolean(tag.Value.Byte);
                case DataType.WORD:
                    return Convert.ToBoolean(tag.Value.Word);
                case DataType.SHORT:
                    return Convert.ToBoolean(tag.Value.Int16);
                case DataType.DWORD:
                    return Convert.ToBoolean(tag.Value.DWord);
                case DataType.INT:
                    return Convert.ToBoolean(tag.Value.Int32);
                case DataType.FLOAT:
                    return Convert.ToBoolean(tag.Value.Single);
                case DataType.STR:
                    return Convert.ToBoolean(tag.ToString());
                default:
                    return false;
            }
        }

        public float GetFloat(string tagName)
        {
            if (_server == null) return 0f;
            ITag tag = _server[tagName];
            return tag.ScaleToValue(tag.Value);
        }

        public int GetInt(string tagName)
        {
            if (_server == null) return 0;
            ITag tag = _server[tagName];
            switch (tag.Address.VarType)
            {
                case DataType.BOOL:
                    return tag.Value.Boolean ? 1 : 0;
                case DataType.BYTE:
                    return tag.Value.Byte;
                case DataType.WORD:
                    return tag.Value.Word;
                case DataType.SHORT:
                    return tag.Value.Int16;
                case DataType.DWORD:
                    return (int)tag.Value.DWord;
                case DataType.INT:
                    return tag.Value.Int32;
                case DataType.FLOAT:
                    return Convert.ToInt32(tag.Value.Single);
                case DataType.STR:
                    return int.Parse(tag.ToString());
                default:
                    return 0;
            }
        }

        public string GetString(string tagName)
        {
            return _server == null ? null : _server[tagName].ToString();
        }

        public int WriteTag(string tagName, object value)
        {
            if (_server == null || value == null) return -1;
            ITag tag = _server[tagName];
            if (tag.Address.VarType == DataType.BOOL || tag.Address.VarType == DataType.STR)
                return tag.Write(value);
            else
            {
                float temp;
                string str = value as string;
                if (str == null) temp = Convert.ToSingle(value);
                else
                {
                    if (!float.TryParse(str, out temp))
                        return -1;
                }
                return tag.Write(tag.ValueToScale(temp));
            }
        }

        private bool IsConstant(string str, out object value)
        {
            if (str.Length > 1 & str[0] == '\'' && str[str.Length - 1] == '\'')
            {
                value = str.Trim('\'');
                return true;
            }
            string upp = str.ToUpper();
            if (upp == "TRUE")
            {
                value = true;
                return true;
            }
            else if (upp == "FALSE")
            {
                value = false;
                return true;
            }
            if (_server != null)
            {
                var tag = _server[upp];
                if (tag != null)
                {
                    if (!_tagList.Contains(tag))
                        _tagList.Add(tag);
                    value = null;
                    return false;
                }
            }
            int dotcount = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char opr = str[i];
                if (opr < '0' || opr > '9')
                {
                    if (opr != '.')
                    {
                        value = str;
                        return true;
                    }
                    else
                    {
                        if (dotcount > 0)
                        {
                            value = str;
                            return true;
                        }
                        dotcount++;
                    }
                }
            }
            //value = (dotcount == 0 ? int.Parse(str) : float.Parse(str));
            if (dotcount == 0)
                value = int.Parse(str);
            else value = float.Parse(str);
            return true;
        }

        public void Clear()
        {
            //_param1 = null;
            _tagList.Clear();
            //_tagList = null;
        }

        public void Dispose()
        {
            _param1 = null;
            _tagList.Clear();
            _tagList = null;
            _boolinfo = _floatinfo = _stringinfo = null;
        }
    }
}
