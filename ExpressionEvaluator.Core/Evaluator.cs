using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpressionEvaluator.Core;

public class Evaluator
{
    public static double Evaluate(string infix)
    {
        // 1. Convert the expression to Postfix using a Queue
        var postfixQueue = InfixToPostfix(infix);

        // 2. Evaluate the resulting queue
        return EvaluatePostfix(postfixQueue);
    }

    private static Queue<string> InfixToPostfix(string infix)
    {
        var postfixQueue = new Queue<string>(); // Queue required by the PDF
        var stack = new Stack<char>();
        var currentNumber = string.Empty; // Accumulator for numbers with multiple digits

        // Clean white spaces for safety
        infix = infix.Replace(" ", "");

        foreach (var item in infix)
        {
            // If it is a number or a decimal point, we save it
            if (char.IsDigit(item) || item == '.')
            {
                currentNumber += item;
            }
            else if (IsOperator(item))
            {
                // If we find an operator, the number is complete. Put it in the queue.
                if (!string.IsNullOrEmpty(currentNumber))
                {
                    postfixQueue.Enqueue(currentNumber);
                    currentNumber = string.Empty;
                }

                if (stack.Count == 0)
                {
                    stack.Push(item);
                }
                else
                {
                    if (item == '(')
                    {
                        stack.Push(item);
                    }
                    else if (item == ')')
                    {
                        while (stack.Count > 0 && stack.Peek() != '(')
                        {
                            postfixQueue.Enqueue(stack.Pop().ToString());
                        }
                        if (stack.Count > 0) stack.Pop(); // Remove the opening parenthesis '('
                    }
                    else
                    {
                        // Keeping the original priority logic
                        while (stack.Count > 0 && stack.Peek() != '(' && PriorityInfix(item) <= PriorityStack(stack.Peek()))
                        {
                            postfixQueue.Enqueue(stack.Pop().ToString());
                        }
                        stack.Push(item);
                    }
                }
            }
        }

        // If there is a pending number at the end, put it in the queue
        if (!string.IsNullOrEmpty(currentNumber))
        {
            postfixQueue.Enqueue(currentNumber);
        }

        // Empty any remaining operator in the stack to the queue
        while (stack.Count > 0)
        {
            postfixQueue.Enqueue(stack.Pop().ToString());
        }

        return postfixQueue;
    }

    private static int PriorityStack(char item) => item switch
    {
        '^' => 3,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 0,
        _ => throw new Exception("Syntax error."),
    };

    private static int PriorityInfix(char item) => item switch
    {
        '^' => 4,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 5,
        _ => throw new Exception("Syntax error."),
    };

    private static double EvaluatePostfix(Queue<string> postfixQueue)
    {
        var stack = new Stack<double>();

        // Dequeue until the queue is empty
        while (postfixQueue.Count > 0)
        {
            var token = postfixQueue.Dequeue();

            // If the token is a single character and is an operator
            if (token.Length == 1 && IsOperator(token[0]))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                stack.Push(token[0] switch
                {
                    '+' => a + b,
                    '-' => a - b,
                    '*' => a * b,
                    '/' => a / b,
                    '^' => Math.Pow(a, b),
                    _ => throw new Exception("Syntax error."),
                });
            }
            else
            {
                // If it's not an operator, it is a number (e.g. "144" or "3.14")
                // InvariantCulture ensures the dot (.) always works as a decimal separator
                stack.Push(double.Parse(token, CultureInfo.InvariantCulture));
            }
        }

        return stack.Pop();
    }

    private static bool IsOperator(char item) => "+-*/^()".Contains(item);
}