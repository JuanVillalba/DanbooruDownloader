﻿using DanbooruDownloader3.DAO;
using DanbooruDownloader3.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DanbooruDownloader3
{
    public partial class FormAddBatchJob : Form
    {
        public List<DanbooruBatchJob> Jobs { get; set; }

        private List<CheckBox> chkList;
        private List<DanbooruProvider> providerList;

        public FormAddBatchJob(List<DanbooruProvider> ProviderList)
        {
            InitializeComponent();

            //Auto populate Rating
            cbxRating.DataSource = new BindingSource(Constants.Rating, null);
            cbxRating.DisplayMember = "Key";
            cbxRating.ValueMember = "Value";
            cbxRating.SelectedIndex = 0;

            chkList = new List<CheckBox>();
            this.providerList = ProviderList;
        }

        private void FillProvider()
        {
            if (providerList == null)
            {
                providerList = DanbooruProviderDao.GetInstance().Read();
            }

            foreach (DanbooruProvider p in providerList)
            {
                var controls = pnlProvider.Controls.Find(p.Name, true);
                if (controls.Length == 0)
                {
                    CheckBox chk = new CheckBox();
                    chk.Name = p.Name;
                    chk.Text = p.Name;
                    chk.AutoSize = true;
                    chkList.Add(chk);
                }
            }

            foreach (CheckBox c in chkList)
            {
                this.pnlProvider.Controls.Add(c);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool providerFlag = false;
            this.DialogResult = DialogResult.OK;
            Jobs = new List<DanbooruBatchJob>();

            var jobTags = txtTagQuery.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var tags in jobTags)
            {
                foreach (CheckBox c in chkList)
                {
                    if (c.Checked)
                    {
                        var p = providerList.Where(x => x.Name == c.Text).FirstOrDefault();
                        if (p != null)
                        {
                            providerFlag = true;
                            DanbooruBatchJob Job = new DanbooruBatchJob();
                            Job.Provider = p;

                            try
                            {
                                if (!string.IsNullOrWhiteSpace(txtLimit.Text)) Job.Limit = Convert.ToInt32(txtLimit.Text);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error at Limit." + Environment.NewLine + ex.Message);
                                txtLimit.Focus();
                                txtLimit.SelectAll();
                                return;
                            }

                            try
                            {
                                if (!string.IsNullOrWhiteSpace(txtPage.Text)) Job.StartPage = Convert.ToInt32(txtPage.Text);
                                else Job.StartPage = -1;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error at StartPage." + Environment.NewLine + ex.Message);
                                txtPage.Focus();
                                txtPage.SelectAll();
                                return;
                            }

                            if (cbxRating.SelectedValue != null && chkNotRating.Checked) Job.Rating = "-" + cbxRating.SelectedValue;
                            else Job.Rating = (string)cbxRating.SelectedValue;

                            // do encoding later on main form.
                            Job.TagQuery = tags;

                            if (string.IsNullOrWhiteSpace(txtFilenameFormat.Text))
                            {
                                MessageBox.Show("Filename Format is empty!");
                                txtFilenameFormat.Focus();
                                return;
                            }
                            Job.SaveFolder = txtFilenameFormat.Text;
                            Job.Filter = txtFilter.Text;
                            Job.IsExclude = chkIsExclude.Checked;

                            Jobs.Add(Job);
                        }
                    }
                }
            }
            if (!providerFlag)
            {
                MessageBox.Show("Please select at least 1 provider.");
                pnlProvider.Focus();
                this.DialogResult = DialogResult.None;
                this.Jobs = null;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Jobs = null;
            //this.Close();
            this.Hide();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (CheckBox chk in chkList)
            {
                chk.Checked = chk.Checked == true ? false : true;
            }
        }

        private void FormAddBatchJob_Load(object sender, EventArgs e)
        {
            FillProvider();
        }

        private void pnlProvider_ControlAdded(object sender, ControlEventArgs e)
        {
            if (txtFilenameFormat.Top < pnlProvider.Top + pnlProvider.Height)
            {
                this.Height = this.Height + pnlProvider.Top + pnlProvider.Height - txtFilenameFormat.Top;
            }
        }
    }
}