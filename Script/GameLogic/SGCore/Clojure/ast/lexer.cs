using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Clojure
{
    public class lexer
    {
        string ScriptText;
        int ScriptTexti;
        string FileName;
        ASTPos pos;
        ASTPos start;
        ASTPos lastPos;
        string val;
        List<ASTToken> tokens;
        public lexer(string fileName, string scriptText)
        {
            this.ScriptText = scriptText;
            this.FileName = fileName;
            this.ScriptTexti = 0;
            this.pos = new ASTPos(fileName, 1, 1, 0);
            this.start = new ASTPos(fileName, 1, 1, 0);
            this.lastPos = new ASTPos(fileName, 1, 1, 0);
            this.val = string.Empty;
            this.tokens = new List<ASTToken>();
        }
        public void Run()
        {
            stateFn fn = lexOuter;
            while (fn != null)
            {
                fn = fn();
            }
        }
        public ASTToken nextToken()
        {
            if (tokens.Count < 1) 
            {
                return null;// new ASTToken(ASTToken.TokenType.tokEOF, this.start, this.val);
            }
            ASTToken tok = tokens[0];
            tokens.RemoveAt(0);
            return tok;
        }
        public char next(ref bool isEOF)
        {
            if (ScriptTexti >= ScriptText.Length)
            {
                isEOF = true;
                return '0';
            }
            char chr = ScriptText[ScriptTexti];
            ScriptTexti++;
            this.lastPos.Set(this.pos);
            this.pos.Col++;
            this.pos.Offset++;
            if (chr == '\n')
            {
                this.pos.Line++;
                this.pos.Col = 1;
            }
            isEOF = false;
            this.val += chr;
            return chr;
        }
        public void emit(ASTToken.TokenType typ)
        {
            this.tokens.Add(new ASTToken(typ, this.start, this.val));
            this.skip();
        }
        public void skip()
        {
            this.start.Set(this.pos);
            this.val = string.Empty;
        }
        public void back()
        {
            if (this.lastPos == null)
            {
                Debug.LogError("back() call not preceded by a next()");
                return;
            }
            this.ScriptTexti--;
            this.pos.Set(this.lastPos);
            this.val = this.val.Substring(0, this.val.Length - 1);
        }
        public stateFn errorf(string str)
        {
            this.tokens.Add(new ASTToken(ASTToken.TokenType.tokError, this.start, str));
            return null;
        }
        public void scanUntil(string set)
        {
            bool isEOF = false;

            while (isEOF == false)
            {
                char chr = this.next(ref isEOF);
                for (int i = 0; i < set.Length; i++)
                {
                    if (chr == set[i])
                    {
                        this.back();
                        return;
                    }
                }
            }
        }

        public void scanWhile(System.Func<char, bool> fn)
        {
            bool isEOF = false;
            while (!isEOF)
            {
                char chr = this.next(ref isEOF);
                if (isEOF) return;
                if (!fn(chr))
                {
                    this.back();
                    return;
                }
            }
        }
        public bool IsSpace(char chr)
        {
            //'\t', '\n', '\v', '\f', '\r', ' '
            if (chr == ' ' || chr == '\r' || chr == '\t' || chr == '\n' || chr == '\v' || chr == '\f')
            {
                return true;
            }
            return false;
        }
        public bool isWhitespace(char chr)
        {

            return (IsSpace(chr) && chr != '\n') || chr == ',';
        }
        public bool isSymbolChar(char chr)
        {
            if (char.IsLetter(chr) || char.IsDigit(chr))
            {
                return true;
            }
            switch (chr)
            {
                case '*':
                case '+':
                case '!':
                case '-':
                case '_':
                case '?':
                case '/':
                case '.':
                case ':':
                case '$':
                case '=':
                case '>':
                case '<':
                case '&':
                case '#':
                    return true;
            }
            return false;
        }

        public bool isNumberChar(char chr) 
        {
            if (chr >= '0' && chr <= '9'||chr=='.') 
            {
                return true;
            }

            return false;
        }
        public delegate stateFn stateFn();
        public stateFn lexOuter()
        {
            bool isEOF = false;
            char chr = this.next(ref isEOF);
            if (isEOF == true)
            {
                this.emit(ASTToken.TokenType.tokEOF);
                return null;
            }
            switch (chr)
            {
                case ';':
                    return lexComment; //注释
                case '"':
                    return lexString;  //字符串
                case '\\':
                    return lexCharLiteral; //TODO 字符 未实行
                //
            }
            switch (chr)
            {
                case '\'':
                    this.emit(ASTToken.TokenType.tokApostrophe);
                    break;
                case '{':
                    this.emit(ASTToken.TokenType.tokLeftBrace);
                    break;
                case '}':
                    this.emit(ASTToken.TokenType.tokRightBrace);
                    break;
                case '[':
                    this.emit(ASTToken.TokenType.tokLeftBracket);
                    break;
                case ']':
                    this.emit(ASTToken.TokenType.tokRightBracket);
                    break;
                case '(':
                    this.emit(ASTToken.TokenType.tokLeftParen);
                    break;
                case ')':
                    this.emit(ASTToken.TokenType.tokRightParen);
                    break;
                case '\n':
                    this.emit(ASTToken.TokenType.tokNewline);
                    break;
                default:
                    goto afterSingles;
            }
            return lexOuter;
        afterSingles:
            if (isWhitespace(chr))
            {
                return lexOuter;
            }
            else if (chr >= '0' && chr <= '9'||chr=='-')
            {
                return lexNumber;
            }
            else if (isSymbolChar(chr))
            {
                return lexSymbol;
            }
            return this.errorf("unrecognized token starting with " + chr.ToString());
        }
        //lex Functions
        //向后扫描注释
        public stateFn lexComment()
        {
            this.scanUntil("\n");
            this.emit(ASTToken.TokenType.tokComment);
            return lexOuter;
        }
        //字符串
        public stateFn lexString()
        {
            bool escaped = false;
            bool isEOF = false;
            while (!isEOF)
            {
                char chr = this.next(ref isEOF);
                if (isEOF)
                {
                    return this.errorf("reached EOF before string closing quote");
                }
                switch (chr)
                {
                    case '"':
                        if (!escaped)
                        {
                            this.emit(ASTToken.TokenType.tokString);
                            return lexOuter;
                        }
                        escaped = false;
                        break;
                    case '\\':
                        escaped = !escaped;
                        break;
                    default:
                        escaped = false;
                        break;
                }
            }
            return null;
        }
        //字符
        public stateFn lexCharLiteral()
        {
            return lexOuter;
        }
        //空白符.
        public stateFn lexWhitespace()
        {
            this.scanWhile(isWhitespace);
            this.skip();
            return lexOuter;
        }
        //Number
        public stateFn lexNumber()
        {
            this.scanWhile(isNumberChar);
            this.emit(ASTToken.TokenType.tokNumber);
            return lexOuter;
        }
        //
        public stateFn lexSymbol()
        {
            this.scanWhile(isSymbolChar);
            this.emit(ASTToken.TokenType.tokSymbol);
            return lexOuter;
        }

    }

    public class ASTToken
    {
        public enum TokenType
        {
            tokEOF,     //文件结束.
            tokComment, //; 注释.
            tokError,
            tokString, //字符串
            tokApostrophe, //单引号
            tokLeftBrace,// {
            tokRightBrace,// }
            tokLeftBracket,//[
            tokRightBracket,//]
            tokBool,//true false
            tokNil,  //nil
            tokNumber,//number
            tokLeftParen,// (
            tokRightParen,// )
            tokNewline,// \n newline
            tokSymbol,

        }
        public ASTPos Pos;
        public TokenType Typ;
        public string Val;

        public ASTToken(TokenType typ, ASTPos pos, string val)
        {
            this.Typ = typ;
            this.Pos = pos;
            this.Val = val;
        }
    }

    public class ASTPos
    {
        public string Name;
        public int Offset;
        public int Line;
        public int Col;
        public void Set(ASTPos pos)
        {
            this.Name = pos.Name;
            this.Offset = pos.Offset;
            this.Line = pos.Line;
            this.Col = pos.Col;
        }
        public ASTPos(string name, int line, int col, int offset)
        {
            this.Name = name;
            this.Line = line;
            this.Col = col;
            this.Offset = offset;
        }

    }

}