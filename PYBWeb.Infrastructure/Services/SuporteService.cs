using Microsoft.EntityFrameworkCore;
using PYBWeb.Domain.Entities;
using PYBWeb.Domain.Interfaces;
using PYBWeb.Infrastructure.Data;

namespace PYBWeb.Infrastructure.Services
{
    public class SuporteService
    {
        private readonly ColaboradoresDbContext _context;
        private readonly ILogService _logService;

        public SuporteService(ColaboradoresDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<List<MembroSuporte>> ObterTodosAsync()
        {
            return await _context. Suporte
                .OrderBy(m => m. Categoria)
                .ThenBy(m => m.Nome)
                .ToListAsync();
        }

        public async Task<MembroSuporte? > ObterCoordenadorAsync()
        {
            return await _context. Suporte
                .FirstOrDefaultAsync(m => m. Categoria == "Coordenador");
        }

        public async Task<MembroSuporte? > ObterLiderTecnicoAsync()
        {
            return await _context.Suporte
                .FirstOrDefaultAsync(m => m.Categoria == "Lider Técnico CICS");
        }

        public async Task<List<MembroSuporte>> ObterPorCategoriaAsync(string categoria)
        {
            return await _context.Suporte
                .Where(m => m. Categoria == categoria)
                .OrderBy(m => m.Nome)
                .ToListAsync();
        }

        public async Task<MembroSuporte? > ObterPorIdAsync(int id)
        {
            return await _context. Suporte. FindAsync(id);
        }

        public async Task<MembroSuporte> AdicionarAsync(MembroSuporte membro)
        {
            // Se for Coordenador ou Líder Técnico, garantir que só tenha um
            if (membro.Categoria == "Coordenador" || membro.Categoria == "Lider Técnico CICS")
            {
                var existente = await _context.Suporte
                    .FirstOrDefaultAsync(m => m.Categoria == membro.Categoria);
                
                if (existente != null)
                {
                    throw new Exception($"Já existe um {membro.Categoria} cadastrado.  Remova o existente antes de adicionar outro.");
                }
            }

            membro.DataCriacao = DateTime.Now;
            _context.Suporte.Add(membro);
            await _context.SaveChangesAsync();

            await _logService. RegistrarAsync(
                "Adicionar",
                "Suporte",
                membro.Id,
                membro.Nome,
                $"Novo membro adicionado: {membro.Nome} ({membro.Email}) - Categoria: {membro.Categoria}"
            );

            return membro;
        }

        public async Task<MembroSuporte> AtualizarAsync(MembroSuporte membro)
        {
            var membroExistente = await _context.Suporte.FindAsync(membro.Id);
            if (membroExistente == null)
                throw new Exception("Membro não encontrado");

            // Verificar se está mudando para Coordenador ou Líder e já existe outro
            if ((membro.Categoria == "Coordenador" || membro. Categoria == "Lider Técnico CICS") &&
                membroExistente.Categoria != membro.Categoria)
            {
                var existente = await _context. Suporte
                    .FirstOrDefaultAsync(m => m. Categoria == membro.Categoria && m.Id != membro.Id);
                
                if (existente != null)
                {
                    throw new Exception($"Já existe um {membro.Categoria} cadastrado.");
                }
            }

            membroExistente.Nome = membro.Nome;
            membroExistente.Email = membro.Email;
            membroExistente.Ramal = membro.Ramal;
            membroExistente. Categoria = membro.Categoria;
            membroExistente. DataAtualizacao = DateTime. Now;

            await _context.SaveChangesAsync();

            await _logService.RegistrarAsync(
                "Atualizar",
                "Suporte",
                membro.Id,
                membro.Nome,
                $"Membro atualizado: {membro.Nome} - Categoria: {membro. Categoria}"
            );

            return membroExistente;
        }

        public async Task ExcluirAsync(int id)
        {
            var membro = await _context.Suporte.FindAsync(id);
            if (membro == null)
                throw new Exception("Membro não encontrado");

            var nome = membro.Nome;
            _context.Suporte.Remove(membro);
            await _context.SaveChangesAsync();

            await _logService.RegistrarAsync(
                "Excluir",
                "Suporte",
                id,
                nome,
                $"Membro removido: {nome}"
            );
        }
    }
}