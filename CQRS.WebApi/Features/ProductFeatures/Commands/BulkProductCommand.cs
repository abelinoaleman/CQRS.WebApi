using CQRS.WebApi.Domain.Models;
using CQRS.WebApi.Infrastructure.Context;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CQRS.WebApi.Features.ProductFeatures.Commands
{
    public class BulkProductCommand : IRequest<bool>
    {
        //public string FileName { get; set; }

        public IFormFile File { get; set; }
               
        public class BulkProductCommandHandler : IRequestHandler<BulkProductCommand, bool>
        {
            private readonly IApplicationContext _context;

            public BulkProductCommandHandler(IApplicationContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(BulkProductCommand command, CancellationToken cancellationToken)
            {
               
                command.File.OpenReadStream();

                //long size = command.File.Sum(f => f.Length);

                // full path to file in temp location
                var filePath = Path.GetTempFileName();

             
                    if (command.File.Length > 0)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await command.File.CopyToAsync(stream);
                        }
                    }
                var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Encoding = Encoding.UTF8, // Our file uses UTF-8 encoding.
                    Delimiter = "," // The delimiter is a comma.
                };

                using (var reader = new StreamReader(command.File.OpenReadStream()))
                using (var csv = new CsvReader( reader, configuration))
                {
                    var records = csv.GetRecords<Product>();
               
                    foreach (var item in records)
                    {
                        var product = new Product();
                        product.Id = item.Id;
                        product.Barcode = item.Barcode;
                        product.Name = item.Name;
                        product.BuyingPrice = item.BuyingPrice;
                        product.Rate = item.Rate;
                        product.Description = item.Description;
                        _context.Products.Add(product);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
