using DHelp.Models;
using DHelp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DHelp.Controllers
{
    public class MainController
    {
        private readonly DatabaseService _databaseService;
        public MainController()
        {
            _databaseService = new DatabaseService();
        }
        public (List<Doctor>, string) GetDoctorsWithSpecialist(string table, string symptom, string city)
        {
            return _databaseService.GetDoctorsBySymptom(table, symptom, city);
        }

        public void SubmitFeedback(string doctorName, float rating)
        {
            Feedback feedback = new Feedback
            {
                DoctorName = doctorName,
                Rating = rating
            };
            _databaseService.SaveFeedback(feedback);
        }
    }
}