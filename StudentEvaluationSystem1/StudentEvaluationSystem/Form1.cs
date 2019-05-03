using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace StudentEvaluationSystem
{

    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=ProjectB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
        SqlCommand cmd;
        SqlDataAdapter adapt;
        //ID variable used in Updating and Deleting Record  
        int ID = 0;
        DataClass o = new DataClass();
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }
        // Add students 

        private void btnRubric_Click(object sender, EventArgs e)
        {
            string df= "cloid,details";
            string val="'" + cmbClo.SelectedValue + "','" + txtRubric.Text.Replace("'", "''") + "'";
            string Result = o.Insert("Rubric", df, val);
            o.popGrid(gvRub, "select id,details from Rubric where cloid='" + cmbClo.SelectedValue + "'"); txtRubric.Text = "";
            MessageBox.Show(Result);

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            o.popGrid(gvClo, "select id,name from clo");
           o.popCmb(cmbClo, "select * from clo", "id", "name");
            o.popCmb(cmbRubrics, "select * from Rubric where CloId='" + cmbClo.SelectedValue + "'", "id", "Details");
            o.popCmb(cmbStatus, "select * from Lookup where category='STUDENT_STATUS'", "Lookupid", "Name");

            o.popGrid(gvRub, "select id,details from Rubric where cloid='" + cmbClo.SelectedValue + "'");
            o.popGrid(gvRubSettings, "select id,details,MeasurementLevel from RubricLevel where RubricId='" + cmbRubrics.SelectedValue + "'");

            string qry = "SELECT        Student.Id, Student.FirstName, Student.LastName, Student.Contact, Student.Email, Student.RegistrationNumber, Lookup.Name, Lookup.LookupId " +
" FROM            Student INNER JOIN " +
                        " Lookup ON Student.Status = Lookup.LookupId ";
            o.popGrid(gvStudents, qry); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";
            gvAttandance.DataSource = GetAttendanceRecord();
        }

        private DataTable GetAttendanceRecord()
        {
            DataTable Table = new DataTable();
            Table.Columns.Add("AttendanceId", typeof(string));
            Table.Columns.Add("RegNo", typeof(string));
            Table.Columns.Add("FirstName", typeof(string));
            Table.Columns.Add("LastName", typeof(string));
            Table.Columns.Add("Status", typeof(string));
            string sql;

            sql = "SELECT        TOP (100) PERCENT StudentAttendance.AttendanceId, Student.RegistrationNumber AS RegNo, Student.FirstName, Student.LastName, Lookup.Name AS Status " +
" FROM Student INNER JOIN " +
  "                        StudentAttendance ON Student.Id = StudentAttendance.StudentId INNER JOIN " +
    "                      ClassAttendance ON StudentAttendance.AttendanceId = ClassAttendance.Id INNER JOIN " +
      "                    Lookup ON StudentAttendance.AttendanceStatus = Lookup.LookupId " +
" ORDER BY ClassAttendance.AttendanceDate, Student.FirstName";
            DataSet dsx = new DataSet();
            dsx = o.GetRecords("tbl", sql);

            foreach (DataRow dr in dsx.Tables[0].Rows)
            {
                Table.Rows.Add(dr["AttendanceId"].ToString(), dr["RegNo"].ToString(), dr["FirstName"].ToString(), dr["LastName"].ToString(), dr["Status"].ToString());
            }
            return Table;
        }

        private void popClo(DataGridView gv, string qry)
        {

        }


        private void btnAddCLO_Click(object sender, EventArgs e)
        {
            string df= "Name, datecreated,dateupdated";
            string Val = "'" + txtCLO.Text.Replace("'", "''") + "','" + DateTime.Now.ToShortDateString() + "','" + DateTime.Now.ToShortDateString() + "'";
            string Result = o.Insert("clo", df,Val);
            o.popGrid(gvClo, "select id,name from clo");
           o.popCmb(cmbClo, "select * from clo", "id", "name");

        }



        private void gvClo_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.KeyCode == Keys.Delete)
            {
                if (Ask() == false) return;
                int selectedrowindex = gvClo.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = gvClo.Rows[selectedrowindex];
                string _id = Convert.ToString(selectedRow.Cells["id"].Value);
                DataClass o= new DataClass();
                string s = o.Delete("delete from clo where id='" + _id + "'");
                o.popGrid(gvClo, "select id,name from clo");
               o.popCmb(cmbClo, "select * from clo", "id", "name");
                lblrubricLevels.Text = "Rubrics Level - Settings Clo:" + cmbClo.Text;
            }
        }

        public bool Ask()
        {
            bool r= false;
            DialogResult dialogResult = MessageBox.Show("Sure", "You you want to delete", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                r = true;
            }
            else if (dialogResult == DialogResult.No)
            {
                r = false;
            }
            return r;
        }

        private void btnUpdateClo_Click(object sender, EventArgs e)
        {
            for (int rows = 0; rows < gvClo.Rows.Count - 1; rows++)
            {
                string id = gvClo.Rows[rows].Cells[0].Value.ToString().Trim().Replace("'", "''");
                string name = gvClo.Rows[rows].Cells[1].Value.ToString().Trim().Replace("'", "''");
                string dateupdated = DateTime.Now.ToString();
                string s = o.Update("update clo set name='" + name + "',dateupdated='" + dateupdated + "' where id='" + id + "'");

            }
           o.popGrid(gvClo, "select id,name from clo");
            o.popCmb(cmbClo, "select * from clo", "id", "name");
            lblrubricLevels.Text = "Rubrics Level - Settings Clo:" + cmbClo.Text;
        }
        private void cmbClo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
               o.popGrid(gvRub, "select id,details from Rubric where cloid='" + cmbClo.SelectedValue + "'");
               o.popCmb(cmbRubrics, "select * from Rubric where CloId='" + cmbClo.SelectedValue + "'", "id", "Details");
                lblrubricLevels.Text = "Rubrics Level - Settings Clo:" + cmbClo.Text;
                o.popGrid(gvRubSettings, "select id,details,MeasurementLevel from RubricLevel where RubricId='" + cmbRubrics.SelectedValue + "'");
            }
            catch (Exception ex) { }
        }
        private void gvRub_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (Ask() == false) return;
                int index = gvRub.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = gvRub.Rows[index];
                string _id = Convert.ToString(selectedRow.Cells["id"].Value);

                string s = o.Delete("delete from rubric where id='" + _id + "'");
                o.popGrid(gvRub, "select id,details from Rubric where cloid='" + cmbClo.SelectedValue + "'");
                o.popGrid(gvRubSettings, "select id,details,MeasurementLevel from RubricLevel where RubricId='" + cmbRubrics.SelectedValue + "'"); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";

            }
        }
        private void btnUpdateRubric_Click(object sender, EventArgs e)
        {
            for (int rows = 0; rows < gvRub.Rows.Count - 1; rows++)
            {
                string _id = gvRub.Rows[rows].Cells[0].Value.ToString().Trim().Replace("'", "''");
                string _details = gvRub.Rows[rows].Cells[1].Value.ToString().Trim().Replace("'", "''");
                string s =o.Update("update Rubric set details='" + _details + "' where id='" + _id + "'");

            }

        }
        private void btnAddRubricLevel_Click(object sender, EventArgs e)
        {
            string df= "RubricId,details,MeasurementLevel";
            string val="'" + cmbRubrics.SelectedValue + "','" + txtRubricSetDetails.Text.Replace("'", "''") + "','" + txtRubricSetLevel.Text.Replace("'", "''") + "'";
            string Result = o.Insert("RubricLevel", df, val);
           o.popGrid(gvRubSettings, "select id,details,MeasurementLevel from RubricLevel where RubricId='" + cmbRubrics.SelectedValue + "'"); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";
            MessageBox.Show(Result);
        }

        private void cmbRubrics_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
               o.popGrid(gvRubSettings, "select id,details,MeasurementLevel from RubricLevel where RubricId='" + cmbRubrics.SelectedValue + "'"); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";
            }
            catch (Exception ex) { }
        }
        private void gvRubSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (Ask() == false) return;
                int index = gvRubSettings.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = gvRubSettings.Rows[index];
                string id = Convert.ToString(selectedRow.Cells["id"].Value);

                string s = o.Delete("delete from RubricLevel where id='" + id + "'");
             o.popGrid(gvRubSettings, "select id,details,MeasurementLevel from RubricLevel where RubricId='" + cmbRubrics.SelectedValue + "'"); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";
            }

        }

        private void btnUpdateRubricLevel_Click(object sender, EventArgs e)
        {
            for (int rows = 0; rows < gvRubSettings.Rows.Count - 1; rows++)
            {
                string id = gvRubSettings.Rows[rows].Cells[0].Value.ToString().Trim().Replace("'", "''");
                string _details = gvRubSettings.Rows[rows].Cells[1].Value.ToString().Trim().Replace("'", "''");
                string _mlevel = gvRubSettings.Rows[rows].Cells[2].Value.ToString().Trim().Replace("'", "''");
                string s = o.Update("update RubricLevel set details='" + _details + "',MeasurementLevel='" + _mlevel + "' where id='" + id + "'");

            }
            o.popGrid(gvRubSettings, "select id,details,MeasurementLevel from RubricLevel where RubricId='" + cmbRubrics.SelectedValue + "'"); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";

        }

        private void gvStudents_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (gvStudents.SelectedCells.Count > 0)
                {
                    int selectedrowindex = gvStudents.SelectedCells[0].RowIndex;
                    DataGridViewRow selectedRow = gvStudents.Rows[selectedrowindex];
                    string _id = Convert.ToString(selectedRow.Cells["id"].Value);
                    lblidStud.Text = _id;
                    string _qry = "";
                    DataSet ds = new DataSet();
                    string qry = "SELECT        Student.Id, Student.FirstName, Student.LastName, Student.Contact, Student.Email, Student.RegistrationNumber, Lookup.Name, Lookup.LookupId " +
                   " FROM  Student INNER JOIN " + " Lookup ON Student.Status = Lookup.LookupId where id='" + _id + "'";

                    ds = o.GetRecords("tbl", qry);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        txtfname.Text = dr["FirstName"].ToString().Trim();
                        txtlname.Text = dr["LastName"].ToString().Trim();
                        txtcon.Text = dr["Contact"].ToString().Trim();
                        txtemail.Text = dr["Email"].ToString().Trim();
                        txtRegNo.Text = dr["RegistrationNumber"].ToString().Trim();
                        cmbStatus.SelectedValue = dr["LookupId"].ToString().Trim();

                    }


                }


            }

            if (e.KeyCode == Keys.Delete)
            {
                if (Ask() == false) return;
                int index = gvStudents.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = gvStudents.Rows[index];
                string _id = Convert.ToString(selectedRow.Cells["id"].Value);

                string s = o.Delete("delete from student where id='" + _id + "'");
                if (s != "Delete Record(s) Successfully") s = "Unable to Delete. Record might be required in other tables";
                MessageBox.Show(s);

                string qry = "SELECT Student.Id, Student.FirstName, Student.LastName, Student.Contact, Student.Email, Student.RegistrationNumber, Lookup.Name, Lookup.LookupId " +
              " FROM Student INNER JOIN " + " Lookup ON Student.Status = Lookup.LookupId ";
               o.popGrid(gvStudents, qry); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";

            }
        }

        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            string df= "FirstName,LastName,Contact,Email,RegistrationNumber,Status";
            string val= "'" + txtfname.Text.Trim().Replace("'", "''") + "','" + txtlname.Text.Trim().Replace("'", "''") + "','" + txtcon.Text.Trim().Replace("'", "''") + "','" + txtemail.Text.Trim().Replace("'", "''") + "','" + txtRegNo.Text.Trim().Replace("'", "''") + "','" + cmbStatus.SelectedValue + "'";
            string Result = o.Insert("Student", df, val);
            string qry = "SELECT Student.Id, Student.FirstName, Student.LastName, Student.Contact, Student.Email, Student.RegistrationNumber, Lookup.Name, Lookup.LookupId " +
            " FROM  Student INNER JOIN " + " Lookup ON Student.Status = Lookup.LookupId ";
            o.popGrid(gvStudents, qry); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";
            MessageBox.Show(Result);
        }

        private void btnUpdateStudent_Click(object sender, EventArgs e)
        {
            if (lblidStud.Text == "0") return;
            string QryUpdate = " FirstName='" + txtfname.Text.Trim().Replace("'", "''") + "', LastName='" + txtlname.Text.Trim().Replace("'", "''") + "', Contact='" + txtcon.Text.Trim().Replace("'", "''") + "', Email='" + txtemail.Text.Trim().Replace("'", "''") + "', RegistrationNumber='" + txtRegNo.Text.Trim().Replace("'", "''") + "', status='" + cmbStatus.SelectedValue + "'";
            string Result = o.Update("update Student set " + QryUpdate + " where id='" + lblidStud.Text + "'");
            string qry = "SELECT        Student.Id, Student.FirstName, Student.LastName, Student.Contact, Student.Email, Student.RegistrationNumber, Lookup.Name, Lookup.LookupId " +
            " FROM Student INNER JOIN " + " Lookup ON Student.Status = Lookup.LookupId ";
            o.popGrid(gvStudents, qry); txtRubricSetDetails.Text = ""; txtRubricSetLevel.Text = "";
            MessageBox.Show(Result);
        }

        private void btnAddAttendance_Click(object sender, EventArgs e)
        {

            int t = o.TRec("select * from ClassAttendance");
            string _DataFields = "AttendanceDate";
            string _Values = "'" + dtpAttendance.Value + "'";
            string Result = o.Insert("ClassAttendance", _DataFields, _Values);
            System.Threading.Thread.Sleep(1000);
            DataSet ds = new DataSet();
            ds = o.GetRecords("tbl", "SELECT TOP (1) Id, AttendanceDate FROM  ClassAttendance order by id desc");
            string _lastid = "";
            foreach (DataRow dr in ds.Tables[0].Rows) { _lastid = dr["id"].ToString().Trim(); }
            DataSet dsStudents = new DataSet();
            dsStudents =o.GetRecords("tbl", "select * from Student where status='5'");
            foreach (DataRow dr in dsStudents.Tables[0].Rows)
            {
                string _atid = _lastid;
                string stid = dr["id"].ToString();
                _DataFields = "AttendanceId, studentid,AttendanceStatus";
                _Values = "'" + _lastid.Replace("'", "''") + "','" + stid + "','1'";
                Result = o.Insert("StudentAttendance", _DataFields, _Values);
                System.Threading.Thread.Sleep(500);
            }
            o.popList(lstAttendace, "select * from ClassAttendance order by AttendanceDate desc", "AttendanceDate");
        }

        private void fillcombo()
        {
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            combo.HeaderText = "status";
            combo.Name = "combo";
            ArrayList row = new ArrayList();
            foreach (DataRow dr in dt.Rows)
            {
                row.Add(dr["Name"].ToString());
            }
            combo.Items.AddRange(row.ToArray());
            gvAttandance.Columns.Add(combo);

        }

        private void gvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void gvAttandance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > -1)
            {
                DataGridViewComboBoxCell my_objGridDropbox = new DataGridViewComboBoxCell();
                if (gvAttandance.Columns[e.ColumnIndex].Name.Contains("Status"))
                {
                    gvAttandance[e.ColumnIndex, e.RowIndex] = my_objGridDropbox;
                    o.popCmbGrid(my_objGridDropbox, "select name as Status from lookup where lookupid <> '5' and Lookupid <> '6' ", "status", "status");
                }

            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string df= "Name, datecreated,dateupdated";
            string val="'" + txtCLO.Text.Replace("'", "''") + "','" + DateTime.Now.ToShortDateString() + "','" + DateTime.Now.ToShortDateString() + "'";
            string Result = o.Insert("clo", df, val);
            o.popGrid(gvClo, "select id,name from clo");
            o.popCmb(cmbClo, "select * from clo", "id", "name");

        }


        private void btnAdd_Click(object sender, EventArgs e)
        {


            int t = o.TRec("select * from ClassAttendance");
            string _DataFields = "AttendanceDate";
            string _Values = "'" + dtpAttendance.Value + "'";
            string Result = o.Insert("ClassAttendance", _DataFields, _Values);
            System.Threading.Thread.Sleep(1000);
            DataSet ds = new DataSet();
            ds = o.GetRecords("tbl", "SELECT TOP (1) Id, AttendanceDate FROM  ClassAttendance order by id desc");
            string _lastid = "";
            foreach (DataRow dr in ds.Tables[0].Rows) { _lastid = dr["id"].ToString().Trim(); }
            DataSet dsStudents = new DataSet();
            dsStudents = o.GetRecords("tbl", "select * from Student where status='5'");
            foreach (DataRow dr in dsStudents.Tables[0].Rows)
            {
                string _atid = _lastid;
                string stid = dr["id"].ToString();
                _DataFields = "AttendanceId, studentid,AttendanceStatus";
                _Values = "'" + _lastid.Replace("'", "''") + "','" + stid + "','1'";
                Result = o.Insert("StudentAttendance", _DataFields, _Values);
                System.Threading.Thread.Sleep(500);
            }
            o.popList(lstAttendace, "select * from ClassAttendance order by AttendanceDate desc", "AttendanceDate");

        }
    }

  

}


