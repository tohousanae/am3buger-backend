﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using am3burger.Models;
using Microsoft.CodeAnalysis.Scripting;
using am3burger.DTO.Users;

namespace am3burger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Am3burgerContext _context;

        public UserController(Am3burgerContext context)
        {
            _context = context;
        }

        // 登入註冊api參考資料：https://ithelp.ithome.com.tw/articles/10337994
        public static User user = new User();//宣告了一個名為 **user** 的**User**對象，並設為static靜態成員，表示無論何時訪問這個類別都會使用相同的User對象。

        // 註冊api
        [HttpPost("register")]
        public ActionResult<User> Register(LoginDTO request)
        {
            //使用了 **BCrypt.Net** 函式庫來將 **request.Password** 中的密碼進行哈希加密演算法處理，處理後的密碼存儲在 **passwordHash** 變數中。
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.Email = request.Email;
            user.Password = passwordHash;

            return Ok(user);
        }

        // 登入api
        [HttpPost("login")]
        public ActionResult<User> Login(LoginDTO request)
        {
            if (user.Email != request.Email) { return BadRequest("該使用者不存在"); }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return BadRequest("信箱或密碼錯誤");
            }
            return Ok(user);
        }

        // 查詢所有會員
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }

        // 查詢個別會員
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // 修改個別會員資料
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // 新增會員資料
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // 刪除會員資料
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}