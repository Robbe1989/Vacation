// Controls declaration
private System.Windows.Forms.TextBox txtName;
private System.Windows.Forms.TextBox txtEmail;
private System.Windows.Forms.ComboBox cmbDepartment;
private System.Windows.Forms.ComboBox cmbFte;
private System.Windows.Forms.CheckBox chkUseFte;
private System.Windows.Forms.Label label1, label2, label3, label5;
private System.Windows.Forms.Button btnOk, btnCancel;

// In InitializeComponent(): füge vor cmbFte die Checkbox ein
// chkUseFte
this.chkUseFte = new System.Windows.Forms.CheckBox();
this.chkUseFte.Location = new System.Drawing.Point(110, 99);
this.chkUseFte.Name = "chkUseFte";
this.chkUseFte.Size = new System.Drawing.Size(140, 24);
this.chkUseFte.Text = "FTE verwenden";
this.chkUseFte.Checked = true;
this.chkUseFte.CheckedChanged += new System.EventHandler(this.chkUseFte_CheckedChanged);

// adjust cmbFte position to y=129
this.cmbFte.Location = new System.Drawing.Point(110, 129);
this.cmbFte.Name = "cmbFte";
this.cmbFte.Size = new System.Drawing.Size(180, 23);

// add controls to Form in the Controls.Add order
this.Controls.Add(this.chkUseFte);
this.Controls.Add(this.cmbFte);