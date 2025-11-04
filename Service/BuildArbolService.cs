
using System.Threading.Channels;
using AppAdminEmployed.Service;
using AppAdminEmployed.Models;
using Microsoft.Extensions.Caching.Memory;
public class ArbolBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<Guid> _cola;
    public ArbolBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _cola = Channel.CreateUnbounded<Guid>();
    }
    // Método para encolar trabajos
    public async Task EncolarConstruccion(Guid supervisorUuid)
    {
        await _cola.Writer.WriteAsync(supervisorUuid);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var supervisorUuid in _cola.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var empleadoService = scope.ServiceProvider.GetRequiredService<EmpleadosService>();
            var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
            var arbol = await empleadoService.ConstruirJerarquiaSubordinados(supervisorUuid);
            cache.Set($"Arbol_{supervisorUuid}", arbol, TimeSpan.FromMinutes(30));
        }
    }

    public bool EstaConstruido(Guid supervisorUuid)
    {
        using var scope = _serviceProvider.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        return cache.TryGetValue($"Arbol_{supervisorUuid}", out _);
    }

    public EmpleadoModel? GetArbol(Guid supervisorUuid)
    {
        using var scope = _serviceProvider.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        cache.TryGetValue($"Arbol_{supervisorUuid}", out EmpleadoModel? arbol);
        return arbol;
    }

    public void EliminarArbol(Guid supervisorUuid)
    {
        using var scope = _serviceProvider.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        cache.Remove($"Arbol_{supervisorUuid}");
    }

    public List<EmpleadoModel> ArbolALista(EmpleadoModel? root)
    {
        var lista = new List<EmpleadoModel>();
        if (root == null) return lista;
    
        RecorrerArbol(root, lista);
        return lista;
    }
    
    private void RecorrerArbol(EmpleadoModel nodo, List<EmpleadoModel> lista)
    {
        lista.Add(nodo);
    
        if (nodo.Subordinados != null)
        {
            foreach (var sub in nodo.Subordinados)
            {
                RecorrerArbol(sub, lista);
            }
        }
    }

    public async Task ReConstruirArbol(Guid supervisorUuid)
    {
        EliminarArbol(supervisorUuid);
        await EncolarConstruccion(supervisorUuid);
    }
}