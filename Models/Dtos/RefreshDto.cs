using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos;

public record RefreshDto(string AccessToken, string RefreshToken);
