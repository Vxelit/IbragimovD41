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
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {

        public ProductPage(User user)
        {
            InitializeComponent();

            if (user == null)
            {
                FIOTB.Text += "Гость";
                RoleTB.Text += "Гость";
            } else
            {
                FIOTB.Text += user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;

                RoleTB.Text += user.UserRoleText;
            }

            var currentProducts = IbragimovD41Entities.GetContext().Product.ToList();

            ProductListView.ItemsSource = currentProducts;

            ComboType.SelectedIndex = 0;
        }

        public void UpdateProducts() {
            var currentProducts = IbragimovD41Entities.GetContext().Product.ToList();

            if (ComboType.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && Convert.ToInt32(p.ProductDiscountAmount) <= 10)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 10 && Convert.ToInt32(p.ProductDiscountAmount) <= 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 15)).ToList();
            }

            currentProducts = currentProducts.Where(p => (p.ProductName.ToLower().Contains(SearchTBox.Text.ToLower()))).ToList();
        
            if (RButtonDown.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            }

            if (RButtonUp.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderBy(p => p.ProductCost).ToList();
            }

            ProductsCountText.Text = "кол-во " + currentProducts.Count + " из " + IbragimovD41Entities.GetContext().Product.ToList().Count(); ;

            ProductListView.ItemsSource = currentProducts;
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();

        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();

        }

        private void SearchTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
