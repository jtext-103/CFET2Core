using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.CFET2App
{
    public class Dller
    {
        /// <summary>
        /// 拷贝指定目录下dll到执行目录，并获取执行目录下的所有dll
        /// </summary>
        /// <returns></returns>
        public string[] CopyAndGetAllDlls(string excuteDir)
        {
            var dlls = new List<string>();

            //拷贝dll文件夹下所有文件到执行文件，不覆盖拷贝
            string dllsDirPath = excuteDir + "DynamicLoad" + Path.DirectorySeparatorChar + "dll";
            DirectoryInfo dllroot = new DirectoryInfo(dllsDirPath);
            foreach (FileInfo f in dllroot.GetFiles())
            {
                if (f.Name.EndsWith(".dll"))
                {
                    if(!File.Exists(excuteDir + f.Name))
                    {
                        File.Copy(f.FullName, excuteDir + f.Name, false);
                    }  
                }
            }
            
            DirectoryInfo root = new DirectoryInfo(excuteDir);
            foreach (FileInfo f in root.GetFiles())
            {
                if (f.Name.EndsWith(".dll") || f.Name.EndsWith(".exe"))
                {
                    dlls.Add(f.Name);
                }
            }

            //去掉一些用不了的dll，比如c++ x86的
            dlls = GetUsefulDlls(dlls);

            string[] result = new string[dlls.Count];
            for(int i = 0; i < dlls.Count; i++)
            {
                result[i] = dlls[i];
            }            
            return result;
        }

        //通过报错提示中的.dll来判断，以后找到更好的方法的话直接重构
        private List<string> GetUsefulDlls(List<string> dlls)
        {
            string errorMessage = null;
            var usefulDlls = new List<string>();
            foreach(var d in dlls)
            {
                CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
                CodeDomProvider objICodeCompiler = objCSharpCodePrivoder;
                CompilerParameters objCompilerParameters = new CompilerParameters();
                objCompilerParameters.ReferencedAssemblies.Add(d);
                objCompilerParameters.GenerateExecutable = false;
                objCompilerParameters.GenerateInMemory = true;
                CompilerResults cr = objICodeCompiler.CompileAssemblyFromSource(objCompilerParameters, "");
                if (cr.Errors.HasErrors)
                {
                    foreach (CompilerError err in cr.Errors)
                    {
                        if(errorMessage == null)
                        {
                            errorMessage += "加载dll时出错，请留意可能带来的影响：\n";
                        }
                        errorMessage += err.ErrorText + "\n";
                    }
                }
                else
                {
                    usefulDlls.Add(d);
                }
            }
            if(errorMessage != null)
            {
                Console.Write(errorMessage);
            }
            return usefulDlls;
        }
    }
}
