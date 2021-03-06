﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedeSocial.Model.Entity;
using RedeSocial.Model.Exceptions;
using RedeSocial.Model.Interfaces.Services;
using RedeSocial.Model.UoW;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedeSocial.Aplicacao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmigosController : ControllerBase
    {
        private readonly IAmigosServices _amigosServices;
        private readonly IUnitOfWork _unitOfWork;

        public AmigosController(IAmigosServices amigosServices,IUnitOfWork unitOfWork)
        {
            _amigosServices = amigosServices;
            _unitOfWork = unitOfWork;
        }

        // POST: api/Usuario
        // To protect from overamigosModeling attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        [HttpPost]
        public async Task<ActionResult<AmigosModel>> PostAmigosModel([Bind("Id,UserId1,UserId2,DataInicioAmizade,StatusAmizade")] AmigosModel amigosModel)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _unitOfWork.BeginTransaction();
                await _amigosServices.CreateAsync(amigosModel);
                await _unitOfWork.CommitAsync();
            }
            catch (ModelValidationExceptions e)
            {
                ModelState.AddModelError(e.PropertyName, e.Message);
                return BadRequest(ModelState);
            }

            return base.Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AmigosModel>> PutAmigosModel(int id, [Bind("Id,UserId1,UserId2,DataInicioAmizade,StatusAmizade")] AmigosModel amigosModel)
        {
            if (id != amigosModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _unitOfWork.BeginTransaction();
                await _amigosServices.UpdateAsync(amigosModel);
                await _unitOfWork.CommitAsync();
            }
            catch (ModelValidationExceptions e)
            {
                ModelState.AddModelError(e.PropertyName, e.Message);
                return BadRequest(ModelState);
            }

            return base.Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AmigosModel>>> GetAmigosModel()
        {
            var amigosModel = await _amigosServices.GetAllAsync();
            return amigosModel.ToList();
        }


        [HttpGet("{userLogado}/{perfilAcessado}")]
        public async Task<ActionResult<AmigosModel>> GetAmigosModel(string userLogado, string perfilAcessado)
        {
            if (userLogado == null || perfilAcessado == null)
            {
                return NotFound();
            }
            var amigosModel = await _amigosServices.GetByUsersAsync(userLogado, perfilAcessado);


            return amigosModel;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AmigosModel>> GetAmigosModel(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var amigosModel = await _amigosServices.GetByIdAsync(id);
        
            if (amigosModel == null)
            {
                return NotFound();
            }
        
            return amigosModel;
        }

        [HttpGet("pendentes/{user}")]
        public async Task<ActionResult<IEnumerable<AmigosModel>>> GetAmigosModel(string user)
        {
            try
            {
                if (user == null)
                {
                    return NotFound();
                }
                var amigosModel = await _amigosServices.GetSolicitacoesPendentes(user);


                return amigosModel.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("usuario/{userLogado}")]
        public async Task<ActionResult<IEnumerable<AmigosModel>>> GetAmigosModelList(string userLogado)
        {
            if (userLogado == null)
            {
                return NotFound();
            }
            var amigosModel = await _amigosServices.GetListByUserAsync(userLogado);


            return amigosModel.ToList();
        }

         [HttpDelete("{id}")]
         public async Task<ActionResult<AmigosModel>> DeleteAmigosModel(int id)
         {
             if (id <= 0)
             {
                 return NotFound();
             }
        
             var amigosModel = await _amigosServices.GetByIdAsync(id);
        
             if (amigosModel == null)
             {
                 return NotFound();
             }
        
             _unitOfWork.BeginTransaction();
             await _amigosServices.DeleteAsync(id);
             await _unitOfWork.CommitAsync();

            return amigosModel;
         }

    }
}
