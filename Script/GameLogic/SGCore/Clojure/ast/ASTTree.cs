using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
namespace Clojure
{
    public class ASTTree
    {
        public List<ASTNode> Roots = new List<ASTNode>();
        int peekCount;
        ASTToken tok;
        lexer lex;
        public static ASTTree ParseString(string fileName, string scriptText)
        {
            ASTTree tree = new ASTTree();
            tree.Parse(fileName, scriptText);
            return tree;
        }
        public void Parse(string fileName, string scriptText)
        {
            //解析出所有token
            this.lex = new lexer(fileName, scriptText);
            this.lex.Run();

            while (true)
            {
                ASTNode node = this.parse();
                if (node == null)
                {
                    break;
                }
                if (ASTNode.isSemantic(node))
                {
                    this.Roots.Add(node);
                }
            }
        }

        public ASTNode parse()
        {
            ASTToken tok = this.next();
            switch (tok.Typ)
            {
                case ASTToken.TokenType.tokSymbol:
                    switch (tok.Val.Trim())
                    {
                        case "nil":
                            NilNode nilNode = new NilNode();
                            nilNode.Position = tok.Pos;
                            return nilNode;
                        case "true":
                        case "false":
                            BoolNode boolNode = new BoolNode();
                            boolNode.Position = tok.Pos;
                            boolNode.Val = tok.Val == "true";
                            return boolNode;
                        default:
                            SymbolNode symNode = new SymbolNode();
                            symNode.Position = tok.Pos;
                            symNode.Val = tok.Val.Trim();
                            return symNode;
                    }
                case ASTToken.TokenType.tokComment:
                    CommentNode comeNode = new CommentNode();
                    comeNode.Position = tok.Pos;
                    comeNode.Text = tok.Val;
                    return comeNode;
                case ASTToken.TokenType.tokLeftParen:
                    return parseList(tok);
                case ASTToken.TokenType.tokLeftBrace:
                    return parseMap(tok);
                case ASTToken.TokenType.tokNewline:
                    NewlineNode lineNode = new NewlineNode();
                    lineNode.Position = tok.Pos;
                    return lineNode;
                case ASTToken.TokenType.tokNumber:
                    return parseNumber(tok);
                case ASTToken.TokenType.tokString:
                    StringNode strNode = new StringNode();
                    strNode.Position = tok.Pos; 
                    strNode.Val = tok.Val.Substring(2, tok.Val.Length - 3);
                    return strNode;
                case ASTToken.TokenType.tokLeftBracket:
                    return this.parseVector(tok);
                case ASTToken.TokenType.tokEOF:
                    return null;
                default:
                    this.unexpected(tok);
                    return null;
            }

        }

        public void backup()
        {
            this.peekCount++;
            if (this.peekCount > 1)
            {
                Debug.LogError("backup() called twice consecutively");
            }
        }
        public ASTToken next()
        {
            if (this.peekCount > 0)
            {
                this.peekCount--;
            }
            else
            {
                this.tok = nextToken();
            }
            return this.tok;
        }
        public ASTToken nextToken()
        {
            ASTToken tok = this.lex.nextToken();

            if (tok.Typ == ASTToken.TokenType.tokError)
            {
                Debug.LogError(tok.ToString());
            }
            return tok;
        }

        public ASTNode parseNumber(ASTToken tok)
        {
            NumberNode numberNode = new NumberNode();
            numberNode.Position = tok.Pos;
            int intNum;
            if (int.TryParse(tok.Val, out intNum))
            {
                numberNode.Type = NumberNode.NumberType.INT;
                numberNode.NumberVal = intNum;
            }
            else 
            {
                numberNode.Type = NumberNode.NumberType.FLOAT;
                numberNode.NumberVal = float.Parse(tok.Val);
            }
            return numberNode;
        }
        public ASTNode parseList(ASTToken start)
        {
            List<ASTNode> nodes = new List<ASTNode>();
            while (true)
            {
                ASTToken tok = this.next();
                switch (tok.Typ)
                {
                    case ASTToken.TokenType.tokRightParen:
                        ListNode lstNode = new ListNode();
                        lstNode.Position = start.Pos;
                        lstNode.nodes = nodes;
                        return lstNode;
                    case ASTToken.TokenType.tokEOF:
                        this.unexpectedEOF(tok);
                        break;
                }
                this.backup();
                ASTNode node = this.parse();
                if (ASTNode.isSemantic(node))
                {
                    nodes.Add(node);
                }
            }
        }

        public ASTNode parseMap(ASTToken start)
        {
            List<ASTNode> nodes = new List<ASTNode>();
            while (true)
            {
                ASTToken tok = this.next();
                switch (tok.Typ)
                {
                    case ASTToken.TokenType.tokRightBrace:
                        MapNode mapNode = new MapNode();
                        mapNode.Position = start.Pos;
                        mapNode.nodes = nodes;
                        return mapNode;
                    case ASTToken.TokenType.tokEOF:
                        this.unexpectedEOF(tok);
                        break;
                }
                this.backup();
                ASTNode node = this.parse();
                if (ASTNode.isSemantic(node))
                {
                    nodes.Add(node);
                }
            }
        }

        public ASTNode parseVector(ASTToken start)
        {
            List<ASTNode> nodes = new List<ASTNode>();
            while (true)
            {
                ASTToken tok = this.next();
                switch (tok.Typ)
                {
                    case ASTToken.TokenType.tokRightBracket:
                        VectorNode vecNode = new VectorNode();
                        vecNode.Position = tok.Pos;
                        vecNode.nodes = nodes;
                        return vecNode;
                    case ASTToken.TokenType.tokEOF:
                        this.unexpectedEOF(tok);
                        break;
                }
                this.backup();
                ASTNode node = this.parse();
                if (ASTNode.isSemantic(node))
                {
                    nodes.Add(node);
                }
            }
        }
        public void unexpected(ASTToken token)
        {
            this.errorf(tok.Pos, "unexpected token " + tok.Val);
        }
        public void unexpectedEOF(ASTToken tok)
        {
            this.errorf(tok.Pos, "unexpected token " + tok.Val);
        }
        public void errorf(ASTPos pos, string str)
        {
            Debug.LogError("parseError");
        }
    }

}