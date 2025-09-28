using Microsoft.AspNetCore.SignalR;
using WolfGameBackend.DTO;


namespace WolfGameBackend.Controllers
{
    public class RoomHubController : Hub
    {

        private readonly ILogger<RoomHubController> _logger;
        public static Dictionary<string, string> UsuariosConectados = new();

        public RoomHubController(ILogger<RoomHubController> logger)
        {
            _logger = logger;
        }

        public async Task<HubResponse> RegisterUser(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return new HubResponse { StatusCode = 400, Message = "El nombre de usuario no puede estar vacío" };
                }

                UsuariosConectados[Context.ConnectionId] = username;

                await Clients.All.SendAsync("RecibirMensaje", $"Jugador {username} se conectó.");
                return new HubResponse { StatusCode = 200, Message = "Usuario conectado correctamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RoomHubController)} - {nameof(RegisterUser)} Error: {ex.Message}");
                return new HubResponse { StatusCode = 500, Message = "Se ha producido un error al registrar al usuario" };
            }
        }

        public override async Task<HubResponse> OnConnectedAsync()
        {
            try
            {
                var username = ObtenerNombreUsuario();
                if(string.IsNullOrEmpty(username))
                {
                    return new HubResponse { StatusCode = 400, Message = "No se ha podido obtener el nombre de usuario" };
                }

                await Clients.All.SendAsync("RecibirMensaje", $"Jugador {username} se conectó.");
                await base.OnConnectedAsync();
                return new HubResponse { StatusCode = 200, Message = "Conectado correctamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RoomHubController)} - {nameof(OnConnectedAsync)} Error: {ex.Message}");
                return new HubResponse { StatusCode = 500, Message = "Se ha producido     un error al conectarse a la sala" };
            }
        }

        public override async Task<HubResponse> OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var username = ObtenerNombreUsuario();

                await Clients.All.SendAsync("RecibirMensaje", $"Jugador {username} se desconectó.");
                await base.OnDisconnectedAsync(exception);
                return new HubResponse { StatusCode = 200, Message = "Desconectado correctamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RoomHubController)} - {nameof(OnDisconnectedAsync)} Error: {ex.Message}");
                return new HubResponse { StatusCode = 500, Message = "Se ha producido un error al desconectarse a la sala" };
            }
        }

        public async Task<HubResponse> CreateRoom(string nombreSala)
        {
            try
            {
                var username = ObtenerNombreUsuario();
                if (string.IsNullOrEmpty(username))
                {
                    return new HubResponse { StatusCode = 400, Message = "No se ha podido obtener el nombre de usuario" };
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, nombreSala);
                await Clients.Group(nombreSala).SendAsync("RecibirMensaje", $"Jugador {username} creó/unió la sala {nombreSala}");
                return new HubResponse { StatusCode = 200, Message = "Sala creada correctamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RoomHubController)} - {nameof(CreateRoom)} Error: {ex.Message}");
                return new HubResponse { StatusCode = 500, Message = "Se ha producido un error al crear a la sala" };
            }
        }

        public async Task<HubResponse> ConnectRoom(string nombreSala)
        {
            try
            {
                var username = ObtenerNombreUsuario();
                if (string.IsNullOrEmpty(username))
                {
                    return new HubResponse { StatusCode = 400, Message = "No se ha podido obtener el nombre de usuario" };
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, nombreSala);
                await Clients.Group(nombreSala).SendAsync("RecibirMensaje", $"Jugador {username} se unió a la sala {nombreSala}");
                return new HubResponse { StatusCode = 200, Message = "Unido correctamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RoomHubController)} - {nameof(ConnectRoom)} Error: {ex.Message}");
                return new HubResponse { StatusCode = 500, Message = "Se ha producido un error al unirse a la sala" };
            }
        }


        #region private methods

        private string ObtenerNombreUsuario()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var username = httpContext?.Request.Query["username"].ToString() ?? Context.ConnectionId.ToString();
                UsuariosConectados[Context.ConnectionId] = username;
                return username;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RoomHubController)} - {nameof(ObtenerNombreUsuario)} Error: {ex.Message}");
                return string.Empty;
            }
        }

        #endregion

    }
}
