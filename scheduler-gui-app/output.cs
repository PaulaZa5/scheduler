using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace scheduler_gui_app
{
    public partial class output_gui : Form
    {
        public Time_Scheduler form_time_sched;
        public int initial_index;
        public output_gui()
        {
            InitializeComponent();
        }
        private void output_gui_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = initial_index;
            this.VerticalScroll.Maximum = 0;
            this.HorizontalScroll.Visible = false;
            this.AutoScroll = true;
            draw();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            redraw();
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
                return;
            }
            else if (!float.TryParse(textBox2.Text, out outParse) || outParse <= 0)
            {
                MessageBox.Show("You can't enter anything but positive numbers.");
                textBox2.Text = "Quantum";
                textBox2.ForeColor = SystemColors.InactiveCaption;
                return;
            }
            draw();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                    textBox2.Visible = false;
                    break;
                case 3:
                case 4:
                    if (!(initial_index == 3 || initial_index == 4))
                    {
                        ask_for_priority();
                        initial_index = 3;
                        /*MessageBox.Show("You didn't enter the priority.");
                        comboBox1.SelectedIndex = initial_index;
                        draw();
                        return;*/
                    }
                    textBox2.Visible = false;
                    break;
                case 5:
                    if (textBox2.Text == "Quantum")
                    {
                        MessageBox.Show("Assuming quantum = 1.");
                        textBox2.Text = "1";
                        textBox2.ForeColor = SystemColors.WindowText;
                    }
                    textBox2.Visible = true;
                    break;
                default:
                    break;
            }
            draw();
        }

        private void ask_for_priority()
        {
            TableLayoutPanel priority_table = new TableLayoutPanel();
            priority_table.HorizontalScroll.Maximum = 0;
            priority_table.VerticalScroll.Visible = false;
            priority_table.AutoScroll = true;
            priority_table.Anchor = AnchorStyles.None;

            priority_table.ColumnStyles.Clear();
            priority_table.ColumnCount = 1;
            priority_table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            priority_table.RowStyles.Clear();
            priority_table.RowCount = form_time_sched.scheduler_data.stdno + 1;
            priority_table.AutoSize = false;
            priority_table.Location = new Point(12, 12);
            if (24 * priority_table.RowCount > 700)
                priority_table.Size = new Size(135, 700);
            else
                priority_table.Size = new Size(135, 24 * priority_table.RowCount + 12);
            for (int i = 0; i < priority_table.RowCount; i++)
                priority_table.RowStyles.Add(new ColumnStyle(SizeType.AutoSize));

            List<TextBox> dynamic_priority = new List<TextBox>();
            for (int i = 0; i < form_time_sched.scheduler_data.stdno; i++)
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
                priority_table.Controls.Add(dynamic_priority[i], 0, i);
            }
            Button okay_btn = new Button();
            okay_btn.Text = "Okay";
            okay_btn.Anchor = AnchorStyles.None;
            priority_table.Controls.Add(okay_btn, 0, priority_table.RowCount + 1);
            Form priority_f = new Form();
            okay_btn.Click += (s, ea) =>
            {
                int outi = 0;
                int processes_no = form_time_sched.scheduler_data.stdno;
                for (int i = 0; i < processes_no; i++)
                {
                    if (!Int32.TryParse(dynamic_priority[i].Text, out outi))
                    {
                        MessageBox.Show("You cannot start simulation without completing the input");
                        return;
                    }
                }
                form_time_sched.scheduler_data.priority = new List<int>();
                for (int i = 0; i < processes_no; i++)
                {
                    form_time_sched.scheduler_data.priority.Add(Int32.Parse(dynamic_priority[i].Text));
                }
                priority_f.Hide();
            };

            priority_f.Text = "Priority";
            priority_f.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            priority_f.StartPosition = FormStartPosition.CenterScreen;
            priority_f.MaximizeBox = false;
            priority_f.MinimizeBox = false;
            priority_f.ShowIcon = false;
            priority_f.AutoSize = false;
            if (priority_table.Height + 63 > 700)
                priority_f.Size = new Size(175, 700);
            else
            priority_f.Size = new Size(175, priority_table.Height + 63);
            priority_f.Controls.Add(priority_table);
            priority_f.FormClosing += (s, e) =>
            {
                okay_btn.PerformClick();
            };

            priority_f.ShowDialog();
        }

        private List<Label> form_labels = new List<Label>();
        private List<Label> time_labels = new List<Label>();
        private int labels_no = 0;
        private void delete_labels()
        {
            for (int i = 0; i < labels_no; i++)
            {
                form_labels[i].Dispose();
                time_labels[i].Dispose();
            }
            form_labels = new List<Label>();
            time_labels = new List<Label>();
            labels_no = 0;
        }

        private List<Color> used_colors = new List<Color>();
        private List<string> used_processes = new List<string>();
        private int no_used_processes = 0;
        Random rand = new Random();
        private Color determine_process_color(string process)
        {
            for (int i = 0; i < no_used_processes; i++)
                if (process == used_processes[i])
                    return used_colors[i];

            Color result = Color.FromArgb(rand.Next(-16777216, 0));
            used_processes.Add(process);
            used_colors.Add(result);
            no_used_processes++;

            return result;
        }

        private void draw()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    form_time_sched.First_Come_First_Served();
                    break;
                case 1:
                    form_time_sched.Shortest_Job_First_Preemptive();
                    break;
                case 2:
                    form_time_sched.Shortest_Job_First_Non_Preemptive();
                    break;
                case 3:
                    form_time_sched.Priority_Preemptive();
                    break;
                case 4:
                    form_time_sched.Priority_Non_Preemptive();
                    break;
                case 5:
                    if (textBox2.Text == "Quantum")
                    {
                        MessageBox.Show("Assuming quantum = 1.");
                        textBox2.Text = "1";
                        textBox2.ForeColor = SystemColors.WindowText;
                    }
                    form_time_sched.scheduler_data.quantum = float.Parse(textBox2.Text);
                    form_time_sched.Round_Robin();
                    break;
                default:
                    break;
            }

            /*if (700 < form_time_sched.end_time[form_time_sched.output_size - 1] * trackBar1.Value + 12 * 2)
            {
                this.VerticalScroll.Maximum = 0;
                this.HorizontalScroll.Visible = false;
                this.AutoScroll = true;
            }
            else
            {
                this.Size = new Size((int)(form_time_sched.end_time[form_time_sched.output_size - 1] * trackBar1.Value + 12 * 2.5), 150);
            }*/

            redraw();
            label1.Text = "Average Waiting Time: " + form_time_sched.average_waiting_time.ToString();
            this.Text = "Output - " + comboBox1.Text;
        }
        private void redraw()
        {
            delete_labels();
            int reached_length = 12;
            for (var i = 0; i < form_time_sched.output_size; i++)
            {
                Label lbl = new Label();
                lbl.AutoSize = false;
                lbl.Size = new Size((int)Math.Ceiling((form_time_sched.end_time[i] - form_time_sched.start_time[i]) * trackBar1.Value), 25);
                lbl.Location = new Point(reached_length, 9);
                reached_length += lbl.Size.Width - 1;
                lbl.BorderStyle = BorderStyle.FixedSingle;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = form_time_sched.processes[i];
                /*while (lbl.Width < System.Windows.Forms.TextRenderer.MeasureText(lbl.Text, new Font(lbl.Font.FontFamily, lbl.Font.Size, lbl.Font.Style)).Width)
                {
                    lbl.Font = new Font(lbl.Font.FontFamily, lbl.Font.Size - 0.5f, lbl.Font.Style);
                }*/
                lbl.BackColor = determine_process_color(form_time_sched.processes[i]);
                form_labels.Add(lbl);

                Label tlbl = new Label();
                tlbl.AutoSize = true;
                tlbl.BackColor = Color.Transparent;
                tlbl.Location = new Point(reached_length - 3, 34);
                tlbl.TextAlign = ContentAlignment.MiddleCenter;
                tlbl.Text = form_time_sched.end_time[i].ToString();
                time_labels.Add(tlbl);

                labels_no++;
                this.Controls.Add(lbl);
                this.Controls.Add(tlbl);
            }
        }
    }
}
