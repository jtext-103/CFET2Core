﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.Reflection;
using Jtext103.CFET2.Core;

namespace Jtext103.CFET2.CFET2App.DynamicLoad
{
    public class DynamicThingsLoader
    {
        private string dynamicThingsRootPath;

        private string dynamicDllsFilePath;

        private string separator;

        private List<ThingModel> things;

        private List<string> dlls;

        private Cfet2Program cfetHost;

        public DynamicThingsLoader(Cfet2Program host)
        {
            cfetHost = host;

            separator = Path.DirectorySeparatorChar.ToString();
            dynamicThingsRootPath = "." + separator + "DynamicLoad" + separator + "DynamicThing";
            dynamicDllsFilePath = "." + separator + "DynamicLoad" + separator + "DynamicDll";

            things = new List<ThingModel>();
            dlls = new List<string>();

            LoadAndCopyDllsInDir(dynamicDllsFilePath);

            LoadAllThings(dynamicThingsRootPath);

            SetDlls();

            AddAllThings();
        }

        //获取并存储一个文件夹下所有dll的路径并拷贝到执行目录，不会递归查找
        private void LoadAndCopyDllsInDir(string dirPath)
        {
            DirectoryInfo root = new DirectoryInfo(dirPath);

            foreach (FileInfo file in root.GetFiles())
            {
                if (file.Extension.ToLower() == ".dll")
                {
                    try
                    {
                        File.Copy(file.FullName, "." + separator + file.Name);
                    }
                    catch
                    {
                        //Console.WriteLine("Please note a dll copy failed: " + file.FullName);
                    }
                }
            }
        }

        //递归加载所有的Thing
        private void LoadAllThings(string rootPath)
        {
            DirectoryInfo root = new DirectoryInfo(rootPath);

            //搜索这一层所有文件
            foreach (FileInfo file in root.GetFiles())
            {
                //判断为一个Thing
                if (file.Name == "Config.json")
                {
                    var newThing = new ThingModel();
                    if (newThing.LoadConfig(file.FullName))
                    {
                        things.Add(newThing);
                    }
                    else
                    {
                        Console.WriteLine("Failed to load Thing: " + root.Name + "!");
                    }

                    //如果有dll文件夹，加载其中的内容到dlls中
                    if (Directory.Exists(rootPath + separator + "dll"))
                    {
                        LoadAndCopyDllsInDir(rootPath + separator + "dll");
                    }
                }
            }

            //递归搜索子文件夹
            foreach (DirectoryInfo dir in root.GetDirectories())
            {
                LoadAllThings(dir.FullName);
            }
        }

        private void SetDlls()
        {
            DirectoryInfo root = new DirectoryInfo("./");

            foreach (FileInfo file in root.GetFiles())
            {
                if (file.Extension.ToLower() == ".dll")
                {
                    dlls.Add(file.FullName);
                }
            }
        }

        //将所有Thing实例化并挂载到CFET
        private void AddAllThings()
        {
            var builder = new ContainerBuilder();

            //添加所有dll
            foreach (var dll in dlls)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    builder.RegisterAssemblyTypes(assembly);
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                catch
                {
                    Console.WriteLine("Please note a dll load failed: " + dll);
                }
            }

            IContainer container;
            ILifetimeScope scope;

            try
            {
                container = builder.Build();
                scope = container.BeginLifetimeScope();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }


            //添加每个Thing
            foreach (var thing in things)
            {
                Type type = null;
                bool isTypeOk = false;
                string nowDll;

                //找到第一个和DllName匹配的dll
                foreach (var dll in dlls)
                {
                    FileInfo dllFile = new FileInfo(dll);
                    if (dllFile.Name == thing.Config.DllName)
                    {
                        try
                        {
                            type = Assembly.LoadFrom(dll).GetType(thing.Config.Type);
                            
                        }
                        catch
                        {
                            continue;
                        }
                        isTypeOk = true;
                        break;
                    }
                }

                if (!isTypeOk)
                {
                    throw new Exception("Failed to load thing: " + thing.Name + "!");
                }

                try
                {
                    dynamic ins = scope.Resolve(type);
                    //type.GetType();
                    cfetHost.MyHub.TryAddThing(ins, thing.MountPath, thing.Name, thing.Config.InitObj);

                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }
            }
        }
    }
}
