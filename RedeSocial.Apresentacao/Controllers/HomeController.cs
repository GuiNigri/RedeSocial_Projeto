﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedeSocial.Apresentacao.Models;
using RedeSocial.Model.Interfaces.Services;

namespace RedeSocial.Apresentacao.Controllers
{
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly IAmigosServices _amigosServices;

        public HomeController(IAmigosServices amigosServices, UserManager<IdentityUser> userManager, IUsuarioServices usuarioServices, IPostServices postServices, ICommentPostServices commentPostServices)
            : base(userManager, usuarioServices, postServices, commentPostServices, amigosServices)
        {
            _amigosServices = amigosServices;
        }

        public async Task<IActionResult> Index()
        {
            var solicitacoes = await GetSolicitacoesAmizade();
            var userId = await GetUserIdentityAsync();

            var (postList, usuarioLogado, amigosList) = await HomeIndexAndUsuarioPerfilBase(null, userId);

            var homeViewModel = new HomeViewModel
            {
                IdentityUserLogado = userId,
                NomePerfil = usuarioLogado.Nome + " " + usuarioLogado.Sobrenome,
                FotoPerfil = usuarioLogado.FotoPerfil,
                PostList = postList,
                SolicitacoesPendentes = solicitacoes,
                Amigos = amigosList
            };

            return View(homeViewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<IEnumerable<AmigosViewModel>> GetSolicitacoesAmizade()
        {
            var listaFormatada = new List<AmigosViewModel>();

            var userId = await GetUserIdentityAsync();

            var solicitacoes = await _amigosServices.GetSolicitacoesPendentes(userId);

            foreach (var solicitacao in solicitacoes)
            {
                var amigosSolicitacoesViewModel = await ConverterIdToNameAndModelToViewModel(solicitacao);

                listaFormatada.Add(amigosSolicitacoesViewModel);
            }

            return listaFormatada;

        }
    }
}
