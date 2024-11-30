using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace roomms
{
    public partial class room : Form
    {
        new main Parent;
        string sqlstr;    //쿼리문 저장 변수
        DBClass dbc = new DBClass();

        public room()
        {
            InitializeComponent();
        }
        public void room_header()
        {
            DataGridView1.Columns[0].HeaderText = "방 번호";
            DataGridView1.Columns[1].HeaderText = "상태";
            DataGridView1.Columns[2].HeaderText = "수용 인원";
            DataGridView1.Columns[3].HeaderText = "청소 상태";
            DataGridView1.Columns[4].HeaderText = "기계 번호";

            DataGridView1.Columns[0].Width = 60;
            DataGridView1.Columns[1].Width = 80;
            DataGridView1.Columns[2].Width = 80;
            DataGridView1.Columns[3].Width = 80;
            DataGridView1.Columns[4].Width = 80;
        }

        private void room_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void room_Load(object sender, EventArgs e)
        {
            try
            {
                dbc.DB_ObjCreate(); //*****
                dbc.DB_Open();//*****
                dbc.DB_Access();//***

                sqlstr = "Select * From room ORDER BY room_id ASC";
                dbc.DCom.CommandText = sqlstr;
                dbc.DA.SelectCommand = dbc.DCom;
                dbc.DA.Fill(dbc.DS, "room");
                dbc.DS.Tables["room"].Clear();
                dbc.DA.Fill(dbc.DS, "room");
                DataGridView1.DataSource = dbc.DS.Tables["room"].DefaultView;
                room_header();
            }
            catch (DataException DE)
            {
                MessageBox.Show(DE.Message);
            }
            catch (Exception DE)
            {
                MessageBox.Show(DE.Message);
            }
        }



        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void Editbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // 수정할 행을 찾기 위한 PrimaryKey 설정
                DataColumn[] primaryKey = new DataColumn[1];
                primaryKey[0] = dbc.DS.Tables["room"].Columns["room_id"];
                dbc.DS.Tables["room"].PrimaryKey = primaryKey;

                // 선택된 행을 찾고 수정
                DataRow rowToUpdate = dbc.DS.Tables["room"].Rows.Find(rNumtxt.Text); // 방 번호 기준으로 찾기
                if (rowToUpdate != null)
                {
                    rowToUpdate["machine_id"] = mNumtxt.Text; // 기계 번호 텍스트박스
                    rowToUpdate["status"] = rStatustxt.Text; // 상태 텍스트박스
                    rowToUpdate["capacity"] = rCapacitytxt.Text; // 수용 인원 텍스트박스
                    rowToUpdate["cleaning_status"] = rCleaningtxt.Text; // 청소 상태 텍스트박스

                    // DB에 반영
                    dbc.DA.Update(dbc.DS, "room");
                    dbc.DS.AcceptChanges(); // 변경 사항 적용

                    MessageBox.Show("방 정보가 수정되었습니다.");
                }
                else
                {
                    MessageBox.Show("해당 방 번호를 찾을 수 없습니다.");
                }
            }
            catch (DataException DE)
            {
                MessageBox.Show(DE.Message);
            }
            catch (Exception DE)
            {
                MessageBox.Show(DE.Message);
            }
        }

        private void Delbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // 삭제할 행을 찾기 위한 PrimaryKey 설정
                DataColumn[] primaryKey = new DataColumn[1];
                primaryKey[0] = dbc.DS.Tables["room"].Columns["room_id"];
                dbc.DS.Tables["room"].PrimaryKey = primaryKey;

                // 선택된 행을 찾고 삭제
                DataRow rowToDelete = dbc.DS.Tables["room"].Rows.Find(rNumtxt.Text); // 방 번호 기준으로 찾기
                if (rowToDelete != null)
                {
                    rowToDelete.Delete();

                    // DB에 반영
                    dbc.DA.Update(dbc.DS, "room");
                    dbc.DS.AcceptChanges(); // 변경 사항 적용

                    MessageBox.Show("방 정보가 삭제되었습니다.");
                }
                else
                {
                    MessageBox.Show("해당 방 번호를 찾을 수 없습니다.");
                }
            }
            catch (DataException DE)
            {
                MessageBox.Show(DE.Message);
            }
            catch (Exception DE)
            {
                MessageBox.Show(DE.Message);
            }
        }

        private void rStatustxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Addbtn_Click(object sender, EventArgs e)
        {
            try
            {
                string roomNumber = rNumtxt.Text; // 방 번호 텍스트박스
                string status = rStatustxt.SelectedItem.ToString(); // 선택된 상태
                string machine_id = mNumtxt.Text; // 기계 번호 텍스트박스

                
                int capacity = 0;
                if (!int.TryParse(rCapacitytxt.Text, out capacity))
                {
                    MessageBox.Show("수용 인원은 숫자로 입력해주세요.");
                    return;
                }

                string cleaning_status = rCleaningtxt.SelectedItem.ToString(); // 청소 상태 텍스트박스

                if (string.IsNullOrEmpty(roomNumber) || string.IsNullOrEmpty(status))
                {
                    MessageBox.Show("모든 항목을 입력해주세요.");
                    return;
                }

                sqlstr = "SELECT COUNT(*) FROM room WHERE room_id = :room_id";
                OracleCommand cmdCheck = new OracleCommand(sqlstr, dbc.Con);
                cmdCheck.Parameters.Add("room_id", OracleDbType.Int32).Value = Convert.ToInt32(roomNumber);

                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("이미 존재하는 방 번호입니다.");
                    return;
                }

                sqlstr = "INSERT INTO room (room_id, status, capacity, cleaning_status, machine_id) VALUES (:room_id, :status, :capacity, :cleaning_status, :machine_id)";

                OracleCommand cmd = new OracleCommand(sqlstr, dbc.Con);

                cmd.Parameters.Add("room_id", OracleDbType.Int32).Value = Convert.ToInt32(roomNumber);
                cmd.Parameters.Add("status", OracleDbType.Varchar2).Value = status;  // 방 상태
                cmd.Parameters.Add("capacity", OracleDbType.Int32).Value = capacity;
                cmd.Parameters.Add("cleaning_status", OracleDbType.Varchar2).Value = cleaning_status;  // 청소 상태
                cmd.Parameters.Add("machine_id", OracleDbType.Int32).Value = Convert.ToInt32(machine_id); // 기계 번호

                cmd.ExecuteNonQuery();

                dbc.DS.Tables["room"].Clear();

                sqlstr = "Select * From room ORDER BY room_id ASC";
                dbc.DA.SelectCommand = dbc.DCom;
                dbc.DA.Fill(dbc.DS, "room");
                DataGridView1.DataSource = dbc.DS.Tables["room"].DefaultView;

                rNumtxt.Clear();
                rStatustxt.SelectedItem = null;
                mNumtxt.Clear();
                rCapacitytxt.Clear();
                rCleaningtxt.SelectedItem = null;

                MessageBox.Show("방이 성공적으로 추가되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("추가 오류: " + ex.Message);
            }
        }





        public void Backbtn_Click(object sender, EventArgs e)
        {
            this.Close();

            main form1 = new main();
            form1.Show(); // main 폼을 보이게 함
        }







    }
}
