using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RevitApiWrapper.Assembly
{
    /// <summary>
    /// 外部程序集加载器
    /// </summary>
    /// <remarks>
    /// <example>
    /// 使用方法：
    /// <code>
    ///     var loader = new AssemblyLoader();
    ///     var dlls = new List&lt;string&gt;
    ///     {
    ///         "MyCoolExtention.dll",
    ///         "MyPerfectAddins.dll"
    ///     };
    ///     loader.LoadDlls(dlls);
    /// </code>
    /// </example>
    /// </remarks>
    public class AssemblyLoader
    {

        private const string PATH = "PATH";

        public ReadOnlyCollection<string> SearchPaths { get; private set; }

        /// <summary>
        /// 构造一个默认加载器，搜索默认目录为当前程序集所在目录。
        /// </summary>
        public AssemblyLoader()
        {
            var paths = new List<string>
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Path.GetDirectoryName(GetType().Assembly.Location)
            };
            paths = paths.Distinct().ToList();
            SearchPaths = new ReadOnlyCollection<string>(paths);
        }

        /// <summary>
        /// 加载程序集找不到引用时搜索路径
        /// </summary>
        /// <param name="searchPaths">待搜索的环境目录变量</param>
        public AssemblyLoader(List<string> searchPaths)
        {
            SearchPaths = new ReadOnlyCollection<string>(searchPaths);
        }


        /// <summary>
        /// 加载dlls,paths为绝对路径，或者是在相对于当前dll位置的相对路径
        /// </summary>
        /// <param name="paths">待加载的DLL程序集列表</param>
        public void LoadDlls(List<string> paths)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
                foreach (string path in paths)
                {
                    if (File.Exists(path))
                    {
                        try
                        {
                            System.Reflection.Assembly.LoadFrom(path);
                        }
                        catch (Exception ex)
                        {
                            Debug.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.StackTrace.ToString());
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
            }
        }
        
        private System.Reflection.Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            foreach (var item in SearchPaths)
            {
                var file = string.Format("{0}.dll", Path.Combine(item, assemblyName.Name));
                if (File.Exists(file))
                {
                    try
                    {
                        return System.Reflection.Assembly.LoadFrom(file);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return args.RequestingAssembly;
        }
        
        
        /// <summary>
        /// 增加路径环境变量
        /// </summary>
        /// <param name="input"></param>
        public static void AddEnvironmentPath(params string[] input)
        {
            var path = new[] { Environment.GetEnvironmentVariable(PATH) ?? string.Empty };
            //加在最前面
            var newPath = string.Join(Path.PathSeparator.ToString(), input.Concat(path));
            Environment.SetEnvironmentVariable(PATH, newPath);
        }
    }
}
