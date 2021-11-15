using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Xarial.XCad.SolidWorks;
using PartAutomation.Extension;

namespace PartAutomation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //NuGet Xarial.XCad.SolidWorks
        private ISwApplication _swApp;
        public MainWindow()
        {
            InitializeComponent();
        }

        #region 连接SolidWorks程序
        private void BtnConn_OnClick(object sender, RoutedEventArgs e)
        {
            //从进程中获取程序
            var swProcess = Process.GetProcessesByName("SLDWORKS");

            if (!swProcess.Any())
            {
                msgBox.Text = "没有打开SolidWorks";
                return;
            }
            //可能有多个程序开着,获取第一个
            _swApp = SwApplicationFactory.FromProcess(swProcess.First());
            msgBox.Text = _swApp.Version.ToString();
        }
        #endregion

        private void BtnDraw_OnClick(object sender, RoutedEventArgs e)
        {
            //新建一个草图,并绘制一根直线，模拟真实的操作过程
            #region 判断执行条件
            var doc = _swApp.Sw.IActiveDoc2;
            if (doc == null)
            {
                msgBox.Text = "没有打开文档";
                return;
            }

            if (doc.GetType() != (int)swDocumentTypes_e.swDocPART)
            {
                msgBox.Text = "当前打开的不是零件";
                return;
            }
            #endregion

            //使用扩展方法
            IFeature refFeature = doc.GetRefFeature().FirstOrDefault();//获取集合中的第一个

            refFeature.Select2(false, 0);//false只选中一个特征（基准面）
            var skeMgr = doc.SketchManager;
            skeMgr.InsertSketch(true);//插入草图

            //获取草图激活捕捉设置是否打开,设置选项,在选项下执行特定过程,再回复选项
            _swApp.Sw.WithToggleState(swUserPreferenceToggle_e.swSketchInference, false, () =>
            {
                skeMgr.CreateLine(0, 0, 0, double.Parse(txtEndX.Text) / 1000d, double.Parse(txtEndY.Text) / 1000d, 0);
            });

            skeMgr.InsertSketch(true);
            //缩放视图
            doc.ViewZoomtofit2();
            doc.Save();
        }

        private void BtnExtrude_OnClick(object sender, RoutedEventArgs e)
        {
            #region 判断执行条件
            var doc = _swApp.Sw.IActiveDoc2;
            if (doc == null)
            {
                msgBox.Text = "没有打开文档";
                return;
            }

            if (doc.GetType() != (int)swDocumentTypes_e.swDocPART)
            {
                msgBox.Text = "当前打开的不是零件";
                return;
            }
            #endregion

            doc.WithNoRefresh(() =>
            {
                //早绑定FeatureManager
                var featMgr = doc.FeatureManager;

                //先绘制一个矩形
                var frontPanel = doc.GetRefFeature().FirstOrDefault();
                frontPanel.Select2(false, 0);
                //早绑定SketchManager
                var skeMgr = doc.SketchManager;
                skeMgr.InsertSketch(true);
                _swApp.Sw.WithToggleState(swUserPreferenceToggle_e.swSketchInference, false, () =>
                {
                    skeMgr.CreateCenterRectangle(0, 0, 0, double.Parse(txtWidth.Text) / 2000d, double.Parse(txtWidth.Text) / 2000d, 0);
                });
                //然后拉伸
                featMgr.FeatureExtrusion3(true, false, true, (int)swEndConditions_e.swEndCondBlind, 0, double.Parse(txtDepth.Text) / 1000d, 0, false, false, false, false, 0, 0, false, false, false, false, true, true, true, (int)swStartConditions_e.swStartSketchPlane, 0, false);

                //缩放视图
                doc.ViewZoomtofit2();
                doc.Save();
            });
        }

        private void BtnSelect_OnClick(object sender, RoutedEventArgs e)
        {
            #region 判断执行条件
            var doc = _swApp.Sw.IActiveDoc2;
            if (doc == null)
            {
                msgBox.Text = "没有打开文档";
                return;
            }
            #endregion

            //早绑定选择管理器
            var seleMgr = doc.ISelectionManager;
            //获取选择数量
            var count = seleMgr.GetSelectedObjectCount2(-1);//-1 = All selections regardless of marks
            if (count < 1)
            {
                msgBox.Text = "当前没有任何选择对象";
            }
            var data = new List<string>(count);
            for (int i = 1; i < count; i++)//从1开始
            {
                var mark = seleMgr.GetSelectedObjectMark(i);
                var type = seleMgr.GetSelectedObjectType3(i, mark);//类型
                var obj = seleMgr.GetSelectedObject6(i, mark);//对象
                var selePosition = seleMgr.GetSelectionPoint2(i, mark) as double[];//位置
                string info = selePosition == null ? $"Index:{i};\tMark:{mark};\tType:{(swSelectType_e)type};" : $"Index:{i};\tMark:{mark};\tType:{(swSelectType_e)type};\tPosition:{selePosition[0]},{selePosition[1]},{selePosition[2]}";
                if (obj is IFeature feat)
                {
                    var featInfo = $"Feature Name:{feat.Name};\tFeature Type:{feat.GetTypeName2()}";
                    info += featInfo;
                }
                data.Add(info);
            }
            selectionList.ItemsSource = data;
        }

        private void BtnProperty_OnClick(object sender, RoutedEventArgs e)
        {
            #region 判断执行条件
            var doc = _swApp.Sw.IActiveDoc2;
            if (doc == null)
            {
                msgBox.Text = "没有打开文档";
                return;
            }
            #endregion

            var cusMgr = doc.Extension.CustomPropertyManager[""];
            object names = new object(),
                values = new object(),
                types = new object(),
                resolved = new object(),
                links = new object();
            cusMgr.GetAll3(ref names, ref types, ref values, ref resolved, ref links);
            var cusNames = names as string[];
            var cusValues = values as string[];
            var cusTypes = (types as int[]).Select(t => (swCustomInfoType_e)t).ToArray();
            var list = new List<string>();
            for (int i = 0; i < cusNames.Length; i++)
            {
                var name = cusNames[i];
                var type = cusTypes[i];
                var value = cusValues[i];
                var info = $"Name:{name};\tType:{type};\tValue={value}";
                list.Add(info);
            }
            propertiesList.ItemsSource = list;
        }

        private void BtnAddProperty_OnClick(object sender, RoutedEventArgs e)
        {
            #region 判断执行条件
            var doc = _swApp.Sw.IActiveDoc2;
            if (doc == null)
            {
                msgBox.Text = "没有打开文档";
                return;
            }
            #endregion

            var cusMgr = doc.Extension.CustomPropertyManager[""];
            var result = (swCustomInfoAddResult_e)cusMgr.Add3("Test", (int)swCustomInfoType_e.swCustomInfoText, "Test", (int)swCustomPropertyAddOption_e.swCustomPropertyDeleteAndAdd);
            if (result != swCustomInfoAddResult_e.swCustomInfoAddResult_AddedOrChanged)
            {
                msgBox.Text = $"添加自定义属性失败，失败原因:{result}";
            }
        }

        private void BtnDeleteProperty_OnClick(object sender, RoutedEventArgs e)
        {
            #region 判断执行条件
            var doc = _swApp.Sw.IActiveDoc2;
            if (doc == null)
            {
                msgBox.Text = "没有打开文档";
                return;
            }
            #endregion

            var cusMgr = doc.Extension.CustomPropertyManager[""];
            var result =(swCustomInfoDeleteResult_e) cusMgr.Delete2("Test");
            if (result != swCustomInfoDeleteResult_e.swCustomInfoDeleteResult_OK)
            {
                msgBox.Text = $"删除自定义属性\"Test\"失败，失败原因:{result}";
            }
        }

        private void BtnEquation_OnClick(object sender, RoutedEventArgs e)
        {
            #region 判断执行条件
            var doc = _swApp.Sw.IActiveDoc2;
            if (doc == null)
            {
                msgBox.Text = "没有打开文档";
                return;
            }
            #endregion
            //方程式管理器
            var equMgr = doc.GetEquationMgr();
            var count = equMgr.GetCount();//方程式数量
            List<string> equations = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                var equation = equMgr.Equation[i];
                equations.Add(equation);
            }
            equationList.ItemsSource = equations;
        }
    }
}
