using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using NXPMS.Base.Enums;
using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Base.Repositories.EmployeeRecordRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Data.Repositories.EmployeeRecordRepositories
{
    public class EmployeesRepository : IEmployeesRepository
    {
        public IConfiguration _config { get; }
        public EmployeesRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Employee Read Action Methods
        public async Task<IList<Employee>> FindByNameAsync(string fullname)
        {
            if (String.IsNullOrEmpty(fullname)) { throw new ArgumentNullException("The required parameter [Full Name] is null or has an invalid value."); }

            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE (LOWER(e.fname) LIKE '%'||LOWER(@f_nm)||'%') ");
            sb.Append("OR (LOWER(e.sname) LIKE '%'||LOWER(@f_nm)||'%') ");
            sb.Append("OR (LOWER(e.oname) LIKE '%'||LOWER(@f_nm)||'%') ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var f_nm = cmd.Parameters.Add("@f_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                f_nm.Value = fullname;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<IList<Employee>> FindNonUsersByNameAsync(string fullname)
        {
            if (String.IsNullOrEmpty(fullname)) { throw new ArgumentNullException("The required parameter [Full Name] is null or has an invalid value."); }

            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syssctusr u ON u.emp_id = e.empid ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE (e.is_dx = false) ");
            sb.Append("AND (u.usr_id IS NULL) ");
            sb.Append("AND ((LOWER(e.fname) LIKE '%'||LOWER(@f_nm)||'%') ");
            sb.Append("OR (LOWER(e.sname) LIKE '%'||LOWER(@f_nm)||'%') ");
            sb.Append("OR (LOWER(e.oname) LIKE '%'||LOWER(@f_nm)||'%')); ");

            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var f_nm = cmd.Parameters.Add("@f_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                f_nm.Value = fullname;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<IList<Employee>> GetByNameAsync(string fullname)
        {
            if (String.IsNullOrEmpty(fullname)) { throw new ArgumentNullException("The required parameter [Full Name] is null or has an invalid value."); }

            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE LOWER(e.fullname) = LOWER(@f_nm) ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var f_nm = cmd.Parameters.Add("@f_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                f_nm.Value = fullname;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<IList<Employee>> GetByIdAsync(int employeeId)
        {
            if (employeeId < 1) { throw new ArgumentNullException("The required parameter [Employee ID] is null or has an invalid value."); }

            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE e.empid = @empid ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var empid = cmd.Parameters.Add("@empid", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                empid.Value = employeeId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<EmployeeCardinal> GetEmployeeCardinalsByIdAsync(int employeeId)
        {
            if (employeeId < 1) { throw new ArgumentNullException("The required parameter [Employee ID] is null or has an invalid value."); }

            EmployeeCardinal employeeCardinal = new EmployeeCardinal();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.fullname, e.loc_id, e.dept_cd, e.unit_cd ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("WHERE e.empid = @empid ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var empid = cmd.Parameters.Add("@empid", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                empid.Value = employeeId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeCardinal.EmployeeId = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]);
                    employeeCardinal.EmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString();
                    employeeCardinal.EmployeeLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                    employeeCardinal.EmployeeDepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString();
                    employeeCardinal.EmployeeUnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString();
                }
            }
            await conn.CloseAsync();
            return employeeCardinal;
        }

        public async Task<IList<Employee>> GetAllAsync()
        {
            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE LOWER(e.fullname) = LOWER(@f_nm) ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<IList<Employee>> GetByLocationIdAsync(int locationId = 0)
        {
            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE (e.loc_id = @loc_id) ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                loc_id.Value = locationId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<IList<Employee>> GetByLocationIdAndDepartmentCodeAsync(int locationId, string departmentCode)
        {
            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE (e.loc_id = @loc_id) AND (e.dept_cd = @dept_cd) ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                loc_id.Value = locationId;
                dept_cd.Value = departmentCode;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<IList<Employee>> GetByLocationIdAndUnitCodeAsync(int locationId, string unitCode)
        {
            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE (e.loc_id = @loc_id) AND (e.unit_cd = @unit_cd) ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                loc_id.Value = locationId;
                unit_cd.Value = unitCode;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<IList<Employee>> GetByDepartmentCodeAsync(string departmentCode)
        {
            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE (e.dept_cd = @dept_cd) ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                dept_cd.Value = departmentCode;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }

        public async Task<IList<Employee>> GetByUnitCodeAsync(string unitCode)
        {
            List<Employee> employeesList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.empid, e.title, e.sname, e.fname, e.oname, e.fullname, ");
            sb.Append("e.sex, e.pn1, e.pn2, e.peml, e.oeml, e.raddr, e.imgp, e.bday, ");
            sb.Append("e.bmonth, e.byear, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.start_up_designation, e.place_of_engagement, e.confirmation_date, ");
            sb.Append("e.current_designation, e.sttor, e.lgaor, e.religion, ");
            sb.Append("e.gp_rgn, e.nok_nm, e.nok_rls, e.nok_addr, e.nok_pn, e.nok_eml, ");
            sb.Append("e.loc_id, e.emp_stts, e.mstatus, e.mdb, e.mdt, e.ctb, ");
            sb.Append("e.ctt, e.is_dx, e.dx_by, e.dx_dt, e.dept_cd, e.unit_cd, e.paddr, ");
            sb.Append("e.jbp_id, e.emp_ctg_id, e.emp_typ_id, l.locname, d.dept_nm, n.unit_nm, ");
            sb.Append("j.jb_prf_nm, t.emp_typ_nm, c.emp_ctg_nm ");
            sb.Append("FROM public.ermempinf e ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN public.syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = e.emp_ctg_id ");
            sb.Append("LEFT JOIN public.ermsttemptyp t ON t.emp_typ_id = e.emp_typ_id ");
            sb.Append("LEFT JOIN public.ermsttjbprf j ON j.jb_prf_id = e.jbp_id ");
            sb.Append("WHERE (e.unit_cd = @unit_cd) ");
            sb.Append("AND (e.is_dx = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                unit_cd.Value = unitCode;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeID = reader["empid"] == DBNull.Value ? 0 : (int)(reader["empid"]),
                        Title = reader["title"] == DBNull.Value ? string.Empty : (reader["title"]).ToString(),
                        Surname = reader["sname"] == DBNull.Value ? string.Empty : (reader["sname"]).ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo = reader["pn1"] == DBNull.Value ? string.Empty : reader["pn1"].ToString(),
                        AltPhoneNo = reader["pn2"] == DBNull.Value ? string.Empty : reader["pn2"].ToString(),
                        PersonalEmail = reader["peml"] == DBNull.Value ? string.Empty : reader["peml"].ToString(),
                        OfficialEmail = reader["oeml"] == DBNull.Value ? string.Empty : reader["oeml"].ToString(),
                        ResidenceAddress = reader["raddr"] == DBNull.Value ? string.Empty : reader["raddr"].ToString(),
                        ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                        BirthDay = reader["bday"] == DBNull.Value ? 0 : (int)reader["bday"],
                        BirthMonth = reader["bmonth"] == DBNull.Value ? 0 : (int)reader["bmonth"],
                        BirthYear = reader["byear"] == DBNull.Value ? 0 : (int)reader["byear"],
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CustomNo = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                        StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                        StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? string.Empty : reader["start_up_designation"].ToString(),
                        PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? string.Empty : reader["place_of_engagement"].ToString(),
                        ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        StateOfOrigin = reader["sttor"] == DBNull.Value ? string.Empty : reader["sttor"].ToString(),
                        LocalGovernmentOfOrigin = reader["lgaor"] == DBNull.Value ? string.Empty : reader["lgaor"].ToString(),
                        Religion = reader["religion"] == DBNull.Value ? string.Empty : reader["religion"].ToString(),
                        GeoPoliticalRegion = reader["gp_rgn"] == DBNull.Value ? string.Empty : reader["gp_rgn"].ToString(),
                        NextOfKinName = reader["nok_nm"] == DBNull.Value ? string.Empty : reader["nok_nm"].ToString(),
                        NextOfKinRelationship = reader["nok_rls"] == DBNull.Value ? string.Empty : reader["nok_rls"].ToString(),
                        NextOfKinAddress = reader["nok_addr"] == DBNull.Value ? string.Empty : reader["nok_addr"].ToString(),
                        NextOfKinPhoneNo = reader["nok_pn"] == DBNull.Value ? string.Empty : reader["nok_pn"].ToString(),
                        NextOfKinEmail = reader["nok_eml"] == DBNull.Value ? string.Empty : reader["nok_eml"].ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        EmployeeStatus = reader["emp_stts"] == DBNull.Value ? EmploymentStatus.Exited : (EmploymentStatus)reader["emp_stts"],
                        MaritalStatus = reader["mstatus"] == DBNull.Value ? string.Empty : reader["mstatus"].ToString(),
                        LastModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        LastModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        PermanentHomeAddress = reader["paddr"] == DBNull.Value ? string.Empty : reader["paddr"].ToString(),
                        JobProfileID = reader["jbp_id"] == DBNull.Value ? 0 : (int)reader["jbp_id"],
                        JobProfileName = reader["jb_prf_nm"] == DBNull.Value ? string.Empty : reader["jb_prf_nm"].ToString(),
                        EmployeeCategoryID = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)reader["emp_ctg_id"],
                        EmployeeCategoryDescription = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : reader["emp_ctg_nm"].ToString(),
                        EmployeeTypeID = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)reader["emp_typ_id"],
                        EmployeeTypeDescription = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : reader["emp_typ_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeesList;
        }
        #endregion

        #region Employee Write Action Methods
        public async Task<bool> AddEmployeePersonalInfoOnlyAsync(Employee employee)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ermempinf(title, sname, fname, oname, ");
            sb.Append("fullname, sex, pn1, pn2, peml, oeml, raddr, imgp, bday, ");
            sb.Append("bmonth, byear, emp_no_1, emp_no_2, start_up_date, start_up_designation, ");
            sb.Append("place_of_engagement, confirmation_date, current_designation, sttor, ");
            sb.Append("lgaor, religion, gp_rgn, nok_nm, nok_rls, nok_addr, nok_pn, nok_eml, ");
            sb.Append("loc_id, emp_stts, mstatus, mdb, mdt, ctb, ctt,  ");
            sb.Append("dept_cd, unit_cd, paddr, jbp_id, emp_ctg_id, emp_typ_id) ");
            sb.Append("VALUES (@title, @sname, @fname, @oname, @fullname, @sex, @pn1, @pn2, ");
            sb.Append("@peml, @oeml, @raddr, @imgp, @bday, @bmonth, @byear, @emp_no_1, ");
            sb.Append("@emp_no_2, @start_up_date, @start_up_designation, @place_of_engagement, ");
            sb.Append("@confirmation_date, @current_designation, @sttor, @lgaor, @religion, ");
            sb.Append("@gp_rgn, @nok_nm, @nok_rls, @nok_addr, @nok_pn, @nok_eml, @loc_id, ");
            sb.Append("@emp_stts, @mstatus, @mdb, @mdt, @ctb, @ctt, ");
            sb.Append("@dept_cd, @unit_cd, @paddr, @jbp_id, @emp_ctg_id, @emp_typ_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    var sname = cmd.Parameters.Add("@sname", NpgsqlDbType.Text);
                    var fname = cmd.Parameters.Add("@fname", NpgsqlDbType.Text);
                    var oname = cmd.Parameters.Add("@oname", NpgsqlDbType.Text);
                    var fullname = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    var sex = cmd.Parameters.Add("@sex", NpgsqlDbType.Text);
                    var pn1 = cmd.Parameters.Add("@pn1", NpgsqlDbType.Text);
                    var pn2 = cmd.Parameters.Add("@pn2", NpgsqlDbType.Text);
                    var peml = cmd.Parameters.Add("@peml", NpgsqlDbType.Text);
                    var oeml = cmd.Parameters.Add("@oeml", NpgsqlDbType.Text);
                    var raddr = cmd.Parameters.Add("@raddr", NpgsqlDbType.Text);
                    var imgp = cmd.Parameters.Add("@imgp", NpgsqlDbType.Text);
                    var bday = cmd.Parameters.Add("@bday", NpgsqlDbType.Integer);
                    var bmonth = cmd.Parameters.Add("@bmonth", NpgsqlDbType.Integer);
                    var byear = cmd.Parameters.Add("@byear", NpgsqlDbType.Integer);
                    var emp_no_1 = cmd.Parameters.Add("@emp_no_1", NpgsqlDbType.Text);
                    var emp_no_2 = cmd.Parameters.Add("@emp_no_2", NpgsqlDbType.Text);
                    var start_up_date = cmd.Parameters.Add("@start_up_date", NpgsqlDbType.Date);
                    var start_up_designation = cmd.Parameters.Add("@start_up_designation", NpgsqlDbType.Text);
                    var place_of_engagement = cmd.Parameters.Add("@place_of_engagement", NpgsqlDbType.Text);
                    var confirmation_date = cmd.Parameters.Add("@confirmation_date", NpgsqlDbType.Date);
                    var current_designation = cmd.Parameters.Add("@current_designation", NpgsqlDbType.Text);
                    var sttor = cmd.Parameters.Add("@sttor", NpgsqlDbType.Text);
                    var lgaor = cmd.Parameters.Add("@lgaor", NpgsqlDbType.Text);
                    var religion = cmd.Parameters.Add("@religion", NpgsqlDbType.Text);
                    var gp_rgn = cmd.Parameters.Add("@gp_rgn", NpgsqlDbType.Text);
                    var nok_nm = cmd.Parameters.Add("@nok_nm", NpgsqlDbType.Text);
                    var nok_rls = cmd.Parameters.Add("@nok_rls", NpgsqlDbType.Text);
                    var nok_addr = cmd.Parameters.Add("@nok_addr", NpgsqlDbType.Text);
                    var nok_pn = cmd.Parameters.Add("@nok_pn", NpgsqlDbType.Text);
                    var nok_eml = cmd.Parameters.Add("@nok_eml", NpgsqlDbType.Text);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var emp_stts = cmd.Parameters.Add("@emp_stts", NpgsqlDbType.Integer);
                    var mstatus = cmd.Parameters.Add("@mstatus", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.TimestampTz);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.TimestampTz);
                    var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                    var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                    var paddr = cmd.Parameters.Add("@paddr", NpgsqlDbType.Text);
                    var jbp_id = cmd.Parameters.Add("@jbp_id", NpgsqlDbType.Integer);
                    var emp_ctg_id = cmd.Parameters.Add("@emp_ctg_id", NpgsqlDbType.Integer);
                    var emp_typ_id = cmd.Parameters.Add("@emp_typ_id", NpgsqlDbType.Integer);

                    cmd.Prepare();

                    title.Value = employee.Title ?? (object)DBNull.Value;
                    fname.Value = employee.FirstName ?? (object)DBNull.Value;
                    oname.Value = employee.OtherNames ?? (object)DBNull.Value;
                    sname.Value = employee.Surname ?? (object)DBNull.Value;
                    fullname.Value = employee.FullName ?? (object)DBNull.Value;
                    sex.Value = employee.Sex ?? (object)DBNull.Value;
                    pn1.Value = employee.PhoneNo ?? (object)DBNull.Value;
                    pn2.Value = employee.AltPhoneNo ?? (object)DBNull.Value;
                    peml.Value = employee.PersonalEmail ?? (object)DBNull.Value; 
                    oeml.Value = employee.OfficialEmail ?? (object)DBNull.Value;
                    raddr.Value = employee.ResidenceAddress ?? (object)DBNull.Value;
                    imgp.Value = employee.ImagePath ?? (object)DBNull.Value;
                    bday.Value = employee.BirthDay;
                    bmonth.Value = employee.BirthMonth;
                    byear.Value = employee.BirthYear;
                    emp_no_1.Value = employee.EmployeeNo ?? (object)DBNull.Value;
                    emp_no_2.Value = employee.CustomNo ?? (object)DBNull.Value;
                    start_up_date.Value = employee.StartUpDate ?? (object)DBNull.Value;
                    start_up_designation.Value = employee.StartUpDesignation ?? (object)DBNull.Value;
                    place_of_engagement.Value = employee.PlaceOfEngagement ?? (object)DBNull.Value;
                    confirmation_date.Value = employee.ConfirmationDate ?? (object)DBNull.Value;
                    current_designation.Value = employee.CurrentDesignation ?? (object)DBNull.Value;
                    sttor.Value = employee.StateOfOrigin ?? (object)DBNull.Value;
                    lgaor.Value = employee.LocalGovernmentOfOrigin ?? (object)DBNull.Value;
                    religion.Value = employee.Religion ?? (object)DBNull.Value;
                    gp_rgn.Value = employee.GeoPoliticalRegion ?? (object)DBNull.Value;
                    nok_nm.Value = employee.NextOfKinName ?? (object)DBNull.Value;
                    nok_rls.Value = employee.NextOfKinRelationship ?? (object)DBNull.Value;
                    nok_addr.Value = employee.NextOfKinAddress ?? (object)DBNull.Value;
                    nok_eml.Value = employee.NextOfKinEmail ?? (object)DBNull.Value;
                    nok_pn.Value = employee.NextOfKinPhoneNo ?? (object)DBNull.Value;
                    loc_id.Value = employee.LocationID ?? (object)DBNull.Value;
                    emp_stts.Value = (int)employee.EmployeeStatus;
                    mstatus.Value = employee.MaritalStatus ?? (object)DBNull.Value;
                    mdb.Value = employee.CreatedBy ?? (object)DBNull.Value;
                    mdt.Value = employee.CreatedTime ?? (object)DBNull.Value;
                    ctb.Value = employee.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = employee.CreatedTime ?? (object)DBNull.Value;
                    dept_cd.Value = employee.DepartmentCode ?? (object)DBNull.Value;
                    unit_cd.Value = employee.UnitCode ?? (object)DBNull.Value;
                    paddr.Value = employee.PermanentHomeAddress ?? (object)DBNull.Value;
                    jbp_id.Value = employee.JobProfileID ?? (object)DBNull.Value;
                    emp_ctg_id.Value = employee.EmployeeCategoryID ?? (object)DBNull.Value;
                    emp_typ_id.Value = employee.EmployeeTypeID ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }

        public async Task<bool> UpdateEmployeePersonalInfoOnlyAsync(Employee employee)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ermempinf SET title=@title, sname=@sname, fname=@fname, ");
            sb.Append("oname=@oname, fullname=@fullname, sex=@sex, pn1=@pn1, pn2=@pn2, ");
            sb.Append("peml=@peml, raddr=@raddr, bday=@bday, bmonth=@bmonth, byear=@byear, ");
            sb.Append("sttor=@sttor, lgaor=@lgaor, religion=@religion, gp_rgn=@gp_rgn, ");
            sb.Append("mstatus=@mstatus, mdb=@mdb, mdt=@mdt ");
            sb.Append("WHERE (empid=@empid); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    var sname = cmd.Parameters.Add("@sname", NpgsqlDbType.Text);
                    var fname = cmd.Parameters.Add("@fname", NpgsqlDbType.Text);
                    var oname = cmd.Parameters.Add("@oname", NpgsqlDbType.Text);
                    var fullname = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    var sex = cmd.Parameters.Add("@sex", NpgsqlDbType.Text);
                    var pn1 = cmd.Parameters.Add("@pn1", NpgsqlDbType.Text);
                    var pn2 = cmd.Parameters.Add("@pn2", NpgsqlDbType.Text);
                    var peml = cmd.Parameters.Add("@peml", NpgsqlDbType.Text);
                    var raddr = cmd.Parameters.Add("@raddr", NpgsqlDbType.Text);
                    var imgp = cmd.Parameters.Add("@imgp", NpgsqlDbType.Text);
                    var bday = cmd.Parameters.Add("@bday", NpgsqlDbType.Integer);
                    var bmonth = cmd.Parameters.Add("@bmonth", NpgsqlDbType.Integer);
                    var byear = cmd.Parameters.Add("@byear", NpgsqlDbType.Integer);
                    var sttor = cmd.Parameters.Add("@sttor", NpgsqlDbType.Text);
                    var lgaor = cmd.Parameters.Add("@lgaor", NpgsqlDbType.Text);
                    var religion = cmd.Parameters.Add("@religion", NpgsqlDbType.Text);
                    var gp_rgn = cmd.Parameters.Add("@gp_rgn", NpgsqlDbType.Text);
                    var mstatus = cmd.Parameters.Add("@mstatus", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    var paddr = cmd.Parameters.Add("@paddr", NpgsqlDbType.Text);

                    cmd.Prepare();

                    title.Value = employee.Title ?? (object)DBNull.Value;
                    fname.Value = employee.FirstName ?? (object)DBNull.Value;
                    oname.Value = employee.OtherNames ?? (object)DBNull.Value;
                    sname.Value = employee.Surname ?? (object)DBNull.Value;
                    fullname.Value = employee.FullName ?? (object)DBNull.Value;
                    sex.Value = employee.Sex ?? (object)DBNull.Value;
                    pn1.Value = employee.PhoneNo ?? (object)DBNull.Value;
                    pn2.Value = employee.AltPhoneNo ?? (object)DBNull.Value;
                    peml.Value = employee.PersonalEmail ?? (object)DBNull.Value;
                    raddr.Value = employee.ResidenceAddress ?? (object)DBNull.Value;
                    imgp.Value = employee.ImagePath ?? (object)DBNull.Value;
                    bday.Value = employee.BirthDay;
                    bmonth.Value = employee.BirthMonth;
                    byear.Value = employee.BirthYear;
                    sttor.Value = employee.StateOfOrigin ?? (object)DBNull.Value;
                    lgaor.Value = employee.LocalGovernmentOfOrigin ?? (object)DBNull.Value;
                    religion.Value = employee.Religion ?? (object)DBNull.Value;
                    gp_rgn.Value = employee.GeoPoliticalRegion ?? (object)DBNull.Value;
                    mstatus.Value = employee.MaritalStatus ?? (object)DBNull.Value;
                    mdb.Value = employee.LastModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = employee.LastModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = employee.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = employee.CreatedTime ?? (object)DBNull.Value;
                    paddr.Value = employee.PermanentHomeAddress ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;

        }

        public async Task<bool> UpdateEmployeeInfoOnlyAsync(Employee employee)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ermempinf SET oeml=@oeml, emp_no_1=@emp_no_1, ");
            sb.Append("emp_no_2=@emp_no_2, start_up_date=@start_up_date, ");
            sb.Append("start_up_designation=@start_up_designation, ");
            sb.Append("place_of_engagement=@place_of_engagement,  ");
            sb.Append("confirmation_date=@confirmation_date, ");
            sb.Append("current_designation=@current_designation, loc_id=@loc_id, ");
            sb.Append("emp_stts=@emp_stts,  mdb=@mdb, mdt=@mdt, dept_cd=@dept_cd, ");
            sb.Append("unit_cd=@unit_cd, jbp_id=@jbp_id, emp_ctg_id=@emp_ctg_id, ");
            sb.Append("emp_typ_id=@emp_typ_id  WHERE (empid=@empid); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var empid = cmd.Parameters.Add("@empid", NpgsqlDbType.Integer);
                    var oeml = cmd.Parameters.Add("@oeml", NpgsqlDbType.Text);
                    var emp_no_1 = cmd.Parameters.Add("@emp_no_1", NpgsqlDbType.Text);
                    var emp_no_2 = cmd.Parameters.Add("@emp_no_2", NpgsqlDbType.Text);
                    var start_up_date = cmd.Parameters.Add("@start_up_date", NpgsqlDbType.Date);
                    var start_up_designation = cmd.Parameters.Add("@start_up_designation", NpgsqlDbType.Text);
                    var place_of_engagement = cmd.Parameters.Add("@place_of_engagement", NpgsqlDbType.Text);
                    var confirmation_date = cmd.Parameters.Add("@confirmation_date", NpgsqlDbType.Date);
                    var current_designation = cmd.Parameters.Add("@current_designation", NpgsqlDbType.Text);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var emp_stts = cmd.Parameters.Add("@emp_stts", NpgsqlDbType.Integer);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.TimestampTz);
                    var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                    var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                    var jbp_id = cmd.Parameters.Add("@jbp_id", NpgsqlDbType.Integer);
                    var emp_ctg_id = cmd.Parameters.Add("@emp_ctg_id", NpgsqlDbType.Integer);
                    var emp_typ_id = cmd.Parameters.Add("@emp_typ_id", NpgsqlDbType.Integer);

                    cmd.Prepare();

                    empid.Value = employee.EmployeeID;
                    oeml.Value = employee.OfficialEmail ?? (object)DBNull.Value;
                    emp_no_1.Value = employee.EmployeeNo ?? (object)DBNull.Value;
                    emp_no_2.Value = employee.CustomNo ?? (object)DBNull.Value;
                    start_up_date.Value = employee.StartUpDate ?? (object)DBNull.Value;
                    start_up_designation.Value = employee.StartUpDesignation ?? (object)DBNull.Value;
                    place_of_engagement.Value = employee.PlaceOfEngagement ?? (object)DBNull.Value;
                    confirmation_date.Value = employee.ConfirmationDate ?? (object)DBNull.Value;
                    current_designation.Value = employee.CurrentDesignation ?? (object)DBNull.Value;
                    loc_id.Value = employee.LocationID;
                    emp_stts.Value = (int)employee.EmployeeStatus;
                    mdb.Value = employee.LastModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = employee.LastModifiedTime ?? (object)DBNull.Value;
                    dept_cd.Value = employee.DepartmentCode ?? (object)DBNull.Value;
                    unit_cd.Value = employee.UnitCode ?? (object)DBNull.Value;
                    jbp_id.Value = employee.JobProfileID ?? (object)DBNull.Value;
                    emp_ctg_id.Value = employee.EmployeeCategoryID ?? (object)DBNull.Value;
                    emp_typ_id.Value = employee.EmployeeTypeID ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }

        public async Task<bool> UpdateEmployeeNextOfKinInfoOnlyAsync(Employee employee)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ermempinf  SET nok_nm=@nok_nm, ");
            sb.Append("nok_rls=@nok_rls, nok_addr=@nok_addr, nok_pn=@nok_pn, ");
            sb.Append("nok_eml=@nok_eml, mdb=@mdb, mdt=@mdt ");
            sb.Append("WHERE (empid=@empid);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nok_nm = cmd.Parameters.Add("@nok_nm", NpgsqlDbType.Text);
                    var nok_rls = cmd.Parameters.Add("@nok_rls", NpgsqlDbType.Text);
                    var nok_addr = cmd.Parameters.Add("@nok_addr", NpgsqlDbType.Text);
                    var nok_pn = cmd.Parameters.Add("@nok_pn", NpgsqlDbType.Text);
                    var nok_eml = cmd.Parameters.Add("@nok_eml", NpgsqlDbType.Text);
                    var empid = cmd.Parameters.Add("@empid", NpgsqlDbType.Integer);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.TimestampTz);

                    cmd.Prepare();

                    nok_nm.Value = employee.NextOfKinName ?? (object)DBNull.Value;
                    nok_rls.Value = employee.NextOfKinRelationship ?? (object)DBNull.Value;
                    nok_addr.Value = employee.NextOfKinAddress ?? (object)DBNull.Value;
                    nok_eml.Value = employee.NextOfKinEmail ?? (object)DBNull.Value;
                    nok_pn.Value = employee.NextOfKinPhoneNo ?? (object)DBNull.Value;
                    empid.Value = employee.EmployeeID;
                    mdb.Value = employee.LastModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = employee.LastModifiedTime ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }
        #endregion
    }
}
