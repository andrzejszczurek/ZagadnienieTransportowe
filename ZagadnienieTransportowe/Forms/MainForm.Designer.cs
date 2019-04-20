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
         this.SuspendLayout();
         // 
         // btnAddColumn
         // 
         this.btnAddColumn.Location = new System.Drawing.Point(26, 45);
         this.btnAddColumn.Name = "btnAddColumn";
         this.btnAddColumn.Size = new System.Drawing.Size(115, 23);
         this.btnAddColumn.TabIndex = 0;
         this.btnAddColumn.Text = "Dodaj kolumne";
         this.btnAddColumn.UseVisualStyleBackColor = true;
         this.btnAddColumn.Click += new System.EventHandler(this.BtnAddColumnClicked);
         // 
         // btnAddRow
         // 
         this.btnAddRow.Location = new System.Drawing.Point(172, 45);
         this.btnAddRow.Name = "btnAddRow";
         this.btnAddRow.Size = new System.Drawing.Size(115, 23);
         this.btnAddRow.TabIndex = 0;
         this.btnAddRow.Text = "Dodaj wiersz";
         this.btnAddRow.UseVisualStyleBackColor = true;
         this.btnAddRow.Click += new System.EventHandler(this.BtnAddRowClicked);
         // 
         // grid
         // 
         this.grid.AutoSize = true;
         this.grid.Location = new System.Drawing.Point(12, 96);
         this.grid.Name = "grid";
         this.grid.Size = new System.Drawing.Size(776, 342);
         this.grid.TabIndex = 1;
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.SystemColors.WindowFrame;
         this.ClientSize = new System.Drawing.Size(800, 450);
         this.Controls.Add(this.grid);
         this.Controls.Add(this.btnAddRow);
         this.Controls.Add(this.btnAddColumn);
         this.Name = "MainForm";
         this.Text = "Zagadmienie transportowe";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button btnAddColumn;
      private System.Windows.Forms.Button btnAddRow;
      private System.Windows.Forms.Panel grid;
   }
}

