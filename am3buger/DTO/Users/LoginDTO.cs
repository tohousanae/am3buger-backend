﻿using System.ComponentModel.DataAnnotations;

namespace am3burger.DTO.Users
{
    public class LoginDTO
    {
        // 使用者id
        public int Id { get; set; }

        // 信箱
        public string? Email { get; set; }

        // 密碼
        public string? Password { get; set; }
    }
}
