using App.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZagadnienieTransportowe.Controls;

namespace ZagadnienieTransportowe.Forms
{
   public partial class MainForm : Form
   {
      private int _columnIndex = 0;
      private int _rowIndex = 0;
      private int _maxAvailableRows = 5;
      private int _maxAvailableColumns = 5;

      private int _offset = 1;
      private int _controlY = 20;
      private int _controlX = 50;
      private int _start_X_Offset;
      private int _start_Y_Offset;

      private readonly Dictionary<string, MarkedTextBox> GridMap;
      private readonly Dictionary<int, MarkedTextBox> Odbiorcy;
      private readonly Dictionary<int, MarkedTextBox> Dostawcy;

      public MainForm()
      {
         InitializeComponent();
         _start_X_Offset = 4 * _offset + 2 * _controlX;
         _start_Y_Offset = 4 * _offset + 2 * _controlY;
         grid.AutoScroll = true;
         GridMap = new Dictionary<string, MarkedTextBox>();
         Odbiorcy = new Dictionary<int, MarkedTextBox>();
         Dostawcy = new Dictionary<int, MarkedTextBox>();
      }

      private void BtnAddColumnClicked(object sender, EventArgs e)
      {
         if (_columnIndex >= _maxAvailableColumns)
         {
            MessageBox.Show("Osiągnięto maksymalną możliwą liczbę odbiorców");
            return;
         }

         if (_columnIndex == 0)
         {
            var lblPopyt = CreateLabel("Popyt:", 3 * _offset + _controlX, _offset-1);
            lblPopyt.TextAlign = ContentAlignment.MiddleRight;
            grid.Controls.Add(lblPopyt);
         }

         var x = _start_X_Offset + _offset + ((2 * _offset + _controlX) * _columnIndex); 
         var tbForPopyt = CreateTextBox(x, _offset);
         var lblOdbiorcy = CreateLabel($"O{_columnIndex + 1}", x, 3 *_offset + _controlY);
         Odbiorcy[_columnIndex] = tbForPopyt;

         grid.Controls.Add(tbForPopyt);
         grid.Controls.Add(lblOdbiorcy);

         GenerateMissingGridTextBoxes();
         _columnIndex++;
      }


      private void BtnAddRowClicked(object sender, EventArgs e)
      {
         if (_rowIndex >= _maxAvailableRows)
         {
            MessageBox.Show("Osiągnięto maksymalną możliwą liczbę dostawców");
            return;
         }

         if (_rowIndex == 0)
         {
            var lblPopyt = CreateLabel("Podaż:", _offset, 3 * _offset + _controlY);
            lblPopyt.TextAlign = ContentAlignment.MiddleRight;
            grid.Controls.Add(lblPopyt);
         }

         var y = _start_Y_Offset + _offset + ((2 * _offset + _controlY) * _rowIndex);
         var tbForPodaz = CreateTextBox(_offset, y);
         var lblOdbiorcy = CreateLabel($"D{_rowIndex + 1}", 3 * _offset + _controlX, y);
         Dostawcy[_rowIndex] = tbForPodaz;

         grid.Controls.Add(tbForPodaz);
         grid.Controls.Add(lblOdbiorcy);

         GenerateMissingGridTextBoxes();
         _rowIndex++;
      }

      private Label CreateLabel(string v, int x, int y)
      {
         var lbl = new Label();
         lbl.Width = _controlX;
         lbl.Height = _controlY;
         lbl.Text = v;
         lbl.Location = new Point(x, y);
         lbl.TextAlign = ContentAlignment.MiddleCenter;
         lbl.Font = new Font(lbl.Font, FontStyle.Bold);
         return lbl;
      }


      private MarkedTextBox CreateTextBox(int x, int y)
      {
         var tb = new MarkedTextBox();
         tb.Width = _controlX;
         tb.Height = _controlY;
         tb.Location = new Point(x, y);
         tb.TextAlign = HorizontalAlignment.Center;
         return tb;
      }

      public void GenerateMissingGridTextBoxes()
      {
         int d = 0;
         foreach (var dostawca in Dostawcy)
         {
            int o = 0;
            foreach (var odbiorca in Odbiorcy)
            {
               AddTbForGridIfNotExist(d, o);
               o++;
            }
            d++;
         }
      }

      public void AddTbForGridIfNotExist(int j, int i)
      {
         var locator = j.ToString() + i.ToString();
         if (GridMap.ContainsKey(locator))
            return;

         var x = CalculatePosition_X(i);
         var y = CalculatePosition_Y(j);
         var cellTb = CreateTextBox(x, y);
         GridMap[locator] = cellTb;
         grid.Controls.Add(cellTb);
      }

      public int CalculatePosition_X(int a_columnNumber)
      {
         var x = _start_X_Offset + _offset + ((2 * _offset + _controlX) * a_columnNumber);
         return x;
      }

      public int CalculatePosition_Y(int a_rowNumber)
      {
         var y = _start_Y_Offset + _offset + ((2 * _offset + _controlY) * a_rowNumber);
         return y;
      }

   }
}
