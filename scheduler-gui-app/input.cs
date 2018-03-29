using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace scheduler_gui_app
{
    public partial class scheduler_app : Form
    {
        public List<TextBox> dynamic_processes = new List<TextBox>();
        public List<TextBox> dynamic_arrival_time = new List<TextBox>();
        public List<TextBox> dynamic_duration = new List<TextBox>();
        public List<TextBox> dynamic_priority = new List<TextBox>();
        public Time_Scheduler time_sched;

        public scheduler_app()
        {
            InitializeComponent();
        }
        private void scheduler_app_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        /*private void global_key_handler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (button1.Visible == true)
                    button1.PerformClick();
                else if (submit.Visible == true)
                    submit.PerformClick();
                else if (reset.Visible == true)
                    reset.PerformClick();
            }
        }*/

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Processes Count")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText;
            }
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            int outParse = 0;
            if (textBox1.Text == "")
            {
                textBox1.Text = "Processes Count";
                textBox1.ForeColor = SystemColors.InactiveCaption;
            }
            else if (!Int32.TryParse(textBox1.Text, out outParse) || outParse <= 0)
            {
                MessageBox.Show("You can't enter anything but positive integers.");
                textBox1.Text = "Processes Count";
                textBox1.ForeColor = SystemColors.InactiveCaption;

            }
        }
        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Quantum")
            {
                textBox2.Text = "";
                textBox2.ForeColor = SystemColors.WindowText;
            }
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            float outParse = 0;
            if (textBox2.Text == "")
            {
                textBox2.Text = "Quantum";
                textBox2.ForeColor = SystemColors.InactiveCaption;
            }
            else if (!float.TryParse(textBox2.Text, out outParse) || outParse <= 0)
            {
                MessageBox.Show("You can't enter anything but positive numbers.");
                textBox2.Text = "Quantum";
                textBox2.ForeColor = SystemColors.InactiveCaption;

            }
        }

        Button submit = new Button();
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" && textBox1.Text != "Processes Count")
            {
                comboBox1.Enabled = false;
                textBox1.Enabled = false;
                button1.Visible = false;
                tableLayoutPanel1.RowCount = Int32.Parse(textBox1.Text);
                if (24 * tableLayoutPanel1.RowCount > 700 - 112 - 6)
                    tableLayoutPanel1.Size = new Size(547, 700 - 112 - 6);
                else
                    tableLayoutPanel1.Size = new Size(547, 24 * tableLayoutPanel1.RowCount + 12);

                tableLayoutPanel1.HorizontalScroll.Maximum = 0;
                tableLayoutPanel1.VerticalScroll.Visible = false;
                tableLayoutPanel1.AutoScroll = true;

                if (112 + 24 * tableLayoutPanel1.RowCount + 6 > 700)
                    this.Size = new Size(587, 700);
                else
                    this.Size = new Size(587, 112 + 24 * tableLayoutPanel1.RowCount + 6);
                tableLayoutPanel1.RowStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                
                if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                {
                    tableLayoutPanel1.ColumnCount = 3;
                    tableLayoutPanel1.ColumnStyles.Clear();
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                }
                else if (comboBox1.SelectedIndex == 3 || comboBox1.SelectedIndex == 4)
                {
                    tableLayoutPanel1.ColumnCount = 4;
                    tableLayoutPanel1.ColumnStyles.Clear();
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                }
                else if (comboBox1.SelectedIndex == 5)
                {
                    tableLayoutPanel1.ColumnCount = 3;
                    tableLayoutPanel1.ColumnStyles.Clear();
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                    textBox2.Visible = true;
                }

                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                {
                    dynamic_processes.Add(new TextBox());
                    dynamic_processes[i].Name = i.ToString();
                    dynamic_processes[i].Anchor = AnchorStyles.None;
                    dynamic_processes[i].Text = "Process " + i.ToString();
                    dynamic_processes[i].Enter += (s, ea) => {
                        TextBox temp = (TextBox)s;
                        if (temp.Text == "Process " + temp.Name)
                        {
                            temp.Text = "";
                        }
                    };
                    dynamic_processes[i].Leave += (s, ea) => {
                        TextBox temp = (TextBox)s;
                        if (temp.Text == "")
                        {
                            temp.Text = "Process " + temp.Name;
                        }
                    };
                    dynamic_arrival_time.Add(new TextBox());
                    dynamic_arrival_time[i].Name = i.ToString();
                    dynamic_arrival_time[i].Anchor = AnchorStyles.None;
                    dynamic_arrival_time[i].ForeColor = SystemColors.InactiveCaption;
                    dynamic_arrival_time[i].Text = "Process " + i.ToString() + " arrival time";
                    dynamic_arrival_time[i].Enter += (s, ea) => {
                        TextBox temp = (TextBox)s;
                        if (temp.Text == "Process " + temp.Name + " arrival time")
                        {
                            temp.Text = "";
                            temp.ForeColor = SystemColors.WindowText;
                        }
                    };
                    dynamic_arrival_time[i].Leave += (s, ea) => {
                        TextBox temp = (TextBox)s;
                        float outParse = 0;
                        if (temp.Text == "")
                        {
                            temp.Text = "Process " + temp.Name + " arrival time";
                            temp.ForeColor = SystemColors.InactiveCaption;
                        }
                        else if (!float.TryParse(temp.Text, out outParse) || outParse < 0)
                        {
                            MessageBox.Show("You can't enter anything but positive numbers.");
                            temp.Text = "Process " + temp.Name + " arrival time";
                            temp.ForeColor = SystemColors.InactiveCaption;
                        }
                    };
                    dynamic_duration.Add(new TextBox());
                    dynamic_duration[i].Name = i.ToString();
                    dynamic_duration[i].Anchor = AnchorStyles.None;
                    dynamic_duration[i].ForeColor = SystemColors.InactiveCaption;
                    dynamic_duration[i].Text = "Process " + i.ToString() + " duration";
                    dynamic_duration[i].Enter += (s, ea) => {
                        TextBox temp = (TextBox)s;
                        if (temp.Text == "Process " + temp.Name + " duration")
                        {
                            temp.Text = "";
                            temp.ForeColor = SystemColors.WindowText;
                        }
                    };
                    dynamic_duration[i].Leave += (s, ea) => {
                        TextBox temp = (TextBox)s;
                        float outParse = 0;
                        if (temp.Text == "")
                        {
                            temp.Text = "Process " + temp.Name + " duration";
                            temp.ForeColor = SystemColors.InactiveCaption;
                        }
                        else if (!float.TryParse(temp.Text, out outParse) || outParse <= 0)
                        {
                            MessageBox.Show("You can't enter anything but positive numbers.");
                            temp.Text = "Process " + temp.Name + " duration";
                            temp.ForeColor = SystemColors.InactiveCaption;
                        }
                    };
                    if (comboBox1.SelectedIndex == 3 || comboBox1.SelectedIndex == 4)
                    {
                        dynamic_priority.Add(new TextBox());
                        dynamic_priority[i].Name = i.ToString();
                        dynamic_priority[i].Anchor = AnchorStyles.None;
                        dynamic_priority[i].ForeColor = SystemColors.InactiveCaption;
                        dynamic_priority[i].Text = "Process " + i.ToString() + " priority";
                        dynamic_priority[i].Enter += (s, ea) => {
                            TextBox temp = (TextBox)s;
                            if (temp.Text == "Process " + temp.Name + " priority")
                            {
                                temp.Text = "";
                                temp.ForeColor = SystemColors.WindowText;
                            }
                        };
                        dynamic_priority[i].Leave += (s, ea) => {
                            TextBox temp = (TextBox)s;
                            int outParse = 0;
                            if (temp.Text == "")
                            {
                                temp.Text = "Process " + temp.Name + " priority";
                                temp.ForeColor = SystemColors.InactiveCaption;
                            }
                            else if (!Int32.TryParse(temp.Text, out outParse) || outParse < 0)
                            {
                                MessageBox.Show("You can't enter anything but positive integers.");
                                temp.Text = "Process " + temp.Name + " priority";
                                temp.ForeColor = SystemColors.InactiveCaption;
                            }
                        };
                    }
                    tableLayoutPanel1.Controls.Add(dynamic_processes[i], 0, i);
                    tableLayoutPanel1.Controls.Add(dynamic_arrival_time[i], 1, i);
                    tableLayoutPanel1.Controls.Add(dynamic_duration[i], 2, i);
                    if (comboBox1.SelectedIndex == 3 || comboBox1.SelectedIndex == 4)
                        tableLayoutPanel1.Controls.Add(dynamic_priority[i], 3, i);
                }
            }

            submit.Click += new EventHandler(simulate);
            submit.Text = "Simulate";
            submit.Location = new Point(484, 38);
            this.Controls.Add(submit);
        }
        Button reset = new Button();
        private void simulate(object sender, EventArgs e)
        {
            float outf = 0;
            int outi = 0, processes_no = Int32.Parse(textBox1.Text);
            if (comboBox1.SelectedIndex == 5 && !float.TryParse(textBox2.Text, out outf))
            {
                MessageBox.Show("You cannot start simulation without completing the input");
                return;
            }
            for (int i = 0; i < processes_no; i++)
            {
                if (dynamic_processes[i].Text == "" || !float.TryParse(dynamic_arrival_time[i].Text, out outf) || !float.TryParse(dynamic_duration[i].Text, out outf))
                {
                    MessageBox.Show("You cannot start simulation without completing the input");
                    return;
                }
                if (comboBox1.SelectedIndex == 3 || comboBox1.SelectedIndex == 4)
                    if (!Int32.TryParse(dynamic_priority[i].Text, out outi))
                    {
                        MessageBox.Show("You cannot start simulation without completing the input");
                        return;
                    }
            }

            const int mul_ratio = 60;
            output_gui result = new output_gui();
            result.form_time_sched = new Time_Scheduler();
            result.form_time_sched.scheduler_data = new Scheduler();
            result.form_time_sched.scheduler_data.processes = new List<string>();
            result.form_time_sched.scheduler_data.arrival_time = new List<float>();
            result.form_time_sched.scheduler_data.duration = new List<float>();
            result.form_time_sched.scheduler_data.remaining_duration = new List<float>();
            result.form_time_sched.scheduler_data.priority = new List<int>();
            result.form_time_sched.scheduler_data.status = new List<bool>();
            result.form_time_sched.scheduler_data.no = processes_no;
            result.form_time_sched.scheduler_data.stdno = processes_no;

            for (int i = 0; i < processes_no; i++)
            {
                result.form_time_sched.scheduler_data.processes.Add(dynamic_processes[i].Text);
                dynamic_processes[i].Enabled = false;
                result.form_time_sched.scheduler_data.arrival_time.Add(float.Parse(dynamic_arrival_time[i].Text));
                dynamic_arrival_time[i].Enabled = false;
                result.form_time_sched.scheduler_data.duration.Add(float.Parse(dynamic_duration[i].Text));
                dynamic_duration[i].Enabled = false;
                result.form_time_sched.scheduler_data.remaining_duration.Add(float.Parse(dynamic_duration[i].Text));
                if (comboBox1.SelectedIndex == 3 || comboBox1.SelectedIndex == 4)
                {
                    result.form_time_sched.scheduler_data.priority.Add(Int32.Parse(dynamic_priority[i].Text));
                    dynamic_priority[i].Enabled = false;
                }
                else result.form_time_sched.scheduler_data.priority.Add(0);
                result.form_time_sched.scheduler_data.status.Add(false);
                if (comboBox1.SelectedIndex == 5)
                {
                    result.form_time_sched.scheduler_data.quantum = float.Parse(textBox2.Text);
                    textBox2.Enabled = false;
                }
            }

            result.initial_index = comboBox1.SelectedIndex;
            result.textBox2.Text = textBox2.Text;
            result.textBox2.ForeColor = textBox2.ForeColor;
            result.textBox2.Visible = textBox2.Visible;
            result.trackBar1.Value = mul_ratio;
            result.Show();

            reset.Text = "New Scheduler";
            reset.Location = new Point(484, 38);
            reset.Click += (s, ea) => {
                scheduler_app n = new scheduler_app();
                n.Show();
                this.Dispose(false);
            };
            submit.Visible = false;
            this.Controls.Add(reset);
        }
    }
}
