using System;
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
            IFeature reFeature = doc.GetReFeature().FirstOrDefault();//获取集合中的第一个

            reFeature.Select2(false, 0);//false只选中一个特征（基准面）
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
    }
}
