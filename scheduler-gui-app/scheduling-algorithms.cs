using System.Collections.Generic;

namespace scheduler_gui_app
{
public struct Scheduler
    {
        public List<string> processes;
        public List<float> arrival_time;
        public List<float> duration;
        public List<int> priority;
        public List<float> remaining_duration;
        public List<bool> status; // 1 finished - 0 waiting
        public int no; // number of processes
        public int stdno;
        public float quantum;
        public List<string> processes_std;
        public List<float> arrival_time_std;
        public List<float> duration_std;
        public List<int> priority_std;
        
        public void Reset(int x = 0)
        {
            if (processes_std == null)
            {
                processes_std = new List<string>();
                arrival_time_std = new List<float>();
                duration_std = new List<float>();
                priority_std = new List<int>();
            }
            if (processes_std.Count == 0)
            {
                for (int i = 0; i < stdno; i++)
                {
                    processes_std.Add(processes[i]);
                    arrival_time_std.Add(arrival_time[i]);
                    duration_std.Add(duration[i]);
                }
            }
            if (priority_std.Count == 0 && x != 0)
            {
                processes = new List<string>();
                arrival_time = new List<float>();
                duration = new List<float>();
                no = stdno;
                for (int i = 0; i < stdno; i++)
                {
                    processes.Add(processes_std[i]);
                    arrival_time.Add(arrival_time_std[i]);
                    duration.Add(duration_std[i]);
                    priority_std.Add(priority[i]);
                }
            }
            Add_Idle_Time();
            remaining_duration = new List<float>();
            status = new List<bool>();
            for (int i = 0; i < no; i++)
            {
                remaining_duration.Add(duration[i]);
                status.Add(false);
            }
        }
        private void Sort()
        {
            for (int i = 0; i < no; i++)
                for (int j = i + 1; j < no; j++)
                    if (arrival_time[j] < arrival_time[i])
                    {
                        string temps = processes[i]; processes[i] = processes[j]; processes[j] = temps;
                        float tempf = arrival_time[i]; arrival_time[i] = arrival_time[j]; arrival_time[j] = tempf;
                        tempf = duration[i]; duration[i] = duration[j]; duration[j] = tempf;
                        tempf = remaining_duration[i]; remaining_duration[i] = remaining_duration[j]; remaining_duration[j] = tempf;
                        int tempi = priority[i]; priority[i] = priority[j]; priority[j] = tempi;
                        bool tempb = status[i]; status[i] = status[j]; status[j] = tempb;
                    }
        }
        private void Add_Idle_Time()
        {
            Sort();
            float time_not_spent = 0;
            if (arrival_time[0] > 0)
            {
                processes.Add("idle");
                arrival_time.Add(0);
                duration.Add(arrival_time[0]);
                remaining_duration.Add(arrival_time[0]);
                priority.Add(0);
                status.Add(false);
                no++;
                Sort();
            }
            for (int i = 0, stdno = no; i < stdno - 1; i++)
            {
                if (arrival_time[i + 1] <= arrival_time[i] + duration[i] + time_not_spent)
                {
                    time_not_spent += (arrival_time[i] + duration[i] - arrival_time[i + 1]);
                }
                else
                {
                    processes.Add("idle");
                    arrival_time.Add(arrival_time[i] + duration[i] + time_not_spent);
                    duration.Add(arrival_time[i + 1] - (arrival_time[i] + duration[i] + time_not_spent));
                    remaining_duration.Add(arrival_time[i + 1] - (arrival_time[i] + duration[i] + time_not_spent));
                    priority.Add(0);
                    status.Add(false);
                    no++;
                    time_not_spent = 0;
                }
            }
            Sort();
        }
    }
public class Time_Scheduler
    {
        public Scheduler scheduler_data = new Scheduler();
        public List<string> processes = new List<string>();
        public List<float> start_time = new List<float>();
        public List<float> end_time = new List<float>();
        public int output_size = 0;
        public float average_waiting_time;
        public bool status = false; // 1 finished - 0 waiting

        /*private bool Scheduler_Data_Status()
        {
            for (int i = 0; i < scheduler_data.no; i++)
                if (!scheduler_data.status[i]) return false;
            return true;
        }*/
        private void reset(int x = 0)
        {
            scheduler_data.Reset(x);
            processes = new List<string>();
            start_time = new List<float>();
            end_time = new List<float>();
            output_size = 0;
            status = false;
        }
        public void First_Come_First_Served()
        {
            reset();
            for (int i = 0; i < scheduler_data.no; i++)
            {
                processes.Add(scheduler_data.processes[i]);
                if (i == 0)
                    start_time.Add(0);
                else
                    start_time.Add(end_time[i - 1]);
                end_time.Add(start_time[i] + scheduler_data.duration[i]);
            }
            output_size = scheduler_data.no;
            calculate_average_waiting_time();
        }
        public void Shortest_Job_First_Preemptive()
        {
            reset();
            int next_process = Choose_Next_Process_Shortest_Job_First(0);
            float next_break = Choose_Next_Break(0);
            int processes_finished = 0;
            while (!status)
            {
                processes.Add(""); start_time.Add(0); end_time.Add(0);
                processes[processes_finished] = scheduler_data.processes[next_process];
                if (processes_finished == 0)
                    start_time[processes_finished] = 0;
                else
                    start_time[processes_finished] = end_time[processes_finished - 1];
                if (start_time[processes_finished] + scheduler_data.remaining_duration[next_process] > next_break)
                {
                    end_time[processes_finished] = next_break;
                    scheduler_data.remaining_duration[next_process] -= (next_break - start_time[processes_finished]);
                }
                else
                {
                    end_time[processes_finished] = start_time[processes_finished] + scheduler_data.remaining_duration[next_process];
                    scheduler_data.status[next_process] = true;
                }
                next_break = Choose_Next_Break(end_time[processes_finished]);
                next_process = Choose_Next_Process_Shortest_Job_First(end_time[processes_finished]);
                processes_finished++;
            }
            output_size = processes_finished;
            calculate_average_waiting_time();
        }
        public void Shortest_Job_First_Non_Preemptive()
        {
            reset();
            int next_process = Choose_Next_Process_Shortest_Job_First(0);
            int processes_finished = 0;
            while (!status && processes_finished < scheduler_data.no)
            {
                processes.Add(""); start_time.Add(0); end_time.Add(0);
                processes[processes_finished] = scheduler_data.processes[next_process];
                if (processes_finished == 0)
                    start_time[processes_finished] = 0;
                else
                    start_time[processes_finished] = end_time[processes_finished - 1];
                end_time[processes_finished] = start_time[processes_finished] + scheduler_data.duration[next_process];
                scheduler_data.status[next_process] = true;
                next_process = Choose_Next_Process_Shortest_Job_First(end_time[processes_finished]);
                processes_finished++;
            }
            output_size = processes_finished;
            calculate_average_waiting_time();
        }
        private int Choose_Next_Process_Shortest_Job_First(float current_time)
        {
            int index = -1;
            float shortest_time = float.MaxValue, arrival_time = -1;

            for (int i = 0; i < scheduler_data.no; i++)
            {
                if (current_time >= scheduler_data.arrival_time[i] && scheduler_data.status[i] == false)
                    if (scheduler_data.remaining_duration[i] < shortest_time)
                    {
                        shortest_time = scheduler_data.remaining_duration[i];
                        arrival_time = scheduler_data.arrival_time[i];
                        index = i;
                    }
                    else if (scheduler_data.remaining_duration[i] == shortest_time)
                        if (scheduler_data.arrival_time[i] < arrival_time)
                        {
                            shortest_time = scheduler_data.remaining_duration[i];
                            arrival_time = scheduler_data.arrival_time[i];
                            index = i;
                        }
            }

            if (index == -1) status = true;
            return index;
        }
        public void Priority_Preemptive()
        {
            reset(1);
            int next_process = Choose_Next_Process_Priority(0);
            float next_break = Choose_Next_Break(0);
            int processes_finished = 0;
            while (!status)
            {
                processes.Add(""); start_time.Add(0); end_time.Add(0);
                processes[processes_finished] = scheduler_data.processes[next_process];
                if (processes_finished == 0)
                    start_time[processes_finished] = 0;
                else
                    start_time[processes_finished] = end_time[processes_finished - 1];
                if (start_time[processes_finished] + scheduler_data.remaining_duration[next_process] > next_break)
                {
                    end_time[processes_finished] = next_break;
                    scheduler_data.remaining_duration[next_process] -= (next_break - start_time[processes_finished]);
                }
                else
                {
                    end_time[processes_finished] = start_time[processes_finished] + scheduler_data.remaining_duration[next_process];
                    scheduler_data.status[next_process] = true;
                }
                next_break = Choose_Next_Break(end_time[processes_finished]);
                next_process = Choose_Next_Process_Priority(end_time[processes_finished]);
                processes_finished++;
            }
            output_size = processes_finished;
            calculate_average_waiting_time();
        }
        public void Priority_Non_Preemptive()
        {
            reset(1);
            int next_process = Choose_Next_Process_Priority(0);
            int processes_finished = 0;
            while (!status && processes_finished < scheduler_data.no)
            {
                processes.Add(""); start_time.Add(0); end_time.Add(0);
                processes[processes_finished] = scheduler_data.processes[next_process];
                if (processes_finished == 0)
                    start_time[processes_finished] = 0;
                else
                    start_time[processes_finished] = end_time[processes_finished - 1];
                end_time[processes_finished] = start_time[processes_finished] + scheduler_data.duration[next_process];
                scheduler_data.status[next_process] = true;
                next_process = Choose_Next_Process_Priority(end_time[processes_finished]);
                processes_finished++;
            }
            output_size = processes_finished;
            calculate_average_waiting_time();
        }
        private int Choose_Next_Process_Priority(float current_time)
        {
            int index = -1, priority = int.MaxValue;
            float arrival_time = -1;

            for (int i = 0; i < scheduler_data.no; i++)
            {
                if (current_time >= scheduler_data.arrival_time[i] && scheduler_data.status[i] == false)
                    if (scheduler_data.priority[i] < priority)
                    {
                        priority = scheduler_data.priority[i];
                        arrival_time = scheduler_data.arrival_time[i];
                        index = i;
                    }
                    else if (scheduler_data.priority[i] == priority)
                        if (scheduler_data.arrival_time[i] < arrival_time)
                        {
                            priority = scheduler_data.priority[i];
                            arrival_time = scheduler_data.arrival_time[i];
                            index = i;
                        }
            }

            if (index == -1) status = true;
            return index;
        }
        public void Round_Robin()
        {
            reset();
            Queue<int> rr_queue = new Queue<int>();
            int latest_element_in = 0;
            float current_time = 0;
            int processes_finished = 0;
            while (!status)
            {
                status = true;
                if (latest_element_in < scheduler_data.no)
                    for (int proc = latest_element_in; scheduler_data.arrival_time[proc] <= current_time; proc++)
                    {
                        rr_queue.Enqueue(proc);
                        latest_element_in++;
                        if (latest_element_in == scheduler_data.no) break;
                    }
                while (rr_queue.Count != 0)
                {
                    int proc = rr_queue.Peek();
                    if (scheduler_data.arrival_time[proc] <= current_time && !scheduler_data.status[proc])
                    {
                        processes.Add(""); start_time.Add(0); end_time.Add(0);
                        status = false;
                        processes[processes_finished] = scheduler_data.processes[proc];
                        start_time[processes_finished] = current_time;
                        if (scheduler_data.remaining_duration[proc] <= scheduler_data.quantum)
                        {
                            end_time[processes_finished] = current_time + scheduler_data.remaining_duration[proc];
                            current_time = end_time[processes_finished];
                            scheduler_data.remaining_duration[proc] = 0;
                            scheduler_data.status[proc] = true;
                        }
                        else
                        {
                            end_time[processes_finished] = current_time + scheduler_data.quantum;
                            current_time = end_time[processes_finished];
                            scheduler_data.remaining_duration[proc] -= scheduler_data.quantum;
                            if (latest_element_in < scheduler_data.no)
                                for (int procc = latest_element_in; scheduler_data.arrival_time[procc] <= current_time; procc++)
                                {
                                    rr_queue.Enqueue(procc);
                                    latest_element_in++;
                                    if (latest_element_in == scheduler_data.no) break;
                                }
                            rr_queue.Enqueue(proc);
                        }
                        processes_finished++;
                    }
                    rr_queue.Dequeue();
                }
            }
            output_size = processes_finished;
            calculate_average_waiting_time();
        }
        private float Choose_Next_Break(float current_time)
        {
            float break_time = float.MaxValue;
            for (int i = 0; i < scheduler_data.no; i++)
            {
                if (current_time < scheduler_data.arrival_time[i] && break_time > scheduler_data.arrival_time[i])
                    break_time = scheduler_data.arrival_time[i];
            }
            return break_time;
        }
        private void calculate_average_waiting_time()
        {
            average_waiting_time = 0;
            for (int i = 0; i < scheduler_data.no; i++)
                if (scheduler_data.processes[i] != "idle")
                    for (int j = output_size - 1; j >= 0; j--)
                        if (scheduler_data.processes[i] == processes[j])
                        {
                            average_waiting_time += (end_time[j] - scheduler_data.arrival_time[i] - scheduler_data.duration[i]);
                            break;
                        }
            average_waiting_time = average_waiting_time / scheduler_data.stdno;
        }
    }
}
