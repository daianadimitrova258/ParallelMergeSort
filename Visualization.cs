using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace MergeSort
{
    public partial class Visualization : Form
    {
        int rowsCount = 0;
        List<Row> Rows = new List<Row>();
        string gosho = string.Empty;

        public Visualization()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Rows.Count > 0)
            {
                panel1.Controls.Clear();
                panel2.Controls.Clear();
                Rows.Clear();
            }
            string elements = textBox1.Text;

            string[] sArr = elements.Split(',');
            try
            {
                int[] arr = Array.ConvertAll(sArr, int.Parse);
                rowsCount = arr.Length;

                AddRowsInfo();

                MergeSort(0, arr, 0, arr.Length - 1, Color.Yellow);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Въведения масив не е с правилен формат!", "Внимание", MessageBoxButtons.OK);
            }
            CreateView();
        }

        private void AddRowsInfo()
        {
            int tempYValue = 0;
            int rowElementsCount = rowsCount % 2 == 0 ? rowsCount : rowsCount + 1;
            int rec = 0;
            int zeroRowIndex = 0;

            for (int i = 0; i <= rowsCount; i++)
            {
                Rows.Add(new Row { Number = i, YValue = tempYValue, XValue = 0, MaxElementsCount = rowElementsCount, Elements = new List<Series>() });
                if (rowElementsCount == 1)
                {
                    zeroRowIndex = i;
                    rec++;
                }

                if (rec == 0)
                {
                    rowElementsCount /= 2;
                }
                else
                {
                    var index = zeroRowIndex - (i + 1 - zeroRowIndex);
                    if (index < rowsCount && Rows.LastOrDefault().MaxElementsCount < rowsCount)
                        rowElementsCount = Rows[index].MaxElementsCount;
                }

                if (rowElementsCount % 2 != 0 && rowElementsCount != 1) 
                    rowElementsCount++;
                tempYValue += 50;
            }

            if (rowsCount % 2 != 0)
            {
                Rows.Add(new Row { Number = Rows.Count() + 1, YValue = Rows.LastOrDefault().YValue + 50, XValue = 0, MaxElementsCount = Rows.FirstOrDefault().MaxElementsCount, Elements = new List<Series>() });
            }

            Rows[0].XValue = 65;
            Rows.LastOrDefault().XValue = 65;
        }
        private void createButton(string text, Panel panel, int xCo, int yCo, Color color)
        {
            Button btn = new Button();
            btn.Height = 40;
            btn.Width = 60;
            btn.Location = new Point(xCo, yCo);
            btn.Text = text;
            btn.BackColor = color;
            panel.BeginInvoke((MethodInvoker)delegate () { panel.Controls.Add(btn); }); 
        }
        private void Merge(int dep, int[] input, int left, int middle, int right, Color color)
        {
            int[] leftArray = new int[middle - left + 1];
            int[] rightArray = new int[right - middle];

            List<int> sortedValues = new List<int>();

            Array.Copy(input, left, leftArray, 0, middle - left + 1);
            Array.Copy(input, middle + 1, rightArray, 0, right - middle);

            int i = 0;
            int j = 0;
            for (int k = left; k < right + 1; k++)
            {
                if (i == leftArray.Length)
                {
                    input[k] = rightArray[j];
                    sortedValues.Add(rightArray[j]);
                    j++;
                }
                else if (j == rightArray.Length)
                {
                    input[k] = leftArray[i];
                    sortedValues.Add(leftArray[i]);
                    i++;
                }
                else if (leftArray[i] <= rightArray[j])
                {
                    input[k] = leftArray[i];
                    sortedValues.Add(leftArray[i]);
                    i++;
                }
                else 
                {
                    input[k] = rightArray[j];
                    sortedValues.Add(rightArray[j]);
                    j++;
                }
            }
            if (sortedValues.Count == 1)
                return;

            var zeroRowNum = Rows.FirstOrDefault(r => r.Elements.FirstOrDefault(a=>a != null).Numbers.Count == 1).Number;
            var currentRow = Rows.FirstOrDefault(m => m.Number > zeroRowNum && (m.MaxElementsCount == sortedValues.Count || m.MaxElementsCount == sortedValues.Count + 1));
            currentRow.Elements.Add(new Series() { Color = color, Numbers = sortedValues });
        }
        private void MergeSort(int depth, int[] input, int left, int right, Color color)
        {
            List<int> temp = new List<int>();
            for (int i = left; i <= right; i++)
            {
                temp.Add(input[i]);
            }
            Rows.FirstOrDefault(r => r.Number == depth).Elements.Add(new Series() { Color = color, Numbers = temp });

            if (left < right)
            {
                int middle = (left + right - 1) / 2;
                Parallel.Invoke(
                    () => MergeSort(depth + 1, input, left, middle, Color.Red),
                    () => MergeSort(depth + 1, input, middle + 1, right, Color.Blue)
                );

                Merge(depth + 1, input, left, middle, right, color);
            }
        }
        private void CreateView()
        {
            Task.Factory.StartNew(() => 
            {
                foreach (var row in Rows)
                {
                    foreach (var list in row.Elements)
                    {
                        if (list != null)
                        {
                            for (int el = 0; el < list.Numbers.Count(); el++)
                            {
                                createButton(list.Numbers[el].ToString(), panel2, row.XValue, row.YValue, list.Color);
                                row.XValue += 65;
                            }
                            row.XValue += 65;
                            Thread.Sleep(1000);
                        }
                    }
                }
            });
        }
    }
}