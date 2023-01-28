using System;
using Tarefas.DTO;
using Tarefas.DAO;
using AutoMapper;
using Tarefas.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Tarefas.Web.Controllers
{

    public class LoginController : Controller
    {
        private readonly IUsuarioDAO _usuarioDAO;

         private readonly IMapper _mapper;

        public LoginController(IUsuarioDAO usuarioDAO, IMapper mapper)
        {
            _usuarioDAO = usuarioDAO;
            _mapper = mapper;   
         }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(UsuarioViewModel usuarioViewModel)
        {
            // demais instruções já implementadas do processo de login
            if (ModelState.IsValid)
            {
                try
                {
                    UsuarioDTO user = _usuarioDAO.Autenticar(usuarioViewModel.Email, usuarioViewModel.Senha);
                   
                   var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Nome),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = true,
                    RedirectUri = "/Login"
                    };

                    
                
             

                  HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                    return LocalRedirect("/Home");
                }
                catch (Exception ex)
                {
                    // logica de tratamento da exceção que vamos adicionar
                    ModelState.AddModelError("Email", "Coloque um e-mail válido");
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View();

                }

                // logica de autenticação

                
            }
            return View();
        }

        public IActionResult Sair()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/Login");

        }


    }
        
}      

