﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blitz.Services.ShoppingCartAPI.Models.Dto
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public object Result {get; set; }

        public string DisplayMessage { get; set; } = "";

        public List<string> ErrorMessages { get; set; }

    }
}
