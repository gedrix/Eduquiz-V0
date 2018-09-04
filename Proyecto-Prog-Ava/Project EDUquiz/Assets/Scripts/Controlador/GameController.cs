using System.Collections;
using System.Collections.Generic;
using TMPro; //Libreria que me permite la manipulacion de cajas de texto, inputs, entre otras opciones
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Esta clase es la encargada de controlar la visualizacion de cada una de las pantallas ej.
/// ('principal', 'juego', 'opciones', 'clasificacion', 'logros', 'informacion'). Ademas es la 
/// encargada de controlar los tiempos en que el usuario se toma por partida y por pregunta,
/// calcular los puntajes totales, enviar peticiones al servidor para actualizar informacion del
/// jugador o recibir listados de preguntas, clasificacion, y de los logros.  
/// </summary>
public class GameController : MonoBehaviour {
	public Canvas ui_game; //controla la pantalla de 'juego'
	public Canvas ui_main; //controla la pantalla 'principal'
	public Canvas ui_options; //controla la pantalla de 'opciones'
	public Canvas ui_clasificacion; //controla la pantalla de 'clasificacion'
	public Canvas ui_logros; //controla la pantalla de 'logros'
	public Canvas ui_informacion; //controla la pantalla de 'informacion'
	public TextMeshProUGUI txtNombreUsu; //muestra el nombre del jugador en la pantalla 'principal'
	public TMP_InputField inputNombreUsu; //caja para editar el nombre del jugador actual
	public Image imgUsuario; //muestra la imagen que el usuario tenga actualmente.
	public Image imgUsuarioMain; //muestra la imagen del jugador en la pantalla 'principal'
	public TextMeshProUGUI txtMsjOpcion; //para mostrar de error o exito en la pantalla de 'opciones'
	public TextMeshProUGUI txtNivel; //muestra el nivel actual del jugador.
	public TextMeshProUGUI txtTiempoPregunta; //muestra el tiempo restante de cada pregunta
	public TextMeshProUGUI txtTiempoPartida; //muestra el tiempo total que tardo por partida
	public TextMeshProUGUI txtExperiencia; //muestra la experiencia obtenida en la partida
	public TextMeshProUGUI txtPorcentajeExp; //muesta el porcentaje de experiencia actual
	public TextMeshProUGUI txtTotalExperiencia; //muestra la suma total de la exp obtenida.
	public Slider sliderExperiencia; //muestra una barra de porcentaje para la experiencia
	public TextMeshProUGUI txtResumenNivel; //muestra el nivel del jugador en la vista de 'Resumen partida'
	public TextMeshProUGUI txtPregunta; //muestra la pregunta actual obtenida del servidor
	public TextMeshProUGUI txtCreadaPor; //muestra el jugador que creo la pregunta actual
	public TextMeshProUGUI txtDificultad; //muestra la dificultad de la pregunta actual
	public TextMeshProUGUI txtCategorias; //muestra la(s) categoria(s) a la que pertenece la pregunta
	public TextMeshProUGUI[] txtOpcion; //muestra cada una de las opciones de la pregunta actual
	public Animator[] animOpcion; //me permite controlar las animaciones de las opciones
	public Button[] btnOpcion; //me permite obtener el control de cuando se 'clickeo' algun boton de las opciones
	public GameObject panelAccion; //para activarlo / desactivarlo por script
	public GameObject panelCerrarSesion; //para activarlo / desactivarlo por script
	public GameObject panelIrAlMenu; //para activarlo / desactivarlo por script
	public GameObject panelReportarPregunta; //para activarlo / desactivarlo por script
	public GameObject panelRevPregunta; //para activarlo / desactivarlo por script
	public GameObject panelResumenPartida; //para activarlo / desactivarlo por script
	public GameObject panelElegirImagen; //para activarlo / desactivarlo por script
	public Animator animPanelAccion; //animacion de la ventana al darle clic a una opcion
	public TextMeshProUGUI txtMsjResultado; //muestra si el usuario respondio de manera correcta o no.
	public Animator animTiempoPregunta; //controla la animacion del tiempo por pregunta
	public Animator animSugerirPreg; //controla la animacion de la 'sugerencia de preguntas'
	public TMP_InputField inputCrearPregunta; //caja para escribir la pregunta a crear
	public TMP_Dropdown cbxCategoria; //para seleccionar la categoria de la pregunta
	public TMP_Dropdown cbxDificultad; //para seleccionar la dificultad de la pregunta
	public TMP_InputField[] inputCrearOpcion; //caja para escribir cada una de las opciones
	public TextMeshProUGUI txtMsjCrearPregunta; //muestra un mensaje de error o de exito al intentar enviarlas al servidor
	public TextMeshProUGUI txtContadorPreg; //contador que aparece para saber el numero de la pregunta actual
	public TextMeshProUGUI txtPregAcertadas; //muestra la cantidad de aciertos por partida
	public TextMeshProUGUI txtRachaAciertos; //muestra si tuvo una racha de aciertos por partida
	public TextMeshProUGUI txtRachaPuntos; //muestra si obtuvo una bonificacion por racha de aciertos
	public TextMeshProUGUI txtBonusTiempo; //muestra si obtuvo una bonificacion por tiempo
	//panel de carga
	public GameObject panelCargando; //para activarlo / desactivarlo por script
	public GameObject btnCancelarCarga; //boton que se habilita si ocurrio algun error al intentar recibir info del servidor
	public TextMeshProUGUI txtCargando; //muestra un mensaje de si se encuentra cargando
	public TextMeshProUGUI txtCargaTitulo; //muestra un titulo en la pantalla de 'carga'
	//variables necesarias para la obtencion de 'logros'
	public GameObject panelNuevoLogro; //mini-panel para mostrar si existen nuevos logros...
	public TextMeshProUGUI txtNuevoLogro; //muestra cuantos logros se desbloquearon
	public static int conJugadas; //contador de las veces que se realizo y termino una partida
	public static int numAcertadas = 0; //contador de preguntas acertadas correctamente...
	public static int minPartida = 0; //entero que almacena los minutos de tiempo por partida.
	public static float timerPartida = 0f; //almacena los segundos de tiempo por partida
	//reportar pregunta
	public TMP_Dropdown cbxReportarPregunta; //muestra una lista de opciones para el reporte de la pregunta actual
	public Button btnReportarPregunta; //para activarlo / desactivarlo por script 
	private string imgActual = "user-default"; //compara las imagenes (Imagen por defecto)
	private int conCargaTexto = 0; //sirve para validar el mensaje de 'carga'
	private bool banCarga = false; //bandera que me permite saber si debe aparecer la pantalla de 'carga'
	private float timerPregunta = 15.99f; //tiempo que el jugador tiene para responder cada pregunta
	private bool banTimerPregunta = false; //para saber si el tiempo de la pregunta debe 'reiniciarse' o 'pararse'
	private bool banTimerPartida = false; //para saber si el tiempo de la partida debe 'pararse'
	private int experienciaActual = 0; //para calcular la experiencia obtenida
	private int numPregunta = 0; //contador de la pregunta actual que se muestra en partida
	private int numMaxPreguntas = 10; //el numero de preguntas que se extraera por partida 'clasica'
	private int puntosExp = 0; //puntos de experiencia ganados en esa partida
	private int puntosClasificacion = 0; //puntos ganados (Clasificacion) por partida
	private int racha = 0, rachaMax = 0, rachaPuntos = 0; //para almacenar la racha de las preguntas acertadas
	private int bonusTiempo = 0; //para calcular si obtuvo un bonus de tiempo
	Persona objPersona; //auxiliar para la info del usuario
	Pregunta objPregunta; //auxiliar para la creacion de las preguntas
	Preguntas preguntas; //las N preguntas aleatorias
	ControlNiveles controlNiveles = new ControlNiveles (); //obj que contiene metodos para calcular el nivel y dificultad de las preguntas
	private List<Pregunta> listaPreguntas; //almacena cada una de las preguntas obtenidas de la base de datos
	private List<int> listaAcertadas = new List<int> (); //almacena los id's de las preguntas correctas
	//referencia a otros scripts con Monobehaviour
	ControlClasificacion controlClasificacion; //para hacer referencia al control de la clasificacion
	LogrosController logrosController; //para hacer referencia al control de los logros

	/// <summary>
	/// Este metodo es el primero en iniciarse y se ejecuta solo una vez, necesario para referenciar
	/// componentes o definir el valor inicial de una variable, en este caso utilizado para hacer
	/// referencia a otros scripts necesaarios y obtener la informacion del jugador.
	/// </summary>
	void Awake () {
		//referencia a componentes
		controlClasificacion = FindObjectOfType<ControlClasificacion> (); //busca y obtiene el componente
		logrosController = FindObjectOfType<LogrosController> (); //busca y obtiene el componente
		imgUsuario = imgUsuario.GetComponent<Image> ();
		imgUsuarioMain = imgUsuarioMain.GetComponent<Image> ();
		//esta linea al parecer no funca en movil...
		Application.runInBackground = true; //la aplicacion se mantiene ejecutando en segundo plano (por los timers)
		btnCancelarCarga.SetActive (false); //inabilito el boton hasta ser llamado...
		conJugadas = PlayerPrefs.GetInt ("conJugadas", 0); //por defecto 0, en el contador de veces jugadas
		string json = PlayerPrefs.GetString ("InfoJugador", "");
		if (json == "") {
			SceneManager.LoadScene ("login");
		} else {
			mostrarInfoJugador (json);
		}
		btnIrAlMenu ();
	}

	/// <summary>
	/// Este metodo se ejecuta cada frame por segundo y es utilizado en este caso para controlar
	/// el tiempo de las preguntas y de la partida actual, tambien controla si se habilito la
	/// pantalla de 'carga' mostrando un mensaje de carga.
	/// </summary>
	void Update () {
		//si... es necesario las tres condiciones para no bugear el control del tiempo...
		if (banTimerPregunta && !animPanelAccion.GetBool ("FadeIn")) { //esto da una advertencia rara...
			timerPregunta -= Time.deltaTime;
			txtTiempoPregunta.text = timerPregunta.ToString ("0");
			if (timerPregunta < 0) {
				accionOpciones (-1, "Ups, tiempo agotado"); //-1 es el por defecto para saber si no fue clic del usuario
			}
		}
		if (banTimerPartida) {
			timerPartida += Time.deltaTime;
		}
		if (panelCargando.activeSelf) {
			pantallaCargando ();
		} else {
			if (banCarga) {
				banCarga = false;
				//detengo la coroutina, para no sobrecargar la memoria...
				StopCoroutine ("tiempoCarga");
			}
		}
	}

	/// <summary>
	/// Este metodo es llamado para actualizar el texto que presenta si se desbloquearon nuevos
	/// logros, sino simplemente no lo activa.
	/// </summary>
	public void mostrarNuevosLogros () {
		//verifica si existen nuevos logros
		if (LogrosController.nuevoLogro != 0) {
			//activa notificacion que existen nuevos logros
			panelNuevoLogro.SetActive (true);
			txtNuevoLogro.text = LogrosController.nuevoLogro.ToString ();
		}
	}

	/// <summary>
	/// Este metodo se ejecuta si una bandera es activada y si esto pasa ejecuta una coroutina que
	/// lo que hace es mostrar y cambiar un mensaje en la pantalla de 'carga'
	/// </summary>
	private void pantallaCargando () {
		if (banCarga) {
			banCarga = false;
			StartCoroutine ("tiempoCarga");
		}
	}

	/// <summary>
	/// Es llamado para cambiar un mensaje en un texto que es presentado en la pantalla de 'carga'
	/// y que es espera la cantidad de tiempo que se le es definido y continua con la ejecucion.
	/// </summary>
	/// <returns>Espera los N segundos y continua con la ejecucion</returns>
	IEnumerator tiempoCarga () {
		yield return new WaitForSeconds (0.25f);
		switch (conCargaTexto) {
			case 1:
				txtCargando.text = "Cargando";
				break;
			case 2:
				txtCargando.text = "Cargando.";
				break;
			case 3:
				txtCargando.text = "Cargando..";
				break;
			case 4:
				txtCargando.text = "Cargando...";
				conCargaTexto = 0;
				break;
		}
		conCargaTexto++; //incremento el contador
		banCarga = true;
	}

	/// <summary>
	/// Este metodo se encarga de desglosar la informacion que recibe como formato Json a un objeto
	/// luego de transformar la informacion, esta es asignada hacia donde queremos presentarla para
	/// que pueda ser visualizada por el jugador.
	/// </summary>
	/// <param name="json">recibe la informacion del jugador en formato Json</param>
	void mostrarInfoJugador (string json) {
		print (json);
		objPersona = JsonUtility.FromJson<Persona> (json);
		inputNombreUsu.text = txtNombreUsu.text = objPersona.nombre;
		txtNivel.text = controlNiveles.obtenerNivelUsuario (objPersona.experiencia) [0] + "";
		imgActual = objPersona.imagen;
		imgUsuario.sprite = Resources.Load<Sprite> ("Images/Users/" + imgActual);
		imgUsuarioMain.sprite = Resources.Load<Sprite> ("Images/Users/" + objPersona.imagen);
		//listar los logros una vez obtenido el 'token'
		logrosController.obtenerListaLogros (objPersona.token, objPersona.external_id);
	}

	/// <summary>
	/// Este metodo es ejecutado mediante un boton que es referenciado atravez del editor Unity,
	/// su labor es recbir el nombre que tenga (nombre de la imagen asignada), con este nombre se
	/// puede buscar en los recursos de Unity para cambiar la imagen del jugador actual, una vez
	/// hecho esto cierra la ventana de 'seleccion de imagen'.
	/// </summary>
	/// <param name="name"></param>
	public void btnImagenSeleccion (string name) {
		print (name);
		imgActual = name;
		panelElegirImagen.SetActive (false);
		imgUsuario.sprite = Resources.Load<Sprite> ("Images/Users/" + imgActual);
	}

	/// <summary>
	/// Este metodo es ejecutado cuando el boton 'Jugar' es clickeado o tocado y se encarga de 
	/// desactivar todos los canvas o interfaces (solo para prevenir errores visuales) y habilitar 
	/// la interfaz de 'juego', limpiar las listas de preguntas y aciertos, envia una peticion
	/// al servidor solicitando las N preguntas aleatorias y habilita la pantalla de 'carga'.
	/// ademas restablece los valores necesarios para la nueva partida a un valor por defecto.
	/// </summary>
	public void btnJugar () { //mostrar la vista del "Juego Clasico"
		desactivarTodosCanvas ();
		listaPreguntas = new List<Pregunta> ();
		listaPreguntas.Clear ();
		listaAcertadas.Clear ();

		obtenerPreguntas ();
		habilitarBotones ();
		limpiarOpciones ();
		txtCargaTitulo.text = "Estamos seleccionando tus preguntas";
		//activar panel de 'cargando el juego'
		panelCargando.SetActive (true);
		panelRevPregunta.SetActive (false); //desactivado momentaneamente...
		panelResumenPartida.SetActive (false); //porsia...
		panelAccion.SetActive (false); //porsia...
		btnCancelarCarga.SetActive (false); //porsia...
		banCarga = true;
		//reiniciar el contador de las preguntas, rachas, acertadas y el tiempo
		numPregunta = 0;
		numAcertadas = 0;
		timerPartida = 0f;
		racha = 0;
		rachaMax = 0;
		rachaPuntos = 0;
		bonusTiempo = 0;
		experienciaActual = 0;
		puntosClasificacion = 0;
		//hago visible la vista del juego
		ui_game.enabled = true;
	}

	/// <summary>
	/// Este metodo desactiva todas las interfaces y habilita la pantalla de 'opciones'
	/// restablece algunos valores por defecto y muestra el nombre y la imagen actual del jugador.
	/// </summary>
	public void btnOpciones () { //mostrar la vista de "Opciones"
		desactivarTodosCanvas ();
		txtMsjOpcion.text = "";
		inputNombreUsu.interactable = false;
		imgUsuario.sprite = Resources.Load<Sprite> ("Images/Users/" + objPersona.imagen);
		ui_options.enabled = true;
	}

	/// <summary>
	/// Este metodo desactiva todas las interfaces y habilita la pantalla de 'clasificacion' y
	/// envia una peticion al servidor solicitando la informacion del listados de jugadores para
	/// listar la clasificacion en la vista.
	/// </summary>
	public void btnClasificacion () { //mostrar la vista de "Clasificacion"
		desactivarTodosCanvas ();

		ui_clasificacion.enabled = true;
		controlClasificacion.obtenerListaClasificacion (objPersona.token, objPersona.correo);
	}

	/// <summary>
	/// Este metodo desactiva todas las interfaces y habilita la pantalla de 'logros' y
	/// inabilita la notificacion de nuevos logros porque ya se abrio la ventana.
	/// </summary>
	public void btnLogros () { //mostrar la vista de "Logros"
		desactivarTodosCanvas ();
		panelNuevoLogro.SetActive (false);
		LogrosController.nuevoLogro = 0;
		ui_logros.enabled = true;
	}

	/// <summary>
	/// Este metodo desactiva todas las interfaces y habilita la pantalla de 'informacion'.
	/// </summary>
	public void btnInformacion () { //mostrar la vista de "Informacion"
		desactivarTodosCanvas ();

		ui_informacion.enabled = true;
	}

	/// <summary>
	/// Este metodo se encarga de desactivar todos los canvas (interfaces).
	/// </summary>
	private void desactivarTodosCanvas () {
		ui_game.enabled = false;
		ui_main.enabled = false;
		ui_options.enabled = false;
		ui_clasificacion.enabled = false;
		ui_informacion.enabled = false;
		ui_logros.enabled = false;
	}

	/// <summary>
	/// Este metodo se encarga de habilitar el boton de reportar la pregunta y ejecutar una
	/// animacion, esto cuando se selecciono ya alguna opcion o se acabo el tiempo de la pregunta.
	/// </summary>
	public void btnVerPregunta () {
		panelRevPregunta.SetActive (true);
		animPanelAccion.SetBool ("FadeIn", false); //oculto el panel de accion
	}

	/// <summary>
	/// Este metodo se encarga de borrar todos los datos locales almacenados en la clase 
	/// <PlayerPrefs> y envia al jugador a la escena de 'login'.
	/// </summary>
	public void btnCerrarSesion () {
		PlayerPrefs.DeleteAll (); //USAR CON CUIDADO...
		SceneManager.LoadScene ("login");
	}
	/// <summary>
	/// Este metodo cancela la pantalla de 'cerrar sesion'.
	/// </summary>
	public void btnCancelarCS () {
		panelCerrarSesion.SetActive (false);
	}
	/// <summary>
	/// Este metodo habilita la pantalla de 'cerrar sesion'.
	/// </summary>
	public void btnSalirCS () {
		panelCerrarSesion.SetActive (true);
	}

	/// <summary>
	/// Este metodo habilita la pantalla 'salir al menu'
	/// </summary>
	public void btnHomeIAM () {
		panelIrAlMenu.SetActive (true);
	}

	/// <summary>
	/// Este metodo desactiva la pantalla 'salir al menu'
	/// </summary>
	public void btnCancelarIAM () {
		panelIrAlMenu.SetActive (false);
	}

	/// <summary>
	/// Este metodo habilita la pantalla 'reporte de pregunta'
	/// </summary>
	public void btnHabilitarRP () {
		panelReportarPregunta.SetActive (true);
	}

	/// <summary>
	/// Este metodo desactiva la pantalla 'reporte de pregunta'
	/// </summary>
	public void btnCancelarRP () {
		panelReportarPregunta.SetActive (false);
	}

	/// <summary>
	/// Este metodo se encarga de obtener el reporte seleccionado junto con el id de la pregunta
	/// seleccionada y envia una peticion al servidor transformando esa peticion en un formato Json.
	/// </summary>
	public void btnEnviarRP () {
		panelReportarPregunta.SetActive (false);
		btnReportarPregunta.interactable = false;

		string json = "{\"motivo\":\"" + cbxReportarPregunta.options[cbxReportarPregunta.value].text + "\"," +
			"\"id_pregunta\":\"" + preguntas.ids[numPregunta - 1] + "\"}";
		//string url = "https://eduquiza.azurewebsites.net/public/index.php/reporte/registrar/" +//server azure
		//	objPersona.external_id;
		string url = "https://eduquiz.000webhostapp.com/public/index.php/reporte/registrar/" + //server free
			objPersona.external_id;

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", objPersona.token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestReportePregunta (www));
	}

	/// <summary>
	/// Este metodo se encarga de esperar una respuesta del servidor para presentar un mensaje
	/// de si se envio correctamente el reporte de la pregunta seleccionada.
	/// </summary>
	/// <param name="www">peticion que fue enviada al servicio web</param>
	/// <returns>Espera una respuesta del servidor para continuar la ejecucion del metodo</returns>
	IEnumerator requestReportePregunta (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Operacion existosa")) {
				print ("Pregunta reportada con exito.");
			} else {
				print (www.text);
			}
		} else
			print (www.error);
	}

	/// <summary>
	/// Este metodo se encarga de habilitar la caja de texto para editar el nombre del jugador
	/// actual.
	/// </summary>
	public void activarInputNombre () {
		inputNombreUsu.interactable = true;
		inputNombreUsu.Select ();
	}

	/// <summary>
	/// Este metodo se encarga de desactivar todas los canvas y verifica si se desbloqueo algun logro
	/// como tambien restablece algunos valores por defecto y desactiva paneles que pudieron haberse
	/// habilitado, terminado esto habilita la pantalla 'principal'.
	/// </summary>
	public void btnIrAlMenu () {
		desactivarTodosCanvas ();
		mostrarNuevosLogros ();
		//tiempo pregunta
		animTiempoPregunta.SetBool ("is15s", false);
		banTimerPregunta = false;
		banTimerPartida = false;
		timerPregunta = 0f;
		//desactivacion de paneles
		panelAccion.SetActive (false);
		panelIrAlMenu.SetActive (false);
		panelCargando.SetActive (false); //porsia...
		panelRevPregunta.SetActive (false); // tbm porsia...
		panelReportarPregunta.SetActive (false); //porsia...
		panelElegirImagen.SetActive (false); //porsia...
		panelResumenPartida.SetActive (false);

		ui_main.enabled = true;
	}

	/// <summary>
	/// Este metodo se encarga de ejecutar una animacion que hace aparecer la ventana de 
	/// 'crear pregunta'
	/// </summary>
	public void btnSugerirPregunta () {
		animSugerirPreg.SetBool ("mostrarSugerir", true);
	}

	/// <summary>
	/// Este metodo se encarga de ejecutar una animacion que hace desaparecer la ventana de 
	/// 'crear pregunta'
	/// </summary>
	public void btnRegresarSugerir () {
		animSugerirPreg.SetBool ("mostrarSugerir", false);
	}

	/// <summary>
	/// Este metodo se encarga de habilitar la ventana de 'elegir imagen' para el jugador
	/// </summary>
	public void btnAbrirPanelElegirImagen () {
		panelElegirImagen.SetActive (true);
	}

	/// <summary>
	/// Este metodo se encarga de desactivar la ventana de 'elegir imagen'.º
	/// </summary>
	public void btnCerrarPanelElegirImagen () {
		panelElegirImagen.SetActive (false);
	}

	/// <summary>
	/// Este metodo se encarga de verificar si el jugador selecciono en la opcion 1 la respuesta
	/// correcta, ejecuta una animacion cambiando el color de esta opcion
	/// </summary>
	public void btnOpcion1 () {
		//pregunto por el .opcion[0], porque la respuesta correcta siempre se ubicara en esta posicion
		if (txtOpcion[0].text == listaPreguntas[numPregunta - 1].opcion[0]) {
			animOpcion[0].SetBool ("Correcta", true);
		} else
			animOpcion[0].SetBool ("Incorrecta", true);
		accionOpciones (0);
	}

	/// <summary>
	/// Este metodo se encarga de verificar si el jugador selecciono en la opcion 2 la respuesta
	/// correcta, ejecuta una animacion cambiando el color de esta opcion
	/// </summary>
	public void btnOpcion2 () {
		if (txtOpcion[1].text == listaPreguntas[numPregunta - 1].opcion[0]) {
			animOpcion[1].SetBool ("Correcta", true);
		} else
			animOpcion[1].SetBool ("Incorrecta", true);
		accionOpciones (1);
	}

	/// <summary>
	/// Este metodo se encarga de verificar si el jugador selecciono en la opcion 3 la respuesta
	/// correcta, ejecuta una animacion cambiando el color de esta opcion
	/// </summary>
	public void btnOpcion3 () {
		if (txtOpcion[2].text == listaPreguntas[numPregunta - 1].opcion[0]) {
			animOpcion[2].SetBool ("Correcta", true);
		} else
			animOpcion[2].SetBool ("Incorrecta", true);
		accionOpciones (2);
	}

	/// <summary>
	/// Este metodo se encarga de verificar si el jugador selecciono en la opcion 4 la respuesta
	/// correcta, ejecuta una animacion cambiando el color de esta opcion
	/// </summary>
	public void btnOpcion4 () {
		if (txtOpcion[3].text == listaPreguntas[numPregunta - 1].opcion[0]) {
			animOpcion[3].SetBool ("Correcta", true);
		} else
			animOpcion[3].SetBool ("Incorrecta", true);
		accionOpciones (3);
	}

	/// <summary>
	/// Este metodo se encarga de determinar si se hicieron cambios en las 'opciones' del jugador
	/// si es asi se envia una peticion con los cambios al servicio web para actualizar la infor
	/// del jugador.
	/// </summary>
	public void btnEditarUsuario () { //boton de aceptar en las opciones
		if ((inputNombreUsu.text != "" && inputNombreUsu.text != objPersona.nombre) ||
			objPersona.imagen != imgActual) {
			Persona personaAux = new Persona (); //cada vez que se da al boton iniciar se crea un nuevo objeto persona
			personaAux.nombre = inputNombreUsu.text.Trim (); //quitando los espacios en blanco (L & R)
			personaAux.imagen = imgUsuario.sprite.name;
			string json = JsonUtility.ToJson (personaAux);
			//string url = "https://eduquiza.azurewebsites.net/public/index.php/usuario/editar/" + //server azure
			//	objPersona.external_id;
			string url = "https://eduquiz.000webhostapp.com/public/index.php/usuario/editar/" + //server free
				objPersona.external_id;

			Dictionary<string, string> headers = new Dictionary<string, string> ();
			headers.Add ("Content-Type", "application/json; charset=utf-8");
			headers.Add ("token", objPersona.token);

			byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

			WWW www = new WWW (url, pData, headers);
			StartCoroutine (requestEditarNombre (www));
		} else {
			btnIrAlMenu ();
		}
	}

	/// <summary>
	/// Este metodo espera la respuesta del servicio web y a su vez muestra un mensaje de exito o
	/// error para informar al jugador acerca de los cambios que realizo
	/// </summary>
	/// <param name="www">peticion que se envio al servicio web</param>
	/// <returns>espera la respuesta del servicio web y continua con la ejecucion del metodo</returns>
	IEnumerator requestEditarNombre (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("No se ha encontrado ningun dato"))
				txtMsjOpcion.text = "Error:\n<color=red><i>No se encontraron datos, vuelva a iniciar sesion.</color>"; //error
			if (www.text.Contains ("La data no tiene el formato deseado"))
				txtMsjOpcion.text = "Error:\n<color=red><i>Ha ocurrido un error al enviar los datos al servidor, intente mas tarde.</color>"; //error
			if (www.text.Contains ("Operacion existosa")) {
				txtMsjOpcion.text = "<align=center><color=green>Se ha actualizado la informacion correctamente";
				verificarSesion ();
				btnIrAlMenu ();
			}
		} else
			txtMsjOpcion.text = "Error:\n<color=red>" + www.error + "</color>";
	}

	/// <summary>
	/// Este metodo se encarga de validar que la sesion actual del jugador no haya sido alterada
	/// y si en cuyo caso no llega a autenticar la sesion se vuelve a solicitar la informacion
	/// del jugador borrando sus datos locales y enviandolo a la escena de 'login'.
	/// </summary>
	/// <param name="espera">esto sirve para hacer un cambio extra al recibir la respuesta del ws</param>
	private void verificarSesion (string espera = "nada") {
		string json = PlayerPrefs.GetString ("InfoJugador", "");
		//necesario por el encryptamiento de las claves
		Persona auxJson = JsonUtility.FromJson<Persona> (json);
		auxJson.clave = PlayerPrefs.GetString ("InfoClave", "");
		json = JsonUtility.ToJson (auxJson);
		//string url = "https://eduquiza.azurewebsites.net/public/index.php/login";// server azure
		string url = "https://eduquiz.000webhostapp.com/public/index.php/login"; // server free

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestSesion (www, espera));
	}

	/// <summary>
	/// Este metodo se encarga de esperar la respuesta del servicio web y si esta todo correcto
	/// almaceno la informacion que viene en formato Json en un archivo local que me sirve para
	/// validar la sesion de forma automatica la proxima vez que se vuelva a abrir la aplicacion.
	/// </summary>
	/// <param name="www">peticion que se envio al servicio web</param>
	/// <param name="espera">sirve de bandera para actualizar la informacion del jugador</param>
	/// <returns>espera la respuesta del servicio web y continua con la ejecucion del metodo</returns>
	IEnumerator requestSesion (WWW www, string espera) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Operacion existosa")) {
				objPersona = JsonUtility.FromJson<Persona> (www.text);
				PlayerPrefs.SetString ("InfoJugador", www.text);
				mostrarInfoJugador (www.text);
				if (espera == "actualizarInfoUsuario") {
					actualizarInfoNivelUsuario ();
				}
			} else {
				//si no pudo encontrar la info del jugador, borro los datos locales y vuelvo a pedir iniciar sesion
				btnCerrarSesion ();
			}
		} else {
			btnCerrarSesion ();
		}
	}

	/// <summary>
	/// Este metodo se encarga de enviar una peticion al servicio web para obtener el listado de
	/// preguntas aleaorias de acuerdo a la dificultad que se deriva del nivel actual del jugador
	/// </summary>
	private void obtenerPreguntas () {
		objPregunta = new Pregunta ();
		objPregunta.dificultad = obtenerDificultad (); // esto obtendra la dificultad de acuerdo al nivel...
		objPregunta.cantidad = numMaxPreguntas; //obtengo el numero de preguntas seleccionado

		string json = JsonUtility.ToJson (objPregunta);

		//string url = "localhost:80/Proyecto-Prog-Ava/Juego/public/index.php/preguntas/ramdon"; //ruta local
		//string url = "https://eduquiza.azurewebsites.net/public/index.php/preguntas/ramdon"; //ruta servidor
		string url = "https://eduquiz.000webhostapp.com/public/index.php/preguntas/ramdon"; //server free

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", objPersona.token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestPreguntas (www));
	}

	/// <summary>
	/// Este metodo espera la respuesta del servicio web y si esta todo correcto lo que hace es
	/// almacenar en una lista cada una de las preguntas con sus respectivas opciones para empezar
	/// a jugar.
	/// </summary>
	/// <param name="www">peticion que es enviada al servicio web</param>
	/// <returns>espera la respuesta del servicio web y continua la ejecucion del metodo</returns>
	IEnumerator requestPreguntas (WWW www) {
		yield return www;
		btnCancelarCarga.SetActive (true);
		if (www.text.Contains ("Operacion existosa")) {
			btnCancelarCarga.SetActive (false);
			preguntas = JsonUtility.FromJson<Preguntas> (www.text);
			for (int i = 0; i < preguntas.preguntas.Length; i++) {
				Pregunta obId = new Pregunta ();
				obId.id = preguntas.ids[i];
				string jsonOp = JsonUtility.ToJson (obId);
				//string urlOp = "localhost:80/Proyecto-Prog-Ava/Juego/public/index.php/pregunta/obtenerPreguntaId"; //ruta local
				//string urlOp = "https://eduquiza.azurewebsites.net/public/index.php/preguntas/obtenerPreguntaId"; //ruta servidor
				string urlOp = "https://eduquiz.000webhostapp.com/public/index.php/preguntas/obtenerPreguntaId"; //server free

				Dictionary<string, string> headersOp = new Dictionary<string, string> ();
				headersOp.Add ("Content-Type", "application/json; charset=utf-8");
				headersOp.Add ("token", objPersona.token);

				byte[] pData = System.Text.Encoding.UTF8.GetBytes (jsonOp.ToCharArray ());
				WWW wwwObPreg = new WWW (urlOp, pData, headersOp);

				yield return wwwObPreg;

				if (wwwObPreg.text.Contains ("Operacion existosa")) {
					btnCancelarCarga.SetActive (false);
					Pregunta auxOpciones = new Pregunta ();
					auxOpciones = JsonUtility.FromJson<Pregunta> (wwwObPreg.text);
					auxOpciones.pregunta = preguntas.preguntas[i];
					auxOpciones.dificultad = preguntas.dificultad[i];
					listaPreguntas.Add (auxOpciones);
				} else {
					btnCancelarCarga.SetActive (true);
					banCarga = false;
					StopCoroutine ("tiempoCarga");
					txtCargaTitulo.text = "Ha ocurrido un error, comprueba tu conexion a internet.\n" +
						"Detalles: " + wwwObPreg.text;
					break;
				}
			}
			yield return new WaitForSeconds (0.01f);
			siguientePregunta ();
			Handheld.Vibrate (); //vibrar el movil... (avisando que ya se cargaron las preguntas)
		} else {
			print ("Error: " + www.error);
			btnCancelarCarga.SetActive (true);
			banCarga = false;
			StopCoroutine ("tiempoCarga");
			txtCargaTitulo.text = "<color=red>Error:\n</color>" +
				"Verifica tu conexion a internet \n" + www.error;
		}
	}

	/// <summary>
	/// Este metodo se encarga de refrescar los textos de las preguntas y opciones que son
	/// mostrados en el juego, y cada vez que es llamado se encarga de mostrar la info de la
	/// nueva pregunta mostrando de una manera aleatoria las opciones.
	/// </summary>
	public void siguientePregunta () {
		if (numPregunta < numMaxPreguntas) {
			limpiarOpciones (); //se limpian las opciones y pregunta
			//luego de obtener y mostrar la pregunta, habilito el timer y lo reinicio(por pregunta)
			timerPregunta = 15f; //esto deberia modificarse de acuerdo a la dificultad...
			//titulo pregunta
			txtPregunta.text = listaPreguntas[numPregunta].pregunta;
			//definir categorias
			if (listaPreguntas[numPregunta].categoria.Length != 1) {
				for (int i = 0; i < listaPreguntas[numPregunta].categoria.Length; i++) {
					if (i > 0)
						txtCategorias.text += ", " + listaPreguntas[numPregunta].categoria[i];
					else
						txtCategorias.text = listaPreguntas[numPregunta].categoria[i];
				}
			} else {
				txtCategorias.text = listaPreguntas[numPregunta].categoria[0];
			}
			//aleatoriedad de las opciones
			List<int> nums = new List<int> ();
			int nOp = Random.Range (0, 4);
			txtOpcion[0].text = listaPreguntas[numPregunta].opcion[nOp];
			nums.Add (nOp);
			while (true) {
				nOp = Random.Range (0, 4);
				if (!nums.Contains (nOp)) {
					if (txtOpcion[1].text == "") {
						txtOpcion[1].text = listaPreguntas[numPregunta].opcion[nOp];
						nums.Add (nOp);
						continue;
					}
					if (txtOpcion[2].text == "") {
						txtOpcion[2].text = listaPreguntas[numPregunta].opcion[nOp];
						nums.Add (nOp);
						continue;
					}
					if (txtOpcion[3].text == "") {
						txtOpcion[3].text = listaPreguntas[numPregunta].opcion[nOp];
						break;
					}
				}
			}
			//cargo la dificultad actual de la pregunta y quien la creó
			txtDificultad.text = listaPreguntas[numPregunta].dificultad;
			txtCreadaPor.text = "<b>Creada por:</b> " + listaPreguntas[numPregunta].creadaPor;
			//desactivo el panel de 'carga'
			panelCargando.SetActive (false);
			banCarga = false;
			StopCoroutine ("tiempoCarga");
			//al final incremento en 1 la el contador de las preguntas
			numPregunta++;
			txtContadorPreg.text = numPregunta.ToString ("0") + " / " + numMaxPreguntas.ToString ("0"); //esto puede cambiar mas adelante...
			banTimerPregunta = true; //activo el tiempo por pregunta
			banTimerPartida = true; //activo el tiempo de la partida actual
			animTiempoPregunta.SetBool ("is15s", true);
			btnReportarPregunta.interactable = true; //se la reactiva, por si anteriormente fue desactivada...
		} else { //En este caso quiere decir que ya completo las preguntas y debo mostrar la pantalla de completado
			banTimerPartida = false;
			panelCargando.SetActive (true);
			banCarga = true;
			txtCargaTitulo.text = "Comprobando respuestas";
			//contabilizar experiencia, nivel, etc
			contabilizarPuntosExperiencia ();
		}
	}

	/// <summary>
	/// Este metodo es llamado al terminar la partida, entonces su funcion es calcular los puntos
	/// de experiencia obtenidos en la partida, incluyendo los puntos de bonificacion.
	/// </summary>
	public void contabilizarPuntosExperiencia () {
		experienciaActual = 0;
		puntosClasificacion = 0; //porsia...
		for (int i = 0; i < listaAcertadas.Count; i++) {
			int[] infoPuntaje = controlNiveles.obtenerPuntosPorDificultad (objPersona.nivel,
				listaPreguntas[listaAcertadas[i]].dificultad);
			experienciaActual += infoPuntaje[0];
			puntosClasificacion += infoPuntaje[1];
		}
		//antes de enviar, tambien calculo la racha de aciertos y la incremento
		if (rachaMax == 2)
			rachaPuntos = Random.Range (0, 3); //incremento un aleatorio de 0 a 2 puntos
		if (rachaMax >= 3 && racha <= 5)
			rachaPuntos = Random.Range (5, 11); //incremento un aleatorio de 5 a 10 puntos
		if (rachaMax >= 6 && racha <= 8)
			rachaPuntos = Random.Range (10, 26); //incremento un aleatorio de 10 a 25 puntos
		if (rachaMax >= 9)
			rachaPuntos = Random.Range (25, 51); //incremento un aleatorio de 25 a 50 puntos
		txtExperiencia.text = experienciaActual + " Pts";
		experienciaActual += rachaPuntos; //incremento el bonus a la experiencia
		//calcular el bonus de tiempo
		minPartida = 0; //controla los minutos
		for (int i = 0; i < 10; i++) {
			if (timerPartida >= 60f) {
				timerPartida -= 60f;
				minPartida++;
			} else {
				break;
			}
		}
		//control del bonus de tiempo
		if (numAcertadas >= 3) //para acceder al bonus de tiempo es necesario tener minimo 3 preguntas acertadas
		{
			if (minPartida < 1) // si es menos de 1 minuto (aplico un bonus de tiempo)
			{
				bonusTiempo = Random.Range (10, 21); //el bonus sera aleatorio entre 10 y 20 puntos
				if (timerPartida <= 30f) // si es menos de 30 segundos (bonus de tiempo mayor)
					bonusTiempo = Random.Range (20, 36); //el bonus sera aleatorio entre 20 y 35 puntos
				if (numAcertadas >= 5) //para acceder al bonus mayor se necesita minimo 5 acertadas
				{
					if (timerPartida <= 15f) // si es menos de 15 segundos (el mejor bonus de tiempo)
						bonusTiempo = Random.Range (35, 51); //el bonus sera aleatorio entre 35 y 50 puntos	
				}
			} else if (minPartida == 1) {
				if (timerPartida <= 30f) // si es menos de 1:30 segundos (bonus de tiempo mayor)
					bonusTiempo = Random.Range (0, 11); //el bonus sera aleatorio entre 0 y 10 puntos
			}
		}
		verificarSesion ("actualizarInfoUsuario");
	}

	/// <summary>
	/// Este metodo se enarga de modificar la informacion del jugador de acuerdo a los puntajes
	/// que obtuvo el jugador en la partida que jugó, envia una peticion al servicio web con la
	/// informacion actualizada, transformada en formato Json. 
	/// </summary>
	private void actualizarInfoNivelUsuario () {
		puntosExp = experienciaActual + bonusTiempo;
		objPersona.experiencia += puntosExp; //incremento la experiencia ganada	
		objPersona.nivel = controlNiveles.obtenerNivelUsuario (objPersona.experiencia) [0];
		objPersona.puntaje += puntosClasificacion; //incremento el puntaje

		string json = JsonUtility.ToJson (objPersona);

		//string url = "localhost:80/Proyecto-Prog-Ava/Juego/public/index.php/usuario/actualizarinfo/" +
		//	objPersona.external_id; //ruta local
		string url = "https://eduquiz.000webhostapp.com/public/index.php/usuario/actualizarinfo/" +
			objPersona.external_id; //ruta servidor free

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", objPersona.token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestInfoNivelUser (www));
	}

	/// <summary>
	/// Este metodo espera la respuesta del servicio web que se encarga de actualizar la infor
	/// del jugador, verifica la sesion del jugador y da por terminada la partida actual.
	/// </summary>
	/// <param name="www">peticion que fue enviada al servicio web</param>
	/// <returns>espera la respuesta del servicio web y continua la ejecucion de metodo</returns>
	IEnumerator requestInfoNivelUser (WWW www) {
		yield return www;
		btnCancelarCarga.SetActive (true);
		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Operacion existosa")) {
				btnCancelarCarga.SetActive (false);
				verificarSesion ();
				//desactivo el panel de 'carga'
				panelCargando.SetActive (false);
				banCarga = false;
				StopCoroutine ("tiempoCarga");
				partidaClasicaTerminada ();
			} else {
				banCarga = false;
				btnCancelarCarga.SetActive (true);
				StopCoroutine ("tiempoCarga");
				txtCargaTitulo.text = "Ha ocurrido un error, comprueba tu conexion a internet.\n" +
					"Detalles: " + www.text;
			}
		} else {
			print ("Error: " + www.error);
			btnCancelarCarga.SetActive (true);
			banCarga = false;
			StopCoroutine ("tiempoCarga");
			txtCargaTitulo.text = "<color=red>Error:\n</color>" + www.error;
		}
	}

	/// <summary>
	/// Este metodo se encarga de dar por terminada la partida actual y restablece algunos valores
	/// necesarios para una proxima partida, asi como tambien se encarga de mostrar el resumen de
	/// la partida jugada.
	/// </summary>
	private void partidaClasicaTerminada () {
		numPregunta = 0; //reinicio el contador
		conJugadas++; //incremento el contador de veces jugadas para los 'logros'.
		PlayerPrefs.SetInt ("conJugadas", conJugadas);
		panelResumenPartida.SetActive (true);

		txtTiempoPartida.text = minPartida + timerPartida.ToString (":00") + "s";
		txtPregAcertadas.text = numAcertadas + " de " + numMaxPreguntas;
		txtRachaAciertos.text = rachaMax + "";
		txtBonusTiempo.text = bonusTiempo + " Pts";
		txtRachaPuntos.text = rachaPuntos + " Pts";
		txtResumenNivel.text = "Nivel " + objPersona.nivel;
		int[] infoNivel = controlNiveles.obtenerNivelUsuario (objPersona.experiencia);
		//verifica si el jugador consiguio algun logro 
		logrosController.controlLogros (objPersona.external_id, objPersona.token);
		logrosController.controlLogrosDeNivel (objPersona.nivel, objPersona.external_id, objPersona.token);

		StartCoroutine ("contarPuntaje");
		StartCoroutine (contarPorcentaje (infoNivel));
	}

	/// <summary>
	/// Este metodo se encarga de mostrar un contador activo en la pantalla de 'resumen partida'
	/// del puntaje total obtenido por el jugador.
	/// </summary>
	/// <returns>espera el tiempo definido y continua con la ejecucion</returns>
	IEnumerator contarPuntaje () {
		for (int i = 0; i < puntosExp; i += 3) {
			yield return new WaitForSeconds (0.02f);
			txtTotalExperiencia.text = i + " Pts";
		}
		txtTotalExperiencia.text = puntosExp + " Pts";
	}

	/// <summary>
	/// Este metodo se encarga de mostrar una barra de experiencia 'cargandose' en la pantalla de
	/// 'resumen partida' de la experiencia total obtenida por el jugador.
	/// </summary>
	/// <returns>espera el tiempo definido y continua con la ejecucion</returns>
	IEnumerator contarPorcentaje (int[] infoNivel) {
		for (int i = 0; i < infoNivel[1]; i += 2) {
			yield return new WaitForSeconds (0.02f);
			txtPorcentajeExp.text = i + "%";
			sliderExperiencia.value = i;
		}
		txtPorcentajeExp.text = infoNivel[1] + "%";
		sliderExperiencia.value = infoNivel[1];
		//verifica y muestra en la pantalla 'principal' si se desbloqueo algun logro
		mostrarNuevosLogros ();
	}

	/// <summary>
	/// Este metodo se encarga de verificar que las cajas de texto de la ventana 'crear pregunta'
	/// y si esta todo correctamente llenado, se procede a almacenar esta informacion en un objeto
	/// que pasa a ser transformado en informacion Json para hacer una peticion al servicio web
	/// para que se guarde como una pregunta sugerida (con diferente estado).
	/// </summary>
	public void btnCrearPregunta () {
		if (inputCrearPregunta.text != "" && inputCrearOpcion[0].text != "" && inputCrearOpcion[1].text != "" &&
			inputCrearOpcion[2].text != "" && inputCrearOpcion[3].text != "") {

			Pregunta objPregunta = new Pregunta (); //cada vez que se da al boton iniciar se crea un nuevo objeto persona
			objPregunta.pregunta = inputCrearPregunta.text;
			objPregunta.dificultad = cbxDificultad.options[cbxDificultad.value].text;
			objPregunta.categoria = new string[1]; //definir el tamaño del arreglo para que no te joda...
			objPregunta.categoria[0] = cbxCategoria.options[cbxCategoria.value].text;
			for (int i = 0; i < 4; i++) {
				objPregunta.opcion[i] = inputCrearOpcion[i].text;
			}

			string jsonSg = JsonUtility.ToJson (objPregunta);

			//string url = "https://eduquiza.azurewebsites.net/public/index.php/preguntas/registro/" +
			//	objPersona.external_id; // server azure
			string url = "https://eduquiz.000webhostapp.com/public/index.php/preguntas/registro/" +
				objPersona.external_id; // server free

			Dictionary<string, string> headers = new Dictionary<string, string> ();
			headers.Add ("Content-Type", "application/json; charset=utf-8");
			headers.Add ("token", objPersona.token);

			byte[] pData = System.Text.Encoding.UTF8.GetBytes (jsonSg.ToCharArray ());

			WWW www = new WWW (url, pData, headers);
			txtMsjCrearPregunta.text = "Espere...";
			StartCoroutine (requestCrearPregunta (www));
		} else {
			txtMsjCrearPregunta.text = "Complete todos los campos";
		}
	}

	/// <summary>
	/// Este metodo se encarga de esperar una respuesta del servicio web y si todo esta correctamente
	/// muestra un mensaje de exito y se limpian las cajas de texto, luego de esto se desactiva la
	/// pantalla de 'crear pregunta'.
	/// </summary>
	/// <param name="www">peticion que fue enviada al servicio web</param>
	/// <returns>espera la respuesta del servicio web</returns>
	IEnumerator requestCrearPregunta (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Faltan datos en formulario"))
				txtMsjCrearPregunta.text = "Error:\n<color=red><i>Llene todos los campos</color>"; //error
			if (www.text.Contains ("La data no tiene el formato deseado"))
				txtMsjCrearPregunta.text = "Error:\n<color=red><i>Ha ocurrido un error al enviar los datos al servidor, intente mas tarde.</color>"; //error
			if (www.text.Contains ("Correo ya registrado"))
				txtMsjCrearPregunta.text = "Error:\n<color=red><i>Esta pregunta ya existe</color>"; //error
			if (www.text.Contains ("Operacion existosa")) {
				txtMsjCrearPregunta.text = "<align=center><color=green>Se ha registrado correctamente"; //text of success
				//Se limpian las cajas de texto del sugerir pregunta
				inputCrearPregunta.text = "";
				for (int i = 0; i < inputCrearOpcion.Length; i++) {
					inputCrearOpcion[i].text = "";
				}
				//se vuelve al menu de opciones
				btnRegresarSugerir ();
			}
		} else
			txtMsjCrearPregunta.text = "Error:\n<color=red>" + www.error + "</color>"; //error
	}

	/// <summary>
	/// Este metodo se encarga de limpiar cada una de los textos que se muestran en la pantalla de
	/// 'juego', esto para evitar errores visuales.
	/// </summary>
	private void limpiarOpciones () {
		txtPregunta.text = "";
		for (int i = 0; i < 4; i++) {
			txtOpcion[i].text = "";
		}
		txtMsjResultado.text = "";
	}

	/// <summary>
	/// Este metodo se encarga de habilitar los botones de opcion que son mostrados en la 
	/// pantalla de 'juego'.
	/// </summary>
	private void habilitarBotones () {
		for (int i = 0; i < 4; i++) {
			btnOpcion[i].interactable = true;
			animOpcion[i].SetBool ("Correcta", false);
			animOpcion[i].SetBool ("Incorrecta", false);
		}
	}

	/// <summary>
	/// Este metodo se encarga de determinar si la opcion que selecciono el jugador fue la 
	/// respuesta correcta, tambien incrementa el numero de aciertos que lleva y la maxima racha
	/// de aciertos que tiene actualmente el jugador.
	/// </summary>
	/// <param name="n">para determinar si fue el usuario quien llamo al metodo</param>
	/// <param name="str">para mostrar el mensaje de exito o de error al jugador</param>
	private void accionOpciones (int n = -1, string str = "¡Has fallado!") {
		animTiempoPregunta.SetBool ("is15s", false);
		banTimerPregunta = false;
		banTimerPartida = false;
		for (int i = 0; i < 4; i++) {
			btnOpcion[i].interactable = false;
		}
		panelAccion.SetActive (true);
		if (n != -1) { //si se seleciono una opcion... sea correcta o no
			if (animOpcion[n].GetBool ("Correcta")) { //si el usuario encontro la correcta
				txtMsjResultado.text = "¡Correcto!";
				racha++; //incremento la racha de aciertos
				numAcertadas++; //incremento los aciertos
				listaAcertadas.Add (numPregunta - 1); //almaceno los id's de las preguntas acertadas
				if (rachaMax < racha)
					rachaMax = racha;
			} else { //incorrecta y mostrar al usuario la correcta
				racha = 0; //pierde la racha
				txtMsjResultado.text = str;
				buscarCorrecta ();
			}
		} else { //busca la respuesta correcta, si se acabo el tiempo
			racha = 0; //pierde la racha
			txtMsjResultado.text = str;
			buscarCorrecta ();
		}
		animPanelAccion.SetBool ("FadeIn", true);
	}

	/// <summary>
	/// Este metodo se encarga de buscar en cada una de las opciones a la respuesta correcta
	/// y mostrarsela al jugador mediante una animacion.
	/// </summary>
	private void buscarCorrecta () {
		for (int i = 0; i < 4; i++) {
			if (txtOpcion[i].text == listaPreguntas[numPregunta - 1].opcion[0]) {
				animOpcion[i].SetBool ("Correcta", true);
				break;
			}
		}
	}

	/// <summary>
	/// Este metodo esta vinculado con el boton 'continuar' que aparece cada vez que termina de
	/// seleccionar una opcion de alguna pregunta y se encarga de mostrar la siguiente pregunta.
	/// </summary>
	public void btnContinuar () {
		animPanelAccion.SetBool ("FadeIn", false);
		panelRevPregunta.SetActive (false);
		siguientePregunta ();
		habilitarBotones ();
	}

	/// <summary>
	/// Este metodo se encarga de buscar la dificultad adecuada de acuerdo al nivel actual en el
	/// que el jugador se encuentre.
	/// </summary>
	/// <returns>devuelve la dificultad de acuerdo al nivel en el que se encuentre el jugador</returns>
	private string obtenerDificultad () {
		string strDefault = "Facil";

		if (objPersona.nivel > 0)
			strDefault = "Facil";
		if (objPersona.nivel > 5)
			strDefault = "Normal";
		if (objPersona.nivel > 10)
			strDefault = "Dificil";
		if (objPersona.nivel > 15)
			strDefault = "Muy Dificil";
		if (objPersona.nivel > 20)
			strDefault = "Maestro";

		return strDefault;
	}
}