#if WINDOWS
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Gnomic;
using Microsoft.Xna.Framework;

namespace Gnomic.Util
{
    /// <summary>
    /// A helper class for compiling scripts
    /// </summary>
    public class AssemblyBuilder
    {
        public static Assembly BuildInMemory(string code, out string errors)
        {
            CSharpCodeProvider cp = new CSharpCodeProvider();
            CompilerParameters cpar = new CompilerParameters();

            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;

            cpar.ReferencedAssemblies.Add("system.dll");
            cpar.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(Vector2)).Location); //"Microsoft.Xna.Framework, Version=3.0.0.0, PublicKeyToken=6d5c3888ef60e27d");
            cpar.ReferencedAssemblies.Add("Gnomic.dll");
            cpar.ReferencedAssemblies.Add("SquareHeroesTypes.dll");
            
            CompilerResults cr = cp.CompileAssemblyFromSource(cpar, new string[] { code });
            cp.Dispose();

            errors = "";
            if (cr.Errors.Count == 0 && cr.CompiledAssembly != null)
            {
                return cr.CompiledAssembly;
            }

            for (int i = 0; i < cr.Errors.Count; i++)
            {
                errors += cr.Errors[i].ErrorText + "\n";
            }
            return null;
        }

        public static T BuildInMemory<T>(string code, string className, object[] constructArgs, out string errors)
        {
            Assembly asm = BuildInMemory(code, out errors);
            if (asm != null)
            {
                Type ObjType = asm.GetType(className);
                return (T)Activator.CreateInstance(ObjType, constructArgs);
            }
            return default(T);
        }
    }
}
#endif