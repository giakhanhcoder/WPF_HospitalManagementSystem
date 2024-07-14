#nullable enable
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_HospitalManagementSystem._data;
using WPF_HospitalManagementSystem.ViewModels;

namespace WPF_HospitalManagementSystem.Views
{
    /// <summary>
    /// Interaction logic for Doctors.xaml
    /// </summary>
    public partial class Doctors : Page
    {
        public Doctors() => InitializeComponent();

        private static DbHospitalManagementSystemContext _db = new();

        IQueryable<DoctorViewModel> initialData = _db.TblDoctors.Select(x => new DoctorViewModel()
        {
            Id = x.Id,
            Name = x.Name,
            Surname = x.Surname,
            Branch = x.BranchNavigation.Branch,
            Birthofdate = x.Birthofdate,
            Status = x.Status
        });

        private void Doctors_Load(object sender, RoutedEventArgs e) => Read();

        private void Clear()
        {
            txtName.Text = "";
            txtSurname.Text = "";
            txtId.Text = "";
            txtSearch.Text = "";
            comboBranch.Text = "";
            dateBirth.Text = "";
        }

        private void Read()
        {
            dg.ItemsSource = null;
            dg.ItemsSource = initialData.ToList();

            comboBranch.ItemsSource = (from x in _db.TblBranches
                                       select new
                                       {
                                           x.Id,
                                           x.Branch
                                       }).ToList();
        }

        private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int? docId = (dg.SelectedItem as DoctorViewModel)?.Id;
            txtId.Text = docId.ToString();
            txtName.Text = (dg.SelectedItem as DoctorViewModel)?.Name;
            txtSurname.Text = (dg.SelectedItem as DoctorViewModel)?.Surname;
            comboBranch.Text = (dg.SelectedItem as DoctorViewModel)?.Branch;
            dateBirth.SelectedDate = (dg.SelectedItem as DoctorViewModel)?.Birthofdate;
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string name = CleanUpWhitespace(txtName.Text);
            string surname = CleanUpWhitespace(txtSurname.Text);
            DateTime? birthDate = dateBirth.SelectedDate;
            string branch = comboBranch.Text;

            if (!IsValidName(name))
            {
                MessageBox.Show("Name is not valid.");
                return;
            }

            if (!IsValidSurname(surname))
            {
                MessageBox.Show("Surname is not valid.");
                return;
            }

            if (!IsValidBirthDate(birthDate))
            {
                MessageBox.Show("Birth date is not valid. It must be in the past and at least 18 years ago.");
                return;
            }
            if (string.IsNullOrEmpty(branch))
            {
                MessageBox.Show("You must choose your branch.");
                return;
            }

            TblDoctor newDoctor = new TblDoctor()
            {
                Name = name,
                Surname = surname,
                Branch = _db.TblBranches.SingleOrDefault(x => x.Branch == comboBranch.Text)?.Id,
                Birthofdate = birthDate,
                Status = true
            };

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname))
            {
                _db.TblDoctors.Add(newDoctor);
                _ = await _db.SaveChangesAsync();
                Clear();
                Read();
            }
        }

        private void btnRead_Click(object sender, RoutedEventArgs e) => Read();

        private async void btnUptd_Click(object sender, RoutedEventArgs e)
        {
            int? docId = (dg.SelectedItem as DoctorViewModel)?.Id;
            if (docId != null)
            {
                string name = CleanUpWhitespace(txtName.Text);
                string surname = CleanUpWhitespace(txtSurname.Text);
                DateTime? birthDate = dateBirth.SelectedDate;
                string branch = comboBranch.Text;

                if (!IsValidName(name))
                {
                    MessageBox.Show("Name is not valid.");
                    return;
                }

                if (!IsValidSurname(surname))
                {
                    MessageBox.Show("Surname is not valid.");
                    return;
                }

                if (!IsValidBirthDate(birthDate))
                {
                    MessageBox.Show("Birth date is not valid. It must be in the past and at least 18 years ago.");
                    return;
                }
                if (string.IsNullOrEmpty(branch))
                {
                    MessageBox.Show("You must choose your branch.");
                    return;
                }

                TblDoctor docToUpdate = (from a in _db.TblDoctors where a.Id == docId select a).Single();
                docToUpdate.Name = name;
                docToUpdate.Surname = surname;
                docToUpdate.Birthofdate = birthDate;
                docToUpdate.Branch = _db.TblBranches.Single(x => x.Branch == comboBranch.Text).Id;

                _ = await _db.SaveChangesAsync();
                Clear();
                Read();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Read();
            dg.ItemsSource = initialData
                .Where(x =>
                    x.Name.Contains(txtSearch.Text) ||
                    x.Surname.Contains(txtSearch.Text) ||
                    x.Branch.Contains(txtSearch.Text)
                ).ToList();
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            int? docId = (dg.SelectedItem as DoctorViewModel)?.Id;
            if (docId != null)
            {
                TblDoctor docToDel = _db.TblDoctors.Single(a => a.Id == docId);
                _db.TblDoctors.Remove(docToDel);
                _ = await _db.SaveChangesAsync();
                Clear();
                Read();
            }
        }

        private bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.All(char.IsLetter);
        }

        private bool IsValidSurname(string surname)
        {
            return !string.IsNullOrEmpty(surname) && surname.Split(' ').All(word => word.All(char.IsLetter));
        }

        private bool IsValidBirthDate(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
                return false;

            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Value.Year;
            if (birthDate.Value > today.AddYears(-age)) age--;

            return birthDate.Value <= today && age >= 18;
        }

        private string CleanUpWhitespace(string input)
        {
            return string.Join(" ", input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
