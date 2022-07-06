# 日志记录器 Logger
实现程序调试和运行时的日志信息输出

# **具体实现**

先创建日志工厂 LoggerFactory

```plain
//常规方式
Public class Test1
{
  public static ILoggerFactory LogFactory{get;set;}
  public Test()
  {
    //日志配置
    ILoggerConfiguration config = LoggerConfiguration.Default;
    config.SetMinimumLevel(LogLevel.Information);
    config.SetOverride(new Dictionary<string, LogLevel>
    {
      {"Microsoft",LogLevel.Error },
      {"System",LogLevel.Error }
    }
    config.AddAction((x) =>
    {
      Console.WriteLine(x);
    }
    
    //日志工厂
    LogFactory = LoggerFactory.Build(config);
  }
}
```
同样支持依赖注入
```plain
//依赖注入
builder.Services.AddSingleton<ILoggerFactory>(provider=>{
    //日志配置
    ILoggerConfiguration config = LoggerConfiguration.Default;
    config.SetMinimumLevel(LogLevel.Information);
    config.SetOverride(new Dictionary<string, LogLevel>
    {
      {"Microsoft",LogLevel.Error },
      {"System",LogLevel.Error }
    }
    config.AddAction((x) =>
    {
      Console.WriteLine(x);
    }
    
    //日志工厂
    return LoggerFactory.Build(config);
})
```
然后根据不同类库创建对象日志记录器
```plain
public class Test2
{
  private readonly ILogger _logger;
  public Test2()
  {
    _logger=Test1.LoggerFactory.CreateLogger<LoggerTestClass>();
  }
}
```
最后根据程序需求进行日志信息输出
```plain
public void logTest()
{
   _logger.Information("this is a test");
}
```
# 特色

### 自定义扩展

本项目以开闭原则为目标，满足对扩展开放，对修改关闭，支持IOC注入，只需通过对本项目ILoggerConfiguration、ILoggerFactory、ILogger 等接口继承实现，重新编写符合自己业务的日志记录器

### 日志事件记录扩展

通过对ILoggerConfiguration日志配置注册日志记录事件

```plain
/// <summary>
/// <summary>
/// Register logging events
/// 注册日志记录事件
/// </summary>
/// </summary>
/// <param name="action">日志记录触发事件</param>
/// <returns></returns>
ILoggerConfiguration AddAction(Action<LogMessage> action);
```
比如控制台输出：
```plain
config.AddAction((x) =>
{
  Console.WriteLine(x);
}
```
本项目通过扩展 RevitApiWrapper.Logger.FileActuator 以支持写入文件日志
```plain
/// <summary>
/// Write to file log
/// 写入文件日志
/// </summary>
/// <param name="configure">日志配置实例</param>
/// <param name="path">日志文件地址</param>
/// <param name="outputTemplate">日志输出模板</param>
/// <param name="interval">日志文件分隔周期</param>
/// <returns></returns>
static ILoggerConfiguration AddFile(
         this ILoggerConfiguration configure,
         string path = "logs/log.txt", 
         string outputTemplate 
         = "{LoggerTime:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}", 
         WrittenInterval interval = WrittenInterval.None)
```
具体实现如下：
```plain
using RevitApiWrapper.Logger.FileActuator.Extension;
using RevitApiWrapper.Logger.FileActuator.Model;

config.AddFile("logs/log.txt",interval:WrittenInterval.Day);
```
### 支持Json日志配置

本项目同时支持Json配置功能，代码如下

```plain
ILoggerConfiguration config = LoggerConfiguration.Default
                    .FromJson("LoggerSetting.json", "RAWLogger");
```
其中“LoggerSetting.json”为本地Json文件地址，“RAWLogger”为日志配置节点，本项目提供默认配置Json文件
```plain
{
  "RAWLogger": {
    "MinimumLevel": {
      "Default": "Verbose",
      "Override": {
        "Microsoft": "Error",
        "System": "Error",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "Extension": [
      {
        "AssemblyName": "RevitApiWrapper",
        "TypeName": "RevitApiWrapper.Logger.FileActuator.Actuator",
        "Args": {
          "Path": "Logs/log.txt",
          "OutputTemplate": "{LoggerTime:yyyy-MM-dd HH:mm:ss.fff zzz} || {Level} || {SourceContext} || {Message} || {Exception} ||end",
          "Interval": "Day"
        }
      }
    ]
  }
}
```
其中 MinimumLevel 为最小日志记录等级配置，Default 为默认最小等级，Override 为具有指定字段覆盖等级。
日志记录扩展同样支持Json文件配置，通过继承 RevitApiWrapper.Logger.Interface.LoggerEvent 抽象类，借助反射机制实现日志记录扩展自动注册，Json配置文件中 Extension 节点即为日志记录扩展，与IOC注入类似，AssemblyName 为注入程序集名称，TypeName 为类库全名，Args 为构造参数字典Key 和 Value 列表，另同样支持 Property 为扩展类库设置属性参数。本项目默认配置Json文件即添加了 RevitApiWrapper.Logger.FileActuator.Actuator 事件执行器扩展。

```plain
/// <summary>
/// Logger Event Actuator
/// 日志记录事件执行器
/// </summary>
public class Actuator : LoggerEvent
{
  public Actuator(Dictionary<string, string> args):base(args)
}
```
### AOP扩展

#### 实现

本项目通过引用 Castle.Core [官网点此处](http://www.castleproject.org/)，对日志记录器进行AOP拦截扩展

可通过继承 Castle.DynamicProxy.IInterceptor 和本项目 RevitApiWrapper.Logger.AOP.Interface.LoggerInterceptor 抽象类库，实现类库AOP拦截

```plain
namespace RevitApiWrapper.Logger.AOP.Interface
{
    /// <summary>
    /// Logger Interceptor
    /// 日志拦截器
    /// </summary>
    public abstract class LoggerInterceptor
    {
        /// <summary>
        /// 日志记录器实例
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public LoggerInterceptor(ILogger logger)
        {
            _logger=logger;
        }
    }
}
```
本项目通过继承第三方类库 Castle 和 本项目 LoggerInterceptor 创建了 RevitApiWrapper.Logger.AOP.VerboseLoggerInterceptor AOP，实现 Verbose 级别代码块运行流程记录，包括方法开始调用、方法结束调用。注：该方法会一定程度上消耗资源，请生产模式时，关闭 Verbose 级别。
```plain
/// <summary>
/// Logger Interceptor
/// 代码流程日志拦截器
/// </summary>
public class VerboseLoggerInterceptor : 
                 LoggerInterceptor, IInterceptor
```
ILogger 代理
```plain
/// <summary>
/// Loggger AOP
/// Logger AOP代理
/// </summary>
/// <typeparam name="T">日志类</typeparam>
/// <param name="logger">日志记录器实例</param>
/// <param name="interceptorType">拦截器实现类型器</param>
/// <returns></returns>
public static ILogger<T> AOP<T>(this ILogger<T> logger, 
             params Type[] interceptorType) where T : class
```
实现如下：
```plain
_logger = LogFactory.CreateLogger<LoggerTestClass>()
         .AOP(typeof(VerboseLoggerInterceptor));
```
#### Json注入

AOP 同样支持 Json 文件注入，通过 LoggerAOPHelper 类对Json文件进行读取

```plain
/// <summary>
/// Get interceptors from Json files
/// 从Json文件获取拦截器字典
/// </summary>
/// <param name="fileName">Json文件名</param>
/// <param name="nodeName">节点名</param>
/// <returns></returns>
/// <exception cref="InvalidOperationException"></exception>
public static Dictionary<string, string> LoadAOPJson
      (string fileName= "LoggerAOP.json",
       string nodeName= "RAWLoggerAOP")
```
再对 ILogger 进行代理
```plain
/// <summary>
/// Loggger AOP
/// Logger AOP代理
/// </summary>
/// <typeparam name="T">日志类</typeparam>
/// <param name="logger">日志记录器实例</param>
/// <param name="args">拦截器字典</param>
/// <returns></returns>
/// <exception cref="InvalidOperationException"></exception>
public static ILogger AOP(this ILogger logger,
                      Dictionary<string,string> args)
```
默认 AOP Json 文件如下
```plain
{
  "RAWLoggerAOP": {
    "RevitApiWrapper.Logger.AOP.VerboseLoggerInterceptor": "RevitApiWrapper"
  }
}
```
其中 RAWLoggerAOP 为节点名，子节点 Key 为类全名，Value 为程序集
# 完整实现

```plain
public static ILoggerFactory LogFactory { get; set; }
private readonly ILogger _logger;
public LoggerTestClass()
{
    ILoggerConfiguration config = LoggerConfiguration.Default
                                 .FromJson();
    LogFactory = LoggerFactory.Build(config);
    _logger = LogFactory.CreateLogger<LoggerTestClass>()
             .AOP(LoggerAOPHelper.LoadAOPJson());
}
public void logTest()
{
    _logger.Information("this is a test");
}
```

