using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace DominoTable13
{
    public partial class Form1 : Form
    {

        private Object lockshow = new object(); // блокируем для отображения. чтобы совместить вывод нескольких потоков
        private Thread[] Threads; // перечень потоков поиска для завершения
        private DominoTable[] DTables; // Таблицы домино для потоков

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DominoTable Table = new DominoTable(Decimal.ToByte(inputK.Value),
                Decimal.ToByte(inputN.Value), 0, 0, SetInfo, SetText, chkShow.Checked ? 0 : 1000000000);
            textBox1.Text = Table.GetAllDominoText();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            byte MaxI = 28; // число костяшек домино
            byte ThreadNum = Decimal.ToByte(InputThreadNum.Value); // число потоков

            if (Threads != null)
            {
                // завершим потоки
                foreach (Thread thr in Threads) thr.Abort();
            }

            if (ThreadNum == 0)
            {
                DominoTable DTtemp = new DominoTable(Decimal.ToByte(inputK.Value),
                        Decimal.ToByte(inputN.Value), 0, 0, SetInfo, SetText, chkShow.Checked ? 0 : 1000000000);
                DTtemp.Solve();
            }
            else
            {
                Threads = new Thread[ThreadNum];

                DTables = new DominoTable[ThreadNum];

                // разные для разных потоков значения поиска
                byte Step = (byte)(MaxI / ThreadNum); // число домино на один поток в первой позиции таблицы (0,0)
                if (MaxI % ThreadNum > 0) Step++;

                byte StartI = 0; // начальная костяшка домино для потока в первой позиции таблицы (0,0)
                byte EndI = Step; // конечная костяшка домино для потока в первой позиции таблицы (0,0)

                for (byte tmp = 0; tmp < ThreadNum; tmp++)
                {
                    DTables[tmp] = new DominoTable(Decimal.ToByte(inputK.Value),
                        Decimal.ToByte(inputN.Value), StartI, EndI, SetInfo, SetText, chkShow.Checked ? 0 : 1000000000);

                    Threads[tmp] = new Thread(DTables[tmp].Solve);
                    Threads[tmp].Start();

                    //SetText("Thread #" + tmp.ToString() + ", start " + StartI.ToString() + ", end " + EndI.ToString() + Environment.NewLine);

                    // значения для следующего потока
                    StartI = (byte)(EndI + 1);
                    EndI += Step;
                    if (EndI >= MaxI) EndI = (byte)(MaxI - 1); // работаем с номерами костяшек от 0 до MaxI-1
                }
            }
        }

        public void SetInfo(String str)
        {
            lock (lockshow)
            {
                if (labelx.InvokeRequired)
                {
                    SetText d = new SetText(SetInfoSafe);
                    Invoke(d, new object[] { str });
                }
                else SetInfoSafe(str);
            }
        }
        private void SetInfoSafe(String str)
        {
                labelx.Text = str;
                labelx.Refresh();
        }
        public void SetText(String text)
        {
            lock (lockshow)
            {
                if (textBox1.InvokeRequired)
                {
                    SetText d = new SetText(SetTextSafe);
                    Invoke(d, new object[]{ text });
                }
                else SetTextSafe(text);
            }
        }

        private void SetTextSafe(String text)
        {
            textBox1.Text += text;
            textBox1.SelectionStart = textBox1.Text.Length - 1;
            textBox1.ScrollToCaret();
            textBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DominoTable Table = new DominoTable(Decimal.ToByte(inputK.Value),
                Decimal.ToByte(inputN.Value), 0, 0, SetInfo, SetText, chkShow.Checked ? 0 : 1000000000);
            textBox1.Text = Table.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime Start = DateTime.Now;

            textBox1.Text += Environment.NewLine + "старт: " + Start.ToString() + Environment.NewLine;

            //RepSeq(4);
            //UInt64 count = perm(4, true, 2); //Decimal.ToInt32(numericUpDown1.Value), chkShow.Checked, Decimal.ToInt32(inputK.Value));
            UInt64 count = ShowAllCombinations(Decimal.ToByte(inputN.Value), Decimal.ToByte(inputK.Value), chkShow.Checked);

            DateTime End = DateTime.Now;

            textBox1.Text += "конец: " + End.ToString() + Environment.NewLine;
            textBox1.Text += "Всего (сек.): " + (End - Start).TotalSeconds + Environment.NewLine;
            textBox1.Text += "Кол-во перестановок: " + count.ToString() + Environment.NewLine;
        }

        // показываем все сочетания из n элементов в k позиций.
        // N - число элементов
        // K - число позиций в которые устанавливаем элементы
        UInt64 ShowAllCombinations(int n, int k, bool show = false, int row=4)
        {
            if (k > n || k <= 0 || n <= 0) return 0;

            UInt64 count = 0; // число перестановок

            
            // каждый поток имеет свой массив элементов для сочетаний.
            //
            int ThreadMax = 2; // число потоков поиска
            uint step = (uint)(n / ThreadMax); // число элементов на один поток в первой позиции
            
            int[] arr = new int[k]; // массив сочетаний
            

            

            // заполняем массив первым сочетанием
            for (int i = 0; i < k; i++) arr[i] = i+1;

            count++;
            if(show) ShowArray(arr, k, k);

            while(getNextCombin(arr, k, n))
            {
                count++;
                if (show) ShowArray(arr, k, k);
            }
            return count;
        }

        bool getNextCombin(int[] a, int k, int n)
        {
             if (k > n || k <= 0 || n <= 0) return false;


            //set it to the last position
            //in the array (the last column)
            int i = k - 1;
            int col = 0;


            //find the first column in which
            //the (value+1) does not exceed (n-col_num)
            while (i >= 0)
            {
                if (++a[i] <= n - col)
                    break;

                --i;
                ++col;
            }

            //if no combinations remain
            if (i < 0)
                return false;

            //increase the next right-hand combinations
            for (; i < k - 1; ++i)
            {
                a[i + 1] = a[i] + 1;
            }

            return true;
        }

        UInt64 perm(int N, bool show = false, int row=4)
        {
            UInt64 count = 0; // число перестановок

            int[] p;
            int[] pc;

            p = new int[N+1];
            pc = new int[N+1];
            bool[] pr;

            pr = new bool[N+1];
            
            int i,x,k,d;

            for(i=1;i<=N; i++)
            {
	            p[i]=i;
	            pc[i]=1;
                pr[i] = true; 
	        }
            pc[N]=0;
            i=1;
  
            if(show) ShowArray(p, N, row, "f");
            count++;
  
            while(i < N)
	        {
		        i=1;
		        x=0;
		        while( pc[i] == N-i+1)
		        {
		            pr[i]= !pr[i];
		            pc[i]=1;
		            if( pr[i])
		            {
			            x++;
			            i++;
		            }
		        }
		        if( i < N)
		        {
		            if( pr[i]) k=pc[i]+x;
		            else k = N-i+1-pc[i]+x;
		  
		            d=p[k];
		            p[k]=p[k+1];
		            p[k+1]=d;

                    if (show) ShowArray(p, N, row, ".");
                    count++;
		  
		            pc[i]++;
                }
            }

            return count;
        }

        void ShowArray(int[] arr, int N, int row, string pref = "")
        {
            textBox1.Text += pref;
            for (int iter_ = 0, x = 1; iter_ < N; iter_++, x++)
            {
                if (x > row)
                {
                    textBox1.Text += Environment.NewLine + " ";
                    x = 1;
                }
                textBox1.Text += arr[iter_].ToString("D2") + "_";
            }
            textBox1.Text += Environment.NewLine;
        }

        void RepSeq(int N)
        {
            int[] A = new int[N+1];
            for (int iter_ = 1; iter_ <= N; iter_++) A[iter_] = iter_;

            textBox1.Text = "";

            for (int ind = 1; ; ind++)
            {
                ShowArray(A, N, 4);

                int a = ind;
                int p = 0;
                int j;

                for (j = N; j > 0; j--)
                {
                    int x = a % j;
                    a /= j;
                    if (x == 0)
                    {
                        if (a % 2 == 0) x = j - x;
                        // меняем местами  A[p + x] и A[p - 1 + x]
                        textBox1.Text += (p + x).ToString() + "<->" + (p - 1 + x).ToString() + Environment.NewLine;
                        //int tmp_ = A[p + x];
                        //A[p + x] = A[p - 1 + x];
                        //A[p - 1 + x] = tmp_;
                        break;
                    }
                    else p += 1 - a % 2;
                }

                if (j != 0) break;
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            if (Threads == null) return;

            // завершим потоки
            foreach (Thread thr in Threads) thr.Abort();
        }

    }
}
