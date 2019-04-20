namespace ZagadnienieTransportowe.Forms
{
   partial class AddInputForm
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
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.btnSave = new System.Windows.Forms.Button();
         this.btnAnuluj = new System.Windows.Forms.Button();
         this.comboBox1 = new System.Windows.Forms.ComboBox();
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(65, 40);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(35, 13);
         this.label1.TabIndex = 0;
         this.label1.Text = "label1";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(65, 74);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(35, 13);
         this.label2.TabIndex = 0;
         this.label2.Text = "label1";
         // 
         // btnSave
         // 
         this.btnSave.Location = new System.Drawing.Point(109, 136);
         this.btnSave.Name = "btnSave";
         this.btnSave.Size = new System.Drawing.Size(75, 23);
         this.btnSave.TabIndex = 1;
         this.btnSave.Text = "Zapisz";
         this.btnSave.UseVisualStyleBackColor = true;
         // 
         // btnAnuluj
         // 
         this.btnAnuluj.Location = new System.Drawing.Point(224, 136);
         this.btnAnuluj.Name = "btnAnuluj";
         this.btnAnuluj.Size = new System.Drawing.Size(75, 23);
         this.btnAnuluj.TabIndex = 2;
         this.btnAnuluj.Text = "button2";
         this.btnAnuluj.UseVisualStyleBackColor = true;
         // 
         // comboBox1
         // 
         this.comboBox1.FormattingEnabled = true;
         this.comboBox1.Location = new System.Drawing.Point(205, 40);
         this.comboBox1.Name = "comboBox1";
         this.comboBox1.Size = new System.Drawing.Size(121, 21);
         this.comboBox1.TabIndex = 3;
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(146, 74);
         this.textBox1.Name = "textBox1";
         this.textBox1.Size = new System.Drawing.Size(100, 20);
         this.textBox1.TabIndex = 4;
         // 
         // AddInputForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.SystemColors.WindowFrame;
         this.ClientSize = new System.Drawing.Size(383, 186);
         this.Controls.Add(this.textBox1);
         this.Controls.Add(this.comboBox1);
         this.Controls.Add(this.btnAnuluj);
         this.Controls.Add(this.btnSave);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Name = "AddInputForm";
         this.Text = "AddInputForm";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Button btnSave;
      private System.Windows.Forms.Button btnAnuluj;
      private System.Windows.Forms.ComboBox comboBox1;
      private System.Windows.Forms.TextBox textBox1;
   }
}