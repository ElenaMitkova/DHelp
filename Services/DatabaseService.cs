using DHelp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DHelp.Services
{
    public class DatabaseService
    {
        static readonly string connectionString = @"Server=.\SQLEXPRESS; Database=DHelpEN; Integrated Security=True";
        public (List<Doctor>, string specialistName) GetDoctorsBySymptom(string table, string symptom, string city)
        {
            List<Doctor> doctors = new List<Doctor>();
            string specialistName = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Вземане на името на специалиста
                string q = $"SELECT specialist FROM Doctors AS d JOIN {table} AS t ON t.SpecialistID = d.ID WHERE t.{table} = @Symptom";
                SqlCommand cmd1 = new SqlCommand(q, connection);
                cmd1.Parameters.AddWithValue("@Symptom", symptom);
                using (SqlDataReader reader1 = cmd1.ExecuteReader())
                {
                    if (reader1.Read())
                        specialistName = reader1[0].ToString();
                }

                // Вземане на докторите
                string query = $@"select DoctorName, Location, Address, PhoneNumber, ROUND((Sum_Rating / Count_Rating), 1) as Rating from DoctorsInformation di join Doctors d on d.ID = di.SpecializationNumber where specializationNumber = (select specialistID from {table} as symptom join Doctors d on d.ID = symptom.SpecialistID where {table} = '{symptom}')order by Rating desc;";

                SqlCommand cmd2 = new SqlCommand(query, connection);
                cmd2.Parameters.AddWithValue("@Symptom", symptom);
                using (SqlDataReader reader2 = cmd2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        if (reader2[1].Equals(city))
                        {
                            doctors.Add(new Doctor
                            {
                                Name = reader2["DoctorName"].ToString(),
                                Location = reader2["Location"].ToString(),
                                Address = reader2["Address"].ToString(),
                                PhoneNumber = reader2["PhoneNumber"].ToString(),
                                Rating = float.Parse(reader2["Rating"].ToString())
                            });
                        }
                    }
                }
            }

            return (doctors, specialistName);
        }

        public void SaveFeedback(Feedback feedback)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE DoctorsInformation SET Sum_Rating = Sum_Rating + @Rating, Count_Rating = Count_Rating + 1 WHERE DoctorName = @DoctorName";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Rating", feedback.Rating);
                cmd.Parameters.AddWithValue("@DoctorName", feedback.DoctorName);
                cmd.ExecuteNonQuery();
            }
        }
    }
}