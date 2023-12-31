﻿
using System.Data;
using System.Diagnostics;
//using Android.Locations;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using UniVerse.Components;
using UniVerse.Models;
using UniVerse.ViewModels;

namespace UniVerse.Screens
{
	public class StaffMemberOverviewScreen : ContentPage
	{
        public int StaffId { get; private set; }
        private PeopleViewModel _viewModel;

        private readonly Label name;
        private readonly Label role;
        private readonly Label mail;
        private readonly Label cell;
        private readonly Label hourlyRate;

        private List<SubjectEnrollments> enrollments;
        private readonly FlexLayout layout;

        public StaffMemberOverviewScreen()
		{
            _viewModel = new PeopleViewModel(new Services.RestService());
            Shell.SetBackgroundColor(this, Color.FromArgb("#F6F7FB"));
            NavigationPage.SetHasNavigationBar(this, false);
         
            Shell.SetTabBarIsVisible(this, false);

            Style textStyle = new(typeof(Label))
            {
                Setters =
                {
                    new Setter { Property = Label.FontSizeProperty, Value =  16},
                    new Setter { Property = Label.TextColorProperty, Value = Color.FromArgb("#2B2B2B") }
                }
            };

            // Top 
            Image image = new()
            {
                Aspect = Aspect.AspectFill,
                WidthRequest = 230,
                HeightRequest = 230,
                Source = ImageSource.FromFile("lecturer_profile.png"),

            };
            
            var clip1 = new EllipseGeometry { Center = new Point(230 / 2, 230 / 2), RadiusX = 230 / 2, RadiusY = 230 / 2 };

            image.Clip = clip1;

            Border imgBorder = new()
            {
                WidthRequest = 235,
                HeightRequest = 235,
                StrokeShape = new Ellipse(),
                HorizontalOptions = LayoutOptions.Center,
                StrokeThickness = 6,
                //Margin = new Thickness(0, -7, 0, 15),
                Stroke = Color.FromArgb("#407BFF"),
                Content = image

            };

            name = new Label
            {
                TextColor = Color.FromArgb("#2B2B2B"),
                FontAttributes = FontAttributes.Bold,
                FontSize = 24
            };

            role = new Label
            {
                TextColor = Color.FromArgb("#C5C5C5"),
                FontAttributes = FontAttributes.Bold,
                FontSize = 20
            };

            hourlyRate = new Label
            {
                Text = "\U0001F4B2 R400/h",
                Style = textStyle,
                Margin = new Thickness(0, 20, 0, 0)
            };

            cell = new Label
            {
                Text = "\U0000260E 076 887 6675",
                Style = textStyle,
            };

            mail = new Label
            {
                Style = textStyle,
            };

            StackLayout stackLayout = new()
            {
                Padding = new Thickness(10, 0),
                Margin = new Thickness(30, 15, 0, 0),

                Children =
                {
                    name,
                    role,
                    hourlyRate,
                    cell,
                    mail
                }
            };

            FlexLayout topContainer = new()
            {
                Direction = FlexDirection.Row,

                Children =
                {
                    imgBorder,
                    stackLayout,
                }
            };

            // Cards 

            layout = new FlexLayout()
            {
                Direction = FlexDirection.Row,
                Wrap = FlexWrap.Wrap,
                JustifyContent = FlexJustify.Start,
                AlignItems = FlexAlignItems.Start,
            };

            enrollments = new List<SubjectEnrollments>();

            ScrollView scrollView = new()
            {
             
                Content = layout
            };

            StaffMemberRightBar right = new();


            //Delete
            Button delete = new()
            {
                Margin = new Thickness(8, 12),
                Text = "Delete Staff Member",
                BackgroundColor = Color.FromArgb("#FF4040"),
                ImageSource = ImageSource.FromFile("trash.png")
            };
            delete.Clicked += DeleteStaffMember;

            StackLayout deleteStack = new()
            {
                VerticalOptions = LayoutOptions.End,
                Children =
                {
                    delete
                }
            };


            Grid grid = new()
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition{Height = 245},
                    new RowDefinition{Height = GridLength.Star}
                },

                ColumnDefinitions = new ColumnDefinitionCollection
                {
                 new ColumnDefinition { Width = new GridLength(70, GridUnitType.Star) },
                 new ColumnDefinition { Width = new GridLength(30, GridUnitType.Star) }
                }
            };


            grid.Children.Add(topContainer);
            Grid.SetRow(topContainer, 0);
            Grid.SetColumn(topContainer, 0);
            Grid.SetColumnSpan(topContainer, 1);

            grid.Children.Add(scrollView);
            Grid.SetRow(scrollView, 1);
            Grid.SetColumn(scrollView, 0);
            Grid.SetColumnSpan(scrollView, 1);
            grid.BackgroundColor = Color.FromArgb("#F6F7FB");


            grid.Children.Add(right);
            Grid.SetColumn(right, 1);
            Grid.SetColumnSpan(right, 2);
            Grid.SetRowSpan(right, 2);

            grid.Children.Add(deleteStack);
            Grid.SetColumn(deleteStack, 1);
            Grid.SetColumnSpan(deleteStack, 1);
            Grid.SetRowSpan(deleteStack, 3);



            grid.BackgroundColor = Color.FromArgb("#F6F7FB");
            grid.Padding = 6;


            Content = grid;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is NavOverviewViewModel viewModel)
            {
                if (viewModel.NavigationParameter is int memberIdValue)
                {
                    StaffId = memberIdValue;
                }
            }

            var staffMember = await _viewModel.GetStaffMember(StaffId);

            if (staffMember != null)
            {
                String payment = staffMember.role == "Admin" ? "/Month" : "/hour";
                name.Text = staffMember.lecturer_name;
                role.Text = staffMember.role;
                mail.Text = staffMember.email;
                cell.Text = staffMember.lecturer_phoneNumber;
                hourlyRate.Text = staffMember.lecturer_rate.ToString() + payment;

                enrollments = staffMember.enrollments;

                CreateAndAddEnrollmentCards();
            }
        }

        private async void DeleteStaffMember(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete Staff Member", "Are you sure you want to delete this staff member?", "Yes", "No");

            if (answer)
            {
                await _viewModel.DeletePerson(StaffId);
                await DisplayAlert("Success!", "Staff member deleted successfully.", "OK");
                _ = Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Oops!", "The staff member was not deleted.", "OK");
            }
        }

        private void CreateAndAddEnrollmentCards()
        {
            layout.Children.Clear();

            foreach (var enrollment in enrollments)
            {
                if (enrollment.subject_color != null && enrollment.subject_id != 0)
                {
                    var card = new EnrolledSubjects(enrollment.subject_name, enrollment.subject_code, enrollment.subject_color)
                    {
                        BindingContext = enrollment
                    };
                    FlexLayout.SetBasis(card, new FlexBasis(0.50f, true));
                    layout.Children.Add(card);
                }
                else
                {
                    // If subject_id is null, display an image
                    var image = new Image
                    {
                        Source = "notfound.png",
                        Aspect = Aspect.AspectFit,
                        WidthRequest = 700,
                        HeightRequest = 700,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid imageGrid = new()
                    {
                        RowDefinitions = new RowDefinitionCollection
                        {
                            new RowDefinition
                            {
                                Height = GridLength.Star
                            }
                        },
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition
                            {
                                Width = GridLength.Star
                            }
                        }
                    };
                    imageGrid.Children.Add(image);
                    Grid.SetRow(image, 1);
                    Grid.SetColumn(image, 1);
                    FlexLayout.SetBasis(imageGrid, new FlexBasis(1f, true));
                    layout.Children.Add(imageGrid);
                }
            }
        }
    }
}