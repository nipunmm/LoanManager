using Microsoft.Data.SqlClient;
using LoanManager.Models;
using Microsoft.Data.SqlClient;

namespace LoanManager.Service
{
    public class LoanService
    {
        private readonly string _connectionString;
        public LoanService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<LoanListItem> GetAllLoans()
        {
            var list = new List<LoanListItem>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT l.loan_id, l.customer_id, c.customer_name, c.identity_number, l.loan_type_id, t.loan_type_name, l.loan_amount, l.interest_rate, l.loan_duration, l.current_flow, l.created_by, l.created_at
                               FROM lm_loan_account_master l
                               LEFT JOIN lm_customer c ON l.customer_id = c.customer_id
                               LEFT JOIN lm_loan_type t ON l.loan_type_id = t.loan_type_id";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LoanListItem
                        {
                            LoanId = (int)reader["loan_id"],
                            CustomerId = reader["customer_id"] == DBNull.Value ? 0 : (int)reader["customer_id"],
                            CustomerName = reader["customer_name"]?.ToString(),
                            IdentityNumber = reader["identity_number"]?.ToString(),
                            LoanTypeId = reader["loan_type_id"] == DBNull.Value ? 0 : (int)reader["loan_type_id"],
                            LoanTypeName = reader["loan_type_name"]?.ToString(),
                            LoanAmount = reader["loan_amount"] == DBNull.Value ? 0m : (decimal)reader["loan_amount"],
                            InterestRate = reader["interest_rate"] == DBNull.Value ? 0m : (decimal)reader["interest_rate"],
                            LoanDuration = reader["loan_duration"] == DBNull.Value ? 0 : (int)reader["loan_duration"],
                            // MonthlyRental removed from list item mapping
                            CurrentFlow = reader["current_flow"]?.ToString(),
                            CreatedBy = reader["created_by"]?.ToString(),
                            CreatedAt = reader["created_at"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["created_at"]
                        });
                    }
                }
            }
            return list;
        }

        public LoanListItem? GetLoanById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT l.loan_id, l.customer_id, c.customer_name, c.identity_number, l.loan_type_id, t.loan_type_name, l.loan_amount, l.interest_rate, l.loan_duration, l.monthly_rental, l.current_flow, l.created_by, l.created_at
                               FROM lm_loan_account_master l
                               LEFT JOIN lm_customer c ON l.customer_id = c.customer_id
                               LEFT JOIN lm_loan_type t ON l.loan_type_id = t.loan_type_id
                               WHERE l.loan_id = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LoanListItem
                            {
                                LoanId = (int)reader["loan_id"],
                                CustomerId = reader["customer_id"] == DBNull.Value ? 0 : (int)reader["customer_id"],
                                CustomerName = reader["customer_name"]?.ToString(),
                                IdentityNumber = reader["identity_number"]?.ToString(),
                                LoanTypeId = reader["loan_type_id"] == DBNull.Value ? 0 : (int)reader["loan_type_id"],
                                LoanTypeName = reader["loan_type_name"]?.ToString(),
                                LoanAmount = reader["loan_amount"] == DBNull.Value ? 0m : (decimal)reader["loan_amount"],
                                InterestRate = reader["interest_rate"] == DBNull.Value ? 0m : (decimal)reader["interest_rate"],
                                LoanDuration = reader["loan_duration"] == DBNull.Value ? 0 : (int)reader["loan_duration"],
                                MonthlyRental = reader["monthly_rental"] == DBNull.Value ? 0m : (decimal)reader["monthly_rental"],
                                CurrentFlow = reader["current_flow"]?.ToString(),
                                CreatedBy = reader["created_by"]?.ToString(),
                                CreatedAt = reader["created_at"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["created_at"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<LoanType> GetLoanTypes()
        {
            var list = new List<LoanType>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT loan_type_id, loan_type_name, interest_rate FROM lm_loan_type", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LoanType
                        {
                            LoanTypeId = (int)reader["loan_type_id"],
                            LoanTypeName = reader["loan_type_name"].ToString(),
                            InterestRate = reader["interest_rate"] == DBNull.Value ? 0m : (decimal)reader["interest_rate"]
                        });
                    }
                }
            }
            return list;
        }

        public LoanType GetLoanTypeById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT loan_type_id, loan_type_name, interest_rate FROM lm_loan_type WHERE loan_type_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LoanType
                            {
                                LoanTypeId = (int)reader["loan_type_id"],
                                LoanTypeName = reader["loan_type_name"].ToString(),
                                InterestRate = reader["interest_rate"] == DBNull.Value ? 0m : (decimal)reader["interest_rate"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public int? GetCustomerIdByIdentityNumber(string identityNumber)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT customer_id FROM lm_customer WHERE identity_number = @idn", conn))
                {
                    cmd.Parameters.AddWithValue("@idn", identityNumber ?? string.Empty);
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                        return Convert.ToInt32(obj);
                }
            }
            return null;
        }

        public void CreateLoan(LoanAccount loan)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO lm_loan_account_master
                               (customer_id, loan_type_id, loan_amount, interest_rate, loan_duration, monthly_rental, current_flow, created_by)
                               VALUES (@customer_id, @loan_type_id, @loan_amount, @interest_rate, @loan_duration, @monthly_rental, @current_flow, @created_by)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@customer_id", loan.CustomerId);
                    cmd.Parameters.AddWithValue("@loan_type_id", loan.LoanTypeId);
                    cmd.Parameters.AddWithValue("@loan_amount", loan.LoanAmount);
                    cmd.Parameters.AddWithValue("@interest_rate", loan.InterestRate);
                    cmd.Parameters.AddWithValue("@loan_duration", loan.LoanDuration);
                    cmd.Parameters.AddWithValue("@monthly_rental", loan.MonthlyRental);
                    cmd.Parameters.AddWithValue("@current_flow", loan.CurrentFlow ?? "New");
                    cmd.Parameters.AddWithValue("@created_by", loan.CreatedBy ?? "System");

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
