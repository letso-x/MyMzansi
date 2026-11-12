using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using MyMzansi.Models;

namespace MyMzansi.Data
{
    public sealed class ServiceRequestRepository
    {
        private readonly string filePath;
        private readonly JavaScriptSerializer serializer;

        public string FilePath { get { return filePath; } }

        public ServiceRequestRepository()
            : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyMzansi", "service-requests.json"))
        {
        }

        public ServiceRequestRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("A data file path is required.", "path");
            filePath = path;
            serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 };
        }

        public IList<ServiceRequest> Load(out string error)
        {
            error = null;
            if (!File.Exists(filePath)) return new List<ServiceRequest>();

            try
            {
                List<ServiceRequest> requests = serializer.Deserialize<List<ServiceRequest>>(File.ReadAllText(filePath));
                return requests ?? new List<ServiceRequest>();
            }
            catch (Exception exception)
            {
                error = "Saved service request data could not be read: " + exception.Message;
                return new List<ServiceRequest>();
            }
        }

        public void Save(IEnumerable<ServiceRequest> requests)
        {
            if (requests == null) throw new ArgumentNullException("requests");
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string temporaryPath = filePath + ".tmp";
            File.WriteAllText(temporaryPath, serializer.Serialize(requests));
            if (File.Exists(filePath)) File.Replace(temporaryPath, filePath, null);
            else File.Move(temporaryPath, filePath);
        }
    }
}
