using AutoMapper;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FornecedoresController : BaseController
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IFornecedorService _fornecedorService;
    private readonly IMapper _mapper;

    public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                  IEnderecoRepository enderecoRepository,
                                  IFornecedorService fornecedorService,
                                  INotificador notificador,
                                  IMapper mapper,
                                  IUser appUser) : base(notificador, appUser)
    {
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
        _fornecedorService = fornecedorService;
        _enderecoRepository = enderecoRepository;
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
    {
        var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
        return fornecedores;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
    {
        var fornecedor = await ObterFornecedorProdutosEndereco(id);
        if (fornecedor is null)
            return NotFound();

        return fornecedor;
    }

    [ClaimsAuthorize("Fornecedor", "Adicionar")]
    [HttpPost]
    public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));

        return CustomResponse(fornecedorViewModel);
    }

    [ClaimsAuthorize("Fornecedor", "Atualizar")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
    {
        if (id != fornecedorViewModel.Id) return BadRequest();

        if (!ModelState.IsValid) CustomResponse(ModelState);

        await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));

        return CustomResponse(fornecedorViewModel);
    }

    [ClaimsAuthorize("Fornecedor", "Remover")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var fornecedorViewModel = await ObterFornecedorEndereco(id);
        if (fornecedorViewModel == null) return NotFound();

        await _fornecedorService.Remover(id);

        return CustomResponse();
    }

    [HttpGet("endereco/{id:guid}")]
    public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id)
    {
        return _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
    }

    [ClaimsAuthorize("Fornecedor", "Atualizar")]
    [HttpPut("endereco/{id:guid}")]
    public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
    {
        if (id != enderecoViewModel.Id) return BadRequest();

        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var endereco = _mapper.Map<Endereco>(enderecoViewModel);
        await _fornecedorService.AtualizarEndereco(endereco);

        return CustomResponse(enderecoViewModel);
    }

    private async Task<FornecedorViewModel?> ObterFornecedorProdutosEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
    }

    private async Task<FornecedorViewModel?> ObterFornecedorEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
    }
}