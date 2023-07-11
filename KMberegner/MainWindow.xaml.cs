using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace KMberegner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Read and validate user inputs
                double distance = double.Parse(distanceTextBox.Text);

                if (passengerComboBox.SelectedItem != null)
                {
                    int passengers = int.Parse(((ComboBoxItem)passengerComboBox.SelectedItem).Content.ToString());

                    // Calculate the base fare based on the pricing model rules
                    double baseFare = CalculateBaseFare(distance, passengers, DateTime.Now);
                    // Apply additional charges if any
                    double totalPrice = ApplyAdditionalCharges(baseFare, passengers);

                    // Display the calculated price
                    priceLabel.Content = $"Price: {totalPrice:C}";
                }
                else
                {
                    MessageBox.Show("Please select the number of passengers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double CalculateBaseFare(double distance, int passengers, DateTime currentTime)
        {
            const double ordinaryCarDayRatePerKilometer = 12.75; // Day rate per kilometer for ordinary car
            const double ordinaryCarNightRatePerKilometer = 10.5; // Night rate per kilometer for ordinary car
            const double bigCarDayRatePerKilometer = 15.5; // Day rate per kilometer for big car
            const double bigCarNightRatePerKilometer = 13.25; // Night rate per kilometer for big car

            // Determine the base rate per kilometer based on the number of passengers and time of day
            double baseRatePerKilometer;
            if (passengers <= 6)
            {
                baseRatePerKilometer = IsNightTime(currentTime) ? ordinaryCarNightRatePerKilometer : ordinaryCarDayRatePerKilometer;
            }
            else
            {
                baseRatePerKilometer = IsNightTime(currentTime) ? bigCarNightRatePerKilometer : bigCarDayRatePerKilometer;
            }

            // Calculate the base fare based on the distance and base rate per kilometer
            double baseFare = distance * baseRatePerKilometer;
            return baseFare;
        }

        private bool IsNightTime(DateTime currentTime)
        {
            // Define the start and end times for night hours (e.g., 10 PM to 6 AM)
            TimeSpan nightStartTime = new TimeSpan(22, 0, 0); // 10 PM
            TimeSpan nightEndTime = new TimeSpan(6, 0, 0); // 6 AM

            // Check if the current time falls within the night hours
            if (currentTime.TimeOfDay >= nightStartTime || currentTime.TimeOfDay < nightEndTime)
            {
                return true;
            }

            return false;
        }

        private double ApplyAdditionalCharges(double baseFare, int passengers)
        {
            const double extraPassengerCharge = 2.0; // Additional charge per passenger

            // Apply additional charges based on the number of passengers
            double totalPrice = baseFare + (extraPassengerCharge * (passengers - 1));
            return totalPrice;
        }

        private void ClockTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Ensure that the colon (":") is always present in the clockTextBox
            string text = clockTextBox.Text;

            // Check if the last character is not a colon
            if (!text.EndsWith(":"))
            {
                // Remove any colon characters from the text
                text = text.Replace(":", "");

                // Insert a colon at the appropriate position
                if (text.Length >= 2)
                    text = text.Insert(2, ":");

                // Update the clockTextBox with the modified text
                clockTextBox.Text = text;
            }
        }

        private void SetTimeButton_Click(object sender, RoutedEventArgs e)
        {
            // Parse the time string to extract hours and minutes
            if (TryParseTime(clockTextBox.Text, out int hours, out int minutes))
            {
                // Get the current date and combine it with the parsed time
                DateTime currentTime = DateTime.Now;
                DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hours, minutes, 0);

                // Display the selected time in the displayLabel
                displayLabel.Content = selectedTime.ToString("HH:mm");
            }
            else
            {
                displayLabel.Content = "Invalid time format";
            }
        }

        private bool TryParseTime(string timeString, out int hours, out int minutes)
        {
            hours = 0;
            minutes = 0;

            // Split the time string into hours and minutes based on the colon (":")
            string[] timeParts = timeString.Split(':');

            if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int parsedHours) && int.TryParse(timeParts[1], out int parsedMinutes))
            {
                if (parsedHours >= 0 && parsedHours <= 23 && parsedMinutes >= 0 && parsedMinutes <= 59)
                {
                    hours = parsedHours;
                    minutes = parsedMinutes;
                    return true;
                }
            }

            return false;
        }
    }
}
