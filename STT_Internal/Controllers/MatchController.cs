using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STT_Internal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace STT_Internal.Controllers
{
    public class MatchController : ApiController
    {
        // POST api/Match

        public HttpResponseMessage Post(Payload payload)
        {
            int currentDistance = payload.Distance;
            string matchedWord = "";
            foreach(var item in payload.ExpectedWords)
            {
                var distance = DamerauLevenshteinDistance(payload.Word, item.Description);
                if(distance <= currentDistance)
                {
                    currentDistance = distance;
                    matchedWord = item.Description;
                }
            }

            var resp = new Payload()
            {
                Word = matchedWord,
                Distance = currentDistance
            };

            string jsonObject = JsonConvert.SerializeObject(resp);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");

            return response;
        }

        // The Damerau-Levenshtein distance algorithm
        // Finds the number of steps it would take to transform one string into another string using the following operations:
        // Insertion, deletion, substitution and transposition
        public static int DamerauLevenshteinDistance(string string1, string string2)
        {
            if (String.IsNullOrEmpty(string1))
            {
                if (!String.IsNullOrEmpty(string2))
                    return string2.Length;

                return 0;
            }

            if (String.IsNullOrEmpty(string2))
            {
                if (!String.IsNullOrEmpty(string1))
                    return string1.Length;

                return 0;
            }

            int length1 = string1.Length;
            int length2 = string2.Length;

            int[,] d = new int[length1 + 1, length2 + 1];

            int cost, del, ins, sub;

            for (int i = 0; i <= d.GetUpperBound(0); i++)
                d[i, 0] = i;

            for (int i = 0; i <= d.GetUpperBound(1); i++)
                d[0, i] = i;

            for (int i = 1; i <= d.GetUpperBound(0); i++)
            {
                for (int j = 1; j <= d.GetUpperBound(1); j++)
                {
                    if (string1[i - 1] == string2[j - 1])
                        cost = 0;
                    else
                        cost = 1;

                    del = d[i - 1, j] + 1;
                    ins = d[i, j - 1] + 1;
                    sub = d[i - 1, j - 1] + cost;

                    d[i, j] = Math.Min(del, Math.Min(ins, sub));

                    if (i > 1 && j > 1 && string1[i - 1] == string2[j - 2] && string1[i - 2] == string2[j - 1])
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }
    }
}
