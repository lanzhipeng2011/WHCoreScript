using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Clojure
{
    public class ASTNode
    {
        public enum ASTNodeType
        {
            BoolNode,
            CharacterNode,
            CommentNode,
            DerefNode,
            KeywordNode,
            ListNode,
            MapNode,
            MetadataNode,
            NewlineNode,
            NilNode,
            NumberNode,
            SymbolNode,
            QuoteNode,
            StringNode,
            SyntaxQuoteNode,
            UnquoteNode,
            UnquoteSpliceNode,
            VectorNode,
            FnLiteralNode,
            IgnoreFormNode,
            RegexNode,
            SetNode,
            VarQuoteNode,
            TagNode,
            FunctionNode,
            ObjectNode,
        }

        public ASTNodeType NodeType; 
 
        public ASTPos Position;

        public virtual List<ASTNode> Children() { return null; }
        public virtual string String() { return ""; }

        public static int countSemantic(List<ASTNode> nodes)
        {
            int count = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (isSemantic(nodes[i]))
                {
                    count++;
                }
            }
            return count;
        }

        public static bool isSemantic(ASTNode n)
        {
            if (n == null) 
            {
                return false;
            }
            if (n.GetType() == typeof(CommentNode) || n.GetType() == typeof(NewlineNode))
            {
                return false;
            }
            return true;
        }
    }

    public class BoolNode : ASTNode
    {
        public bool Val;
        public BoolNode() { this.NodeType = ASTNodeType.BoolNode; }
        public override string String()
        {
            if (this.Val)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }
    }

    public class CharacterNode : ASTNode
    {
        public char Val;
        public string Text;

        public CharacterNode() { this.NodeType = ASTNodeType.CharacterNode; }
        public override string String()
        {
            return "char(" + Val.ToString() + ")";
        }
    }

    public class CommentNode : ASTNode
    {
        public string Text;
        public CommentNode() { this.NodeType = ASTNodeType.CommentNode; }
        public override string String()
        {
            return "comment(" + this.Text + ")";
        }
    }

    public class DerefNode : ASTNode
    {
        public ASTNode Node;
        public DerefNode() { this.NodeType = ASTNodeType.DerefNode; }
        public override string String()
        {
            return "deref";
        }
        public override List<ASTNode> Children()
        {
            List<ASTNode> nodes = new List<ASTNode>();
            nodes.Add(this.Node);
            return nodes;
        }
    }

    public class KeywordNode : ASTNode
    {
        public string Val;

        public KeywordNode() { this.NodeType = ASTNodeType.KeywordNode; }
        public override string String()
        {
            return "keyword(" + Val + ")";
        }
    }

    public class ListNode : ASTNode
    {
        public List<ASTNode> nodes;

        public ListNode() { this.NodeType = ASTNodeType.ListNode; }
        public override string String()
        {
            return "list(length=" + countSemantic(nodes) + ")";
        }
        public override List<ASTNode> Children()
        {
            return this.nodes;
        }
    }

    public class MapNode : ASTNode
    {
        public List<ASTNode> nodes;

        public MapNode() { this.NodeType = ASTNodeType.MapNode; }
        public override string String()
        {
            return "map(length=" + countSemantic(nodes) / 2 + ")";
        }
        public override List<ASTNode> Children()
        {
            return this.nodes;
        }
    }

    public class MetadataNode : ASTNode
    {
        ASTNode node;
        public MetadataNode() { this.NodeType = ASTNodeType.MetadataNode; }
        public override string String()
        {
            return "metadata";
        }
        public override List<ASTNode> Children()
        {
            List<ASTNode> nodes = new List<ASTNode>();
            nodes.Add(node);
            return nodes;
        }
    }

    public class NewlineNode : ASTNode
    {
        public NewlineNode() { this.NodeType = ASTNodeType.NewlineNode; }
        public override string String()
        {
            return "newline";
        }
    }

    public class NilNode : ASTNode
    {
        public NilNode() { this.NodeType = ASTNodeType.NilNode; }
        public override string String()
        {
            return "nil";
        }
    }

    public class NumberNode : ASTNode
    {
        public object NumberVal; 
        public enum NumberType 
        {
            INT,
            FLOAT,
        }
        public NumberType Type; 
        public NumberNode() { this.NodeType = ASTNodeType.NumberNode; }
        public override string String()
        {
            return "num(" + this.NumberVal.ToString() + ")";
        }
    }

    public class SymbolNode : ASTNode
    {
        public string Val;
        public SymbolNode() { this.NodeType = ASTNodeType.SymbolNode; }
        public override string String()
        {
            return "sym(" + this.Val + ")";
        }
    }

    public class FunctionNode : ASTNode, IFunction
    {
        public delegate ASTNode ASTFunc(List<ASTNode> args,Context context);

        public ASTFunc mFunc;
        public FunctionNode() { this.NodeType = ASTNodeType.FunctionNode; }
        public FunctionNode(ASTFunc fn) { this.NodeType = ASTNodeType.FunctionNode; this.mFunc = fn; }
        public  ASTNode Exec(List<ASTNode> args, Context context) { return mFunc(args,context); }
        
    }
    public class ObjectNode : ASTNode 
    {
        public object Val;
        public ObjectNode() { this.NodeType = ASTNodeType.ObjectNode; }

        public ObjectNode(object val) { this.NodeType = ASTNodeType.ObjectNode; this.Val = val; }

    }

    public class QuoteNode : ASTNode
    {
        public ASTNode node;

        public QuoteNode() { this.NodeType = ASTNodeType.QuoteNode; }
        public override string String()
        {
            return "quote";
        }
        public override List<ASTNode> Children()
        {
            List<ASTNode> nodes = new List<ASTNode>();
            nodes.Add(node);
            return nodes;
        }
    }

    public class StringNode : ASTNode
    {
        public string Val;

        public StringNode() { this.NodeType = ASTNodeType.StringNode; }
        public override string String()
        {
            return "string(" + Val + ")";
        }
    }

    public class SyntaxQuoteNode : ASTNode
    {
        ASTNode node;

        public SyntaxQuoteNode() { this.NodeType = ASTNodeType.SyntaxQuoteNode; }
        public override string String()
        {
            return "syntax quote";
        }
        public override List<ASTNode> Children()
        {
            List<ASTNode> nodes = new List<ASTNode>();
            nodes.Add(node);
            return nodes;
        }
    }

    public class UnquoteNode : ASTNode
    {
        ASTNode node;

        public UnquoteNode() { this.NodeType = ASTNodeType.UnquoteNode; }
        public override string String()
        {
            return "unquote";
        }

        public override List<ASTNode> Children()
        {
            List<ASTNode> nodes = new List<ASTNode>();
            nodes.Add(node);
            return nodes;
        }
    }

    public class UnquoteSpliceNode : ASTNode
    {
        ASTNode node;

        public UnquoteSpliceNode() 
        {
            this.NodeType = ASTNodeType.UnquoteSpliceNode;
        }
        public override string String()
        {
            return "unquote splice";
        }

        public override List<ASTNode> Children()
        {
            List<ASTNode> nodes = new List<ASTNode>();
            nodes.Add(node);
            return nodes;
        }
    }

    public class VectorNode : ASTNode
    {
        public List<ASTNode> nodes;

        public VectorNode() { this.NodeType = ASTNodeType.VectorNode; }
        public override string String()
        {
            return "vector(length=" + countSemantic(nodes) + ")";
        }
        public override List<ASTNode> Children()
        {
            return this.nodes;
        }
    }

    public class FnLiteralNode : ASTNode
    {
        List<ASTNode> nodes;

        public FnLiteralNode() { this.NodeType = ASTNodeType.FnLiteralNode; }
        public override string String()
        {
            return "lambda(length=" + countSemantic(this.nodes) + ")";
        }
        public override List<ASTNode> Children()
        {
            return this.nodes;
        }
    }

    public class IgnoreFormNode : ASTNode
    {
        ASTNode node;

        public IgnoreFormNode() { this.NodeType = ASTNodeType.IgnoreFormNode; }
        public override string String()
        {
            return "ignore";
        }

        public override List<ASTNode> Children()
        {
            List<ASTNode> nodes = new List<ASTNode>();
            nodes.Add(node);
            return nodes;
        }
    }

    public class RegexNode : ASTNode
    {
        public string Val;

        public RegexNode() { this.NodeType = ASTNodeType.RegexNode; }
        public override string String()
        {
            return "regex(" + this.Val + ")";
        }
    }

    public class SetNode : ASTNode
    {
        List<ASTNode> nodes;

        public SetNode() { this.NodeType = ASTNodeType.SetNode; }
        public override string String()
        {
            return "set(length" + countSemantic(this.nodes) + ")";
        }
        public override List<ASTNode> Children()
        {
            return this.nodes;
        }
    }

    public class VarQuoteNode : ASTNode
    {
        public string Val;

        public VarQuoteNode() { this.NodeType = ASTNodeType.VarQuoteNode; }
        public override string String()
        {
            return "varquote(" + Val.ToString() + ")";
        }
    }

    public class TagNode : ASTNode
    {
        public string Val;

        public TagNode() { this.NodeType = ASTNodeType.TagNode; }
        public override string String()
        {
            return "tag(" + Val + ")";
        }
    }


}