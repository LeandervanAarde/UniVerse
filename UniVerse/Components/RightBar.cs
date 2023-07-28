﻿using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace UniVerse.Components
{
    public class RightBar : ContentView
    {
        public RightBar()
        {
            Style inputStyle = new(typeof(Entry))
            {
                Setters =
                {
                    new Setter { Property = InputView.BackgroundColorProperty, Value = Color.FromHex("#F6F7FB") },
                    new Setter { Property = InputView.MarginProperty, Value = new Thickness(6, 3) }
                }
            };

            ProfileView profileContainer = new();

            Label heading = new()
            {
                Text = "Add new Student",
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(5, 3),
                TextColor = Color.FromHex("#2B2B2B"),
                FontAttributes = FontAttributes.Bold,

            };


            Image defaultimage = new()
            {
                Source = ImageSource.FromFile("image_picker.png"),
                Aspect = Aspect.AspectFit,
                MaximumHeightRequest = 170,
                MaximumWidthRequest = 160,
            };

            Button imageUploadButton = new()
            {
                Text = "Upload Image",
                BackgroundColor = Colors.Transparent,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(5,0),
                TextColor = Color.FromHex("#407BFF"),
            };

            FlexLayout innerLayout = new FlexLayout()
            {
                MaximumHeightRequest = 200,
                MaximumWidthRequest = 200,

                Direction = FlexDirection.Column,
                Children =
                {
                    defaultimage,
                    imageUploadButton
                }
            };

            Border border = new()
            {
                StrokeThickness = 1,
                Stroke = Color.FromHex("#2B2B2B"),
                StrokeDashArray = new DoubleCollection(new double[] { 8, 3 }),
                StrokeDashOffset = 6,
                Content = innerLayout,
                BackgroundColor = Color.FromHex("#F6F7FB"),
                Margin = new Thickness(6),
                Padding = new Thickness(10,2),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(6)
                },
            };

            Entry name = new Entry()
            {
                Placeholder = "Student Name",
                Style = inputStyle

            };

            Entry surname = new()
            {
                Placeholder = "Student Surname",
                Style = inputStyle
            };

            Entry studentNumber = new()
            {
                Placeholder = "Student Number",
                Style = inputStyle
            };

            var listOptions = new List<String>      
            {
                "Student Type",
                "Degree Student",
                "Certificate Student"
            };

            Picker studentRole = new()
            {
                Style = inputStyle,
              

            };

             foreach (var option in listOptions)
            {
                studentRole.Items.Add(option);
            }
            studentRole.SelectedItem = "Student Type";
            studentRole.TextColor = Colors.Black;
            studentRole.TitleColor = Colors.Black;

            Entry email = new()
            {
                Placeholder = "email",
                Style = inputStyle

            };

            Entry phoneNumber = new()
            {
                Placeholder = "email",
                Style = inputStyle,
                MaxLength = 10
            };


            Button button = new()
            {
                Text =  "Add Student",
                BackgroundColor = Color.FromHex("#2B2B2B"),
                Margin = new Thickness(6, 3)
            };



            FlexLayout layout = new()
            {
                BackgroundColor = Colors.White,
                Direction = FlexDirection.Column,
       
                Children =
                {
                    profileContainer,
                    heading,
                    border,
                    name,
                    surname,
                    studentNumber, 
                    studentRole,
                    phoneNumber,
                    button
                },

               
            };
            Content = layout;
        }
    }
}
