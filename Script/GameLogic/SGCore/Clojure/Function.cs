using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Clojure
{
 
    public interface IFunction 
    {
        ASTNode Exec(List<ASTNode> args, Context context);
    }


    public static class CoreFunction
    {

        public static void SetCoreFunction(Context context) 
        {
            context.Define("log", new FunctionNode(CoreFunction.Log));
            context.Define("+" ,new FunctionNode(CoreFunction.Add));
            context.Define("<", new FunctionNode(CoreFunction.Less));
            context.Define("do", new FunctionNode(CoreFunction._do));
            context.Define("==",new FunctionNode(_Equal));
            context.Define("&&",new FunctionNode(bool_and));
        }

        public static List<ASTNode> EvalArgs(List<ASTNode> args, Context context)
        {
            for (int i = 0; i < args.Count; i++)
            {
                args[i] = RT.Evaluate(args[i], context);
            }
            return args;
        }
        public static ASTNode bool_and(List<ASTNode> args, Context context) 
        {
            BoolNode b = new BoolNode();
            b.Val = false;
            args = EvalArgs(args, context);
            if (args.Count>0&&args[0].NodeType == ASTNode.ASTNodeType.BoolNode && args[1].NodeType == ASTNode.ASTNodeType.BoolNode) 
            {
                BoolNode b0 = ((BoolNode)args[0]);
                BoolNode b1 = ((BoolNode)args[1]);
                if (b0.Val && b1.Val) 
                {
                    b.Val = true;
                }
                else 
                {
                    b.Val = false;
                }
            }
            return b;
        }
        public static ASTNode Log(List<ASTNode> args, Context context) 
        {
            args[0] = RT.Evaluate(args[0], context);
            switch (args[0].NodeType) 
            {
                case ASTNode.ASTNodeType.StringNode:
                    Debug.Log(((StringNode)args[0]).Val);
                break;
                case ASTNode.ASTNodeType.NumberNode:
                    Debug.Log(((NumberNode)args[0]).NumberVal);
                break;
                case ASTNode.ASTNodeType.BoolNode:
                  Debug.Log(((BoolNode)args[0]).Val);
                break;
            }
            return null;
        }

        public static ASTNode Add(List<ASTNode> args, Context context) 
        {
            NumberNode retNode = new NumberNode();
            retNode.NumberVal = 0;
            while(args.Count>0)
            {
              NumberNode numNode = (NumberNode)RT.Evaluate(args[0],context);
              args.RemoveAt(0);
              switch (numNode.Type) 
              {
                  case NumberNode.NumberType.INT:
                      if (retNode.Type == NumberNode.NumberType.INT)
                      {
                          retNode.NumberVal = (int)retNode.NumberVal + (int)numNode.NumberVal;
                      }
                      else 
                      {
                          retNode.NumberVal = (float)retNode.NumberVal + (int)numNode.NumberVal;
                      }
                  break;
                  case NumberNode.NumberType.FLOAT:
                    retNode.Type = NumberNode.NumberType.FLOAT;
                    if (retNode.Type == NumberNode.NumberType.INT)
                    {
                        retNode.NumberVal = (float)retNode.NumberVal + (int)numNode.NumberVal;
                    }
                    else 
                    {
                        retNode.NumberVal = (float)retNode.NumberVal + (float)numNode.NumberVal;
                    }
                  break;
              }
            }
            return retNode;
        }

        public static ASTNode IF(List<ASTNode> args, Context context) 
        {
            

            return null;
        }

        //<
        public static ASTNode Less(List<ASTNode> args, Context context) 
        {
            BoolNode boolNode = new BoolNode();
            NumberNode numberNode0 = (NumberNode)RT.Evaluate(args[0], context);
            NumberNode numberNode1 = (NumberNode)RT.Evaluate(args[1], context);

            float num0 = numberNode0.Type == NumberNode.NumberType.INT ?(float)(int)numberNode0.NumberVal:(int)numberNode0.NumberVal;
            float num1 = numberNode1.Type == NumberNode.NumberType.INT ? (float)(int)numberNode1.NumberVal : (int)numberNode1.NumberVal;
              
            if (num0 < num1)
            {
                boolNode.Val = true;
            }
            else 
            {
                boolNode.Val = false;   
            }
            return boolNode;
        }

        public static ASTNode _do(List<ASTNode> args, Context context)
        {
            for (int i = 0; i < args.Count;i++)
            {
                RT.Evaluate(args[i], context);
            }
            return null;
        }

        public static ASTNode _Equal(List<ASTNode> args, Context context) 
        {
            args[0] = RT.Evaluate(args[0],context);
            args[1] = RT.Evaluate(args[1], context);
            BoolNode b = new BoolNode();
            //都是数字
            if (args[0].NodeType == ASTNode.ASTNodeType.NumberNode && args[1].NodeType == ASTNode.ASTNodeType.NumberNode) 
            {
                NumberNode node1 = (NumberNode)args[0];
                NumberNode node2 = (NumberNode)args[1];
                if (node1.Type == NumberNode.NumberType.INT && node2.Type == NumberNode.NumberType.INT) 
                {
                    if ((int)node1.NumberVal == (int)node2.NumberVal)
                    {
                        b.Val = true;
                        return b;
                    }
                    else 
                    {
                        b.Val = false;
                        return b;
                    }
                }
            }


            return b;
        } 

    }
}
 