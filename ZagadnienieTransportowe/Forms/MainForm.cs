using App.Core.Enums;
using App.Core.Solver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZagadnienieTransportowe.Controls;
using ZagadnienieTransportowe.DataAdapter;

namespace ZagadnienieTransportowe.Forms
{
   public partial class MainForm : Form
   {
      private static readonly string TAB_KEY_PATTERN = "Iteration_{0}_tab";
      private static readonly string COST_PANEL_KEY_PATTERN = "_costs_panel";
      private static readonly string OPTI_KEY_PATTERN = "_opt_panel";

      private int _columnIndex = 0;
      private int _rowIndex = 0;
      private int _maxAvailableRows = 8;
      private int _maxAvailableColumns = 6;

      private int _offset = 1;
      private int _controlY = 20;
      private int _controlX = 50;
      private int _start_X_Offset;
      private int _start_Y_Offset;

      private readonly Dictionary<string, LocalizedTextBox> GridMap;
      private readonly Dictionary<int, (LocalizedTextBox Popyt, LocalizedTextBox Cena)> Odbiorcy;
      private readonly Dictionary<int, (LocalizedTextBox Podaz, LocalizedTextBox Cena)> Dostawcy;
      private readonly List<(int Iteracja, List<LocalizedLabel> Labelki)> m_resultLabels;

      private const bool IsControlsDebugMode = false;

      private JobType m_jt = JobType.Profit;

      public MainForm()
      {
         InitializeComponent();
         _start_X_Offset = 4 * _offset + 2 * _controlX;
         _start_Y_Offset = 4 * _offset + 2 * _controlY;
         grid.AutoScroll = true;
         GridMap = new Dictionary<string, LocalizedTextBox>();
         Odbiorcy = new Dictionary<int, (LocalizedTextBox Popyt, LocalizedTextBox Cena)>();
         Dostawcy = new Dictionary<int, (LocalizedTextBox Podaz, LocalizedTextBox Cena)>();
         m_resultLabels = new List<(int Iteracja, List<LocalizedLabel> Labelki)>();
         InitBaseGrid();
         if (IsControlsDebugMode)
            tabResult.TabPages.Add(GenerateResultTabInternal(5, 5, new IterationTransportCosts(null, JobType.TransportCosts, 1)));
      }

      private void ResolveJob()
      {
         var gridData = new UserDataGridDefinition() { Cells = GridMap, Odbiorcy = this.Odbiorcy, Dostawcy = this.Dostawcy };
         var userData = new UserDataAdapter().Adapt(gridData);
         var solver = new Solver(userData);
         solver.Init(m_jt);

         try
         {
            solver.Resolve(m_jt);
            if (!solver.OptimalSolutionFound)
            {
               MessageBox.Show(solver.ErrorMessage);
               return;
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show($"Podczas rozwiązywania zadania wystąpił błąd: {ex.StackTrace}");
         }
         PrepareResultData(solver.Iterations.Where(x => x.IsCorrect).ToList());
      }

      private void PrepareResultData(List<IterationBase> iteracje)
      {
         lblBasicCostsResult.Text = iteracje.Single(i => i.Number == 1)?.IterationResultValue.ToString();
         lblOptimalCostResult.Text = iteracje.Single(i => i.Number == iteracje.Count()).IterationResultValue.ToString();

         foreach (var iteracja in iteracje)
         {
            var tab = GenerateResultTab(iteracja);
            tabResult.TabPages.Add(tab);
         }
      }

      private IEnumerable<T> GetControlsByType<T>(Control.ControlCollection a_controlsBag)
      {
         var controls = new List<T>();
         foreach (var cb in a_controlsBag)
         {
            if (cb.GetType() is T)
               yield return (T)cb;
         }
      }

      #region [Events]


      private void BtnClearGridClicked(object sender, EventArgs e)
      {
         foreach (var cell in GridMap)
            cell.Value.Text = string.Empty;

         foreach (var o in Odbiorcy)
         {
            o.Value.Popyt.Text = string.Empty;
            o.Value.Cena.Text = string.Empty;
         }

         foreach (var d in Dostawcy)
         {
            d.Value.Podaz.Text = string.Empty;
            d.Value.Cena.Text = string.Empty;
         }
      }


      private void BtnResetGridClicked(object sender, EventArgs e)
      {
         grid.Controls.Clear();
         GridMap.Clear();
         Odbiorcy.Clear();
         Dostawcy.Clear();
         _rowIndex = 0;
         _columnIndex = 0;
         InitBaseGrid();
         tabResult.TabPages.Clear();
      }


      private void BtnOptimalizeClicked(object sender, EventArgs e)
      {
         if (!AllDataValid())
         {
            lblGridError.Visible = true;
            return;
         }
         lblGridError.Visible = false;
         SetInputDataAccess(false);
         ResolveJob();
      }


      private void BtnAddColumnClicked(object sender, EventArgs e)
      {
         if (_columnIndex >= _maxAvailableColumns)
         {
            MessageBox.Show("Osiągnięto maksymalną możliwą liczbę odbiorców");
            return;
         }
         AddOdbiorca();
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
         AddDostawca();
         GenerateMissingGridTextBoxes();
         _rowIndex++;
      }

      #endregion [Events]

      #region [Controls]

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


      private LocalizedTextBox CreateTextBox(int x, int y)
      {
         var tb = new LocalizedTextBox();
         tb.Width = _controlX;
         tb.Height = _controlY;
         tb.Location = new Point(x, y);
         tb.TextAlign = HorizontalAlignment.Center;
         return tb;
      }


      private LocalizedLabel CreateLblForResult(string a_text, int x, int y, LocalizedLabel.LocalizatorType a_localizator, string a_position)
      {
         var lbl = new LocalizedLabel();
         lbl.Text = a_text;
         lbl.Width = 30;
         lbl.Height = 15;
         lbl.Location = new Point(x, y);
         lbl.TextAlign = ContentAlignment.MiddleCenter;
         lbl.Localizator = a_localizator;
         lbl.GridPosition = a_position;
         if (IsControlsDebugMode)
            lbl.BackColor = Color.RosyBrown;
         return lbl;
      }


      private bool AllDataValid()
      {
         int invalidCount = 0;
         foreach (var cell in GridMap)
            invalidCount += MarkControlIfNotValid(cell.Value);
         foreach (var cell in Odbiorcy)
         {
            invalidCount += MarkControlIfNotValid(cell.Value.Popyt);
            invalidCount += MarkControlIfNotValid(cell.Value.Cena);
         }
         foreach (var cell in Dostawcy)
         {
            invalidCount += MarkControlIfNotValid(cell.Value.Podaz);
            invalidCount += MarkControlIfNotValid(cell.Value.Cena);
         }
         return invalidCount == 0;
      }

      private TabPage GenerateResultTab(IterationBase a_iteracja)
      {
         return GenerateResultTabInternal(a_iteracja.DataGrid[0].Length, a_iteracja.DataGrid.Length, a_iteracja);
      }

      private TabPage GenerateResultTabInternal(int a_x, int a_y, IterationBase a_iteracja)
      {
         var tpKey = string.Format(TAB_KEY_PATTERN, a_iteracja.Number);
         var p1Key = $"{tpKey}{COST_PANEL_KEY_PATTERN}";
         var p2Key = $"{tpKey}{OPTI_KEY_PATTERN}";

         var tp = new TabPage();
         tp.BackColor = Color.Gray;
         tp.Text = $"Iteracja {a_iteracja.Number}";
         tp.Name = tpKey;

         var lblTable = new Label();
         lblTable.Text = "Tabela przydziału";
         lblTable.Width = 150;
         lblTable.Height = 15;
         lblTable.TextAlign = ContentAlignment.MiddleCenter;
         lblTable.Location = new Point(45, 3);
         lblTable.Font = new Font(lblTable.Font, FontStyle.Bold);
         if (IsControlsDebugMode)
            lblTable.BackColor = Color.Red;
         tp.Controls.Add(lblTable);

         var lblTable2 = new Label();
         lblTable2.Text = "Tabela optymalizacji";
         lblTable2.Width = 150;
         lblTable2.Height = 15;
         lblTable2.TextAlign = ContentAlignment.MiddleCenter;
         lblTable2.Location = new Point(45, 155);
         lblTable2.Font = new Font(lblTable2.Font, FontStyle.Bold);
         if (IsControlsDebugMode)
            lblTable2.BackColor = Color.Red;
         tp.Controls.Add(lblTable2);

         var koszty = IsControlsDebugMode ? "-" : a_iteracja.IterationResultValue.ToString();
         var lblKoszty = new Label();
         lblKoszty.Text = $"Koszty transportu: {koszty}";
         lblKoszty.Width = 150;
         lblKoszty.Height = 15;
         lblKoszty.TextAlign = ContentAlignment.MiddleCenter;
         lblKoszty.Location = new Point(40, tabResult.Height - 45);
         lblKoszty.Font = new Font(lblKoszty.Font, FontStyle.Bold);
         if (IsControlsDebugMode)
            lblKoszty.BackColor = Color.Red;
         tp.Controls.Add(lblKoszty);

         var panelKoszty = new Panel();
         panelKoszty.Width = 220;
         panelKoszty.Height = 130;
         panelKoszty.Location = new Point(10, 21);
         panelKoszty.AutoScroll = true;
         panelKoszty.Name = p1Key;
         if (IsControlsDebugMode)
            panelKoszty.BackColor = Color.Red;

         var panelOptymalizacja = new Panel();
         panelOptymalizacja.Width = 220;
         panelOptymalizacja.Height = 130;
         panelOptymalizacja.Location = new Point(10, 174);
         panelOptymalizacja.AutoScroll = true;
         panelOptymalizacja.Name = p2Key;
         if (IsControlsDebugMode)
            panelOptymalizacja.BackColor = Color.Red;

         tp.Controls.Add(panelKoszty);
         tp.Controls.Add(panelOptymalizacja);


         var offset = 2;
         var lbl_width = 30;
         var lbl_height = 15;

         var cell_width = 2 * offset + lbl_width;
         var cell_height = 2 * offset + lbl_height;

         var cellGrid = a_iteracja.DataGrid;
         var cyklPoints = a_iteracja.Cycle?.ToPointsList();
         // labelki informacyjne dostawcy
         for (int y = 0; y < a_y; y++)
         {
            var text = $"D{y + 1}";
            var localizator = LocalizedLabel.LocalizatorType.OdbiorcaInfo;
            var y_pos = cell_height + (2 * offset + cell_height) * y;

            var lbl1 = CreateLblForResult(text, offset, y_pos, localizator, text);
            lbl1.Font = new Font(lbl1.Font, FontStyle.Bold);
            panelKoszty.Controls.Add(lbl1);
            var lbl2 = CreateLblForResult(text, offset, y_pos, localizator, text);
            lbl2.Font = new Font(lbl2.Font, FontStyle.Bold);
            panelOptymalizacja.Controls.Add(lbl2);

            // pełny grid
            for (int x = 0; x < a_x; x++)
            {
               var x_pos = cell_width + (2 * offset + cell_width) * x;
               var position = $"{y}{x}";
               var lblgridText = !IsControlsDebugMode ? (cellGrid[y][x].Przydzial ?? 0).ToString() : "-";
               var cellLbl1 = CreateLblForResult(lblgridText, x_pos, y_pos, LocalizedLabel.LocalizatorType.TabelaPrzydzial, position);
               panelKoszty.Controls.Add(cellLbl1);
               lblgridText = !IsControlsDebugMode ? (cellGrid[y][x].DeltaNiebazowa ?? 0).ToString() : "-";
               var cellLbl2 = CreateLblForResult(lblgridText, x_pos, y_pos, LocalizedLabel.LocalizatorType.TabelaOptymalizacji, position);
               panelOptymalizacja.Controls.Add(cellLbl2);

               var cyclePoint = cyklPoints?.SingleOrDefault(c => c.Id == position);
               if (!(cyclePoint is null))
               {
                  if (cyclePoint.IsStart)
                     cellLbl2.BackColor = Color.FromArgb(65, 148, 181);
                  else if (cyclePoint.Type == CyclePointType.Positive)
                     cellLbl2.BackColor = Color.FromArgb(50, 130, 46);
                  else
                     cellLbl2.BackColor = Color.FromArgb(211, 88, 88);
               }

            }
         }

         // labelki informacyjne odbiorcy
         for (int x = 0; x < a_x; x++)
         {
            var text = $"O{x + 1}";
            var localizator = LocalizedLabel.LocalizatorType.OdbiorcaInfo;
            var lbl1 = CreateLblForResult(text, cell_width + (2 * offset + cell_width) * x, offset, localizator, text);
            lbl1.Font = new Font(lbl1.Font, FontStyle.Bold);
            panelKoszty.Controls.Add(lbl1);
            var lbl2 = CreateLblForResult(text, cell_width + (2 * offset + cell_width) * x, offset, localizator, text);
            lbl2.Font = new Font(lbl2.Font, FontStyle.Bold);
            panelOptymalizacja.Controls.Add(lbl2);
         }
         return tp;
      }


      private int MarkControlIfNotValid(TextBox cell)
      {
         if (int.TryParse(cell.Text, out int value))
         {
            cell.BackColor = Color.White;
            return 0;
         }
         cell.BackColor = Color.Red;
         return 1;
      }


      private void SetInputDataAccess(bool a_readonly)
      {
         foreach (var cell in GridMap)
            cell.Value.ReadOnly = !a_readonly;
         foreach (var cell in Odbiorcy)
         {
            cell.Value.Popyt.ReadOnly = !a_readonly;
            cell.Value.Cena.ReadOnly = !a_readonly;
         }
         foreach (var cell in Dostawcy)
         {
            cell.Value.Podaz.ReadOnly = !a_readonly;
            cell.Value.Cena.ReadOnly = !a_readonly;
         }
         btnAddColumn.Enabled = a_readonly;
         btnAddRow.Enabled = a_readonly;
         btnOptimalize.Enabled = a_readonly;
      }

      #endregion [Controls]

      #region [Utility]

      private void InitBaseGrid()
      {
         lblBasicCostsResult.Text = "-";
         lblOptimalCostResult.Text = "-";
         lblGridError.Visible = false;
         SetInputDataAccess(true);

         var lblPopyt = CreateLabel("Popyt:", 2 * (3 * _offset + _controlX), (3 * _offset + _controlY) - 1);
         var lblKupno = CreateLabel("Buy:", 2 * (3 * _offset + _controlX), _offset - 1);
         lblPopyt.TextAlign = ContentAlignment.MiddleRight;
         lblKupno.TextAlign = ContentAlignment.MiddleRight;
         grid.Controls.Add(lblPopyt);
         grid.Controls.Add(lblKupno);

         var lblPodaz = CreateLabel("Podaż:", 3 * _offset + _controlX, 2 * (3 * _offset + _controlY));
         var lblSprzedaz = CreateLabel("Sell:", _offset, 2 * (3 * _offset + _controlY));
         lblPopyt.TextAlign = ContentAlignment.MiddleRight;
         lblSprzedaz.TextAlign = ContentAlignment.MiddleCenter;
         grid.Controls.Add(lblPodaz);
         grid.Controls.Add(lblSprzedaz);
         AddDostawca();
         AddOdbiorca();
         AddTbForGridIfNotExist(0, 0);
         _columnIndex++;
         _rowIndex++;
      }

      private void AddDostawca()
      {
         var y = _start_Y_Offset + _offset + ((2 * _offset + _controlY) * _rowIndex) + 2 * _offset + _controlY;
         var baseX = 3 * _offset + _controlX;

         var tbForPrice = CreateTextBox(_offset, y);
         var tbForPodaz = CreateTextBox(baseX, y);
         var lblOdbiorcy = CreateLabel($"D{_rowIndex + 1}", 2 * baseX, y);
         Dostawcy[_rowIndex] = (tbForPodaz, tbForPrice);

         grid.Controls.Add(tbForPodaz);
         grid.Controls.Add(tbForPrice);
         grid.Controls.Add(lblOdbiorcy);
      }


      private void AddOdbiorca()
      {
         var x = _start_X_Offset + _offset + ((2 * _offset + _controlX) * _columnIndex) + 2 * _offset + _controlX;
         var baseY = 3 * _offset + _controlY;

         var tbForPopyt = CreateTextBox(x, baseY);
         var tbForPrice = CreateTextBox(x, _offset);

         var lblOdbiorcy = CreateLabel($"O{_columnIndex + 1}", x, 2 * baseY);
         Odbiorcy[_columnIndex] = (tbForPopyt, tbForPrice);

         grid.Controls.Add(tbForPopyt);
         grid.Controls.Add(tbForPrice);
         grid.Controls.Add(lblOdbiorcy);
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
         var x = _start_X_Offset + _offset + ((2 * _offset + _controlX) * a_columnNumber) + 3 * _offset + _controlX;
         return x;
      }

      public int CalculatePosition_Y(int a_rowNumber)
      {
         var y = _start_Y_Offset + _offset + ((2 * _offset + _controlY) * a_rowNumber) + 3 * _offset + _controlY;
         return y;
      }


      #endregion [Utility]

   }
}
