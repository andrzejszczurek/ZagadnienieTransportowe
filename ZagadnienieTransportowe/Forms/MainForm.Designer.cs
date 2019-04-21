namespace ZagadnienieTransportowe.Forms
{
   partial class MainForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.btnAddColumn = new System.Windows.Forms.Button();
         this.btnAddRow = new System.Windows.Forms.Button();
         this.grid = new System.Windows.Forms.Panel();
         this.gbInputData = new System.Windows.Forms.GroupBox();
         this.lblGridError = new System.Windows.Forms.Label();
         this.grResults = new System.Windows.Forms.GroupBox();
         this.tabResult = new System.Windows.Forms.TabControl();
         this.btnClearGrid = new System.Windows.Forms.Button();
         this.button2 = new System.Windows.Forms.Button();
         this.btnOptimalize = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.lblBasicCostsResult = new System.Windows.Forms.Label();
         this.lblOptimalCostResult = new System.Windows.Forms.Label();
         this.gbInputData.SuspendLayout();
         this.grResults.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnAddColumn
         // 
         this.btnAddColumn.Location = new System.Drawing.Point(81, 19);
         this.btnAddColumn.Name = "btnAddColumn";
         this.btnAddColumn.Size = new System.Drawing.Size(77, 45);
         this.btnAddColumn.TabIndex = 0;
         this.btnAddColumn.Text = "Nowy odbiorca";
         this.btnAddColumn.UseVisualStyleBackColor = true;
         this.btnAddColumn.Click += new System.EventHandler(this.BtnAddColumnClicked);
         // 
         // btnAddRow
         // 
         this.btnAddRow.Location = new System.Drawing.Point(6, 69);
         this.btnAddRow.Name = "btnAddRow";
         this.btnAddRow.Size = new System.Drawing.Size(69, 45);
         this.btnAddRow.TabIndex = 0;
         this.btnAddRow.Text = "Nowy dostawca";
         this.btnAddRow.UseVisualStyleBackColor = true;
         this.btnAddRow.Click += new System.EventHandler(this.BtnAddRowClicked);
         // 
         // grid
         // 
         this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.grid.AutoScroll = true;
         this.grid.Location = new System.Drawing.Point(81, 70);
         this.grid.Name = "grid";
         this.grid.Size = new System.Drawing.Size(400, 246);
         this.grid.TabIndex = 1;
         // 
         // gbInputData
         // 
         this.gbInputData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.gbInputData.Controls.Add(this.lblGridError);
         this.gbInputData.Controls.Add(this.grid);
         this.gbInputData.Controls.Add(this.btnAddColumn);
         this.gbInputData.Controls.Add(this.btnAddRow);
         this.gbInputData.Location = new System.Drawing.Point(12, 58);
         this.gbInputData.Name = "gbInputData";
         this.gbInputData.Size = new System.Drawing.Size(488, 325);
         this.gbInputData.TabIndex = 2;
         this.gbInputData.TabStop = false;
         this.gbInputData.Text = "Dane wejściowe";
         // 
         // lblGridError
         // 
         this.lblGridError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblGridError.ForeColor = System.Drawing.Color.Maroon;
         this.lblGridError.Location = new System.Drawing.Point(182, 19);
         this.lblGridError.Name = "lblGridError";
         this.lblGridError.Size = new System.Drawing.Size(265, 41);
         this.lblGridError.TabIndex = 2;
         this.lblGridError.Text = "Tabela z danymi wejściowymi zawiera błędy. Popraw je i spróbuj ponownie";
         // 
         // grResults
         // 
         this.grResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.grResults.Controls.Add(this.tabResult);
         this.grResults.Location = new System.Drawing.Point(506, 4);
         this.grResults.Name = "grResults";
         this.grResults.Size = new System.Drawing.Size(260, 379);
         this.grResults.TabIndex = 3;
         this.grResults.TabStop = false;
         this.grResults.Text = "Szczegoły iteracji";
         // 
         // tabResult
         // 
         this.tabResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.tabResult.Location = new System.Drawing.Point(6, 19);
         this.tabResult.Name = "tabResult";
         this.tabResult.SelectedIndex = 0;
         this.tabResult.Size = new System.Drawing.Size(248, 351);
         this.tabResult.TabIndex = 0;
         // 
         // btnClearGrid
         // 
         this.btnClearGrid.Location = new System.Drawing.Point(12, 12);
         this.btnClearGrid.Name = "btnClearGrid";
         this.btnClearGrid.Size = new System.Drawing.Size(100, 40);
         this.btnClearGrid.TabIndex = 4;
         this.btnClearGrid.Text = "Wyczyść dane";
         this.btnClearGrid.UseVisualStyleBackColor = true;
         this.btnClearGrid.Click += new System.EventHandler(this.BtnClearGridClicked);
         // 
         // button2
         // 
         this.button2.Location = new System.Drawing.Point(120, 12);
         this.button2.Name = "button2";
         this.button2.Size = new System.Drawing.Size(100, 40);
         this.button2.TabIndex = 5;
         this.button2.Text = "Reseruj siatkę";
         this.button2.UseVisualStyleBackColor = true;
         this.button2.Click += new System.EventHandler(this.BtnResetGridClicked);
         // 
         // btnOptimalize
         // 
         this.btnOptimalize.Location = new System.Drawing.Point(226, 12);
         this.btnOptimalize.Name = "btnOptimalize";
         this.btnOptimalize.Size = new System.Drawing.Size(100, 40);
         this.btnOptimalize.TabIndex = 6;
         this.btnOptimalize.Text = "Optymalizuj";
         this.btnOptimalize.UseVisualStyleBackColor = true;
         this.btnOptimalize.Click += new System.EventHandler(this.BtnOptimalizeClicked);
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(332, 12);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(90, 13);
         this.label1.TabIndex = 7;
         this.label1.Text = "Koszty pierwotne:";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(332, 39);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(92, 13);
         this.label2.TabIndex = 7;
         this.label2.Text = "Koszty optymalne:";
         this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // lblBasicCostsResult
         // 
         this.lblBasicCostsResult.AutoSize = true;
         this.lblBasicCostsResult.Location = new System.Drawing.Point(428, 12);
         this.lblBasicCostsResult.Name = "lblBasicCostsResult";
         this.lblBasicCostsResult.Size = new System.Drawing.Size(31, 13);
         this.lblBasicCostsResult.TabIndex = 7;
         this.lblBasicCostsResult.Text = "0000";
         this.lblBasicCostsResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblOptimalCostResult
         // 
         this.lblOptimalCostResult.AutoSize = true;
         this.lblOptimalCostResult.Location = new System.Drawing.Point(428, 39);
         this.lblOptimalCostResult.Name = "lblOptimalCostResult";
         this.lblOptimalCostResult.Size = new System.Drawing.Size(31, 13);
         this.lblOptimalCostResult.TabIndex = 7;
         this.lblOptimalCostResult.Text = "0000";
         this.lblOptimalCostResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.SystemColors.WindowFrame;
         this.ClientSize = new System.Drawing.Size(776, 391);
         this.Controls.Add(this.lblOptimalCostResult);
         this.Controls.Add(this.lblBasicCostsResult);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.btnOptimalize);
         this.Controls.Add(this.button2);
         this.Controls.Add(this.btnClearGrid);
         this.Controls.Add(this.grResults);
         this.Controls.Add(this.gbInputData);
         this.Name = "MainForm";
         this.Text = "Zagadmienie transportowe";
         this.gbInputData.ResumeLayout(false);
         this.grResults.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button btnAddColumn;
      private System.Windows.Forms.Button btnAddRow;
      private System.Windows.Forms.Panel grid;
      private System.Windows.Forms.GroupBox gbInputData;
      private System.Windows.Forms.GroupBox grResults;
      private System.Windows.Forms.Button btnClearGrid;
      private System.Windows.Forms.Button button2;
      private System.Windows.Forms.Button btnOptimalize;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label lblBasicCostsResult;
      private System.Windows.Forms.Label lblOptimalCostResult;
      private System.Windows.Forms.Label lblGridError;
      private System.Windows.Forms.TabControl tabResult;
   }
}

