using System.Globalization;

namespace AppAdminEmployed.Services
{
    public class CSVService
    {
        public async Task<List<Dictionary<string, string>>> LeerCsvAsync(Stream archivo)
        {
            var resultado = new List<Dictionary<string, string>>();

            using (var reader = new StreamReader(archivo))
            {
                string? headerLine = await reader.ReadLineAsync();
                if (headerLine == null)
                    return resultado;

                var headers = headerLine.Split(',').Select(h => h.Trim()).ToArray();

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var values = line.Split(',');
                    var fila = new Dictionary<string, string>();

                    for (int i = 0; i < headers.Length && i < values.Length; i++)
                    {
                        fila[headers[i]] = values[i].Trim();
                    }

                    resultado.Add(fila);
                }
            }

            return resultado;
        }
    }
}