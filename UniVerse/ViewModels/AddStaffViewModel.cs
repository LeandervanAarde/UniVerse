﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UniVerse.Controls.RadialBarChart;
using UniVerse.Models;
using UniVerse.Services;
using static UniVerse.Models.LecturerOverviewModel;

namespace UniVerse.ViewModels
{
    internal class AddStaffViewModel : BaseViewModel
    {
        public RestService _restService;
        // All of myt observerd properties 

        public ObservableCollection<Person> StaffList { get; set; }
        public ObservableCollection<Person> AllStaffList { get; set; }
        public ObservableCollection<Student> StudentList { get; set; }
        public ObservableCollection<LecturerWithCourses> StaffMember { get; set; }
        public ObservableCollection<SingleStudentWithCourses> Student { get; set; }
        public ObservableCollection<ChartEntry> Chart { get; set; }
        public ObservableCollection<ChartEntry> StaffChart { get; set; }

        public string _nameEntry = string.Empty;
        public string NameEntry
        {
            get { return _nameEntry; }
            set
            {
                _nameEntry = value;
                OnPropertyChanged(nameof(NameEntry));
            }
        }

        public string _surnameEntry = string.Empty;
        public string SurnameEntry
        {
            get { return _surnameEntry; }
            set
            {
                _surnameEntry = value;
                OnPropertyChanged(nameof(SurnameEntry));
            }
        }

        public string _identifier = string.Empty;
        public string Identifier
        {
            get { return _identifier; }
            set
            {
                _identifier = value;
                OnPropertyChanged(nameof(Identifier));
            }
        }

        public string _emailEntry = string.Empty;
        public string EmailEntry
        {
            get { return _emailEntry; }
            set
            {
                _emailEntry = value;
                OnPropertyChanged(nameof(EmailEntry));
            }
        }

        public string _number = string.Empty;
        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        public int _role_Input;
        public int RoleInput
        {
            get { return _role_Input; }
            set
            {
                _role_Input = value;
                OnPropertyChanged(nameof(RoleInput));
            }
        }

        public AddStaffViewModel(RestService restService) //instance of the restservice goes here
        {
            _restService = restService;
            StaffList = new ObservableCollection<Person>();
            StudentList = new ObservableCollection<Student>();
            StaffMember = new ObservableCollection<LecturerWithCourses>();
            Student = new ObservableCollection<SingleStudentWithCourses>();
            Chart = new ObservableCollection<ChartEntry>();
            StaffChart = new ObservableCollection<ChartEntry>();
            AllStaffList = new ObservableCollection<Person>();
            NameEntry = String.Empty;
            EmailEntry = String.Empty;
            Number = String.Empty;
            Identifier = String.Empty;
            SurnameEntry = String.Empty;
        }

        // Get Staff
        public async Task GetAllStaffMembers()
        {
            var members = await _restService.RefreshDataAsync();
            StaffList.Clear();

            foreach (var member in members)
            {
                StaffList.Add(member);
            }
        }

        //Get staff member by id
        public async Task<LecturerWithCourses> GetStaffMember(int id)
        {
            var member = await _restService.GetLecturerByIdAsync(id);
            StaffMember.Add(member);
            return member;
        }

        public async Task GetAllStaff()
        {
            var members = await _restService.GetStaffMembersAsync();
            AllStaffList.Clear();
            var LecturerStaff = 0;
            var AdminStaff = 0;
            var maximumChartValue = 0;


            foreach (var member in members)
            {
                AllStaffList.Add(member);
                maximumChartValue++;

                if (member.role == "Lecturer")
                {
                    LecturerStaff++;
                }
                else
                {
                    AdminStaff++;
                }
            }
            double LecturerPerecent = Math.Round((double)LecturerStaff / (double)maximumChartValue * 100, 1);
            StaffChart.Add(new ChartEntry
            {
                Value = LecturerPerecent,
                Color = Color.FromArgb("#6023FF"),
                Text = "Visual Studio Code"
            });

            StaffChart.ToArray();
        }

        public async Task<AddpersonModel> AddStaff()
        {

            int userRole = RoleInput == 1 ? 1 : 2;

            AddpersonModel person = new()
            {
                person_id = 0,
                person_system_identifier = Identifier,
                first_name = NameEntry,
                last_name = SurnameEntry,
                person_email = EmailEntry,
                added_date = DateTime.UtcNow,
                person_active = true,
                role = userRole,
                person_image = "None",
                person_credits = 0,
                person_cell = Number,
                needed_credits = 0,
                person_password = "password",
            };

            
            await _restService.AddStaffAsync(person);

            GetAllStaff();

            NameEntry = String.Empty;
            EmailEntry = String.Empty;
            Number = String.Empty;
            Identifier = String.Empty;
            SurnameEntry = String.Empty;

            //NameEntry = String.Empty;
            //EmailEntry = String.Empty;
            //Number = String.Empty;
            //Identifier = String.Empty;
            //SurnameEntry = String.Empty;

            return person;
        }
    }
}
