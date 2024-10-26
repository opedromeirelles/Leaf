﻿using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Repository.Materiais;

namespace Leaf.Services.Materiais
{
    public class ProdutoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;
        // Construtor que injeta a dependência de DbConnectionManager
        public ProdutoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<Produto> ListarProdutos()
        {
            ProdutoRepository _produtosRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                return _produtosRepository.GetProdutos();
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao listar os produtos, erro: {ex.Message}");
            }
        }

        public List<Produto> ListarProdutosFiltrados(string descricao, int status)
        {
            ProdutoRepository _produtosRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                return _produtosRepository.GetProdutosFiltro(descricao, status);
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao listar os produtos, erro: {ex.Message}");
            }
        }

        public int GetQuantidadeEstoque(int idProduto)
        {
            ProdutoRepository _produtosRepository = new ProdutoRepository(_dbConnectionManager);
            return _produtosRepository.ConsultaEstoqueProduto(idProduto);
        }

        public Produto GetProduto(int idProduto)
        {
            ProdutoRepository _produtosRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                return _produtosRepository.GetProdutoById(idProduto);
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao consultar produto, erro: {ex.Message}");
            }
        }

        public bool AtualizarStatusProduto(int idProduto)
        {
            ProdutoRepository _produtoRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                _produtoRepository.AtualizarSatusProduto(idProduto);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AtualizarProduto(Produto produto)
        {
            ProdutoRepository _produtoRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                _produtoRepository.AtualizarProduto(produto);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CadastrarProduto(Produto produto)
        {
            ProdutoRepository _produtoRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                _produtoRepository.CadastrarProduto(produto);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}