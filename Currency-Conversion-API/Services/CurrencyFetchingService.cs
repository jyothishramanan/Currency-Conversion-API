using Currency_Conversion_Business.BusinessLayer;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Currency_Conversion_API.Services
{
    public class CurrencyFetchingService : IHostedService, IDisposable
    {
        private Timer _timer;
        IConfiguration _configuration;
        public CurrencyFetchingService(IConfiguration configuration) 
        {
            _configuration = configuration;
            
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            int Interval;
            int.TryParse(_configuration.GetSection("TimerInterval").Value,out Interval);

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(Interval));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            // Method to be invoked every 10 minutes
            var href =
                _configuration.GetSection("CurrencyURL").Value;
            var DataSource =
                           _configuration.GetSection("DataSource").Value;
            var FileName=_configuration.GetSection("Filename").Value;

            using (WebClient client = new WebClient())
            {
                try
                {
                    DateTime today = DateTime.Today;
                    // Create the directory path using today's date
                    string directoryPath = Path.Combine(Environment.CurrentDirectory+ "\\"+ DataSource, today.ToString("yyyy-MM-dd"));

                    // Check if the directory exists
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    client.DownloadFile(href, directoryPath+"\\"+FileName);
                    Console.WriteLine("XML file downloaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading XML file: {ex.Message}");
                }
            }
        }

        

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
