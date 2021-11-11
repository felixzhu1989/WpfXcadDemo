using System.Collections.Generic;
using SolidWorks.Interop.sldworks;

namespace PartAutomation.Extension
{
    public static class IModelDocExtension
    {
        //通过顺序获取基准面集合，扩展方法
        public static IEnumerable<IFeature> GetReFeature(this IModelDoc2 doc)
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
    }
}
