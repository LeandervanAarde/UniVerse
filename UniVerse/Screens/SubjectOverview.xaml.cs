﻿using System;
using System.Diagnostics;
using UniVerse.ViewModels;

namespace UniVerse.Screens;

public partial class SubjectOverview : ContentPage
{
    public int SubjectId { get; set; }
    private readonly SubjectViewModel _viewModel;
    private readonly StaffViewModel _staffViewModel;
    private readonly StudentViewModel _studentViewModel;

    public SubjectOverview()
	{
		InitializeComponent();

        _viewModel = new SubjectViewModel(new Services.SubjectServices.SubjectService());
        _staffViewModel = new StaffViewModel(new Services.StaffService.StaffService());
        _studentViewModel = new StudentViewModel(new Services.StudentServices.StudentService());
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        studentStackLayout.Clear();

        await _staffViewModel.GetAllStaffMembers();
        await _studentViewModel.GetAllstudents();

        // Populate lectPicker with staff members
        foreach (var staffMember in _staffViewModel.AllStaffList)
        {
            lectPicker.Items.Add(staffMember.name);
        }

        // Populate studentPicker with student names
        foreach (var student in _studentViewModel.StudentList)
        {
            studentPicker.Items.Add(student.name);
        }

        if (BindingContext is NavOverviewViewModel viewModel)
        {
            if (viewModel.NavigationParameter is int id)
            {
                SubjectId = id;
            }

            var subject = await _viewModel.GetSubject(SubjectId);

            if (subject != null)
            {
                nameOfSub.Text = subject.subjectName;
                descOfSub.Text = subject.subjectDescription;
                nameOfLect.Text = subject.lecturer_name;
                emailOfLect.Text = subject.lecturer_email;
            }

            foreach (var enrollment in subject.enrollments)
            {
                if (enrollment.enrollment_id != 0)
                {
                    var studentCard = new Components.StudentCard(_viewModel)
                    {
                        BindingContext = enrollment
                    };
                    studentStackLayout.Children.Add(studentCard);
                }
                else
                {
                    var image = new Image
                    {
                        Source = "nostudents.png",
                        Aspect = Aspect.AspectFit,
                        WidthRequest = 700,
                        HeightRequest = 700,
                    };
                    studentStackLayout.Children.Add(image);
                }
            }
        }
    }

    private async void DeleteSubject(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete Subject", "Are you sure you want to delete this subject?", "Yes", "No");

        if (answer)
        {
            await _viewModel.DeleteSubject(SubjectId);
            await DisplayAlert("Success!", "Subject deleted successfully.", "OK");
            _ = Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Oops!", "The subject was not deleted.", "OK");
        }
    }

    private void AssignStudent(object sender, EventArgs e)
    {
        var selectedStudent = studentPicker.SelectedItem?.ToString();
        Debug.WriteLine(selectedStudent);
    }

    private void AssignLecturer(object sender, EventArgs e)
    {
        var selectedLecturer = lectPicker.SelectedItem?.ToString();
        Debug.WriteLine(selectedLecturer);
    }

}