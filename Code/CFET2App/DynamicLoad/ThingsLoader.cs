using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Globalization;
using System.CodeDom;
using System.IO;

namespace Jtext103.CFET2.CFET2App
{
    public class DynamicThingsLoader
    {
        public DynamicThingsLoader(Cfet2Program host)
        {
            //得到带分隔符的执行文件夹路径
            string excuteDir = Environment.CurrentDirectory;
            if (!excuteDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                excuteDir += Path.DirectorySeparatorChar;
            }

            //如果文件夹不存在，新建Things和dll文件夹
            if(!Directory.Exists(excuteDir + "DynamicLoad" + Path.DirectorySeparatorChar + "Things"))
            {
                Directory.CreateDirectory(excuteDir + "DynamicLoad" + Path.DirectorySeparatorChar + "Things");
            }
            if (!Directory.Exists(excuteDir + "DynamicLoad" + Path.DirectorySeparatorChar + "dll"))
            {
                Directory.CreateDirectory(excuteDir + "DynamicLoad" + Path.DirectorySeparatorChar + "dll");
            }

            //获取代码
            var coder = new Coder();
            string code = coder.GetFullCode(excuteDir);

            //获取可用dll合集
            var dller = new Dller();
            string[] dlls = dller.CopyAndGetAllDlls(excuteDir);

            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
            CodeDomProvider objICodeCompiler = objCSharpCodePrivoder;

            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.AddRange(dlls);
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            CompilerResults cr = objICodeCompiler.CompileAssemblyFromSource(objCompilerParameters, code);

            if (cr.Errors.HasErrors)
            {
                string info = "动态加载Thing编译错误:\n";
                foreach (CompilerError err in cr.Errors)
                {
                    info += err.ErrorText + "\n";
                }
                throw new Exception(info);
            }

            Assembly objAssembly = cr.CompiledAssembly;
            object objAddThings = objAssembly.CreateInstance("DynamicAddThing");
            MethodInfo objMI = objAddThings.GetType().GetMethod("AddThing");

            var p = new object[1];
            p[0] = host;
            objMI.Invoke(objAddThings, p);
        }
    }
}
