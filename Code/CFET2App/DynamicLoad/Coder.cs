using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.CFET2App
{
    public class Coder
    {
        private string result;

        /// <summary>
        /// 从固定路径中查找所有以Thing.txt结尾的文件并将其添加到动态加载Thing的代码中
        /// </summary>
        /// <returns></returns>
        public string GetFullCode(string excuteDir)
        {
            result = null;
            result += "public class DynamicAddThing\n";
            result += "{\n";
            result += "public void AddThing(Jtext103.CFET2.CFET2App.Cfet2Program host)\n";
            result += "{\n";

            result += GetCodeFromFiles(excuteDir);

            result += "}\n";
            result += "}\n";
            return result;
        }

        private string GetCodeFromFiles(string excuteDir)
        {
            string code = null;

            string thingsDirPath = excuteDir + "DynamicLoad" + Path.DirectorySeparatorChar + "Things";

            DirectoryInfo root = new DirectoryInfo(thingsDirPath);
            foreach (FileInfo f in root.GetFiles())
            {
                if (f.Name.EndsWith("Thing.txt"))
                {
                    code += File.ReadAllText(f.FullName);
                    code += "\n";
                }
            }

            return code;
        }
    }
}
