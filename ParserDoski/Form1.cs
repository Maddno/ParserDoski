using OfficeOpenXml;
using ParserDoski.Core;
using ParserDoski.Core._1000Dosok;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserDoski
{
    public partial class Form1 : Form
    {
        ParserWorker <Task<string[]>> parser;
        LinksParserWorker<Task<string[]>> linkParser;
        private string[] newDataArray;
        private List<string> linksList = new List<string>();

        public Form1()
        {
            InitializeComponent();

            DoskiParser doskiParser = new DoskiParser();
            DoskiParser doskiLinkParser = new DoskiParser();

            parser = new ParserWorker<Task<string[]>>(doskiParser);
            linkParser = new LinksParserWorker<Task<string[]>>(doskiLinkParser);

            LinksSavePath.TextChanged += DataLoadPath_TextChanged;
            parser.OnCompleted += Parser_OnCompleted;
            //parser.StatusUpdate += Parser_StatusUpdate;
            parser.OnNewData += Parser_OnNewData;
            linkParser.OnNewLinkData += Parser_OnNewLinkData;
            linkParser.OnCompleted += Parser_OnCompleted;
        }

        
        private void Parser_OnNewLinkData(object arg1, string[] arg2)
        {
            if (LoadingVisData.Value < LoadingVisData.Maximum)
            {
                LoadingVisData.Value++;
            }

            DataTitles.Items.AddRange(arg2);
        }

        private void Parser_OnNewData(object arg1, string[] arg2)
        {
            if (LoadingVisLinks.Value < LoadingVisLinks.Maximum)
            {
                LoadingVisLinks.Value++;
            }

            ListTitles.Items.AddRange(arg2);
        }

        private void Parser_OnCompleted(object obj)
        {
            MessageBox.Show("All works done!");
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            // Инициализация LoadingVisLinks
            int startValue = (int)NumericStart.Value;
            int endValue = (int)NumericEnd.Value;
            LoadingVisLinks.Minimum = 0;
            LoadingVisLinks.Maximum = endValue - startValue;
            LoadingVisLinks.Value = 0;

            parser.Settings = new DoskiSettings((int)NumericStart.Value, (int)NumericEnd.Value, CategorySelector.Text, LinksSavePath.Text, DataSavePath.Text);
            parser.Start(LinksSavePath.Text);
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            parser.Stop();
            linkParser.Stop();
        }

        private void ButtonLinkStart_Click(object sender, EventArgs e)
        {
            int rowCount = GetExcelRowCount(DataLoadPath.Text);

            int startIndex = (int)StartParseDataFrom.Value;
            int endIndex = rowCount;
            if (StopParseDataOn.Value != 0 ) { endIndex = (int)StopParseDataOn.Value +1; }
            LoadingVisData.Minimum = 0;
            LoadingVisData.Maximum = endIndex - startIndex;
            LoadingVisData.Value = 0;

            linkParser.Settings = new DoskiSettings((int)NumericStart.Value, (int)NumericEnd.Value, CategorySelector.Text, LinksSavePath.Text, DataSavePath.Text);
            linkParser.LinkStart(DataLoadPath.Text, DataSavePath.Text, (int)StartParseDataFrom.Value, (int)StopParseDataOn.Value);
        }

        private int GetExcelRowCount(string filePath)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Предполагается, что данные находятся на первом листе
                int rowCount = worksheet.Dimension.Rows;
                return rowCount;
            }
        }

        private void LinksSavePathSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл";
            openFileDialog.Filter = "Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LinksSavePath.Text = openFileDialog.FileName;
            }
        }

        private void DataSavePathSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл";
            openFileDialog.Filter = "Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                DataSavePath.Text = openFileDialog.FileName;
            }
        }

        private void DataParsePathSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл";
            openFileDialog.Filter = "Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                DataLoadPath.Text = openFileDialog.FileName;
            }
        }

        private void DataLoadPath_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DataLoadPath.Text))
            {
                // Установка значения второго текстового поля, если оно не задано
                DataLoadPath.Text = LinksSavePath.Text;
            }
        }

        private void ButtonClean_Click(object sender, EventArgs e)
        {
            ListTitles.Items.Clear();
            DataTitles.Items.Clear();
        }

    }
}
