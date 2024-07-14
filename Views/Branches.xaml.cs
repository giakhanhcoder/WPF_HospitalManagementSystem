using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using WPF_HospitalManagementSystem._data;

namespace WPF_HospitalManagementSystem.Views
{
    /// <summary>
    /// Interaction logic for Branches.xaml
    /// </summary>
    public partial class Branches : Page
    {
        public Branches()
        {
            InitializeComponent();
        }
        private DbHospitalManagementSystemContext _db = new();

        private void Branches_Load(object sender, EventArgs e)
        {
            Read();
        }

        private void Read()
        {
            dg.ItemsSource = null;
            dg.ItemsSource = _db.TblBranches.Where(x => x.Status == true).ToList();
        }

        private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            txtId.Text = (dg.SelectedItem as TblBranch)?.Id.ToString();
            txtBranch.Text = (dg.SelectedItem as TblBranch)?.Branch;
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string branchName = txtBranch.Text.Trim();

            if (string.IsNullOrEmpty(branchName))
            {
                MessageBox.Show("Branch name cannot be empty.");
                return;
            }

            if (!IsValidBranchName(branchName))
            {
                MessageBox.Show("Branch name is not valid. It must contain letters and can also contain numbers, but cannot be only numbers or special characters.");
                return;
            }

            // Kiểm tra xem Branch đã tồn tại với Status = 1 chưa
            bool branchExists = _db.TblBranches.Any(x => x.Branch == branchName && x.Status == true);
            if (branchExists)
            {
                MessageBox.Show("Branch is already existed.");
            }
            else
            {
                TblBranch newBranch = new TblBranch()
                {
                    Branch = branchName,
                    Status = true
                };

                _db.TblBranches.Add(newBranch);
                _ = await _db.SaveChangesAsync();
                Read();
            }
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            Read();
        }

        private async void btnUptd_Click(object sender, RoutedEventArgs e)
        {
            int? branchId = (dg.SelectedItem as TblBranch)?.Id;
            string branchName = txtBranch.Text.Trim();

            if (branchId != null)
            {
                if (!IsValidBranchName(branchName))
                {
                    MessageBox.Show("Branch name is not valid. It must contain letters and can also contain numbers, but cannot be only numbers or special characters.");
                    return;
                }

                // Kiểm tra xem Branch đã tồn tại với Status = 1 chưa
                bool branchExists = _db.TblBranches.Any(x => x.Branch == branchName && x.Status == true && x.Id != branchId);
                if (branchExists)
                {
                    MessageBox.Show("Branch is already existed.");
                }
                else
                {
                    TblBranch branchToUpdate = (from a in _db.TblBranches where a.Id == branchId select a).Single();
                    branchToUpdate.Branch = branchName;

                    _ = await _db.SaveChangesAsync();
                    Read();
                }
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            int? branchId = (dg.SelectedItem as TblBranch)?.Id;
            if (branchId != null)
            {
                TblBranch branchToDelete = _db.TblBranches.Single(a => a.Id == branchId);
                branchToDelete.Status = false; // Chỉ cập nhật Status thành 0

                _ = await _db.SaveChangesAsync();
                Read();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            dg.ItemsSource = _db.TblBranches
                .Where(x => x.Status == true && x.Branch.ToLower().Contains(searchText))
                .ToList();
        }

        private bool IsValidBranchName(string branchName)
        {
            // Kiểm tra nếu tên chi nhánh chứa ít nhất một chữ cái
            return Regex.IsMatch(branchName, @"^(?=.*[a-zA-Z])[a-zA-Z0-9\s]*$");
        }
    }
}
