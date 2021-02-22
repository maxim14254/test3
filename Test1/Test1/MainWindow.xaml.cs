using System;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using System.Data;
using System.Text.RegularExpressions;


namespace Test1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        OpenFileDialog openFileDialog = new OpenFileDialog();
        List<Data> data;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage excelFile = null;

            openFileDialog.Filter = "Exel файлы: (*.xls,*.xlsx)|*.xls;*.xlsx";

            if (openFileDialog.ShowDialog() == true)
            {

                excelFile = new ExcelPackage(new FileInfo(openFileDialog.FileName));
            }
            else { return; }
            data = new List<Data>();
            ExcelWorksheet worksheet = excelFile.Workbook.Worksheets[0];
            var cls = worksheet.Cells[$"B2:B{worksheet.Cells.End.Row}"].Select(s => s.Text).ToList();
            var GroupCls = cls.GroupBy(s => s).Select(a => a.Key).ToList();
            for (int i = 2; i < worksheet.Dimension.End.Row; i++)
            {
                data.Add(new Data() { Name = worksheet.Cells[i, 1].Text, Cls = cls[i - 2] });
                data[i - 2].Cls.Insert(0, cls[i - 2]);
            }
            comboBox.ItemsSource = cls.GroupBy(s => s).Select(a => a.Key).ToList();
            dataGrid.ItemsSource = data;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedItem == null) return;
            dataGrid.ItemsSource = data.Where(s => s.Cls== comboBox.SelectedItem.ToString()).ToList();
        }

        string oldCls;
        private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                Data rezult = (Data)dataGrid.SelectedCells.Select(s => s.Item).First();
                var cc = comboBox.Items.Cast<string>();
                bool d = cc.Contains(rezult.Cls);
                if(d)
                {
                    return;
                }
                else
                {
                    dataGrid.SelectedCells.Select(s => s.Item).Cast<Data>().First().Cls = oldCls;
                    dataGrid.Items.Refresh();
                }
            }
            catch 
            {
                return; 
            }
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {

            Regex regex = new Regex("^" + textBox.Text + "(.*)");

            MatchCollection match = null;

            List<Data> data1 = new List<Data>();

            for (int i = 0; i < data.Count; i++)
            {
                match = regex.Matches(data[i].Name);

                if (match.Count > 0)
                {
                    data1.Add(data[i]);
                }
            }
            dataGrid.ItemsSource = data1;
        }

        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
           oldCls= dataGrid.SelectedCells.Select(s => s.Item).Cast<Data>().First().Cls;
        }
    }
}
