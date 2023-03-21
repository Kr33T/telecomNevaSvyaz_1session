using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace telecomNevaSvyaz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer timer = new DispatcherTimer();
        int countTime = 10;
        string code;

        public MainWindow()
        {
            InitializeComponent();
            DBHelper.bm = new Entities();
            numberTB.Focus();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (countTime != 0)
            {
                timerOutputTB.Text = $"У Вас осталось {countTime} секунд";
                countTime -= 1;
            }
            else
            {
                timer.Stop();
                timerOutputTB.Text = $"";
                countTime = 10;
                if (!codeTB.Text.Equals(code))
                {
                    MessageBox.Show("Время истекло");
                    codeTB.Text = "";
                    codeTB.IsEnabled = false;
                }
                else
                {
                    Employee emp = DBHelper.bm.Employee.FirstOrDefault(x => x.Nomer.Equals(numberTB.Text) && x.Password.Equals(passwordPB.Password));
                    MessageBox.Show($"Вы успешно авторизовались, {emp.Surname} {emp.Name} {emp.Patronymic}!\nВаша роль: {emp.Role.Role1}");
                }
                
            }
        }
        //mmOf#$u3
        private void numberTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                List<Employee> emp = DBHelper.bm.Employee.Where(x => x.Nomer == numberTB.Text).ToList();
                if (emp.Count >= 1)
                {
                    numberTB.IsEnabled = false;
                    passwordPB.IsEnabled = true;
                    passwordPB.Focus();
                }
                else
                {
                    MessageBox.Show("сотрудник с таким номером не найден");
                }
            }
        }

        private void refreshI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            codeTB.IsEnabled = true;
            refreshI.IsEnabled = true;
            enterBTN.IsEnabled = true;
            codeTB.Focus();
            generateCode();
            MessageBox.Show($"Чтобы авторизоваться Вам необходимо ввести следующий код: {code}\nНа данное действие Вам будет дано 10 секунд.");
            timer.Start();
        }

        private void enterBTN_Click(object sender, RoutedEventArgs e)
        {
            login();
        }
        
        private void cancelBTN_Click(object sender, RoutedEventArgs e)
        {
            numberTB.IsEnabled = true;
            numberTB.Text = "";
            passwordPB.IsEnabled = false;
            passwordPB.Password = "";
            codeTB.IsEnabled = false;
            codeTB.Text = "";
            refreshI.IsEnabled = false;
            enterBTN.IsEnabled = false;
            timer.Stop();
            timerOutputTB.Text = $"";
            countTime = 10;
        }

        private void passwordPB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                List<Employee> emp = DBHelper.bm.Employee.Where(x => x.Password == passwordPB.Password).ToList();
                if (emp.Count == 1)
                {
                    passwordPB.IsEnabled = false;
                    codeTB.IsEnabled = true;
                    refreshI.IsEnabled = true;
                    enterBTN.IsEnabled = true;
                    codeTB.Focus();
                    generateCode();
                    MessageBox.Show($"Чтобы авторизоваться Вам необходимо ввести следующий код: {code}\nНа данное действие Вам будет дано 10 секунд.");
                    timer.Start();
                }
                else
                {
                    MessageBox.Show("Пароль введен неверно");
                }
            }
        }

        private void codeTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                login();    
            }
        }

        void login()
        {
            if (codeTB.Text == code)
            {
                Employee emp = DBHelper.bm.Employee.FirstOrDefault(x => x.Nomer.Equals(numberTB.Text) && x.Password.Equals(passwordPB.Password));
                MessageBox.Show($"Вы успешно авторизовались, {emp.Surname} {emp.Name} {emp.Patronymic}!\nВаша роль: {emp.Role.Role1}");
                timer.Stop();
                timerOutputTB.Text = "";
                countTime = 10;
            }
            else
            {
                MessageBox.Show("Код введен неверно");
            }
        }

        void generateCode()
        {
            Random rnd = new Random();
            while (true)
            {
                code = "";
                for (int i = 0; i < 8; i++)
                {
                    int symbol = rnd.Next(4);
                    switch (symbol)
                    {
                        case 0:
                            code += rnd.Next(9).ToString();
                            break;
                        case 1:
                            code += (char)rnd.Next('a', 'z');
                            break;
                        case 2:
                            code += (char)rnd.Next('A', 'Z');
                            break;
                        case 3:
                            code += (char)rnd.Next(33, 39);
                            break;
                    }
                }
                if (Regex.IsMatch(code, "^[0-9a-zA-Z!@#$%^&*?]{8}$"))
                {
                    break;
                }
            }
        }
    }
}
