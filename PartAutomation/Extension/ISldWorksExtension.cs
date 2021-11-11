using System;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace PartAutomation.Extension
{
    public static class ISldWorksExtension
    {
        //一种模式，在某种状态下执行某些事情的方法，扩展方法
        public static void WithToggleState(this ISldWorks sw, swUserPreferenceToggle_e swUserPreference, bool desState, Action action)
        {
            //获取用户初始状态
            var sourceState = sw.GetUserPreferenceToggle((int)swUserPreference);

            //设置用户需要的状态，期望状态
            sw.SetUserPreferenceToggle((int)swUserPreference, desState);

            //执行我们需要的操作
            action?.Invoke();//无返回值的action委托

            //还原用户设置
            sw.SetUserPreferenceToggle((int)swUserPreference, sourceState);
        }
    }
}
