using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Clojure
{
     
    public class RT
    {
        public  static  Context MainContext;
        static Dictionary<string, ASTTree> AstTrees = new Dictionary<string,ASTTree>();

        public static void InitRuntime() 
        {
            if (MainContext == null)
            {
                MainContext = CreateMainContext();
            }
        }

        public static void LoadASTTree(string key,string strTxt) 
        {
            if (!AstTrees.ContainsKey(key))
            {
                AstTrees.Add(key, ASTTree.ParseString(key, strTxt));
            }
        }

        public static ASTTree GetASTTree(string strKey) 
        {
          return AstTrees[strKey];
        }
        public static Context CreateMainContext() 
        {
            Context context = new Context();
            CoreFunction.SetCoreFunction(context);
            return context;
        }

        public static void EvaluateTree(ASTTree tree) 
        {
            for (int i = 0; i < tree.Roots.Count; i++)
            {
                Evaluate(tree.Roots[i],MainContext);
            }
        }
        public static void EvaluateTree(ASTTree tree, Context context) 
        {
            for (int i = 0; i < tree.Roots.Count; i++)
            {
                Evaluate(tree.Roots[i], context);
            }
        }
        public static ASTNode Evaluate(ASTNode node,Context context)
        {
            switch (node.NodeType)
            {
                case ASTNode.ASTNodeType.ListNode:
                    if (node.Children()[0].NodeType == ASTNode.ASTNodeType.SymbolNode)
                    {
                        SymbolNode symNode = (SymbolNode)node.Children()[0];
                        ASTNode curVar = context.Find(symNode.Val);
                        if (curVar.NodeType == ASTNode.ASTNodeType.FunctionNode)
                        {
                            List<ASTNode> args = new List<ASTNode>(node.Children());
                            args.RemoveAt(0);
                            return ((FunctionNode)curVar).Exec(args, context);
                        }
                    }
                    else
                    {
                        Debug.LogError("list first element must  symbolNode");
                    }
                    break;
                case ASTNode.ASTNodeType.SymbolNode:
                    return context.Find(((SymbolNode)node).Val);
                case ASTNode.ASTNodeType.NumberNode:
                case ASTNode.ASTNodeType.StringNode:
                case ASTNode.ASTNodeType.VectorNode:
                case ASTNode.ASTNodeType.MapNode:
                    return node;

            }
            return null;
        }
        
    }


}