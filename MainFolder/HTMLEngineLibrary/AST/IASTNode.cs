using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal interface IASTNode
    {
        VariableStorage Variables { get; }
        object Eval();
    }
}
