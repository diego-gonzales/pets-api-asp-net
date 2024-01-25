using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace pets_web_api;

public class MyCustomControllerBase<T> : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<T> _logger;

    public MyCustomControllerBase(ApplicationDbContext dbContext, IMapper mapper, ILogger<T> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    protected async Task<ActionResult<List<TDto>>> Get<TEntity, TDto>()
        where TEntity : class
    {
        try
        {
            var entities = await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
            return _mapper.Map<List<TDto>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting entities.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    protected async Task<ActionResult<TDto>> Get<TEntity, TDto>(int id)
        where TEntity : class, IId
    {
        try
        {
            var entity = await _dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            return _mapper.Map<TDto>(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the entity.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    protected async Task<ActionResult> Post<TEntity, TDto, TCreationDto>(
        TCreationDto creationDto,
        string methodName
    )
        where TEntity : class, IId
    {
        try
        {
            var entity = _mapper.Map<TEntity>(creationDto);

            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync();

            var dto = _mapper.Map<TDto>(entity);

            return CreatedAtRoute(methodName, new { id = entity.Id }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the entity");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    protected async Task<ActionResult> Put<TEntity, TCreationDto>(int id, TCreationDto updatingDto)
        where TEntity : class, IId
    {
        try
        {
            var entity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id); // no colocar AsNoTracking(), de lo contrario no actualizará

            if (entity == null)
            {
                return NotFound();
            }

            entity = _mapper.Map(updatingDto, entity);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the entity.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    protected async Task<ActionResult> Patch<TEntity, TPatchingDto>(
        int id,
        JsonPatchDocument<TPatchingDto> jsonPatchDocument
    )
        where TEntity : class, IId
        where TPatchingDto : class
    {
        try
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var entity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id); // no colocar AsNoTracking(), de lo contrario no actualizará

            if (entity == null)
            {
                return NotFound();
            }

            var patchingDto = _mapper.Map<TPatchingDto>(entity);

            jsonPatchDocument.ApplyTo(patchingDto, ModelState);

            var isValid = TryValidateModel(patchingDto);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(patchingDto, entity);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while patching the entity.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    protected async Task<ActionResult> Delete<TEntity>(int id)
        where TEntity : class, IId, new() // con el new() indicamos que nuestro TEntity va a tener un constructor vacío (🚩)
    {
        try
        {
            var entityExists = await _dbContext.Set<TEntity>().AnyAsync(x => x.Id == id);

            if (!entityExists)
            {
                return NotFound();
            }

            _dbContext.Remove(new TEntity() { Id = id }); // (🚩)
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the entity.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
