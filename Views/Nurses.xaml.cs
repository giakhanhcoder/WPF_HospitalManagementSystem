using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_HospitalManagementSystem._data;
using WPF_HospitalManagementSystem.ViewModels;

namespace WPF_HospitalManagementSystem.Views
{
    /// <summary>
    /// Interaction logic for Nurses.xaml
    /// </summary>
    public partial class Nurses : Page
    {
        public Nurses() => InitializeComponent();

        private static DbHospitalManagementSystemContext _db = new();

        IQueryable<NurseViewModel> initialData = _db.TblNurses.Select(x => new NurseViewModel()
        {
            Id = x.Id,
            Name = x.Name,
            Surname = x.Surname,
            Policlinic = x.PoliclinicNavigation.Branch,
            Birthofdate = x.Birthofdate,
            Status = x.Status
        });

        private void Nurses_OnLoaded(object sender, RoutedEventArgs e) => Read();

        private void Clear()
        {
            txtName.Text = "";
            txtSurname.Text = "";
            txtId.Text = "";
            txtSearch.Text = "";
            comboClinic.Text = "";
            dateBirth.Text = "";
        }

        private void Read()
        {
            dg.ItemsSource = null;

            dg.ItemsSource = initialData.ToList();

            comboClinic.ItemsSource = (from x in _db.TblBranches
                                       select new
                                       {
                                           x.Id,
                                           x.Branch
                                       }).ToList();
        }

        private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int? nurseId = (dg.SelectedItem as NurseViewModel)?.Id;
            txtId.Text = nurseId.ToString();
            txtName.Text = (dg.SelectedItem as NurseViewModel)?.Name;
            txtSurname.Text = (dg.SelectedItem as NurseViewModel)?.Surname;
            comboClinic.Text = (dg.SelectedItem as NurseViewModel)?.Policlinic;
            dateBirth.SelectedDate = (dg.SelectedItem as NurseViewModel)?.Birthofdate;
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string name = CleanUpWhitespace(txtName.Text);
            string surname = CleanUpWhitespace(txtSurname.Text);
            DateTime? birthDate = dateBirth.SelectedDate;
            string clinic = comboClinic.Text;

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

            if (string.IsNullOrEmpty(clinic))
            {
                MessageBox.Show("You must choose your clinic.");
                return;
            }

            TblNurse newNurse = new TblNurse()
            {
                Name = name,
                Surname = surname,
                Policlinic = _db.TblBranches.SingleOrDefault(x => x.Branch == clinic)?.Id,
                Birthofdate = birthDate,
                Status = true
            };

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname))
            {
                _db.TblNurses.Add(newNurse);
                _ = await _db.SaveChangesAsync();
                Clear();
                Read();
            }
        }

        private void btnRead_Click(object sender, RoutedEventArgs e) => Read();

        private async void btnUptd_Click(object sender, RoutedEventArgs e)
        {
            int? nurseId = (dg.SelectedItem as NurseViewModel)?.Id;
            if (nurseId != null)
            {
                string name = CleanUpWhitespace(txtName.Text);
                string surname = CleanUpWhitespace(txtSurname.Text);
                DateTime? birthDate = dateBirth.SelectedDate;
                string clinic = comboClinic.Text;

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

                if (string.IsNullOrEmpty(clinic))
                {
                    MessageBox.Show("You must choose your clinic.");
                    return;
                }

                TblNurse nurseToUpdate = (from a in _db.TblNurses where a.Id == nurseId select a).Single();
                nurseToUpdate.Name = name;
                nurseToUpdate.Surname = surname;
                nurseToUpdate.Birthofdate = birthDate;
                nurseToUpdate.Policlinic = _db.TblBranches.Single(x => x.Branch == clinic).Id;

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
                    x.Policlinic.Contains(txtSearch.Text)
                ).ToList();
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            int? nurseId = (dg.SelectedItem as NurseViewModel)?.Id;
            if (nurseId != null)
            {
                TblNurse nurseToDel = _db.TblNurses.Single(a => a.Id == nurseId);
                _db.TblNurses.Remove(nurseToDel);
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
