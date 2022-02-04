using AutoMapper;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProdutosController : BaseController
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IProdutoService _produtoService;
    private readonly IMapper _mapper;

    public ProdutosController(INotificador notificador,
                              IProdutoRepository produtoRepository,
                              IProdutoService produtoService,
                              IMapper mapper,
                              IUser appUser) : base(notificador, appUser)
    {
        _produtoRepository = produtoRepository;
        _produtoService = produtoService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ProdutoViewModel> ObterTodos()
    {
        return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutosFornecedores());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
    {
        var produtoViewModel = await ObterProduto(id);
        if (produtoViewModel is null)
            return NotFound();

        return produtoViewModel;
    }

    [ClaimsAuthorize("Produto", "Adicionar")]
    [HttpPost]
    public async Task<IActionResult> Adicionar(ProdutoImagemViewModel produtoViewModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var imgPrefixo = Guid.NewGuid() + "_";
        if (!await UploadArquivoStream(produtoViewModel.ImagemUpload, imgPrefixo))
        {
            return CustomResponse();
        }

        produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
        await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

        return CustomResponse(produtoViewModel);
    }

    [ClaimsAuthorize("Produto", "Atualizar")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
    {
        if (id != produtoViewModel.Id) return BadRequest();

        var produtoAtualizacao = await ObterProduto(id);
        if (produtoAtualizacao is null) return NotFound();

        produtoViewModel.Imagem = produtoAtualizacao.Imagem;
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        if (!string.IsNullOrWhiteSpace(produtoViewModel.ImagemUpload))
        {
            var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
            if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return CustomResponse();
            }

            produtoAtualizacao.Imagem = imagemNome;
        }

        produtoAtualizacao.Nome = produtoViewModel.Nome;
        produtoAtualizacao.Descricao = produtoViewModel.Descricao;
        produtoAtualizacao.Valor = produtoViewModel.Valor;
        produtoAtualizacao.Ativo = produtoViewModel.Ativo;

        await _produtoService.Atualizar(_mapper.Map<Produto>(produtoViewModel));

        return CustomResponse();
    }


    [ClaimsAuthorize("Produto", "Remover")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var produtoViewModel = await ObterProduto(id);

        if (produtoViewModel == null) return BadRequest();

        await _produtoService.Remover(id);

        return NoContent();
    }

    private bool UploadArquivo(string arquivo, string imgNome)
    {
        if (string.IsNullOrWhiteSpace(arquivo))
        {
            NotificarErro("Forneça uma imagem para este produto!");
            return false;
        }

        var imageDataByteArray = Convert.FromBase64String(arquivo);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

        if (System.IO.File.Exists(filePath))
        {
            NotificarErro("Já existe um arquivo com este nome!");
            return false;
        }

        System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

        return true;
    }

    private async Task<bool> UploadArquivoStream(IFormFile arquivo, string imgPrefixo)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            NotificarErro("Forneça uma imagem para este produto!");
            return false;
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

        if (System.IO.File.Exists(path))
        {
            NotificarErro("Já existe um arquivo com este nome!");
            return false;
        }

        await using var stream = new FileStream(path, FileMode.Create);
        await arquivo.CopyToAsync(stream);

        return true;
    }

    private async Task<ProdutoViewModel?> ObterProduto(Guid id)
    {
        return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
    }
}