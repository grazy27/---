using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        int[] Bj = new int[4];
        int[] Ai = new int[4];
        public Form1(int[] a, int[] b)
        {
            InitializeComponent();
            for (int i = 0; i < 4; i++)
            {
                Ai[i] = a[i];
                Bj[i] = b[i];
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {
            int[,] Weights = new int[4, 4];
            int[,] Amount = new int[4, 4];

            int[] PotencialA = new int[4];
            int[] PotencialB = new int[4];
            int[] tempBj = new int[4];
            int[] tempAi = new int[4];
            int Str = -1, Stlb = -1;

            try
            {
                Weights[0, 0] = int.Parse(textBox1.Text);
                Weights[0, 1] = int.Parse(textBox8.Text);
                Weights[0, 2] = int.Parse(textBox12.Text);
                Weights[0, 3] = int.Parse(textBox16.Text);
                Weights[1, 0] = int.Parse(textBox2.Text);
                Weights[1, 1] = int.Parse(textBox7.Text);
                Weights[1, 2] = int.Parse(textBox11.Text);
                Weights[1, 3] = int.Parse(textBox15.Text);
                Weights[2, 0] = int.Parse(textBox3.Text);
                Weights[2, 1] = int.Parse(textBox6.Text);
                Weights[2, 2] = int.Parse(textBox10.Text);
                Weights[2, 3] = int.Parse(textBox14.Text);
                Weights[3, 0] = int.Parse(textBox4.Text);
                Weights[3, 1] = int.Parse(textBox5.Text);
                Weights[3, 2] = int.Parse(textBox9.Text);
                Weights[3, 3] = int.Parse(textBox13.Text);

            }
            catch (Exception ED)
            { MessageBox.Show(Convert.ToString(ED)); }

            if (!check(ref Ai, ref Bj)) { MessageBox.Show("Задача не может быть решена так как суммы А и В не сходятся."); return; }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    Amount[i, j] = 0;
                tempAi[i] = Ai[i];
                tempBj[i] = Bj[i];
            }
            if (checkBox1.Checked) Show_matrix(ref Weights, "Введённая матрица");


            if (comboBox1.SelectedIndex == 1) Find_plan_using_smallest_item(ref tempAi, ref tempBj, ref Amount, ref Str, ref Stlb, ref Weights);
            else if (comboBox1.SelectedIndex == 0) Find_plan_using_north_west_method(ref tempAi, ref tempBj, ref Amount, ref Weights);
            else
            {
                MessageBox.Show("Выберите план нахождения опорного плана");
                return;
            }

            if (checkBox1.Checked) Show_matrix(ref Amount, "Матрица после нахождения опорного плана");
            //  trying to find potencials
            for (;;)
            {
                for (int i = 0; i < 4; i++)
                {
                    PotencialA[i] = -300;
                    PotencialB[i] = -300;
                }
                PotencialA[0] = 0;

                find_cooficients(ref Amount, ref Weights, ref PotencialA, ref PotencialB);
                if (checkBox1.Checked) Show_cooficients(ref PotencialA, ref PotencialB);

                int X = -1, Y = -1;

                if (check_for_optimal(ref Amount, ref Weights, ref PotencialA, ref PotencialB, ref X, ref Y))
                    break;

                graph solex = new graph(X, Y);
                //need to b 2,1
                solex.findChild(ref Weights, ref Amount);


                Renew_matrix(ref Weights, ref Amount, X, Y);

                if (checkBox1.Checked) Show_matrix(ref Amount, "Матрица после улучшения опорного плана");
            }
            Show_matrix(ref Amount, "Оптимальное решение найдено");
            MessageBox.Show("Минимальные затраты составят - " + Convert.ToString(show_result(ref Weights, ref Amount)));
            PlanAnalysys(ref Amount);
        }


        private int show_result(ref int[,] Weigts, ref int[,] Amount)
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (Amount[i, j] != 0)
                        result += Amount[i, j] * Weigts[i, j];
            return result;
        }

        private void Renew_matrix(ref int[,] Weights, ref int[,] Amounts, int X, int Y)
        {
            int A = 0, B = 0, minVal = 500;
            for (int i = 0; solution[i, 0] != -300 || solution[i, 1] != -300; i += 2)

                if (Amounts[solution[i, 0], solution[i, 1]] < minVal)
                {
                    minVal = (Amounts[solution[i, 0], solution[i, 1]]);
                    A = solution[i, 0];
                    B = solution[i, 1];
                }

            for (int i = 0; solution[i, 0] != -300; i++)
            {
                if (solution[i, 0] == A && solution[i, 1] == B)
                    continue;
                if (i % 2 == 0) Amounts[solution[i, 0], solution[i, 1]] -= minVal; // minus
                else if (i % 2 == 1) Amounts[solution[i, 0], solution[i, 1]] += minVal; // plus
            }
            Amounts[A, B] -= minVal;


        }
        public static int[,] solution = new int[16, 2];
        private class graph
        {
            public static int STR, STLB;
            int depth;
            public static int[,] way = new int[16, 2];
            public int fatherA, fatherB, a, b;
            public string state;
            public static bool cycle_is_finded;
            public graph[] child;
            public int num_of_childs = 0;
            bool up = false, down = false, right = false, left = false;

            public graph(int A, int B)
            {
                STR = A; STLB = B;
                a = A; b = B;
                cycle_is_finded = false;
                depth = 1;
            }
            public graph(int A, int B, int FatherA, int FatherB, int Depth, string s)
            {
                state = s;
                switch (state)
                {
                    case "up":
                        { down = true; up = true; break; }
                    case "down":
                        { up = true; down = true; break; }
                    case "left":
                        { right = true; left = true; break; }
                    case "right":
                        { left = true; right = true; break; }
                }
                fatherA = FatherA;
                fatherB = FatherB;
                a = A;
                b = B;
                depth = Depth + 1;

            }
            public void solution_found()
            {
                for (int i = 0; i < 16; i++)
                {
                    if (i < depth)
                    {
                        solution[i, 0] = way[i, 0];
                        solution[i, 1] = way[i, 1];
                    }
                    else
                    {
                        solution[i, 0] = -300;
                        solution[i, 1] = -300;
                    }
                }
            }

            public bool child_exists(ref int[,] Weights, ref int[,] Amount)
            {
                //UP   
                if (!up)
                    for (int i = a - 1; i >= 0; i--)
                        if ((Amount[i, b] != 0) || (i == STR && b == STLB && depth > 3))
                        {
                            if (i == STR && b == STLB && depth > 3)
                            {
                                way[depth - 1, 0] = i;
                                way[depth - 1, 1] = b;
                                cycle_is_finded = true;
                                solution_found();
                            }
                            state = "up";
                            return true;
                        }
                        else if (i == fatherA && b == fatherB) break;
                // DOWN
                if (!down)
                    for (int i = a + 1; i < 4; i++)
                        if ((Amount[i, b] != 0) || (i == STR && b == STLB && depth > 3))
                        {
                            if (i == STR && b == STLB && depth > 3)
                            {
                                way[depth - 1, 0] = i;
                                way[depth - 1, 1] = b;
                                cycle_is_finded = true;
                                solution_found();
                            }
                            state = "down";
                            return true;
                        }
                        else if (i == fatherA && b == fatherB) break;
                //Right
                if (!right)
                    for (int i = b + 1; i < 4; i++)
                        if ((Amount[a, i] != 0) || (a == STR && i == STLB && depth > 3))

                        {
                            if (a == STR && i == STLB && depth > 3)
                            {
                                way[depth - 1, 0] = a;
                                way[depth - 1, 1] = i;
                                cycle_is_finded = true;
                                solution_found();
                            }
                            state = "right";
                            return true;
                        }
                        else if (a == fatherA && i == fatherB) break;
                //Left
                if (!left)
                    for (int i = b - 1; i >= 0; i--)
                        if ((Amount[a, i] != 0) || (a == STR && i == STLB && depth > 3))
                        {
                            if (a == STR && i == STLB && depth > 3)
                            {
                                way[depth - 1, 0] = a;
                                way[depth - 1, 1] = i;
                                cycle_is_finded = true;
                                solution_found();
                            }
                            state = "left";
                            return true;
                        }
                state = "";
                return false;
            }

            public void findChild(ref int[,] Weights, ref int[,] Amount)
            {
                for (; !cycle_is_finded && child_exists(ref Weights, ref Amount);)
                {
                    if (cycle_is_finded) break;
                    switch (state)
                    {
                        case "up":
                            {
                                for (int i = a - 1; i >= 0; i--)
                                    if (Amount[i, b] != 0 && !cycle_is_finded)
                                    {
                                        num_of_childs++;
                                        up = true;
                                        way[depth - 1, 0] = i;
                                        way[depth - 1, 1] = b;
                                        Array.Resize(ref child, num_of_childs);
                                        child[num_of_childs - 1] = new graph(i, b, a, b, depth, state);
                                        child[num_of_childs - 1].findChild(ref Weights, ref Amount);


                                    }

                                break;
                            }
                        case "down":
                            {
                                for (int i = a + 1; i < 4; i++)
                                    if (Amount[i, b] != 0 && !cycle_is_finded)
                                    {
                                        down = true;
                                        num_of_childs++;
                                        way[depth - 1, 0] = i;
                                        way[depth - 1, 1] = b;
                                        Array.Resize(ref child, num_of_childs);
                                        child[num_of_childs - 1] = new graph(i, b, a, b, depth, state);
                                        child[num_of_childs - 1].findChild(ref Weights, ref Amount);

                                    }
                                break;
                            }
                        case "right":
                            {
                                for (int i = b + 1; i < 4; i++)
                                    if (Amount[a, i] != 0 && !cycle_is_finded)
                                    {
                                        right = true;
                                        num_of_childs++;
                                        way[depth - 1, 0] = a;
                                        way[depth - 1, 1] = i;
                                        Array.Resize(ref child, num_of_childs);
                                        child[num_of_childs - 1] = new graph(a, i, a, b, depth, state);
                                        child[num_of_childs - 1].findChild(ref Weights, ref Amount);

                                    }
                                break;
                            }
                        case "left":
                            {
                                for (int i = b - 1; i >= 0; i--)
                                    if (Amount[a, i] != 0 && !cycle_is_finded)
                                    {
                                        left = true;
                                        num_of_childs++;
                                        way[depth - 1, 0] = a;
                                        way[depth - 1, 1] = i;
                                        Array.Resize(ref child, num_of_childs);
                                        child[num_of_childs - 1] = new graph(a, i, a, b, depth, state);
                                        child[num_of_childs - 1].findChild(ref Weights, ref Amount);

                                    }
                                break;
                            }

                    }

                }
            }
        };

        public bool check_for_optimal(ref int[,] Amount, ref int[,] Weights, ref int[] PotencialA, ref int[] PotencialB, ref int X, ref int Y)
        {
           
                int maxval = -500;
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        if (Amount[i, j] == 0 && (Weights[i, j] < PotencialA[i] + PotencialB[j]))
                        {
                            if (maxval < PotencialA[i] + PotencialB[j] - Weights[i, j])
                            {
                                maxval = PotencialA[i] + PotencialB[j] - Weights[i, j];
                                X = i; Y = j;
                            }
                        }


                return (maxval == -500);
           
        }


        public void find_cooficients(ref int[,] Amount, ref int[,] Weights, ref int[] PotencialA, ref int[] PotencialB)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {

                    if (PotencialA[i] != -300 && PotencialB[j] == -300) //добавить наоборот
                        if (Amount[i, j] != 0)
                            PotencialB[j] = Weights[i, j] - PotencialA[i];
                }

                for (int x = 0; x < 4; x++)
                {
                    if (PotencialB[i] != -300 && PotencialA[x] == -300) //добавить наоборот
                        if (Amount[x, i] != 0)
                        {
                            PotencialA[x] = Weights[x, i] - PotencialB[i];
                        }
                }

                if (i == 3)
                    for (int c = 0; c < 4; c++)//проверка на завершённость
                    {
                        if (PotencialA[c] == -300 || PotencialB[c] == -300)
                            i = -1;
                    }

            }


        }
        private bool summIsZero(ref int[] a)
        {
            int summ = 0;
            for (int i = 0; i < 4; i++)
                summ += a[i];
            return summ == 0;
        }
        private void findmin(ref int[,] a, ref int[,] b, ref int str, ref int stlb, ref int[] Arrstr, ref int[] Arrstrb)
        {
            int min = 500000;
            str = -1; stlb = -1;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (a[i, j] < min && b[i, j] == 0 && Arrstr[i] != 0 && Arrstrb[j] != 0)
                    { min = a[i, j]; str = i; stlb = j; }
        }
        private bool check(ref int[] a, ref int[] b)
        {
            int A = 0, B = 0;
            for (int i = 0; i < 4; i++)
            {
                A += a[i];
                B += b[i];
            }
            if (A == B) return true;
            return false;
        }
        private void Show_matrix(ref int[,] a, string message)
        {
            String res = "";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    res += a[i, j] + " ";
                    if (a[i, j] < 10) res += " ";
                }
                res += '\n';
            }
            MessageBox.Show(message + "\n" + res);

        }
        private void Show_cooficients(ref int[] a, ref int[] b)
        {
            String res = "Коофициенты U";
            for (int j = 0; j < 4; j++)
                res += " " + a[j];

            res += "\nКоофициенты V";
            for (int j = 0; j < 4; j++)
                res += " " + b[j];

            MessageBox.Show(res);

        }

        private void Find_plan_using_smallest_item(ref int[] tempAi, ref int[] tempBj, ref int[,] Amount, ref int Str, ref int Stlb, ref int[,] Weights)
        {
            for (; !summIsZero(ref tempAi) && !summIsZero(ref tempBj);)
            {
                findmin(ref Weights, ref Amount, ref Str, ref Stlb, ref tempAi, ref tempBj);
                if (Str == -1 && Stlb == -1) break;

                if (tempAi[Str] >= tempBj[Stlb])
                {
                    Amount[Str, Stlb] = tempBj[Stlb];
                    tempAi[Str] -= tempBj[Stlb];

                    tempBj[Stlb] = 0;

                }
                else
                {
                    Amount[Str, Stlb] = tempAi[Str];
                    tempBj[Stlb] -= tempAi[Str];
                    tempAi[Str] = 0;

                }
            }

        }
        private void Find_plan_using_north_west_method(ref int[] tempAi, ref int[] tempBj, ref int[,] Amount, ref int[,] Weights)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)

                    if (tempAi[i] != 0 && tempBj[j] != 0)
                    {
                        if (tempAi[i] >= tempBj[j])
                        {
                            Amount[i, j] += tempBj[j];
                            tempAi[i] -= tempBj[j];
                            tempBj[j] = 0;
                        }
                        else
                        {
                            Amount[i, j] += tempAi[i];
                            tempBj[j] -= tempAi[i];
                            tempAi[i] = 0;
                        }
                    }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        private void PlanAnalysys(ref int[,] Amount)
        {
            string[,] words = new string[2, 4] { { "центрального", "южного", "восточного", "северного" }, { "Мастер", "Intertool", "Toptool", "СанМастер" } };
            string message="";
            for (int i = 0; i < 4; i++)
            {
                message += "Из "+words[0, i]+" склада груз необходимо направить в";
                for (int j = 0; j < 4; j++)
                {
                    if (Amount[i, j] != 0)
                        message +=" "+words[1,j]+"("+Amount[i,j]+") единиц,";
                }
                message += '\n';
            }
            MessageBox.Show(message);
            //[x,y] x - склады: Центр юг, вост, север, у - магазины Мастер, интертул, топтул, СанМастер

        }
    }
}
