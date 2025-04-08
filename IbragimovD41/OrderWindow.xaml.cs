﻿using System;
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
using System.Windows.Shapes;

namespace IbragimovD41
{
	/// <summary>
	/// Логика взаимодействия для OrderWindow.xaml
	/// </summary>
	public partial class OrderWindow : Window
	{

		List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
		List<Product> selectedProducts = new List<Product>();
		private Order currentOrder = new Order();
		private OrderProduct currentOrderProduct = new OrderProduct();

		private int currentOrderID;

		private DateTime orderFormDate;
		private DateTime orderDeliveryDate;

		private User user;
		
		public bool isSaved = false;


		public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO, User user)
		{
			InitializeComponent();

			if (user == null)
			{
				this.user = null;
			} else
			{
				this.user = user;
			}

			var currentPickups = IbragimovD41Entities.GetContext().PickUpPoint.ToList();
			PickupCombo.ItemsSource = currentPickups;

			this.selectedOrderProducts = selectedOrderProducts;
			this.selectedProducts = selectedProducts;

			currentOrderID = selectedOrderProducts.First().OrderID;

            ClientTB.Text = FIO;
			TBOrderID.Text = selectedOrderProducts.First().OrderID.ToString();

			ShoeListView.ItemsSource = this.selectedProducts;

			foreach (Product p in this.selectedProducts)
			{
				p.Quantity = 1;

				foreach (OrderProduct q in this.selectedOrderProducts)
				{
					if (p.ProductArticleNumber == q.ProductArticleNumber)
					{
						p.Quantity = q.ProductCount;
					}
				}
			}

			orderFormDate = DateTime.Now;
			OrderFormDP.Text = orderFormDate.ToShortDateString();

			//calcAndChangeOrderSum();

			updateOrderSum();

            SetDeliveryDate();
		}

		private void SetDeliveryDate()
		{

			bool isFast = true;



			foreach (Product selectedProduct in selectedProducts)
			{
				//var product = IbragimovD41Entities.GetContext().Product.Find(selectedProduct.ProductArticleNumber);
				if (selectedProduct.ProductQuantityInStock < 3)
				{
					isFast = false;
				}
			}


			if (!isFast)
			{
				orderDeliveryDate = orderFormDate.AddDays(3);

			} else
			{
				orderDeliveryDate = orderFormDate.AddDays(6);

			}

			OrderDeliveryDP.Text = orderDeliveryDate.ToShortDateString();

		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			// coooooooode!!!!!!!!!!!!!!!!!

			// order

			if (selectedProducts.Count() == 0)
			{
				MessageBox.Show("Заказывать нечего");
				return;
			}
			if (PickupCombo.SelectedItem == null)
			{
                MessageBox.Show("Укажите пункт выдачи");
				return;
            }



            int newOrderCode = IbragimovD41Entities.GetContext().Order.OrderByDescending(p => p.OrderCode).First().OrderCode + 1;

			if (user == null)
			{
				currentOrder.OrderClientID = null;

			} else
			{
				currentOrder.OrderClientID = user.UserID;
			}

			currentOrder.OrderID = currentOrderID;
			currentOrder.OrderCode = newOrderCode;
			currentOrder.OrderStatus = "Новый";
			currentOrder.OrderDate = orderFormDate;
			currentOrder.OrderDeliveryDate = orderDeliveryDate;
			currentOrder.OrderPickupPoint = PickupCombo.SelectedIndex + 1;

			IbragimovD41Entities.GetContext().Order.Add(currentOrder);

			// сомнительно
			foreach(OrderProduct product in selectedOrderProducts)
			{

				var newOrderProduct = new OrderProduct() // если переиспользовать одну и ту же переменную, созданную вне цикла, то сохраняться будет последний товар, поэтому здесь сделано так, а не так как было
				{
					OrderID = currentOrderID,
					ProductArticleNumber = product.ProductArticleNumber,
					ProductCount = product.Quantity
				};

				//currentOrderProduct.ProductArticleNumber = product.ProductArticleNumber;
				//currentOrderProduct.ProductCount = product.Quantity;

				IbragimovD41Entities.GetContext().OrderProduct.Add(newOrderProduct);
            }


            try
            {

				IbragimovD41Entities.GetContext().SaveChanges();
				MessageBox.Show("Заказ сохранен");

				selectedProducts.Clear();
				selectedOrderProducts.Clear();

				isSaved = true;

				BasketPrice.cost = 0;

				this.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message.ToString());
			}

		}

		private void BtnPlus_Click(object sender, RoutedEventArgs e)
		{
			var prod = (sender as Button).DataContext as Product;
			prod.Quantity++;

			var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

			int index = selectedOrderProducts.IndexOf(selectedOP);

			selectedOrderProducts[index].Quantity++;
			SetDeliveryDate();
			ShoeListView.Items.Refresh();


            decimal prodPriceWithDiscount = prod.ProductCost - prod.ProductCost / 100 * (decimal)prod.ProductDiscountAmount;

            BasketPrice.increaseCost(prodPriceWithDiscount);
			updateOrderSum();
            //calcAndChangeOrderSum();


        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
		{
			var prod = (sender as Button).DataContext as Product;

			var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
			

			if (prod.Quantity == 1)
			{
				selectedProducts.Remove(prod);
				selectedOrderProducts.Remove(selectedOP);
			} else
			{
				prod.Quantity--;


				int index = selectedOrderProducts.IndexOf(selectedOP);

				selectedOrderProducts[index].Quantity--;

			}
            //calcAndChangeOrderSum();

            decimal prodPriceWithDiscount = prod.ProductCost - prod.ProductCost / 100 * (decimal)prod.ProductDiscountAmount;
            BasketPrice.decreaseCost(prodPriceWithDiscount);
			updateOrderSum();


			SetDeliveryDate();
			ShoeListView.Items.Refresh();
		}

		public void updateOrderSum()
		{

			//float sum = 0;

			//foreach (OrderProduct selectedProductOrder in this.selectedOrderProducts)
			//{
			//	float discount = 100 - selectedProductOrder.Product.ProductMaxDiscount;
			//	//float price = (float)selectedProductOrder.Product.ProductCost;
			//	//float finalCost = price * discount;

			//	//sum += price;

			//}

			TBOrderSum.Text = "Сумма заказа: " + BasketPrice.cost;
		}

    }
}
