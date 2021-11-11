using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using SolidWorks.Interop.swconst;
using Xarial.XCad.SolidWorks;

namespace NewDocument
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

        #region 新建SolidWorks文档
        private void BtnNewPart_OnClick(object sender, RoutedEventArgs e)
        {
            NewSolidWorksDoc(swUserPreferenceStringValue_e.swDefaultTemplatePart);
        }

        private void BtnNewAssy_OnClick(object sender, RoutedEventArgs e)
        {
            NewSolidWorksDoc(swUserPreferenceStringValue_e.swDefaultTemplateAssembly);
        }
        private void BtnNewDraw_OnClick(object sender, RoutedEventArgs e)
        {
            NewSolidWorksDoc(swUserPreferenceStringValue_e.swDefaultTemplateDrawing);
        }

        private void NewSolidWorksDoc(swUserPreferenceStringValue_e docTemplateType)
        {
            if (_swApp == null)
            {
                msgBox.Text = "未连接SolidWorks";
                return;
            }

            var templatePart = _swApp.Sw.GetUserPreferenceStringValue((int)docTemplateType);
            if (!File.Exists(templatePart))
            {
                msgBox.Text = "未配置默认模版";
                return;
            }

            var doc = _swApp.Sw.NewDocument(templatePart, 0, 300d, 300d);
        }
        #endregion

        #region 打开SolidWorks文档
        //拷贝SolidWorks文件到新建文件夹Model中，显示所有文件，将本地文件包括在项目中，设置属性为如果较新则复制
        private void BtnOpenPart_OnClick(object sender, RoutedEventArgs e)
        {
            OpenSolidWorksDoc("Part1.SLDPRT", swDocumentTypes_e.swDocPART);
        }

        private void BtnOpenAssy_OnClick(object sender, RoutedEventArgs e)
        {
            OpenSolidWorksDoc("Assem1.SLDASM", swDocumentTypes_e.swDocASSEMBLY);
        }

        private void BtnOpenDraw_OnClick(object sender, RoutedEventArgs e)
        {
            OpenSolidWorksDoc("Draw1.SLDDRW", swDocumentTypes_e.swDocDRAWING);
        }

        private void OpenSolidWorksDoc(string docName, swDocumentTypes_e docType)
        {
            if (_swApp == null)
            {
                msgBox.Text = "未连接SolidWorks";
                return;
            }

            string partDocPath =
                Path.Combine(Path.GetDirectoryName(typeof(MainWindow).Assembly.Location), "Model", docName);

            if (!File.Exists(partDocPath))
            {
                _swApp.ShowMessageBox($"{partDocPath} 此文件不存在");
                return;
            }

            int errors = 0;
            int warnings = 0;
            var partDoc = _swApp.Sw.OpenDoc6(partDocPath, (int)docType, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            if (partDoc == null)
            {
                _swApp.ShowMessageBox($"{partDocPath} 打开失败，错误代码 {errors}");
            }
        }

        #endregion
    }
}
