using LoanManager.Models;
using Microsoft.Data.SqlClient;

namespace LoanManager.Service
{
    public class CustomerService
    {

        private readonly string _connectionString;

        public CustomerService(IConfiguration configuration)
        {
            // Use the connection string name from appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Get all customers
        public List<Customer> GetAllCustomers()
        {
            var list = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM lm_customer";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Customer
                            {
                                CustomerId = (int)reader["customer_id"],
                                CustomerName = reader["customer_name"].ToString(),
                                IdentityType = reader["identity_type"].ToString(),
                                IdentityNumber = reader["identity_number"].ToString(),
                                CreatedBy = reader["created_by"].ToString(),
                                CreatedAt = (DateTime)reader["created_at"]
                            });
                        }
                    }
                }
            }

            return list;
        }


        // Add a new customer
        public void AddCustomer(Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO lm_customer 
                           (customer_name, identity_type, identity_number, created_by) 
                           VALUES (@Name, @Type, @Number, @CreatedBy)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@Type", customer.IdentityType);
                    cmd.Parameters.AddWithValue("@Number", customer.IdentityNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", customer.CreatedBy ?? "System");

                    cmd.ExecuteNonQuery();
                }
            }
        }



    }
}
