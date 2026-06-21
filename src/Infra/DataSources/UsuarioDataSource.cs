using Application.Interfaces;
using Infra.Context;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Usuarios.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.DataSources
{
    public class UsuarioDataSource : IUsuarioDataSource
    {
        private readonly AppDbContext _appDbContext;

        public UsuarioDataSource(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(UsuarioInputDTO usuario, CancellationToken ct)
        {
            var usuarioDbModel = new UsuarioDbModel(usuario.Id, usuario.Nome, usuario.Email, usuario.SenhaHash, usuario.Perfil, usuario.IdUsuarioCriacao, usuario.DataCriacao, usuario.IdUsuarioAtualizacao, usuario.DataAtualizacao, usuario.Ativo);

            await _appDbContext.Usuarios.AddAsync(usuarioDbModel, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<UsuarioInputDTO?> GetByEmail(string email, CancellationToken ct)
        {
            var usuario = await _appDbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Ativo, ct);

            if (usuario is null)
                return null;

            return new UsuarioInputDTO
            {
                Email = usuario.Email,
                Id = usuario.Id,
                Nome = usuario.Nome,
                Perfil = usuario.Perfil,
                SenhaHash = usuario.SenhaHash
            };
        }
    }
}
