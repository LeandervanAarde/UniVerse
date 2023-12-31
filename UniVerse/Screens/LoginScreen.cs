﻿using System;
using System.Diagnostics;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using UniVerse.ViewModels;

namespace UniVerse.Screens
{
    public class LoginScreen : ContentPage
    {
        private LoginViewModel AuthVM;
     

        Color borderColor = Colors.Gray;
        public LoginScreen()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                IsVisible = false
            });

            AuthVM = new LoginViewModel(new Services.RestService());
            BindingContext = AuthVM;

            NavigationPage.SetHasNavigationBar(this, false);

            Style inputStyle = new(typeof(Entry))
            {
                Setters =
                {
                    new Setter { Property = InputView.BackgroundColorProperty, Value = Color.FromArgb("#F6F7FB") },
                    new Setter { Property = InputView.MarginProperty, Value = new Thickness(15, 5) },
                    new Setter { Property = InputView.TextColorProperty, Value = Color.FromArgb("#2B2B2B") },
                }
            };
            Style borderStyle = new (typeof(Border))
            {
                Setters =
                {
                    new Setter { Property = Border.BackgroundColorProperty, Value = borderColor },
                    new Setter { Property = Border.StrokeThicknessProperty, Value = new Thickness(3) },
                    new Setter { Property = Border.PaddingProperty, Value = new Thickness(2) },
                    new Setter { Property = Border.MarginProperty, Value = new Thickness(10, 0) }
                }
            };

            Image loginMainImage = new()
            {
                Source = ImageSource.FromFile("login_image.png"),
                Aspect = Aspect.AspectFit,
                MaximumHeightRequest = 730,
                MaximumWidthRequest = 700,
            };

            Label loginTitle = new ()
            {
                Text = "Sign In",
                FontSize = 32,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2B2B2B"),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(15, 10, 0, 10)
            };

            Label ErrorTitle = new()
            {
                
                FontSize = 15,
                TextColor = Color.FromArgb("#FF4040"),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 5)
            };

            ErrorTitle.SetBinding(Label.TextProperty, new Binding("AuthError", source: AuthVM));

            Entry email = new()
            {
                Placeholder = "Email",
                Style = inputStyle,
                Margin = 0,  
              
            };

            email.SetBinding(Entry.TextProperty, new Binding("EmailEntry", source: AuthVM));

            Border emailBorder = new()
            {
                Content = email,
               
                StrokeShape = new RoundRectangle()
                {
                    CornerRadius = 10,
                },
                Style = borderStyle,
            };

            email.Unfocused += ValidateEmail;

            Entry password = new()
            {
                Placeholder = "Password",
                Style = inputStyle,
                IsPassword = true,
                Margin = 0,
                
            };
            password.SetBinding(Entry.TextProperty, new Binding("PasswordEntry", source: AuthVM));

            Border passwordBorder = new()
            {
                Content = password,
                Margin = new Thickness(10),

                StrokeShape = new RoundRectangle()
                {
                    CornerRadius = 10,
                },
                Style = borderStyle,
            };

            password.Unfocused += ValidatePassword;

            Button signinButton = new()
            {
                Text = "Sign In",
                BackgroundColor = Color.FromArgb("#2B2B2B"),
                Margin = new Thickness(18, 15)
            };

            signinButton.Clicked +=  async (sender, args) =>
            {
                try
                {
                 await AuthVM.AuthenticatedStream();
                }
                catch(AuthenticationException authEx)
                { 
                    Debug.WriteLine(authEx.Message);
                    //ErrorTitle.Text = authEx.Message;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                    //ErrorTitle.Text = ex.Message;
                }
            };

            FlexLayout login = new()
            {
                JustifyContent = FlexJustify.Center,
                Direction = FlexDirection.Column,
                
                Children = { loginTitle, ErrorTitle, emailBorder, passwordBorder, signinButton }
            };

            Border loginCard = new()
            {
                WidthRequest = 500,
                HeightRequest = 450,
                BackgroundColor = Color.FromArgb("#DFE9FF"),
                Padding = new Thickness(10, 2),
                Margin = new Thickness(50, 20),
                StrokeThickness = 0,
                Content = login,
                Stroke = Color.FromArgb("#DFE9FF"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(20)
                },
            };

            Image bgImage = new()
            {
                Source = "test.png",
                Aspect = Aspect.AspectFit
            };

            Grid grid = new()
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Star }
                },
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(60, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) },
                },
                BackgroundColor = Color.FromArgb("#FFFFFF"),
            };

            grid.Children.Add(loginMainImage);
            Grid.SetRow(loginMainImage, 0);
            Grid.SetColumn(loginMainImage, 0);
            Grid.SetColumnSpan(loginMainImage, 1);
            grid.Children.Insert(0, bgImage);
            Grid.SetRowSpan(bgImage, 1);
            Grid.SetColumnSpan(bgImage, 2);

            grid.Children.Add(loginCard);
            Grid.SetRow(loginCard, 0);
            Grid.SetColumn(loginCard, 1);
            Grid.SetColumnSpan(loginCard, 1);

            Content = grid;
        }
        private void ValidateEmail(object sender, EventArgs e)
        {
            Entry email = (Entry)sender;

            string emailPattern = @"^[A-Za-z0-9._%+-]+@virtualwindow\.co\.za$";
            string staffEmailPattern = @"^[A-Za-z0-9._%+-]+@openwindow\.co\.za$";
            if (email.Text == null)
                return;
            bool isValid = Regex.IsMatch(email.Text, emailPattern);
            bool staffValid = Regex.IsMatch(email.Text, staffEmailPattern);
            Color colorString;

            Console.WriteLine("Students: " + isValid + " Staff: " + staffValid);
            if (!isValid && !staffValid)
            {
                colorString = Color.FromArgb("#FF4040");
            }
            else
            {
                colorString = Color.FromArgb("#FFFFFF");
            }

            // Find the parent Border by traversing the visual tree
            Border parentBorder = LoginScreen.FindParent<Border>(email);
            if (parentBorder != null)
            {
                parentBorder.BackgroundColor = colorString;
            }
        }

        private void ValidatePassword(object sender, EventArgs e)
        {
            Entry password = (Entry)sender;
            if (password.Text == null)
                return;

            bool isValid = password.Text.Length >= 8;
            Color colorString;

            if (!isValid)
            {
                colorString = Color.FromArgb("#FF4040");
            }
            else
            {
                colorString = Color.FromArgb("#FFFFFF");
            }

            Border parentBorder = LoginScreen.FindParent<Border>(password);
            if (parentBorder != null)
            {
                parentBorder.BackgroundColor = colorString;
            }
        }

        private static E FindParent<E>(Element element) where E : Element
        {
            Element parent = element.Parent;
            while (parent != null)
            {
                if (parent is E typedParent)
                {
                    return typedParent;
                }
                parent = parent.Parent;
            }
            return null;
        }
    }
}
