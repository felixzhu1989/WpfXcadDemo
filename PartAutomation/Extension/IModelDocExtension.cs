using System;
using System.Collections.Generic;
using SolidWorks.Interop.sldworks;

namespace PartAutomation.Extension
{
    public static class IModelDocExtension
    {
        //通过顺序获取基准面集合，扩展方法
        public static IEnumerable<IFeature> GetRefFeature(this IModelDoc2 doc)
        {
            //通过遍历获取基准面
            var feat = doc.FirstFeature() as IFeature; //第一个特征
            while (feat != null)
            {
                var name = feat.GetTypeName2();
                if (name == "RefPlane") //判断是否为基准面，对基准面用lookup查看GetTypeName2()方法的返回值
                {
                    yield return feat;//将所有的基准面压入集合
                }
                feat = feat.GetNextFeature() as IFeature; //遍历
            }
        }
        //在不刷新界面和属性页的情况下执行
        public static void WithNoRefresh(this IModelDoc2 doc, Action action)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            var activeView = doc.ActiveView as IModelView;

            var featMgr = doc.FeatureManager;
            try
            {
                
                //禁用显示详细绘制过程，关闭图形界面刷新，避免资源浪费。
                activeView.EnableGraphicsUpdate = false;
                //同时禁用绘制拉伸属性页更新
                featMgr.EnableFeatureTree = false;
                featMgr.EnableFeatureTreeWindow = false;
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                activeView.EnableGraphicsUpdate = true;
                featMgr.EnableFeatureTree = true;
                featMgr.EnableFeatureTreeWindow = true;
            }
        }
    }
}
