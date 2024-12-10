using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Clojure
{

    public class Context
    {
        Dictionary<string, ASTNode> Vars;
        Context Parent;

        public Context() 
        {
            Vars = new Dictionary<string, ASTNode>();
        }

        public Context(Context parent) 
        {
            this.Vars = new Dictionary<string, ASTNode>();
            this.Parent = parent;
        }

        public ASTNode Replace(string symbolName,ASTNode node) 
        {
            Vars[symbolName] = node;
            return node;
        }
        public ASTNode Find(string symbolName) 
        {
            if (this.Vars.ContainsKey(symbolName))
            {
                return this.Vars[symbolName];
            }
            else
            {
                if (Parent != null) 
                {
                  return  Parent.Find(symbolName);
                }
            }
            return null;
        }

        public void Define(string symbolName, ASTNode var) 
        {
            this.Vars.Add(symbolName,var);
        }


      
     
    }



}