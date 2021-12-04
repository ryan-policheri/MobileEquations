using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DotNetCommon.WebApiClient;
using MobileEquations.Model;

namespace MobileEquations.Benchmarker
{
    public class MobileEquationsClient : WebApiClientBase
    {
        public MobileEquationsClient(HttpClient httpClient) : base(httpClient)
        {
            SerializerOptions = JsonSerializerDefaults.DefaultOptions;
        }

        public async Task<Equation> SolveEquation(Equation equation, string file)
        {
            Equation solvedEquation = await this.PostDataAndFile<Equation>("/api/equations", equation, file);
            return solvedEquation;
        }
    }
}
