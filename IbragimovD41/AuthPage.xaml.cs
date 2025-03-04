using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace IbragimovD41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    /// 


    // loginDEftn2018

    // gPq+a}

    public partial class AuthPage : Page
	{

		private string captchaString = "";
		private bool isFirst = true;

		public AuthPage()
		{
			InitializeComponent();
		}

		private async void LoginButton_Click(object sender, RoutedEventArgs e)
		{

			

			string login = LoginTB.Text;
			string password = PassTB.Text;
			if (login == "" || password == "" || (CaptchaTBox.Text == "" && CaptchaStack.Visibility == Visibility.Visible))
			{
				MessageBox.Show("Есть пустые поля");
				return;
			}

			if (isFirst)
			{
				User user = IbragimovD41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
				if (user != null)
				{
					Manager.MainFrame.Navigate(new ProductPage(user));
					LoginTB.Text = "";
					PassTB.Text = "";
				} else
				{
					MessageBox.Show("Введены некорректные данные");
					CaptchaStack.Visibility = Visibility.Visible;

					string symb = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
					int symbLength = symb.Length;

					Random rnd = new Random();

					captchaString = symb[rnd.Next(0, symbLength)].ToString();
					captchaString += symb[rnd.Next(0, symbLength)];
					captchaString += symb[rnd.Next(0, symbLength)];
					captchaString += symb[rnd.Next(0, symbLength)];

					UpdateCaptchaStrings();

					CaptchaStack.Visibility = Visibility.Visible;
					CaptchaTBox.Visibility = Visibility.Visible;

					isFirst = false;
				}

			} else
			{
				

				if (CaptchaTBox.Text == null)
				{
					MessageBox.Show("Введите капчу");
				}

				if (CaptchaTBox.Text != captchaString)
				{
					MessageBox.Show("Вы непрвильно ввели капчу");

                    LoginButton.IsEnabled = false;

                    string symb = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                    int symbLength = symb.Length;

                    Random rnd = new Random();

                    captchaString = symb[rnd.Next(0, symbLength)].ToString();
                    captchaString += symb[rnd.Next(0, symbLength)];
                    captchaString += symb[rnd.Next(0, symbLength)];
                    captchaString += symb[rnd.Next(0, symbLength)];

                    UpdateCaptchaStrings();

                    await Task.Delay(10000);

                    LoginButton.IsEnabled = true;

                }
                else
				{
                    User user = IbragimovD41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);


                    if (user != null)
					{
						Manager.MainFrame.Navigate(new ProductPage(user));
						LoginTB.Text = "";
						PassTB.Text = "";
						CaptchaStack.Visibility = Visibility.Hidden;
					}
					else
					{

						MessageBox.Show("Введены некорректные данные пользователя");

						LoginButton.IsEnabled = false;

                        string symb = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                        int symbLength = symb.Length;

                        Random rnd = new Random();

                        captchaString = symb[rnd.Next(0, symbLength)].ToString();
                        captchaString += symb[rnd.Next(0, symbLength)];
                        captchaString += symb[rnd.Next(0, symbLength)];
                        captchaString += symb[rnd.Next(0, symbLength)];

						UpdateCaptchaStrings();

                        await Task.Delay(10000);
						
						LoginButton.IsEnabled = true;
					}
					
				}
			}
		}

		void UpdateCaptchaStrings()
		{
			captchaOneWord.Text = captchaString[0].ToString();
			captchaTwoWord.Text = captchaString[1].ToString();
			captchaThreeWord.Text = captchaString[2].ToString();
			captchaFourWord.Text = captchaString[3].ToString();
        }

		private void GuestButton_Click(object sender, RoutedEventArgs e)
		{
			Manager.MainFrame.Navigate(new ProductPage(null));

            LoginTB.Text = "";
            PassTB.Text = "";
			CaptchaTBox.Text = "";

			CaptchaStack.Visibility = Visibility.Hidden;
			LoginButton.IsEnabled = true;
			isFirst = true;
			
		}
	}
}
