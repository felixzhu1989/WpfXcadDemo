using System.Runtime.InteropServices;
using Xarial.XCad.SolidWorks;

namespace MyFirstAddin
{
    //.Net Framework 类库项目 

    /* 1.注册插件（手动）
     * 为了让SolidWorks知道我们插件的路径
     * 需要用到RegAsm.exe注册插件（地址：C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe）
     * 将RegAsm.exe复制到项目下，设置属性将其复制到输出目录
     *
     * 命令行显示如下信息
     * ***************************************
        .\RegAsm.exe
        ---------------------------------------
        Microsoft .NET Framework 程序集注册实用工具版本 4.8.4084.0
        (适用于 Microsoft .NET Framework 版本 4.8.4084.0)
        版权所有 (C) Microsoft Corporation。保留所有权利。

        语法: RegAsm 程序集名称 [选项]
        选项:
        /unregister          注销类型
        /tlb[:文件名]      将程序集导出到指定类型库
                             并注册它
        /regfile[:文件名]  生成具有指定名称的 reg 文件
                             而不是注册类型。此选项
                             不能与 /u 或 /tlb 选项一起使用
        /codebase            设置注册表中的基本代码
        /registered          只引用已注册的类型库
        /asmpath:目录   在此处查找程序集引用
        /nologo              禁止 RegAsm 显示徽标
        /silent              静态模式。禁止显示成功消息
        /verbose             显示额外的信息
        /? or /help          显示此用法消息
        ---------------------------------------
        .\RegAsm.exe MyFirstAddin.dll /codebase
        Microsoft .NET Framework 程序集注册实用工具版本 4.8.4084.0
        (适用于 Microsoft .NET Framework 版本 4.8.4084.0)
        版权所有 (C) Microsoft Corporation。保留所有权利。

        RegAsm : warning RA0000 : 使用 /codebase 注册未签名的程序集可能会导致程序集妨碍可能在同一台计算机上安装的其他应用程序。/codebase 开关旨在仅用于已签名的程序集。请为您的程序集提供一个强名称并重新注册它。
        成功注册了类型
     *  ***************************************
     *
     *  2.卸载插件（手动）
     *  ***************************************
        cd D:\CSharp\repos\WpfXcadDemo\MyFirstAddin\bin\Debug
        .\RegAsm.exe MyFirstAddin.dll /u
        ---------------------------------------
        Microsoft .NET Framework 程序集注册实用工具版本 4.8.4084.0
        (适用于 Microsoft .NET Framework 版本 4.8.4084.0)
        版权所有 (C) Microsoft Corporation。保留所有权利。

        成功注销了类型
     *  ***************************************
     *
     *  3.xCAD已经为我们封装了RegAsm注册和卸载过程，清理项目即可注销插件
     *
     */

    [ComVisible(true)]
    public class MyFirstAddIn:SwAddInEx
    {
        public override void OnConnect()
        {
            Application.ShowMessageBox("Hello xCAD");
        }
    }
}
